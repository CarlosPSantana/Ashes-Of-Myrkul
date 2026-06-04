using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public Image rellenoVida;

    private void Start()
    {
        Mostrar(false);
    }

    public void ActualizarVida(int vidaActual, int vidaMaxima)
    {
        if (rellenoVida == null || vidaMaxima <= 0)
        {
            return;
        }

        rellenoVida.fillAmount = (float)vidaActual / vidaMaxima;
    }

    public void Mostrar(bool mostrar)
    {
        gameObject.SetActive(mostrar);
    }
}