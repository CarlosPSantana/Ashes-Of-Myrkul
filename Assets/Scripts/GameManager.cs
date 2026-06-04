using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI gameOverText2;
    public Button retry;
    public Button salir;

    private bool gameOverActivo = false;
   
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }

    }
    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (retry != null) retry.onClick.AddListener(ReiniciarEscena);
        if (salir != null) salir.onClick.AddListener(IrMenu);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOverActivo)
        {
            if (Input.GetKeyDown(KeyCode.R)) ReiniciarEscena();

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M)) IrMenu();
        }
    }

    public void GameOver()
    {
        if (gameOverActivo) return;

        gameOverActivo = true;

        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverText != null)
        {
            gameOverText.text = "YOU DIED";
        }
        if (gameOverText2 != null)
        {
            gameOverText2.text = "R para reiniciar \nESC para salir";
        }
    }

    private void IrMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    private void ReiniciarEscena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
