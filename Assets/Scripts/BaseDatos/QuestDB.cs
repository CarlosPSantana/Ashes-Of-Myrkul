using Mono.Data.Sqlite;
using System.Collections.Generic;
using UnityEngine;

public class QuestDB
{
    private string conexion;

    public QuestDB(string conexion)
    {
        this.conexion = conexion;
    }

    public bool QuestAccepted(int partidaId, string questId)
    {
        using (SqliteConnection connection = new SqliteConnection(conexion))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"SELECT COUNT(*) FROM QuestPartida
                                        WHERE partida_id = @partidaId AND quest_id = @questId;";

                command.Parameters.AddWithValue("@partidaId", partidaId);
                command.Parameters.AddWithValue("@questId", questId);

                long total = (long) command.ExecuteScalar();
                return total > 0;
            }
        }
    }

    public void AcceptQuest(int partidaId, Quest quest)
    {
        using (SqliteConnection connection = new SqliteConnection(conexion))
        {
            connection.Open();

            using (SqliteTransaction transaction = connection.BeginTransaction())
            {
                
                long questPartidaId;


                using (SqliteCommand command = new SqliteCommand(connection))
                {
                    command.Transaction = transaction;

                    command.CommandText = @"INSERT INTO QuestPartida (partida_id, quest_id, estado)
                                            VALUES (@partidaId, @questId, @estado);";

                    command.Parameters.AddWithValue("@partidaId", partidaId);
                    command.Parameters.AddWithValue("@questId", quest.questID);
                    command.Parameters.AddWithValue("@estado", "InProgress");
                    command.ExecuteNonQuery();

                    command.CommandText = "SELECT last_insert_rowid();";
                    questPartidaId = (long)command.ExecuteScalar();
                }

                foreach (QuestObjective objective in quest.objectives)
                {
                    using (SqliteCommand command = new SqliteCommand(connection))
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"INSERT INTO QuestObjetivoPartida (quest_partida_id, objective_id, current_amount)
                                                VALUES (@questPartidaId, @objectiveId, @currentAmount);";

                        command.Parameters.AddWithValue("@questPartidaId", questPartidaId);
                        command.Parameters.AddWithValue("@objectiveId", objective.objectiveID);
                        command.Parameters.AddWithValue("@currentAmount", 0);
                        command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }          
        }
    }

    public string GetQuestState(int partidaId, string questId)
    {
        using (SqliteConnection connection = new SqliteConnection(conexion))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"SELECT estado FROM QuestPartida
                                        WHERE partida_id = @partidaId AND quest_id = @questId;";

                command.Parameters.AddWithValue("@partidaId", partidaId);
                command.Parameters.AddWithValue("@questId", questId);

                object result = command.ExecuteScalar();
                return result == null ? null : result.ToString();
            }
        }
    }

    public Dictionary<string, int> CargarProgresoObjetivos(int partidaId, string questId)
    {
        Dictionary<string, int> progreso = new Dictionary<string, int>();

        using (SqliteConnection connection = new SqliteConnection(conexion))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"SELECT qop.objective_id, qop.current_amount FROM QuestObjetivoPartida qop
                                        INNER JOIN QuestPartida qp ON qop.quest_partida_id = qp.id
                                        WHERE qp.partida_id = @partidaId AND qp.quest_id = @questId;";

                command.Parameters.AddWithValue("@partidaId", partidaId);
                command.Parameters.AddWithValue("@questId", questId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        progreso[reader.GetString(0)] = reader.GetInt32(1);
                    }
                }
            }
        }

        return progreso;
    }

    public void ActualizarObjetivo(int partidaId, string questId, string objectiveId, int currentAmount)
    {
        using (SqliteConnection connection = new SqliteConnection(conexion))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"UPDATE QuestObjetivoPartida SET current_amount = @currentAmount
                                        WHERE objective_id = @objectiveId AND quest_partida_id = (
                                            SELECT id FROM QuestPartida
                                            WHERE partida_id = @partidaId AND quest_id = @questId
                      );";

                command.Parameters.AddWithValue("@currentAmount", currentAmount);
                command.Parameters.AddWithValue("@objectiveId", objectiveId);
                command.Parameters.AddWithValue("@partidaId", partidaId);
                command.Parameters.AddWithValue("@questId", questId);

                command.ExecuteNonQuery();
            }
        }
    }

    public void ActualizarEstadoQuest(int partidaId, string questId, string estado)
    {
        using (SqliteConnection connection = new SqliteConnection(conexion))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"UPDATE QuestPartida SET estado = @estado
                                        WHERE partida_id = @partidaId AND quest_id = @questId;";

                command.Parameters.AddWithValue("@estado", estado);
                command.Parameters.AddWithValue("@partidaId", partidaId);
                command.Parameters.AddWithValue("@questId", questId);

                command.ExecuteNonQuery();
            }
        }
    }


}
