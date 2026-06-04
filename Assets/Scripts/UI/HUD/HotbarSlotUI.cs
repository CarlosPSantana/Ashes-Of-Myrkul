using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotbarSlotUI : MonoBehaviour, IDropHandler
{
    public Image iconoItem;
    public TMP_Text cantidadText;
    public float escalaItem = 0.75f;

    public int itemId = -1;
    public GameObject objetoActual;
    private Sprite iconoGuardado;

    public bool EstaVacio()
    {
        return itemId == -1 && objetoActual == null;
    }

    public void AsignarItem(int nuevoItemId, Sprite nuevoIcono)
    {
        itemId = nuevoItemId;
        iconoGuardado = nuevoIcono;

        if (iconoItem != null)
        {
            iconoItem.sprite = iconoGuardado;
            iconoItem.enabled = true;
        }
    }

    public void Limpiar()
    {
        itemId = -1;
        iconoGuardado = null;
        objetoActual = null;

        if (iconoItem != null)
        {
            iconoItem.sprite = null;
            iconoItem.enabled = false;
        }

        if (cantidadText != null)
        {
            cantidadText.text = "";
        }
    }

    public void ActualizarCantidad(int cantidad)
    {
        if (cantidadText == null)
        {
            return;
        }

        if (EstaVacio() || cantidad <= 0)
        {
            cantidadText.text = "";
            return;
        }

        cantidadText.text = cantidad.ToString();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject objetoArrastrado = eventData.pointerDrag;

        if (objetoArrastrado == null)
        {
            return;
        }

        Item item = objetoArrastrado.GetComponent<Item>();
        Image imagen = objetoArrastrado.GetComponent<Image>();

        if (item == null || imagen == null)
        {
            return;
        }

        AsignarItem(item.id, imagen.sprite);
    }

    public void ColocarObjeto(GameObject itemGO)
    {
        if (itemGO == null)
        {
            return;
        }

        Item item = itemGO.GetComponent<Item>();
        Image imagen = itemGO.GetComponent<Image>();

        if (item == null || imagen == null)
        {
            return;
        }

        objetoActual = itemGO;
        AsignarItem(item.id, imagen.sprite);

        RectTransform rectTransform = itemGO.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one * escalaItem;
        }
    }
}
