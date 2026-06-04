using TMPro;
using UnityEngine;

public enum TipoUsoItem
{
    Ninguno,
    CurarVida,
    RecuperarMana
}

public class Item : MonoBehaviour
{
    public int id;
    public bool esConsumible;
    public TipoUsoItem tipoUso;
    public int cantidadCuracion;
    public int cantidadMana;

    [Header("Efectos permanentes")]
    public bool otorgaDobleSalto;

    [Header("Stack")]
    public bool esStackeable = true;
    public int cantidad = 1;
    public int maxStack = 99;

    [Header("UI")]
    public TMP_Text cantidadText;

    private void Start()
    {
        ActualizarVisualCantidad();
    }

    public bool PuedeAgregarAlStack()
    {
        return esStackeable && cantidad < maxStack;
    }

    public void AgregarCantidad(int valor)
    {
        cantidad += valor;

        if (cantidad > maxStack)
        {
            cantidad = maxStack;
        }

        ActualizarVisualCantidad();
    }

    public void RestarCantidad(int valor)
    {
        cantidad -= valor;

        if (cantidad < 0)
        {
            cantidad = 0;
        }

        ActualizarVisualCantidad();
    }

    public void ActualizarVisualCantidad()
    {
        if (cantidadText == null)
        {
            return;
        }

        if (!esStackeable || cantidad <= 1)
        {
            cantidadText.text = "";
            return;
        }

        cantidadText.text = cantidad.ToString();
    }
}
