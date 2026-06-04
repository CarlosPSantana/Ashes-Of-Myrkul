using System.Collections;
using UnityEngine;

public class EfectoRebote : MonoBehaviour
{
    public float alturaRebote = 0.3f;
    public float duracionRebote = 0.4f;
    public int contadorRebotes = 2;

    public void EmpezarRebote()
    {
        StartCoroutine(ControlarRebote());
    }

    private IEnumerator ControlarRebote()
    {
        Vector3 inicio = transform.position;
        float alturaActual = alturaRebote;
        float duracionActual = duracionRebote;

        for (int i = 0; i < contadorRebotes; i++)
        {
            yield return Rebote(inicio, alturaActual, duracionActual / 2f);
            alturaActual *= 0.5f;
            duracionActual *= 0.8f;
        }

        transform.position = inicio;

        ObjetoRecogible objetoRecogible = GetComponent<ObjetoRecogible>();
        if (objetoRecogible != null)
        {
            objetoRecogible.PermitirRecogida();
        }
    }

    private IEnumerator Rebote(Vector3 inicio, float altura, float duracion)
    {
        Vector3 pico = inicio + Vector3.up * altura;
        float transcurrido = 0f;

        while (transcurrido < duracion)
        {
            transform.position = Vector3.Lerp(inicio, pico, transcurrido / duracion);
            transcurrido += Time.deltaTime;
            yield return null;
        }

        transform.position = pico;
        transcurrido = 0f;

        while (transcurrido < duracion)
        {
            transform.position = Vector3.Lerp(pico, inicio, transcurrido / duracion);
            transcurrido += Time.deltaTime;
            yield return null;
        }

        transform.position = inicio;
    }
}
