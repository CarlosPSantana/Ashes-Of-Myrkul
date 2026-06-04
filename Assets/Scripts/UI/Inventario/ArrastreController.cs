using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArrastreController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;
    public void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            transform.SetParent(canvas.transform);
        }

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Slot slotOriginal = originalParent.GetComponent<Slot>();
        GameObject objetoDebajo = eventData.pointerCurrentRaycast.gameObject;

        if (objetoDebajo == null)
        {
            objetoDebajo = eventData.pointerEnter;
        }

        HotbarSlotUI hotbarSlot = BuscarHotbarSlot(eventData, objetoDebajo);

        if (hotbarSlot != null)
        {
            if (hotbarSlot.objetoActual != null && slotOriginal != null)
            {
                hotbarSlot.objetoActual.transform.SetParent(slotOriginal.transform);
                RectTransform rectHotbarAnterior = hotbarSlot.objetoActual.GetComponent<RectTransform>();
                rectHotbarAnterior.anchoredPosition = Vector2.zero;
                rectHotbarAnterior.localScale = Vector3.one;
                slotOriginal.objeoActual = hotbarSlot.objetoActual;
            }
            else if (slotOriginal != null)
            {
                slotOriginal.objeoActual = null;
            }

            transform.SetParent(hotbarSlot.transform);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            hotbarSlot.ColocarObjeto(gameObject);
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
            return;
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Slot soltarSlot = objetoDebajo?.GetComponent<Slot>();
        if (soltarSlot == null)
        {
            GameObject soltarItem = objetoDebajo;
            if (soltarItem != null)
            {
                soltarSlot = soltarItem.GetComponentInParent<Slot>();
            }
        }
        if (soltarSlot != null)
        {
            if (soltarSlot.objeoActual != null)
            {
                soltarSlot.objeoActual.transform.SetParent(slotOriginal.transform);
                slotOriginal.objeoActual = soltarSlot.objeoActual;
                RectTransform rectItemIntercambiado = soltarSlot.objeoActual.GetComponent<RectTransform>();
                rectItemIntercambiado.anchoredPosition = Vector2.zero;
                rectItemIntercambiado.localScale = Vector3.one;

            }
            else
            {
                slotOriginal.objeoActual = null;
            }

            transform.SetParent(soltarSlot.transform);
            soltarSlot.objeoActual = gameObject;
            GetComponent<RectTransform>().localScale = Vector3.one;
        }
        else
        {
            transform.SetParent(originalParent);
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    private HotbarSlotUI BuscarHotbarSlot(PointerEventData eventData, GameObject objetoDebajo)
    {
        HotbarSlotUI hotbarSlot = objetoDebajo?.GetComponentInParent<HotbarSlotUI>();

        if (hotbarSlot != null)
        {
            return hotbarSlot;
        }

        if (EventSystem.current == null)
        {
            return null;
        }

        List<RaycastResult> resultados = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, resultados);

        foreach (RaycastResult resultado in resultados)
        {
            if (resultado.gameObject == null || resultado.gameObject.transform.IsChildOf(transform))
            {
                continue;
            }

            hotbarSlot = resultado.gameObject.GetComponentInParent<HotbarSlotUI>();

            if (hotbarSlot != null)
            {
                return hotbarSlot;
            }
        }

        return null;
    }
}
