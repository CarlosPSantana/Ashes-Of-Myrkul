using UnityEngine;

public class Chest : WorldPersistentObject, InterfazInteractuable
{
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }
    public GameObject itemPrefabs;
    public Sprite openedSprite;
    public Animator animator;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoAbrir;

    protected override void Awake()
    {
        base.Awake();
        ChestID = UniqueId;
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        Debug.Log($"Chest {name} UniqueId = {UniqueId}");

        string estado = ObtenerEstadoGuardado();

        if (estado == "Opened")
        {
            MarcarAbierto(true);
        }
    }


    public bool PuedeInteractuar()
    {
        return !IsOpened;
    }

    public void Interactuar()
    {
        if (!PuedeInteractuar())
        {
            return;
        }

        AbrirCofre();
    }

    private void AbrirCofre()
    {
        MarcarAbierto(true);
        GuardarEstado("Chest", "Opened");
        animator.SetBool("abierto", true);

        if (itemPrefabs != null)
        {
            GameObject objDropeado = Instantiate(itemPrefabs, transform.position, Quaternion.identity);

            ObjetoRecogible objetoRecogible = objDropeado.GetComponent<ObjetoRecogible>();
            if (objetoRecogible != null)
            {
                objetoRecogible.BloquearRecogida();
            }

            EfectoRebote efectoRebote = objDropeado.GetComponent<EfectoRebote>();
            if (efectoRebote != null)
            {
                efectoRebote.EmpezarRebote();
            }
            else if (objetoRecogible != null)
            {
                objetoRecogible.PermitirRecogida();
            }
        }
    }

    public void ReproducirSonidoAbrir()
    {
        ReproducirSonido(sonidoAbrir);
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    public void MarcarAbierto(bool abierto)
    {
        IsOpened = abierto;

        if (IsOpened)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = openedSprite;
            }
        }
    }
}
