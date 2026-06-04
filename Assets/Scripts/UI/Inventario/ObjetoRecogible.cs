using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ObjetoRecogible : WorldPersistentObject
{
    public int itemId;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoRecoger;

    private InventoryController inventoryController;
    private bool recogido;

    protected override void Awake()
    {
        base.Awake();

        inventoryController = FindFirstObjectByType<InventoryController>();
        itemCollider = GetComponent<Collider2D>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        PrepararComoObjetoDeMundo();

        Debug.Log($"Pickup {name} UniqueId = {UniqueId}");


        string estado = ObtenerEstadoGuardado();

        if (estado == "Collected")
        {
            recogido = true;
            Destroy(gameObject);
    }
    }
    private bool puedeRecogerse = true;
    private Collider2D itemCollider;

    private void IntentarRecoger(Collider2D other)
    {
        if (recogido || !puedeRecogerse)
        {
            return;
        }

        PlayerController2 player = other.GetComponent<PlayerController2>();

        if (recogido)
        {
            return;
        }

        if (player == null)
        {
            player = other.GetComponentInParent<PlayerController2>();
        }

        if (player == null)
        {
            return;
        }

        if (inventoryController == null)
        {
            inventoryController = FindFirstObjectByType<InventoryController>();
        }

        if (inventoryController == null)
        {
            Debug.LogWarning("No se ha encontrado InventoryController en la escena.");
            return;
        }

        int idParaRecoger = ObtenerItemId();

        if (idParaRecoger <= 0)
        {
            Debug.LogWarning($"El objeto recogible {name} no tiene un itemId valido.");
            return;
        }

        if (inventoryController.AddItemById(idParaRecoger))
        {
            recogido = true;
            GuardarEstado("Pickup", "Collected");

            if (QuestController.Instance != null)
            {
                QuestController.Instance.RegistrarItemRecogido(idParaRecoger, 1);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IntentarRecoger(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        IntentarRecoger(other);
    }

    private int ObtenerItemId()
    {
        if (itemId > 0)
        {
            return itemId;
        }

        Item item = GetComponent<Item>();
        return item != null ? item.id : 0;
    }

    private void PrepararComoObjetoDeMundo()
    {
        if (GetComponentInParent<Canvas>() != null)
        {
            return;
        }

        int itemLayer = LayerMask.NameToLayer("item");
        if (itemLayer >= 0)
        {
            gameObject.layer = itemLayer;
        }

        if (itemCollider != null)
        {
            itemCollider.isTrigger = true;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sortingLayerName = "items";

            if (spriteRenderer.sortingOrder < 10)
            {
                spriteRenderer.sortingOrder = 10;
            }
        }
    }

    public void BloquearRecogida()
    {
        puedeRecogerse = false;

        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }
    }

    public void PermitirRecogida()
    {
        puedeRecogerse = true;

        if (itemCollider != null)
        {
            itemCollider.enabled = true;
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

    public void ReproducirSonidoRecoger()
    {
        ReproducirSonido(sonidoRecoger);
    }
}
