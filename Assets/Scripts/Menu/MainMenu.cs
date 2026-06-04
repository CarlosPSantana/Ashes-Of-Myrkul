using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public SistemaGuardado sistemaGuardado;
    public string escenaInicial = "Bosque";
    public GameObject panelCargarPartida;

    public void NuevaPartida()
    {
        
        sistemaGuardado.IniciarNuevaPartida(escenaInicial);
        SceneManager.LoadScene(escenaInicial);
    }

    public void Continuar()
    {
        sistemaGuardado.CargarUltimaPartida();

        if (SistemaGuardado.datosCargados != null)
        {
            SceneManager.LoadScene(SistemaGuardado.datosCargados.nombreEscena);
        }
        else
        {
            Debug.Log("No hay partidas guardadas.");
        }
    }

    public void AbrirCargarPartida()
    {
        panelCargarPartida.SetActive(true);
    }

    public void CerrarCargarPartida()
    {
        panelCargarPartida.SetActive(false);
    }

    public void CargarSlot1()
    {
        sistemaGuardado.CargarPartidaPorSlot(1);

        if (SistemaGuardado.datosCargados != null)
        {
            SceneManager.LoadScene(SistemaGuardado.datosCargados.nombreEscena);
        }
    }

    public void CargarSlot2()
    {
        sistemaGuardado.CargarPartidaPorSlot(2);

        if (SistemaGuardado.datosCargados != null)
        {
            SceneManager.LoadScene(SistemaGuardado.datosCargados.nombreEscena);
        }
    }

    public void salir()
    {
        Application.Quit();
    }
}