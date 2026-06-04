using UnityEngine;

public abstract class WorldPersistentObject : MonoBehaviour
{
    [SerializeField] private string uniqueId;
    protected WorldStateDB worldStateDB;

    public string UniqueId
    {
        get
        {
            if (string.IsNullOrEmpty(uniqueId))
            {
                uniqueId = AyudaGlobal.GenerarId(gameObject);
            }

            return uniqueId;
        }
    }

    protected virtual void Awake()
    {
        worldStateDB = new WorldStateDB(ConexionDB.conexion);
    }

    protected string ObtenerEstadoGuardado()
    {
        if (SistemaGuardado.partida_id == 0)
        {
            Debug.LogWarning($"[{name}] partida_id es 0 al consultar estado.");
            return null;
        }

        string estado = worldStateDB.ObtenerEstado(SistemaGuardado.partida_id, UniqueId);
        Debug.Log($"[{name}] Consultando estado mundo. partida={SistemaGuardado.partida_id}, id={UniqueId}, estado={estado}");
        return estado;
    }

    protected void GuardarEstado(string tipo, string estado)
    {
        if (SistemaGuardado.partida_id == 0)
        {
            Debug.LogWarning($"[{name}] partida_id es 0 al guardar estado.");
            return;
        }

        Debug.Log($"[{name}] Guardando estado mundo. partida={SistemaGuardado.partida_id}, id={UniqueId}, tipo={tipo}, estado={estado}");
        worldStateDB.GuardarEstado(SistemaGuardado.partida_id, UniqueId, tipo, estado);
    }

}

