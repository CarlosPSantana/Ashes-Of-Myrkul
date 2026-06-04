using Mono.Data.Sqlite;

public class WorldStateDB
{
    private readonly string connectionString;

    public WorldStateDB(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public void GuardarEstado(int partidaId, string objetoId, string tipo, string estado)
    {
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"INSERT INTO EstadoMundo (partida_id, objeto_id, tipo, estado) VALUES (@partida_id, @objeto_id, @tipo, @estado)
                                        ON CONFLICT(partida_id, objeto_id) DO UPDATE SET tipo = excluded.tipo, estado = excluded.estado;";

                command.Parameters.AddWithValue("@partida_id", partidaId);
                command.Parameters.AddWithValue("@objeto_id", objetoId);
                command.Parameters.AddWithValue("@tipo", tipo);
                command.Parameters.AddWithValue("@estado", estado);
                command.ExecuteNonQuery();
            }
        }
    }

    public string ObtenerEstado(int partidaId, string objetoId)
    {
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"SELECT estado FROM EstadoMundo
                                        WHERE partida_id = @partida_id AND objeto_id = @objeto_id
                                        LIMIT 1;";

                command.Parameters.AddWithValue("@partida_id", partidaId);
                command.Parameters.AddWithValue("@objeto_id", objetoId);

                object result = command.ExecuteScalar();
                return result == null ? null : result.ToString();
            }
        }
    }
}
