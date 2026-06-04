using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Image barraVida;
    public Image barraMana;

    public void ActualizarVida(int vidaActual, int vidaMaxima)
    {
        if (barraVida == null || vidaMaxima <= 0)
        {
            return;
        }

        barraVida.fillAmount = (float)vidaActual / vidaMaxima;
    }

    public void ActualizarMana(int manaActual, int manaMaxima)
    {
        if (barraMana == null || manaMaxima <= 0)
        {
            return;
        }

        barraMana.fillAmount = (float)manaActual / manaMaxima;
    }
}
