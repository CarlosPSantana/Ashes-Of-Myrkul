using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public Transform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        // Borrar misiones existentes.
        foreach(Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        if (QuestController.Instance == null)
        {
            return;
        }

        // Crea una nueva entrada de misiones.

        foreach(var quest in QuestController.Instance.activeQuest)
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            TMP_Text questNameText = entry.transform.Find("QuestNameText").GetComponent<TMP_Text>();
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            string estadoTexto = quest.IsCompleted ? " (Completada)" : "";
            questNameText.text = quest.quest.questName + estadoTexto;

            foreach (var objective in quest.objectives)
            {
                Debug.Log("Creando objetivo: " + objective.description);

                GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTextGO.GetComponent<TMP_Text>();

                if (objText == null)
                {
                    Debug.LogError("No se encontró texto en objectiveTextPrefab");
                    continue;
                }

                objText.text = $"{objective.description} ({objective.currentAmount}/{objective.requiredAmount})";
            }
        }
    }

}
