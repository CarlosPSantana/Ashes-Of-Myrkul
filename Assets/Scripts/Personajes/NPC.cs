using System.Collections;
using UnityEngine;

public class NPC : DialogueInteractuableBase
{
    public int rewardItemAmount = 1;
    public int rewardItemId;
    public NPCDialogue dialogoCompletada;
    public NPCDialogue dialogoEnProgreso;

    [Header("Dialogos por estado")]
    public NPCDialogue dialogoSinAceptar;

    void Start()
    {
        dialogueUI = DialogueController.Instance;
    }
    public NPCDialogue dialogoEntregada;

    void ProcesarEstadoQuest()
    {
        if (questToGive == null || QuestController.Instance == null)
        {
            return;
        }

        string estadoQuest = QuestController.Instance.ObtenerEstadoQuest(questToGive.questID);

        if (estadoQuest == "Delivered")
        {
            recompensaEntregada = true;
            return;
        }

        if (QuestController.Instance.EstaQuestCompletada(questToGive.questID) && !recompensaEntregada)
        {
            if (ConsumirObjetosDeMision())
            {
                EntregarRecompensa();
                QuestController.Instance.MarcarQuestComoEntregada(questToGive.questID);
                recompensaEntregada = true;
            }
        }
    }

    void EntregarRecompensa()
    {
        InventoryController inventory = FindFirstObjectByType<InventoryController>();

        if (inventory == null)
        {
            Debug.LogWarning("No se encontró InventoryController para dar recompensa.");
            return;
        }

        inventory.AddItemById(rewardItemId, rewardItemAmount);
        Debug.Log("Recompensa entregada.");
    }
    private bool recompensaEntregada;
    private DialogueController dialogueUI;

    void DisplayChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            bool aceptaQuest = choice.aceptarQuest != null &&
                               choice.aceptarQuest.Length > i &&
                               choice.aceptarQuest[i];

            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOptions(nextIndex, aceptaQuest));
        }
    }

    void ChooseOptions(int nextIndex, bool aceptaQuest)
    {
        if (aceptaQuest && questToGive != null && QuestController.Instance != null)
        {
            if (!QuestController.Instance.IsQuestActive(questToGive.questID))
            {
                QuestController.Instance.AcceptQuest(questToGive);
            }
        }

        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    [Header("Quest")]
    public Quest questToGive;

    bool ConsumirObjetosDeMision()
    {
        if (questToGive == null)
        {
            return true;
        }

        InventoryController inventory = FindFirstObjectByType<InventoryController>();

        if (inventory == null)
        {
            Debug.LogWarning("No se encontró InventoryController.");
            return false;
        }

        foreach (QuestObjective objective in questToGive.objectives)
        {
            if (objective.type == ObjectiveType.CollectItem)
            {
                int itemId = int.Parse(objective.objectiveID);

                if (inventory.CountItemById(itemId) < objective.requiredAmount)
                {
                    Debug.LogWarning("No hay suficientes objetos para entregar la misión.");
                    return false;
                }
            }
        }

        foreach (QuestObjective objective in questToGive.objectives)
        {
            if (objective.type == ObjectiveType.CollectItem)
            {
                int itemId = int.Parse(objective.objectiveID);
                inventory.RemoveItemById(itemId, objective.requiredAmount);
            }
        }

        return true;
    }

    NPCDialogue ElegirDialogoSegunEstado()
    {
        if (questToGive == null || QuestController.Instance == null)
        {
            return dialogoSinAceptar;
        }

        string estadoQuest = QuestController.Instance.ObtenerEstadoQuest(questToGive.questID);

        if (estadoQuest == "Delivered")
        {
            return dialogoEntregada;
        }

        if (!QuestController.Instance.IsQuestActive(questToGive.questID))
        {
            return dialogoSinAceptar;
        }

        if (QuestController.Instance.EstaQuestCompletada(questToGive.questID))
        {
            return dialogoCompletada;
        }

        return dialogoEnProgreso;
    }

    public override void Interactuar()
    {
        if (Pausar.estaPausado && !isDialogueActive)
        {
            return;
        }

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }
    public override void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        Pausar.PonerPausa(false);


    }

    protected override IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.autoProgressLine.Length > dialogueIndex && dialogueData.autoProgressLine[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    protected override void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }

        dialogueUI.ClearChoices();

        if(dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        foreach(DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if(dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }

        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    protected override void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        dialogueData = ElegirDialogoSegunEstado();

        if (dialogueData == null)
        {
            Debug.LogWarning("No hay diálogo asignado para este estado.");
            isDialogueActive = false;
            return;
        }

        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);
        Pausar.PonerPausa(true);

        ProcesarEstadoQuest();
        DisplayCurrentLine();
    }
}

