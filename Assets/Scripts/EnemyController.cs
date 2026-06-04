using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private static readonly int EnMovimientoHash = Animator.StringToHash("enMovimiento");
    private static readonly int RecibeDanioHash = Animator.StringToHash("recibeDanio");
    private static readonly int MuertoHash = Animator.StringToHash("muerto");
    private static readonly int AtacarHash = Animator.StringToHash("atacar");

    [System.Serializable]
    public class DropConfig
    {
        public GameObject prefab;
        public int itemId;
        [Range(0f, 1f)] public float chance = 1f;
    }

    public Transform player;
    public string enemyId;
    public float detectionRadius = 5.0f;
    public float attackRadius = 1.5f;
    public float attackCooldown = 5f;
    public float speed = 2.0f;
    public float fuerzaRebote = 5.0f;
    public int vida = 2;
    public GameObject dropPrefab;
    public int dropItemId = 1;
    [Range(0f, 1f)] public float dropChance = 1f;
    public DropConfig[] drops = new DropConfig[2];
    public float tiempoDesaparicion = 1.2f;

    public bool recibiendoDanio = false;
    public bool enMovimiento;
    public bool playerVivo;
    public bool muerteEnemigo;
    private bool questNotificada;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Collider2D[] colliders;
    private bool muerteProcesada;
    private float nextAttackTime;
    private bool isAttacking;
    private bool attackAnimationStarted;
    private float forcedAttackEndTime;

    public Animator animator;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoAtaque;
    [SerializeField] private AudioClip sonidoDanio;
    [SerializeField] private AudioClip sonidoMuerte;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        colliders = GetComponents<Collider2D>();
        playerVivo = true;
        muerteEnemigo = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (muerteEnemigo)
        {
            ProcesarMuerte();
        }

        if (playerVivo && !muerteEnemigo)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (isAttacking)
            {
                movement = Vector2.zero;
                enMovimiento = false;
                ActualizarEstadoAtaque();
            }
            else if (distanceToPlayer <= attackRadius && Time.time >= nextAttackTime)
            {
                IniciarAtaque();
            }
            else
            {
                Movimiento(distanceToPlayer);
            }
        }

        animator.SetBool(EnMovimientoHash, enMovimiento);
        animator.SetBool(RecibeDanioHash, recibiendoDanio);
        animator.SetBool(MuertoHash, muerteEnemigo);

        if (muerteEnemigo && !questNotificada)
        {
            questNotificada = true;

            if (QuestController.Instance != null)
            {
                QuestController.Instance.RegistrarEnemigoDerrotado(enemyId, 1);
    }
        }
    }


    void FixedUpdate()
    {
        if (!recibiendoDanio && !isAttacking)
        {
            Vector2 vel = rb.linearVelocity;
            vel.x = movement.x * speed;
            rb.linearVelocity = vel;
        }
    }

    public void Movimiento(float distanceToPlayer)
    {
        if (distanceToPlayer < detectionRadius && distanceToPlayer > attackRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            movement = new Vector2(direction.x, 0);
            enMovimiento = true;

            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

        }
        else
        {
            movement = Vector2.zero;
            enMovimiento = false;
        }

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (muerteEnemigo)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);

            PlayerController2 playerScript = collision.gameObject.GetComponent<PlayerController2>();
            
            playerScript.RecibiendoDanio(direccionDanio, 1);
            playerVivo = !playerScript.muerto;

            if (!playerVivo)
            {
                enMovimiento=false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (muerteEnemigo)
        {
            return;
        }

        if (collision.CompareTag("Espada"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);
            RecibeDanio(direccionDanio, 1);
        }

        if (collision.CompareTag("bala"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);
            RecibeDanio(direccionDanio, 1);
        }
    }


    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
            recibiendoDanio = true;
            vida -= cantDanio;

            if (vida <= 0)
            {
                muerteEnemigo = true;
                vida = 0;
                ReproducirSonidoMuerte();
            }
            else
            {
                ReproducirSonidoDanio();
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.002f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
            }
        }
    }

    public void DesactivarDanio()
    {
        recibiendoDanio = false;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private void IniciarAtaque()
    {
        isAttacking = true;
        attackAnimationStarted = false;
        nextAttackTime = Time.time + attackCooldown;
        forcedAttackEndTime = Time.time + 2.5f;
        movement = Vector2.zero;
        enMovimiento = false;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        Vector2 direction = (player.position - transform.position).normalized;

        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        animator.ResetTrigger(AtacarHash);
        animator.SetTrigger(AtacarHash);
    }

    public void FinAtaque()
    {
        isAttacking = false;
        attackAnimationStarted = false;
    }

    private void ActualizarEstadoAtaque()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool enAnimacionAtaque = stateInfo.IsName("attack");

        if (enAnimacionAtaque)
        {
            attackAnimationStarted = true;
            return;
        }

        if (attackAnimationStarted || Time.time >= forcedAttackEndTime)
        {
            FinAtaque();
        }
    }

    private void ProcesarMuerte()
    {
        if (muerteProcesada)
        {
            return;
        }

        muerteProcesada = true;
        playerVivo = false;
        enMovimiento = false;
        movement = Vector2.zero;
        recibiendoDanio = false;
        rb.linearVelocity = Vector2.zero;

        foreach (Collider2D enemyCollider in colliders)
        {
            enemyCollider.enabled = false;
        }

        GenerarDrop();
        Destroy(gameObject, tiempoDesaparicion);
    }

    private void GenerarDrop()
    {
        if (drops != null && drops.Length > 0)
        {
            bool tieneDropsConfigurados = false;

            foreach (DropConfig drop in drops)
            {
                if (drop != null && drop.prefab != null)
                {
                    tieneDropsConfigurados = true;
                    IntentarGenerarDrop(drop.prefab, drop.itemId, drop.chance);
                }
            }

            if (tieneDropsConfigurados)
            {
                return;
            }
        }

        IntentarGenerarDrop(dropPrefab, dropItemId, dropChance);
    }

    private void IntentarGenerarDrop(GameObject prefab, int itemId, float chance)
    {
        if (prefab == null || Random.value > chance)
        {
            return;
        }

        GameObject drop = Instantiate(prefab, transform.position, Quaternion.identity);
        int idDrop = ResolverItemIdDrop(drop, itemId);

        if (idDrop <= 0)
        {
            Debug.LogWarning($"El drop {drop.name} no tiene un itemId valido.");
            Destroy(drop);
            return;
        }

        ObjetoRecogible objetoRecogible = drop.GetComponent<ObjetoRecogible>();

        if (objetoRecogible != null)
        {
            objetoRecogible.itemId = idDrop;
        }

        Item item = drop.GetComponent<Item>();
        if (item != null)
        {
            item.id = idDrop;
        }

        EfectoRebote efectoRebote = drop.GetComponent<EfectoRebote>();
        if (efectoRebote != null)
        {
            efectoRebote.EmpezarRebote();
        }
    }

    private int ResolverItemIdDrop(GameObject drop, int itemIdConfigurado)
    {
        ObjetoRecogible objetoRecogible = drop.GetComponent<ObjetoRecogible>();
        int itemIdPrefab = 0;

        if (objetoRecogible != null && objetoRecogible.itemId > 0)
        {
            itemIdPrefab = objetoRecogible.itemId;
        }

        Item item = drop.GetComponent<Item>();
        if (itemIdPrefab <= 0 && item != null)
        {
            itemIdPrefab = item.id;
        }

        if (itemIdConfigurado > 0 && (itemIdConfigurado != 1 || itemIdPrefab <= 0))
        {
            return itemIdConfigurado;
        }

        return itemIdPrefab > 0 ? itemIdPrefab : itemIdConfigurado;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    public void ReproducirSonidoAtaque()
    {
        ReproducirSonido(sonidoAtaque);
    }

    public void ReproducirSonidoDanio()
    {
        ReproducirSonido(sonidoDanio);
    }

    public void ReproducirSonidoMuerte()
    {
        ReproducirSonido(sonidoMuerte);
    }
}
