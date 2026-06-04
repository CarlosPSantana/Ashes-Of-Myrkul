using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonesMenu : MonoBehaviour
{
    public void MenuPrincipal()
    {
        Pausar.PonerPausa(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}

