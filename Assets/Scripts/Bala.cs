using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    [SerializeField] private float velocidad;

    [SerializeField] private float danio;

    [SerializeField] private float tiempoVida = 3f;

    private void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    private void Update() {
        transform.Translate(Vector2.right * transform.localScale.x * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("enemigo"))
        {
            Destroy(gameObject);
        }

        if (collision.CompareTag("suelo"))
        {
            Destroy(gameObject);
        }
    }
}
