using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DiccionarioItems : MonoBehaviour
{
    public List<Item> itemPrefabs;
    private Dictionary<int, GameObject> diccionarioItems;

    private void Awake()
    {
        diccionarioItems = new Dictionary<int, GameObject>();

        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            if (itemPrefabs[i] != null)
            {
                itemPrefabs[i].id = i + 1;
            }
        }

        foreach (Item item in itemPrefabs)
        {
            diccionarioItems[item.id] = item.gameObject;
        }
    }

    public GameObject GetItemPrefabs(int itemId)
    {
        diccionarioItems.TryGetValue(itemId, out GameObject prefab);

        if (prefab == null)
        {
            Debug.Log($"El item con id {itemId} no se encuentra en el diccionario");
        }

        return prefab;

    }
}
