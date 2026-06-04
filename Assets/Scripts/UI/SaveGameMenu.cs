using TMPro;
using UnityEngine;

public class SaveGameMenu : MonoBehaviour
{
    public SistemaGuardado sistemaGuardado;
    public GameObject panelGuardarSlots;

    public TMP_Text slot1Texto;
    public TMP_Text slot2Texto;

    void Start()
    {
        if (panelGuardarSlots != null)
        {
            panelGuardarSlots.SetActive(false);
        }
    }

    public void Guardar()
    {
        if (SistemaGuardado.slot_actual != 0)
        {
            sistemaGuardado.GuardarPartida();
            RefrescarSlots();
        }
        else
        {
            if (panelGuardarSlots != null)
            {
                RefrescarSlots();
                panelGuardarSlots.SetActive(true);
            }
            else
            {
                Debug.LogWarning("No hay panelGuardarSlots asignado.");
            }
        }
    }

    public void GuardarEnSlot1()
    {
        sistemaGuardado.GuardarPartidaEnSlot(1);
        RefrescarSlots();
    }

    public void GuardarEnSlot2()
    {
        sistemaGuardado.GuardarPartidaEnSlot(2);
        RefrescarSlots();
    }

    public void CerrarPanelGuardar()
    {
        if (panelGuardarSlots != null)
        {
            panelGuardarSlots.SetActive(false);
        }
    }

    public void RefrescarSlots()
    {
        if (sistemaGuardado == null)
        {
            return;
        }

        SlotInfo slot1 = sistemaGuardado.ObtenerInfoSlot(1);
        SlotInfo slot2 = sistemaGuardado.ObtenerInfoSlot(2);

        if (slot1Texto != null)
        {
            slot1Texto.text = FormatearTexto(slot1);
        }

        if (slot2Texto != null)
        {
            slot2Texto.text = FormatearTexto(slot2);
        }
    }

    string FormatearTexto(SlotInfo info)
    {
        if (!info.ocupado)
        {
            return "Slot " + info.slot + "\nVacio";
        }

        return "Slot " + info.slot +
               "\n\nEscena: " + info.escena +
               "\n\nVida: " + info.vida +
               "\n\nFecha: " + info.fecha;
    }
}

