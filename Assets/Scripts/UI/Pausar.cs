using UnityEngine;

public class Pausar : MonoBehaviour
{
    public static bool estaPausado;
    public static bool EstaPausado { get => estaPausado; set => estaPausado = value; }

    public static void PonerPausa(bool pausa)
    {
        estaPausado = pausa;
    }
}
