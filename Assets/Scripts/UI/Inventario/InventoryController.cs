using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private DiccionarioItems diccionarioItems;

    public GameObject panelInventario;
    public GameObject slotPrefab;
    public int numeroSlot;
    public GameObject[] itemPrefabs;

    void Start()
    {
        diccionarioItems = FindFirstObjectByType<DiccionarioItems>();
        AsegurarSlots();

        if (SistemaGuardado.datosCargados != null &&
            SistemaGuardado.datosCargados.inventarioManagement != null)
        {
            SetInventoryItems(SistemaGuardado.datosCargados.inventarioManagement);
        }
    }

    public List<InventarioManagement> GetInventoryItems()
    {
        List<InventarioManagement> invData = new List<InventarioManagement>();

        foreach (Transform slotTransform in panelInventario.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.objeoActual != null)
            {
                Item item = slot.objeoActual.GetComponent<Item>();

                if (item != null)
                {
                    invData.Add(new InventarioManagement
                    {
                        itemId = item.id,
                        slotIndex = slotTransform.GetSiblingIndex(),
                        cantidad = item.cantidad
                    });
                }
            }
        }

        return invData;
    }

    public void SetInventoryItems(List<InventarioManagement> invData)
    {
        AsegurarSlots();
        LimpiarItemsDeSlots();

        foreach (InventarioManagement data in invData)
        {
            if (data.slotIndex >= 0 && data.slotIndex < numeroSlot)
            {
                Slot slot = panelInventario.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                CrearItemEnSlot(slot, data.itemId, data.cantidad);
            }
        }
    }

    public bool AddItemById(int itemId, int cantidad = 1)
    {
        AsegurarSlots();

        int cantidadRestante = cantidad;

        foreach (Transform slotTransform in panelInventario.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.objeoActual != null)
            {
                Item item = slot.objeoActual.GetComponent<Item>();

                if (item != null && item.id == itemId && item.PuedeAgregarAlStack())
                {
                    int espacioDisponible = item.maxStack - item.cantidad;
                    int agregar = Mathf.Min(espacioDisponible, cantidadRestante);

                    item.AgregarCantidad(agregar);
                    cantidadRestante -= agregar;

                    if (cantidadRestante <= 0)
                    {
                        return true;
                    }
                }
            }
        }

        foreach (Transform slotTransform in panelInventario.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.objeoActual == null)
            {
                int crearCantidad = cantidadRestante;
                bool creado = CrearItemEnSlot(slot, itemId, crearCantidad);

                if (creado)
                {
                    return true;
                }
            }
        }

        Debug.Log("Inventario lleno");
        return false;
    }

    public int CountItemById(int itemId)
    {
        int total = 0;

        foreach(Transform slotTransform in panelInventario.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.objeoActual != null)
            {
                Item item = slot.objeoActual.GetComponent<Item>();

                if (item != null && item.id == itemId)
                {
                    total += item.cantidad;
                }
            }
        }

        return total;
    }

    public bool TieneItemDobleSalto()
    {
        foreach (Transform slotTransform in panelInventario.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.objeoActual != null)
            {
                Item item = slot.objeoActual.GetComponent<Item>();

                if (item != null && item.otorgaDobleSalto)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool RemoveItemById(int itemId, int cantidad)
    {
        int restante = cantidad;

        foreach (Transform slotTransform in panelInventario.transform)
        {
            if (restante <= 0)
            {
                break;
            }

            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.objeoActual != null)
            {
                Item item = slot.objeoActual.GetComponent<Item>();

                if (item != null && item.id == itemId)
                {
                    if (item.cantidad > restante)
                    {
                        item.RestarCantidad(restante);
                        restante = 0;
                    }
                    else
                    {
                        restante -= item.cantidad;
                        Destroy(slot.objeoActual);
                        slot.objeoActual = null;
                    }
                }
            }
        }

        return restante == 0;
    }

    public bool RemoveOneItemById(int itemId)
    {
        return RemoveItemById(itemId, 1);
    }

    public Item GetFirstItemById(int itemId)
    {
        foreach (Transform slotTransform in panelInventario.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.objeoActual != null)
            {
                Item item = slot.objeoActual.GetComponent<Item>();

                if (item != null && item.id == itemId)
                {
                    return item;
                }
            }
        }

        return null;
    }

    private bool CrearItemEnSlot(Slot slot, int itemId, int cantidad)
    {
        if (slot == null)
        {
            return false;
        }

        GameObject itemPrefab = diccionarioItems.GetItemPrefabs(itemId);

        if (itemPrefab == null)
        {
            return false;
        }

        GameObject itemGO = Instantiate(itemPrefab, slot.transform);
        RectTransform itemRect = itemGO.GetComponent<RectTransform>();
        Item item = itemGO.GetComponent<Item>();

        if (itemRect != null)
        {
            itemRect.anchoredPosition = Vector2.zero;
            itemRect.localScale = Vector3.one;
        }

        if (item != null)
        {
            item.cantidad = cantidad;
            item.ActualizarVisualCantidad();
        }

        slot.objeoActual = itemGO;
        return true;
    }

    private void AsegurarSlots()
    {
        while (panelInventario.transform.childCount < numeroSlot)
        {
            Instantiate(slotPrefab, panelInventario.transform);
        }
    }

    private void LimpiarItemsDeSlots()
    {
        foreach (Transform slotTransform in panelInventario.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot == null)
            {
                continue;
            }

            if (slot.objeoActual != null)
            {
                Destroy(slot.objeoActual);
                slot.objeoActual = null;
            }
        }
    }
}
