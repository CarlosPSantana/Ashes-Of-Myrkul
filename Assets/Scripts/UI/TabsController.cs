using System;
using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
    public Image[] tabImagenes;
    public GameObject[] paginas;
    void Start()
    {
        ActivarTab(0);
    }

    public void ActivarTab(int tabNo)
    {
        for (int i = 0; i < paginas.Length; i++)
        {
            paginas[i].SetActive(false);
            tabImagenes[i].color = Color.grey;
        }

        paginas[tabNo].SetActive(true);
        tabImagenes[tabNo].color = Color.white;
    }
}
