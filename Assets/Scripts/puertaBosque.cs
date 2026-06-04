using UnityEngine;
using UnityEngine.SceneManagement;

public class puertaBosque : MonoBehaviour
{
    [SerializeField] private string nombreEscenaDestino;
    [SerializeField] private Vector3 posicionSpawnDestino;
    [SerializeField] private SistemaGuardado sistemaGuardado;

    private void Awake()
    {
        if (sistemaGuardado == null)
        {
            sistemaGuardado = FindFirstObjectByType<SistemaGuardado>();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (sistemaGuardado != null)
            {
                sistemaGuardado.PrepararCambioDeEscena(nombreEscenaDestino, posicionSpawnDestino);
            }
            else
            {
                PlayerController2 player = collision.GetComponent<PlayerController2>();

                if (player != null)
                {
                    SistemaGuardado.datosCargados = player.GetDatos();
                    SistemaGuardado.datosCargados.nombreEscena = nombreEscenaDestino;
                    SistemaGuardado.datosCargados.posX = posicionSpawnDestino.x;
                    SistemaGuardado.datosCargados.posY = posicionSpawnDestino.y;
                    SistemaGuardado.datosCargados.posZ = posicionSpawnDestino.z;
                }
            }

            SceneManager.LoadScene(nombreEscenaDestino);
        }
    }
}
