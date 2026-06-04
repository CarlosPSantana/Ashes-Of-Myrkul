using Mono.Data.Sqlite;
using UnityEngine;

// Clase ConexiónDB que hereda de MonoBehaviour (significa que puede añadirse a un game object)

public class ConexionDB : MonoBehaviour
{
    // Atributo conexion de tipo string y se encuentra static para que al usarse en otra clase no se genere un valor nuevo
    public static string conexion;


    /* Método que consigue la ruta absoluta donde se encuentra la base de datos y la devuelve, luego crea la cadena de conexión.
    Se prueba utilizando el debug para ver si conecta y que aparezca en consola. */
    public void Awake()
    {
        string dbPath = Application.streamingAssetsPath + "/AshesDB.db";

        conexion = "URI=file:" + dbPath;

        Debug.Log("Ruta BD: " + dbPath);

        ProbarConexion();
    }

    // Método el cual prueba si consigue conectarse a la base de datos.
    public void ProbarConexion()
    {
        using (var conn = new SqliteConnection(conexion))
        {
            try
            {
                conn.Open();
                Debug.Log("Conexión a la base de datos AAAAAAAAA");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error jodido: " + e.Message);
            }
        }
    }
}
