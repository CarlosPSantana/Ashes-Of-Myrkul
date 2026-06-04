using UnityEngine;

public static class AyudaGlobal
{
    public static string GenerarId(GameObject o)
    {
        Vector3 p = o.transform.position;
        return $"{o.scene.name}_{o.name}_{Mathf.RoundToInt(p.x * 100)}_{Mathf.RoundToInt(p.y * 100)}";
    }
}