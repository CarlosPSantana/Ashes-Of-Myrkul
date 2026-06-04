using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraVidaController : MonoBehaviour
{

    public Image rellenoVida;
    private PlayerController2 playerController;
    private float vidaMax;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GameObject.Find("Juan").GetComponent<PlayerController2>();
        vidaMax = playerController.vida;
    }

    // Update is called once per frame
    void Update()
    {
        rellenoVida.fillAmount = playerController.vida / vidaMax;
    }
}
