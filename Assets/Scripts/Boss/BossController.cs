using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Transform player;
    public Transform shootPoint;
    public GameObject projectilePrefab;

    public float detectionRadius = 10f;
    public float attackRange = 5f;

    [Header("Fase 1")]
    public float speed = 2f;
    public float attackCooldownFase1 = 2f;

    [Header("Fase 2")]
    public float speedFase2 = 3.2f;
    public float attackCooldownFase2 = 1.1f;
    [Range(0.1f, 0.9f)] public float porcentajeActivacionFase2 = 0.5f;

    [Header("Combate")]
    public float retrasoDisparoProyectil = 0.15f;
    public float duracionAtaque = 0.55f;
    public float fuerzaRebote = 5f;
    public float escalaBoss = 1f;
    public float tiempoRecuperacionDanio = 0.35f;

    [Header("Vida y daño")]
    public int vida = 10;
    public int vidaMaxima = 10;
    public int danioContacto = 1;

    [Header("Muerte")]
    public float tiempoDesaparicionMuerte = 4f;

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D[] colliders;

    private bool recibiendoDanio;
    private bool muerto;
    private bool playerVivo = true;
    private bool enMovimiento;
    private bool muerteProcesada;
    private bool segundaFase;
    private bool ejecutandoAtaque;
    private bool combateIniciado;

    private float nextAttackTime;
    private Vector2 movement;

    private Coroutine rutinaRecuperacionDanio;
    private Coroutine rutinaAtaque;

    public BossHealthUI bossHealthUI;
    public CreditsPanel creditsPanel;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        colliders = GetComponents<Collider2D>();

        if (bossHealthUI != null)
        {
            bossHealthUI.Mostrar(true);
        }

        ActualizarHUDBoss();

        if (player == null)
        {
            PlayerController2 playerScript = FindFirstObjectByType<PlayerController2>();
            if (playerScript != null)
            {
                player = playerScript.transform;
            }
        }
    }

    void Update()
    {
        if (muerto)
        {
            ProcesarMuerte();
            return;
        }

        if (player == null || !playerVivo)
        {
            movement = Vector2.zero;
            enMovimiento = false;
            ActualizarAnimaciones();
            return;
        }

        ComprobarSegundaFase();

        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia <= detectionRadius)
        {
            if (!combateIniciado)
            {
                combateIniciado = true;

                if (bossHealthUI != null)
                {
                    bossHealthUI.Mostrar(true);
                }
            }

            MirarAlJugador();

            if (distancia > attackRange)
            {
                Vector2 dir = (player.position - transform.position).normalized;
                movement = new Vector2(dir.x, 0f);
                enMovimiento = true;
            }
            else
            {
                movement = Vector2.zero;
                enMovimiento = false;

                float cooldownActual = segundaFase ? attackCooldownFase2 : attackCooldownFase1;

                if (Time.time >= nextAttackTime && !ejecutandoAtaque)
                {
                    Atacar();
                    nextAttackTime = Time.time + cooldownActual;
                }
            }
        }
        else
        {
            movement = Vector2.zero;
            enMovimiento = false;
        }

        ActualizarAnimaciones();
    }

    void FixedUpdate()
    {
        if (!recibiendoDanio && !muerto && !ejecutandoAtaque)
        {
            Vector2 vel = rb.linearVelocity;
            vel.x = movement.x * speed;
            rb.linearVelocity = vel;
        }
    }

    private void MirarAlJugador()
    {
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-escalaBoss, escalaBoss, 1f);
        }
        else
        {
            transform.localScale = new Vector3(escalaBoss, escalaBoss, 1f);
        }
    }

    private void Atacar()
    {
        if (recibiendoDanio || ejecutandoAtaque)
        {
            return;
        }

        if (rutinaAtaque != null)
        {
            StopCoroutine(rutinaAtaque);
        }

        rutinaAtaque = StartCoroutine(EjecutarAtaque());
    }

    private IEnumerator EjecutarAtaque()
    {
        ejecutandoAtaque = true;
        movement = Vector2.zero;
        enMovimiento = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        animator.SetTrigger("atacar");

        yield return new WaitForSeconds(retrasoDisparoProyectil);
        DispararProyectil();

        yield return new WaitForSeconds(duracionAtaque);

        ejecutandoAtaque = false;
        rutinaAtaque = null;
    }

    private void DispararProyectil()
    {
        if (projectilePrefab == null || shootPoint == null)
        {
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        Vector3 scale = projectile.transform.localScale;
        scale.x = transform.localScale.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        projectile.transform.localScale = scale;
    }

    private void ActualizarAnimaciones()
    {
        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("recibeDanio", recibiendoDanio);
        animator.SetBool("muerto", muerto);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (muerto)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0f);
            PlayerController2 playerScript = collision.gameObject.GetComponent<PlayerController2>();

            if (playerScript != null)
            {
                playerScript.RecibiendoDanio(direccionDanio, danioContacto);
                playerVivo = !playerScript.muerto;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (muerto)
        {
            return;
        }

        if (collision.CompareTag("Espada") || collision.CompareTag("bala"))
        {
            Vector2 direccionDanio = new Vector2(collision.transform.position.x, 0f);
            RecibeDanio(direccionDanio, 1);
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (recibiendoDanio || muerto)
        {
            return;
        }

        recibiendoDanio = true;
        vida -= cantDanio;

        if (vida < 0)
        {
            vida = 0;
        }

        ActualizarHUDBoss();

        if (vida <= 0)
        {
            muerto = true;
            recibiendoDanio = false;
        }
        else
        {
            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.002f).normalized;
            rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);

            if (rutinaRecuperacionDanio != null)
            {
                StopCoroutine(rutinaRecuperacionDanio);
            }

            rutinaRecuperacionDanio = StartCoroutine(RecuperarDeDanio());
        }
    }

    public void DesactivarDanio()
    {
        recibiendoDanio = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private IEnumerator RecuperarDeDanio()
    {
        yield return new WaitForSeconds(tiempoRecuperacionDanio);
        DesactivarDanio();
        rutinaRecuperacionDanio = null;
    }

    private void ComprobarSegundaFase()
    {
        if (segundaFase)
        {
            return;
        }

        int vidaUmbral = Mathf.CeilToInt(vidaMaxima * porcentajeActivacionFase2);

        if (vida <= vidaUmbral)
        {
            segundaFase = true;
            speed = speedFase2;
        }
    }

    private void ProcesarMuerte()
    {
        if (muerteProcesada)
        {
            return;
        }

        muerteProcesada = true;
        movement = Vector2.zero;
        enMovimiento = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = false;

        foreach (Collider2D c in colliders)
        {
            c.enabled = false;
        }

        if (bossHealthUI != null)
        {
            bossHealthUI.Mostrar(false);
        }

        if (creditsPanel != null)
        {
            creditsPanel.MostrarCreditos(tiempoDesaparicionMuerte);
        }

        Destroy(gameObject, tiempoDesaparicionMuerte);
    }

    private void ActualizarHUDBoss()
    {
        if (bossHealthUI != null)
        {
            bossHealthUI.ActualizarVida(vida, vidaMaxima);
        }
    }
}
