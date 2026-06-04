using UnityEngine;

public class HotbarController : MonoBehaviour
{
    public HotbarSlotUI[] slots;

    private InventoryController inventoryController;
    private PlayerController2 player;

    void Start()
    {
        inventoryController = FindFirstObjectByType<InventoryController>();
        player = FindFirstObjectByType<PlayerController2>();
        ActualizarCantidades();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UsarSlot(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UsarSlot(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UsarSlot(2);
        }

        ActualizarCantidades();
    }

    public void UsarSlot(int indice)
    {
        if (slots == null || indice < 0 || indice >= slots.Length)
        {
            return;
        }

        HotbarSlotUI slot = slots[indice];

        if (slot == null || slot.EstaVacio())
        {
            return;
        }

        if (inventoryController == null)
        {
            inventoryController = FindFirstObjectByType<InventoryController>();
        }

        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController2>();
        }

        if (inventoryController == null || player == null)
        {
            return;
        }

        Item item = null;

        if (slot.objetoActual != null)
        {
            item = slot.objetoActual.GetComponent<Item>();
        }

        if (item == null)
        {
            item = inventoryController.GetFirstItemById(slot.itemId);
        }

        if (item == null)
        {
            slot.Limpiar();
            return;
        }

        if (!item.esConsumible)
        {
            return;
        }

        switch (item.tipoUso)
        {
            case TipoUsoItem.CurarVida:
                player.Curar(item.cantidadCuracion);
                break;

            case TipoUsoItem.RecuperarMana:
                player.RecuperarMana(item.cantidadMana);
                break;

            default:
                return;
        }

        if (slot.objetoActual != null)
        {
            Destroy(slot.objetoActual);
            slot.objetoActual = null;
        }
        else
        {
            inventoryController.RemoveOneItemById(item.id);
        }

        int cantidadRestante = inventoryController.CountItemById(item.id);

        if (slot.objetoActual == null && cantidadRestante <= 0)
        {
            slot.Limpiar();
        }

        ActualizarCantidades();
    }

    public void ActualizarCantidades()
    {
        if (slots == null)
        {
            return;
        }

        if (inventoryController == null)
        {
            inventoryController = FindFirstObjectByType<InventoryController>();
        }

        if (inventoryController == null)
        {
            return;
        }

        foreach (HotbarSlotUI slot in slots)
        {
            if (slot == null)
            {
                continue;
            }

            if (slot.EstaVacio())
            {
                slot.ActualizarCantidad(0);
            }
            else
            {
                int cantidad = slot.objetoActual != null ? 1 : inventoryController.CountItemById(slot.itemId);
                slot.ActualizarCantidad(cantidad);

                if (slot.objetoActual == null && cantidad <= 0)
                {
                    slot.Limpiar();
                }
            }
        }
    }
}
