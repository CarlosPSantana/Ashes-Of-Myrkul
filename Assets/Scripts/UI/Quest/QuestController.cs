using System.Collections.Generic;
using UnityEngine;


public class QuestController : MonoBehaviour
{
    public static QuestController Instance { get; private set; }

    public List<QuestProgress> activeQuest = new();
    public List<Quest> questCatalog = new();

    private QuestUI questUI;
    private QuestDB questDB;
    private int partidaActual = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        questUI = FindFirstObjectByType<QuestUI>();

    }

    private void Start()
    {
        questDB = new QuestDB(ConexionDB.conexion);

        SistemaGuardado sistemaGuardado = FindFirstObjectByType<SistemaGuardado>();

        if (SistemaGuardado.partida_id != 0)
        {
            InicializarConPartida(SistemaGuardado.partida_id);
        }
    }

    public void InicializarConPartida(int partidaId)
    {
        partidaActual = partidaId;
        CargarQuestsDesdeBD();
    }

    public void AcceptQuest(Quest quest)
    {
        if (IsQuestActive(quest.questID))
        {
            Debug.Log("La quest está activa tontito."); 
            return;
        }
           

        if (quest == null)
        {
            Debug.Log("La quest no existe.");
            return;
        }

        if (partidaActual < 0)
        {
            Debug.Log("No hay partida actual. Problemón si sale esto.");
            return;
        }

        if (questDB.QuestAccepted(partidaActual, quest.questID))
        {
            Debug.Log("La quest está activa tontito.");
            return;
        }


        questDB.AcceptQuest(partidaActual, quest);

        QuestProgress progress = new QuestProgress(quest);
        activeQuest.Add(progress);

        if (questUI != null)
        {
            questUI.UpdateQuestUI();
        }

        Debug.Log("Quest aceptada: " + quest.questName);
    }

    public bool IsQuestActive(string questId)
    {
        return activeQuest.Exists(q => q.QuestId == questId);
    }

    public void RegistrarItemRecogido(int itemId, int cantidad = 1)
    {
        foreach (QuestProgress quest in activeQuest)
        {
            bool questCompleta = true;

            foreach (QuestObjective objective in quest.objectives)
            {
                if (objective.type == ObjectiveType.CollectItem &&
                    objective.objectiveID == itemId.ToString() &&
                    !objective.IsCompleted)
                {
                    objective.currentAmount += cantidad;

                    if (objective.currentAmount > objective.requiredAmount)
                    {
                        objective.currentAmount = objective.requiredAmount;
                    }

                    questDB.ActualizarObjetivo(
                        partidaActual,
                        quest.QuestId,
                        objective.objectiveID,
                        objective.currentAmount
                    );
                }

                if (!objective.IsCompleted)
                {
                    questCompleta = false;
                }
            }

            if (questCompleta)
            {
                questDB.ActualizarEstadoQuest(partidaActual, quest.QuestId, "Completed");
            }
        }

        if (questUI != null)
        {
            questUI.UpdateQuestUI();
        }
    }

    public void RegistrarEnemigoDerrotado(string enemyId, int cantidad = 1)
    {
        foreach (QuestProgress quest in activeQuest)
        {
            bool questCompleta = true;

            foreach (QuestObjective objective in quest.objectives)
            {
                if (objective.type == ObjectiveType.DefeatEnemy &&
                    objective.objectiveID == enemyId &&
                    !objective.IsCompleted)
                {
                    objective.currentAmount += cantidad;

                    if (objective.currentAmount > objective.requiredAmount)
                    {
                        objective.currentAmount = objective.requiredAmount;
                    }

                    questDB.ActualizarObjetivo(
                        partidaActual,
                        quest.QuestId,
                        objective.objectiveID,
                        objective.currentAmount
                    );
                }

                if (!objective.IsCompleted)
                {
                    questCompleta = false;
                }
            }

            if (questCompleta)
            {
                questDB.ActualizarEstadoQuest(partidaActual, quest.QuestId, "Completed");
            }
        }

        if (questUI != null)
        {
            questUI.UpdateQuestUI();
        }
    }

    public void MarcarQuestComoEntregada(string questId)
    {
        QuestProgress quest = activeQuest.Find(q => q.QuestId == questId);

        if (quest == null)
        {
            Debug.LogWarning("No se encontró la quest para marcar como entregada.");
            return;
        }

        questDB.ActualizarEstadoQuest(partidaActual, questId, "Delivered");
        activeQuest.Remove(quest);

        if (questUI != null)
        {
            questUI.UpdateQuestUI();
        }
    }

    public string ObtenerEstadoQuest(string questId)
    {
        if (partidaActual < 0)
        {
            return null;
        }

        return questDB.GetQuestState(partidaActual, questId);
    }

    public QuestProgress ObtenerQuestPorId(string questId)
    {
        return activeQuest.Find(q => q.QuestId == questId);
    }

    public bool EstaQuestCompletada(string questId)
    {
        QuestProgress quest = activeQuest.Find(q => q.QuestId == questId);
        return quest != null && quest.IsCompleted;
    }

    private void CargarQuestsDesdeBD()
    {
        activeQuest.Clear();

        foreach (Quest quest in questCatalog)
        {
            if (!questDB.QuestAccepted(partidaActual, quest.questID))
            {
                continue;
            }

            QuestProgress progress = new QuestProgress(quest);
            Dictionary<string, int> progresoGuardado =
                questDB.CargarProgresoObjetivos(partidaActual, quest.questID);

            foreach (QuestObjective objective in progress.objectives)
            {
                if (progresoGuardado.ContainsKey(objective.objectiveID))
                {
                    objective.currentAmount = progresoGuardado[objective.objectiveID];
                }
            }

            activeQuest.Add(progress);
        }

        if (questUI != null)
        {
            questUI.UpdateQuestUI();
        }

        Debug.Log($"PERO QUE MALDITA PARTIDA ES ESTA: {partidaActual}");
    }
}




