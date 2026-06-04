using TMPro;
using UnityEngine;

public class SaveSlotMenu : MonoBehaviour
{
    public SistemaGuardado sistemaGuardado;
    public TMP_Text slot1Texto;
    public TMP_Text slot2Texto;

    private void OnEnable()
    {
        RefrescarSlots();
    }

    public void RefrescarSlots()
    {
        SlotInfo slot1 = sistemaGuardado.ObtenerInfoSlot(1);
        SlotInfo slot2 = sistemaGuardado.ObtenerInfoSlot(2);

        slot1Texto.text = FormatearTexto(slot1);
        slot2Texto.text = FormatearTexto(slot2);
    }

    string FormatearTexto(SlotInfo info)
    {
        if (!info.ocupado)
        {
            return "Slot " + info.slot + "\nVacío";
        }

        return "Slot " + info.slot +
               "\n\nEscena: " + info.escena +
               "\n\nVida: " + info.vida +
               "\n\nFecha: " + info.fecha;
    }
}

