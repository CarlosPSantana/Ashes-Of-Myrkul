using UnityEngine;
using UnityEngine.InputSystem;

public class DetectorInteracciones : MonoBehaviour
{

    private InterfazInteractuable rangoInteractuable = null;
    public GameObject interaccionIcono;
    void Start()
    {
        interaccionIcono.SetActive(false);
    }

    public void AlInteractuar(InputAction.CallbackContext contexto)
    {
        if (contexto.performed)
        {
            rangoInteractuable?.Interactuar();
        }
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.TryGetComponent(out InterfazInteractuable interactuable) && interactuable.PuedeInteractuar())
        {
            rangoInteractuable = interactuable;
            interaccionIcono.SetActive(true);


        }
    }

    private void OnTriggerExit2D(Collider2D c)
    {
        if (c.TryGetComponent(out InterfazInteractuable interactuable) && interactuable == rangoInteractuable)
        {
            rangoInteractuable = null;
            interaccionIcono.SetActive(false);


        }
    }


}
