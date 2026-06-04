using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    public int vida = 3;
    public int vidaMaxima = 3;
    public int mana = 3;
    public int manaMaxima = 3;
    public float velocidad = 5f;


    public float fuerzaSalto = 7f;
    public float fuerzaDash = 30f;
    public float longitudRaycast = 0.1f;

    public LayerMask capaSuelo;

    public bool enSuelo;
    private bool dasheando;
    public bool disparando = false;
    private bool dashed;
    public bool recibeDanio = false;
    public bool ataca;
    public bool muerto;
    private Rigidbody2D rb;
    public DetectorInteracciones dectI;

    public Animator animator;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoSalto;
    [SerializeField] private AudioClip sonidoDobleSalto;
    [SerializeField] private AudioClip sonidoAtaque;
    [SerializeField] private AudioClip sonidoDash;
    [SerializeField] private AudioClip sonidoDanio;
    [SerializeField] private AudioClip sonidoCaminar;
    [SerializeField] private AudioClip sonidoMuerte;
    private float fuerzaRebote;
    private HUDController hudController;
    private InventoryController inventoryController;

    [Header("Doble salto")]
    public bool dobleSaltoDesbloqueadoPorDefecto;
    private bool dobleSaltoUsado;

    [Header("Mana")]
    public int costoManaAtaqueDistancia = 1;

    [Header("Duraciones de bloqueo")]
    [SerializeField] private float duracionAtaque = 0.35f;
    [SerializeField] private float duracionDash = 0.25f;

    private Coroutine rutinaAtaque;
    private Coroutine rutinaDash;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        hudController = FindFirstObjectByType<HUDController>();
        inventoryController = FindFirstObjectByType<InventoryController>();

        if (SistemaGuardado.datosCargados != null)
        {
            CargarDatos(SistemaGuardado.datosCargados);
        }

        ActualizarHUDVida();
        ActualizarHUDMana();

    }

    public SaveData GetDatos()
    {
        return new SaveData
        {
            nombreEscena = SceneManager.GetActiveScene().name,
            vida = vida,
            mana = mana,
            manaMaxima = manaMaxima,
            velocidad = velocidad,
            fuerzaSalto = fuerzaSalto,
            fuerzaRebote = fuerzaRebote,
            posX = transform.position.x,
            posY = transform.position.y,
            posZ = transform.position.z
        };
    }

    public void CargarDatos(SaveData datos)
    {
        vida = datos.vida;
        if (datos.manaMaxima > 0)
        {
            manaMaxima = datos.manaMaxima;
            mana = Mathf.Clamp(datos.mana, 0, manaMaxima);
        }

        velocidad = datos.velocidad;
        fuerzaSalto = datos.fuerzaSalto;
        fuerzaRebote = datos.fuerzaRebote;
        transform.position = new Vector3(datos.posX, datos.posY, datos.posZ);

        ActualizarHUDVida();
        ActualizarHUDMana();
    }

    // Update is called once per frame
    void Update(){

        if (Pausar.estaPausado)
        {
            return;
        }

        if (!muerto) 
        { 
        
            if (!ataca && !disparando)
            {
                Movimiento();

                ActualizarEstadoSuelo();

                if (enSuelo)
                {
                    dashed = false;
                    dobleSaltoUsado = false;
                }

                if (Input.GetKeyDown(KeyCode.Space) && !recibeDanio)
                {
                    Saltar();
                }
            }
            if (Input.GetKeyDown(KeyCode.J) && !ataca && !dasheando && enSuelo)
            {
                Atacando();
            }
            if (Input.GetKeyDown(KeyCode.K) && !ataca && !dasheando && !dashed)
            {
                Dash();
            }
            Animaciones();
        }
        
    }

    private void Animaciones()
    {
        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("recibeDanio", recibeDanio);
    }

    public void Movimiento()
    {
        float velocidadX = Input.GetAxis("Horizontal") * Time.deltaTime * velocidad;

        animator.SetFloat("Movement", velocidadX * velocidad);

        if (velocidadX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (velocidadX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        Vector3 posicion = transform.position;

        if (!recibeDanio)
        {
            transform.position = new Vector3(velocidadX + posicion.x, posicion.y, posicion.z);
        }
    }

    private void ActualizarEstadoSuelo()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
        enSuelo = hit.collider != null;
    }

    private void Saltar()
    {
        if (enSuelo)
        {
            animator.Play("jump", 0, 0f);
            ReproducirSonidoSalto();
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
            return;
        }

        if (PuedeDobleSaltar() && !dobleSaltoUsado)
        {
            dobleSaltoUsado = true;
            animator.Play("jump", 0, 0f);
            ReproducirSonidoDobleSalto();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
        }
    }

    private bool PuedeDobleSaltar()
    {
        if (dobleSaltoDesbloqueadoPorDefecto)
        {
            return true;
        }

        if (inventoryController == null)
        {
            inventoryController = FindFirstObjectByType<InventoryController>();
        }

        return inventoryController != null && inventoryController.TieneItemDobleSalto();
    }

    public void RecibiendoDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibeDanio)
        {
            recibeDanio = true;
            vida -= cantDanio;

            if (vida < 0)
            {
                vida = 0;
            }

            ActualizarHUDVida();

            if (vida <= 0)
            {
                muerto = true;
                animator.SetBool("muerto", muerto);
                ReproducirSonidoMuerte();

                if (GameManager.instance != null)
                {
                    GameManager.instance.GameOver();
                }
            }
            else
            {
                ReproducirSonidoDanio();
            }

            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.002f).normalized;
            rb.AddForce(rebote * fuerzaDash, ForceMode2D.Impulse);
        }
    }


    public void DesactivarDanio()
    {
        recibeDanio = false;
        rb.linearVelocity = Vector2.zero;
    }

    public void Atacando()
    {
        ataca = true;
        animator.ResetTrigger("atacar");
        animator.SetTrigger("atacar");
        ReproducirSonido(sonidoAtaque);

        if (rutinaAtaque != null)
        {
            StopCoroutine(rutinaAtaque);
        }

        rutinaAtaque = StartCoroutine(LiberarAtaquePorTiempo());
    }

    public void Desactivaratacando()
    {
        ataca = false;
        rutinaAtaque = null;
    }

    public void Dash()
    {
        float direccion = transform.localScale.x;

        dashed = true;
        dasheando = true;
        animator.ResetTrigger("dash");
        animator.SetTrigger("dash");

        ReproducirSonidoDash();

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(direccion * fuerzaDash, 2f), ForceMode2D.Impulse);

        if (rutinaDash != null)
        {
            StopCoroutine(rutinaDash);
        }

        rutinaDash = StartCoroutine(LiberarDashPorTiempo());
    }


    public void DesactivarDash()
    {
        dasheando = false;
        rb.linearVelocity = Vector2.zero;
        rutinaDash = null;
    }

    private IEnumerator LiberarAtaquePorTiempo()
    {
        yield return new WaitForSeconds(duracionAtaque);
        Desactivaratacando();
    }

    private IEnumerator LiberarDashPorTiempo()
    {
        yield return new WaitForSeconds(duracionDash);
        DesactivarDash();
    }

    public void Curar(int cantidad)
    {
        vida += cantidad;

        if (vida > vidaMaxima)
        {
            vida = vidaMaxima;
        }

        ActualizarHUDVida();
    }

    public bool PuedeUsarAtaqueDistancia()
    {
        return mana >= costoManaAtaqueDistancia;
    }

    public bool ConsumirManaAtaqueDistancia()
    {
        if (!PuedeUsarAtaqueDistancia())
        {
            return false;
        }

        mana -= costoManaAtaqueDistancia;

        if (mana < 0)
        {
            mana = 0;
        }

        ActualizarHUDMana();
        return true;
    }

    public void RecuperarMana(int cantidad)
    {
        mana += cantidad;

        if (mana > manaMaxima)
        {
            mana = manaMaxima;
        }

        ActualizarHUDMana();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }

    private void ActualizarHUDVida()
    {
        if (hudController != null)
        {
            hudController.ActualizarVida(vida, vidaMaxima);
        }
    }

    private void ActualizarHUDMana()
    {
        if (hudController != null)
        {
            hudController.ActualizarMana(mana, manaMaxima);
        }
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    public void ReproducirSonidoSalto()
    {
        ReproducirSonido(sonidoSalto);
    }

    public void ReproducirSonidoDobleSalto()
    {
        ReproducirSonido(sonidoDobleSalto);
    }

    public void ReproducirSonidoAtaque()
    {
        ReproducirSonido(sonidoAtaque);
    }

    public void ReproducirSonidoDash()
    {
        ReproducirSonido(sonidoDash);
    }

    public void ReproducirSonidoDanio()
    {
        ReproducirSonido(sonidoDanio);
    }

    public void ReproducirSonidoMuerte()
    {
        ReproducirSonido(sonidoMuerte);
    }

    public void ReproducirSonidoCaminar()
    {
        ReproducirSonido(sonidoCaminar);
    }
}
