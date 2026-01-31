using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractController : MonoBehaviour
{
    public LayerMask layersToDetect;
    public float distance = 1;
    private bool gamepadInputProcessed = false; // Flag para evitar procesar el mismo input múltiples veces
    private Vector2 previousStickValue = Vector2.zero; // Valor anterior del stick para detectar cambios
    

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distance, layersToDetect))
        {
            if (hit.collider.gameObject.TryGetComponent<IInteractObject>(out IInteractObject component)){
                if (component.isActivatedAlready()){
                    component.execute();
                } else {
                    // Procesar input de teclado
                    ProcessKeyboardInput(hit.collider.gameObject);
                    
                    // Procesar input de gamepad/joycon
                    ProcessGamepadInput(hit.collider.gameObject);
                }
            }
            
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * distance, Color.white);
        }
    }

    private void ProcessKeyboardInput(GameObject objectInteract)
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;
        
        // Movimiento hacia adelante (W) o hacia atrás (S)
        if (keyboard.eKey.wasPressedThisFrame)
        {
            objectInteract.GetComponent<IInteractObject>().execute();
        }
    }
    
    private void ProcessGamepadInput(GameObject objectInteract)
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null)
        {
            // Resetear el valor anterior del stick si no hay gamepad conectado
            previousStickValue = Vector2.zero;
            return;
        }
        
        // Priorizar D-pad para movimiento discreto (más apropiado para movimiento por bloques)
        // D-pad arriba/abajo para movimiento
        if (gamepad.xButton.wasPressedThisFrame)
        {
            objectInteract.GetComponent<IInteractObject>().execute();
        }
    }
}
