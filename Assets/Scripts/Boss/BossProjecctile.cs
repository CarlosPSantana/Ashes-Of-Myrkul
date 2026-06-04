using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float velocidad = 6f;
    public int danio = 1;
    public float tiempoVida = 4f;

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        transform.Translate(Vector2.right * transform.localScale.x * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController2 player = collision.GetComponent<PlayerController2>();

            if (player != null)
            {
                Vector2 direccionDanio = new Vector2(transform.position.x, 0f);
                player.RecibiendoDanio(direccionDanio, danio);
            }

            Destroy(gameObject);
        }

        if (collision.CompareTag("suelo"))
        {
            Destroy(gameObject);
        }
    }
}

