using System.Collections.Generic;
using UnityEngine;

// Atributo que indica que la clase puede ser serializada.
[System.Serializable]

// Clase denominada SaveData, La cual solo existe para guardar información útil y más accesible.
public class SaveData
{
    public string nombreEscena;
    public float velocidad;
    public int vida;
    public int mana;
    public int manaMaxima;
    public float fuerzaSalto;
    public float fuerzaRebote;
    public float posX;
    public float posY;
    public float posZ;

    public List<InventarioManagement> inventarioManagement;
}
