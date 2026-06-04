using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisparoJugador : MonoBehaviour
{

    [SerializeField] private Transform controladorDisparo;
    [SerializeField] private GameObject bala;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController2 playerController;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoAtaqueDistancia;
    [SerializeField] private float duracionBloqueoDisparo = 0.45f;

    private Coroutine rutinaDisparo;


    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }



    private void Update(){

        if (Pausar.estaPausado || playerController == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.L) &&
            playerController.enSuelo &&
            !playerController.ataca &&
            !playerController.disparando &&
            !playerController.muerto &&
            playerController.ConsumirManaAtaqueDistancia())
        {
            animator.ResetTrigger("disparar");
            animator.SetTrigger("disparar");
            playerController.disparando = true;

            if (rutinaDisparo != null)
            {
                StopCoroutine(rutinaDisparo);
            }

            rutinaDisparo = StartCoroutine(LiberarDisparoPorTiempo());
        }
    }

    public void CrearBala()
    {

        if (audioSource != null && sonidoAtaqueDistancia != null)
        {
            audioSource.PlayOneShot(sonidoAtaqueDistancia);
        }

        GameObject nuevaBala = Instantiate(bala, controladorDisparo.position, controladorDisparo.rotation);

        float direccion = transform.localScale.x;
        nuevaBala.transform.localScale = new Vector3(direccion, 1, 1);
    }

    public void TerminarDisparo()
    {
        playerController.disparando = false;
        rutinaDisparo = null;
    }

    private IEnumerator LiberarDisparoPorTiempo()
    {
        yield return new WaitForSeconds(duracionBloqueoDisparo);
        TerminarDisparo();
    }
}
