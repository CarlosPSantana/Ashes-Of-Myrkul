using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class FullScreenScript : MonoBehaviour
{

    public Toggle toggle;


    public TMP_Dropdown resolucionesDropDown;
    private List<Resolution> resolucionesFiltradas = new List<Resolution>();
    

    void Start()
    {
        if (Screen.fullScreen)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }

        
        RevisarResolucion();
        
    }


    void Update()
    {

    }

    public void ActivarFull(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;

    }

    
    public void RevisarResolucion()
    {
        Resolution[] resoluciones = Screen.resolutions;
        resolucionesDropDown.ClearOptions();
        resolucionesFiltradas.Clear();
        List<string> opciones = new List<string>();
        HashSet<string> resolucionesUnicas = new HashSet<string>();

        for (int i = 0; i < resoluciones.Length; i++)
        {
            string claveResolucion = resoluciones[i].width + "x" + resoluciones[i].height;
            if (!resolucionesUnicas.Add(claveResolucion))
            {
                continue;
            }

            resolucionesFiltradas.Add(resoluciones[i]);
            string opcion = resoluciones[i].width + " x " + resoluciones[i].height;
            opciones.Add(opcion);
        }

        int resolucionActual = 0;
        for (int i = 0; i < resolucionesFiltradas.Count; i++)
        {
            if (resolucionesFiltradas[i].width == Screen.currentResolution.width &&
                resolucionesFiltradas[i].height == Screen.currentResolution.height)
            {
                resolucionActual = i;
                break;
            }
        }

        resolucionesDropDown.AddOptions(opciones);
        int resolucionGuardada = PlayerPrefs.GetInt("numeroResolucion", resolucionActual);
        resolucionGuardada = Mathf.Clamp(resolucionGuardada, 0, Mathf.Max(0, resolucionesFiltradas.Count - 1));
        resolucionesDropDown.value = resolucionGuardada;
        resolucionesDropDown.RefreshShownValue();
    }

    public void CambiarResolucion(int indiceResolucion)
    {
        if (indiceResolucion < 0 || indiceResolucion >= resolucionesFiltradas.Count)
        {
            return;
        }

        PlayerPrefs.SetInt("numeroResolucion", indiceResolucion);


        Resolution resolution = resolucionesFiltradas[indiceResolucion];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
  
}
