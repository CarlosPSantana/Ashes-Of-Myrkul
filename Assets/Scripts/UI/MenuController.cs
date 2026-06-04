using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;

    void Start()
    {
        menuCanvas.SetActive(false);
        Pausar.PonerPausa(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool abrirMenu = !menuCanvas.activeSelf;
            menuCanvas.SetActive(abrirMenu);
            Pausar.PonerPausa(abrirMenu);

            if (abrirMenu)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }
}
