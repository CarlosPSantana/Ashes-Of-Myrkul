using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausarJuego : MonoBehaviour
{
    public bool pausa = false;
    public GameObject menuPausa;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausa)
            {
                Seguir();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        menuPausa.SetActive(true);
        Time.timeScale = 0;
        pausa = true;
    }

    public void Seguir()
    {
        menuPausa.SetActive(false);
        Time.timeScale = 1;
        pausa = false;
    }

    public void Salir()
    {
        SceneManager.LoadScene("Menu");
    }
}
