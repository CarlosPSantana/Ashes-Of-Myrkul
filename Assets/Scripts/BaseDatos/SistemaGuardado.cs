using Mono.Data.Sqlite;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SistemaGuardado : MonoBehaviour
{
    public static SaveData datosCargados;
    public PlayerController2 player;


    public InventoryController inventoryController;
    public QuestController questController;
    public static int slot_actual;
    public static int jugador_id;
    public static int partida_id;

    private void Awake()
    {
        ResolverReferencias();
    }

    public void IniciarNuevaPartida(string escenaInicial)
    {
        ReiniciarEstadoActual();

        using (SqliteConnection connection = new SqliteConnection(ConexionDB.conexion))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = "INSERT INTO Jugador (nombre) VALUES (@nombre);";
                command.Parameters.AddWithValue("@nombre", "Jugador1");
                command.ExecuteNonQuery();

                command.CommandText = "SELECT last_insert_rowid();";
                jugador_id = int.Parse(command.ExecuteScalar().ToString());
            }

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"INSERT INTO Partida (jugador_id, slot_guardado, fecha, escena, posX, posY, posZ)
                                        VALUES (@jugador_id, NULL, datetime('now'), @escena, @posX, @posY, @posZ);";

                command.Parameters.AddWithValue("@jugador_id", jugador_id);
                command.Parameters.AddWithValue("@escena", escenaInicial);
                command.Parameters.AddWithValue("@posX", 4.32f);
                command.Parameters.AddWithValue("@posY", 0.79f);
                command.Parameters.AddWithValue("@posZ", 0f);   
                command.ExecuteNonQuery();

                command.CommandText = "SELECT last_insert_rowid();";
                partida_id = int.Parse(command.ExecuteScalar().ToString());
            }

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"INSERT INTO Estadisticas (partida_id, vida, velocidad, fuerzaSalto, fuerzaRebote)
                                        VALUES (@partida_id, @vida, @velocidad, @salto, @rebote);";

                command.Parameters.AddWithValue("@partida_id", partida_id);
                command.Parameters.AddWithValue("@vida", 3);
                command.Parameters.AddWithValue("@velocidad", 5f);
                command.Parameters.AddWithValue("@salto", 7f);
                command.Parameters.AddWithValue("@rebote", 3.6f);
                command.ExecuteNonQuery();
            }
        }

        slot_actual = 0;

        if (questController != null)
        {
            questController.InicializarConPartida(partida_id);
        }

        datosCargados = new SaveData
        {
            nombreEscena = escenaInicial,
            posX = 4.32f,
            posY = 0.79f,
            posZ = 0f,
            vida = 3,
            velocidad = 5f,
            fuerzaSalto = 7f,
            fuerzaRebote = 3.6f,
            inventarioManagement = new List<InventarioManagement>()
        };
    }

    public void GuardarPartida()
    {
        if (slot_actual == 0)
        {
            Debug.LogWarning("La partida aún no tiene slot asignado. Debes elegir un slot para guardar.");
            return;
        }

        GuardarPartidaEnSlot(slot_actual);
    }

    public void GuardarPartidaEnSlot(int slot)
    {
        ResolverReferencias();

        if (player == null || inventoryController == null)
        {
            Debug.LogWarning("No se encontraron PlayerController2 o InventoryController para guardar la partida.");
            return;
        }

        using (SqliteConnection connection = new SqliteConnection(ConexionDB.conexion))
        {
            connection.Open();

            SaveData data = player.GetDatos();

            int partidaEnEseSlot = ObtenerPartidaIdPorSlot(connection, slot);

            if (partidaEnEseSlot != 0 && partidaEnEseSlot != partida_id)
            {
                BorrarPartidaCompleta(connection, partidaEnEseSlot);
            }

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"UPDATE Partida
                                        SET slot_guardado = @slot,
                                            fecha = datetime('now'),
                                            escena = @escena,
                                            posX = @posX,
                                            posY = @posY,
                                            posZ = @posZ
                                        WHERE id = @partida_id;";

                command.Parameters.AddWithValue("@slot", slot);
                command.Parameters.AddWithValue("@escena", data.nombreEscena);
                command.Parameters.AddWithValue("@posX", data.posX);
                command.Parameters.AddWithValue("@posY", data.posY);
                command.Parameters.AddWithValue("@posZ", data.posZ);
                command.Parameters.AddWithValue("@partida_id", partida_id);
                command.ExecuteNonQuery();
            }

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"UPDATE Estadisticas
                                        SET vida = @vida,
                                            velocidad = @velocidad,
                                            fuerzaSalto = @salto,
                                            fuerzaRebote = @rebote
                                        WHERE partida_id = @partida_id;";

                command.Parameters.AddWithValue("@vida", data.vida);
                command.Parameters.AddWithValue("@velocidad", data.velocidad);
                command.Parameters.AddWithValue("@salto", data.fuerzaSalto);
                command.Parameters.AddWithValue("@rebote", data.fuerzaRebote);
                command.Parameters.AddWithValue("@partida_id", partida_id);
                command.ExecuteNonQuery();
            }

            ReemplazarInventario(connection, partida_id);
        }

        slot_actual = slot;

        if (questController != null)
        {
            questController.InicializarConPartida(partida_id);
        }

        Debug.Log("Partida guardada en slot " + slot);
    }

    public void PrepararCambioDeEscena(string escenaDestino, Vector3 posicionDestino)
    {
        ResolverReferencias();

        if (player == null)
        {
            Debug.LogWarning("No se encontró PlayerController2 al preparar el cambio de escena.");
            return;
        }

        datosCargados = player.GetDatos();
        datosCargados.nombreEscena = escenaDestino;
        datosCargados.posX = posicionDestino.x;
        datosCargados.posY = posicionDestino.y;
        datosCargados.posZ = posicionDestino.z;

        if (inventoryController != null)
        {
            datosCargados.inventarioManagement = inventoryController.GetInventoryItems();
        }
        else
        {
            datosCargados.inventarioManagement = new List<InventarioManagement>();
        }
    }

    public void CargarPartidaPorSlot(int slot)
    {
        using (SqliteConnection connection = new SqliteConnection(ConexionDB.conexion))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"
                    SELECT p.id, p.jugador_id, p.slot_guardado, p.escena, p.posX, p.posY, p.posZ,
                           e.vida, e.velocidad, e.fuerzaSalto, e.fuerzaRebote
                    FROM Estadisticas e
                    INNER JOIN Partida p ON e.partida_id = p.id
                    WHERE p.slot_guardado = @slot
                    LIMIT 1;";

                command.Parameters.AddWithValue("@slot", slot);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        partida_id = reader.GetInt32(0);
                        jugador_id = reader.GetInt32(1);
                        slot_actual = reader.GetInt32(2);

                        List<InventarioManagement> inventario = CargarInventario(connection, partida_id);

                        datosCargados = new SaveData
                        {
                            nombreEscena = reader.GetString(3),
                            posX = reader.GetFloat(4),
                            posY = reader.GetFloat(5),
                            posZ = reader.GetFloat(6),
                            vida = reader.GetInt32(7),
                            velocidad = reader.GetFloat(8),
                            fuerzaSalto = reader.GetFloat(9),
                            fuerzaRebote = reader.GetFloat(10),
                            inventarioManagement = inventario
                        };

                        if (questController != null)
                        {
                            questController.InicializarConPartida(partida_id);
                        }
                    }
                }
            }
        }
    }

    public void CargarUltimaPartida()
    {
        using (SqliteConnection connection = new SqliteConnection(ConexionDB.conexion))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"
                    SELECT p.id, p.jugador_id, p.slot_guardado, p.escena, p.posX, p.posY, p.posZ,
                           e.vida, e.velocidad, e.fuerzaSalto, e.fuerzaRebote
                    FROM Estadisticas e
                    INNER JOIN Partida p ON e.partida_id = p.id
                    WHERE p.slot_guardado IS NOT NULL
                    ORDER BY p.fecha DESC
                    LIMIT 1;";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        partida_id = reader.GetInt32(0);
                        jugador_id = reader.GetInt32(1);
                        slot_actual = reader.GetInt32(2);

                        List<InventarioManagement> inventario = CargarInventario(connection, partida_id);

                        datosCargados = new SaveData
                        {
                            nombreEscena = reader.GetString(3),
                            posX = reader.GetFloat(4),
                            posY = reader.GetFloat(5),
                            posZ = reader.GetFloat(6),
                            vida = reader.GetInt32(7),
                            velocidad = reader.GetFloat(8),
                            fuerzaSalto = reader.GetFloat(9),
                            fuerzaRebote = reader.GetFloat(10),
                            inventarioManagement = inventario
                        };

                        if (questController != null)
                        {
                            questController.InicializarConPartida(partida_id);
                        }
                    }
                }
            }
        }
    }

    public SlotInfo ObtenerInfoSlot(int slot)
    {
        SlotInfo info = new SlotInfo
        {
            slot = slot,
            ocupado = false,
            escena = "",
            fecha = "",
            vida = 0
        };

        using (SqliteConnection connection = new SqliteConnection(ConexionDB.conexion))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"
                    SELECT p.fecha, p.escena, e.vida
                    FROM Partida p
                    INNER JOIN Estadisticas e ON e.partida_id = p.id
                    WHERE p.slot_guardado = @slot
                    LIMIT 1;";

                command.Parameters.AddWithValue("@slot", slot);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        info.ocupado = true;
                        info.fecha = reader.GetString(0);
                        info.escena = reader.GetString(1);
                        info.vida = reader.GetInt32(2);
                    }
                }
            }
        }

        return info;
    }

    public List<InventarioManagement> CargarInventario(SqliteConnection connection, int partidaId)
    {
        List<InventarioManagement> inventario = new List<InventarioManagement>();

        using (SqliteCommand command = new SqliteCommand(connection))
        {
            command.CommandText = @"SELECT item_id, slot, cantidad
                                    FROM Inventario
                                    WHERE partida_id = @partida_id
                                    ORDER BY slot;";

            command.Parameters.AddWithValue("@partida_id", partidaId);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    inventario.Add(new InventarioManagement
                    {
                        itemId = reader.GetInt32(0),
                        slotIndex = reader.GetInt32(1),
                        cantidad = reader.GetInt32(2)
                    });
                }
            }
        }

        return inventario;
    }

    private int ObtenerPartidaIdPorSlot(SqliteConnection connection, int slot)
    {
        using (SqliteCommand command = new SqliteCommand(connection))
        {
            command.CommandText = "SELECT id FROM Partida WHERE slot_guardado = @slot LIMIT 1;";
            command.Parameters.AddWithValue("@slot", slot);

            object result = command.ExecuteScalar();
            return result == null ? 0 : int.Parse(result.ToString());
        }
    }

    private void BorrarPartidaCompleta(SqliteConnection connection, int partidaABorrar)
    {
        using (SqliteCommand command = new SqliteCommand(connection))
        {
            command.CommandText = "DELETE FROM Inventario WHERE partida_id = @partida_id;";
            command.Parameters.AddWithValue("@partida_id", partidaABorrar);
            command.ExecuteNonQuery();
        }

        using (SqliteCommand command = new SqliteCommand(connection))
        {
            command.CommandText = "DELETE FROM QuestObjetivoPartida WHERE quest_partida_id IN (SELECT id FROM QuestPartida WHERE partida_id = @partida_id);";
            command.Parameters.AddWithValue("@partida_id", partidaABorrar);
            command.ExecuteNonQuery();
        }

        using (SqliteCommand command = new SqliteCommand(connection))
        {
            command.CommandText = "DELETE FROM QuestPartida WHERE partida_id = @partida_id;";
            command.Parameters.AddWithValue("@partida_id", partidaABorrar);
            command.ExecuteNonQuery();
        }

        using (SqliteCommand command = new SqliteCommand(connection))
        {
            command.CommandText = "DELETE FROM Estadisticas WHERE partida_id = @partida_id;";
            command.Parameters.AddWithValue("@partida_id", partidaABorrar);
            command.ExecuteNonQuery();
        }

        using (SqliteCommand command = new SqliteCommand(connection))
        {
            command.CommandText = "DELETE FROM Partida WHERE id = @partida_id;";
            command.Parameters.AddWithValue("@partida_id", partidaABorrar);
            command.ExecuteNonQuery();
        }
    }

    private void ReemplazarInventario(SqliteConnection connection, int partidaId)
    {
        using (SqliteCommand command = new SqliteCommand(connection))
        {
            command.CommandText = "DELETE FROM Inventario WHERE partida_id = @partida_id;";
            command.Parameters.AddWithValue("@partida_id", partidaId);
            command.ExecuteNonQuery();
        }

        List<InventarioManagement> items = inventoryController.GetInventoryItems();

        foreach (InventarioManagement item in items)
        {
            using (SqliteCommand command = new SqliteCommand(connection))
            {
                command.CommandText = @"INSERT INTO Inventario (partida_id, item_id, slot, cantidad)
                                        VALUES (@partida_id, @item_id, @slot, @cantidad);";

                command.Parameters.AddWithValue("@partida_id", partidaId);
                command.Parameters.AddWithValue("@item_id", item.itemId);
                command.Parameters.AddWithValue("@slot", item.slotIndex);
                command.Parameters.AddWithValue("@cantidad", item.cantidad);
                command.ExecuteNonQuery();
            }
        }
    }

    private void ResolverReferencias()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController2>();
        }

        if (inventoryController == null)
        {
            inventoryController = FindFirstObjectByType<InventoryController>();
        }

        if (questController == null)
        {
            questController = FindFirstObjectByType<QuestController>();
        }
    }

    private void ReiniciarEstadoActual()
    {
        jugador_id = 0;
        partida_id = 0;
        slot_actual = 0;
        datosCargados = null;
    }
}
