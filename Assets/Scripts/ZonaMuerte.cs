using UnityEngine;

public class ZonaMuerte : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController2>().RecibiendoDanio(Vector2.zero,99);
            return;
        }

        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.RecibeDanio(Vector2.zero, enemy.vida);
        }
    }
}
