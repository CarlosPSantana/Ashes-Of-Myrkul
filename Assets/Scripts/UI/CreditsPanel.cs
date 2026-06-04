using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsPanel : MonoBehaviour
{
    public GameObject panelCreditos;
    public TextMeshProUGUI tituloVictoria;
    public TextMeshProUGUI textoCreditos;
    public Button botonMenu;
    public Button botonSalir;

    void Start()
    {
        if (panelCreditos != null)
        {
            panelCreditos.SetActive(false);
        }

        if (botonMenu != null)
        {
            botonMenu.onClick.AddListener(VolverAlMenu);
        }

        if (botonSalir != null)
        {
            botonSalir.onClick.AddListener(SalirDelJuego);
        }
    }

    public void MostrarCreditos(float retraso)
    {
        StartCoroutine(MostrarTrasRetraso(retraso));
    }

    private IEnumerator MostrarTrasRetraso(float retraso)
    {
        yield return new WaitForSeconds(retraso);

        if (panelCreditos != null)
        {
            panelCreditos.SetActive(true);
        }

        if (tituloVictoria != null)
        {
            tituloVictoria.text = "Jefe derrotado";
        }

        if (textoCreditos != null)
        {
            textoCreditos.text =
                "Gracias por jugar\n\n" +
                "Proyecto desarrollado en Unity\n" +
                "Programacion, diseno e integracion:\n" +
                "Tu nombre\n\n" +
                "Apoyo tecnico y documentacion:\n" +
                "OpenAI / IA asistida";
        }

        Time.timeScale = 0f;
    }

    private void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    private void SalirDelJuego()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}

