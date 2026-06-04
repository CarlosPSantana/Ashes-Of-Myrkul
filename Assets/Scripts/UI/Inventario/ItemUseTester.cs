using UnityEngine;

public class ItemUseTester : MonoBehaviour
{
    public int itemIdPocionVida = 0;

    private InventoryController inventoryController;
    private PlayerController2 player;

    void Start()
    {
        inventoryController = FindFirstObjectByType<InventoryController>();
        player = FindFirstObjectByType<PlayerController2>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            UsarPocionVida();
        }
    }

    void UsarPocionVida()
    {
        if (inventoryController == null || player == null)
        {
            return;
        }

        Item item = inventoryController.GetFirstItemById(itemIdPocionVida);

        if (item == null)
        {
            Debug.Log("No tienes esa poción.");
            return;
        }

        if (!item.esConsumible)
        {
            Debug.Log("Ese item no es consumible.");
            return;
        }

        if (item.tipoUso == TipoUsoItem.CurarVida)
        {
            player.Curar(item.cantidadCuracion);
            inventoryController.RemoveOneItemById(itemIdPocionVida);
            Debug.Log("Poción usada.");
        }
    }
}
