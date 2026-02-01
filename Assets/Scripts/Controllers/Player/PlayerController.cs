using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    [Header("Configuración de Movimiento")]
    [SerializeField] public float life = 100f;
    [SerializeField] public float mana = 100f;
    private PlayerStatusEnum status = PlayerStatusEnum.Normal;

    [Header("Configuración de Movimiento")]
    [SerializeField] private float blockSize = 1f; // Tamaño de cada bloque
    [SerializeField] private float moveSpeed = 5f; // Velocidad de movimiento entre bloques
    [SerializeField] private float rotationSpeed = 180f; // Velocidad de rotación (grados por segundo)
    [SerializeField] private float stickDeadZone = 0.5f; // Umbral para el stick analógico del gamepad
    
    private bool canMove = true;
    private bool isMoving = false; // Flag para evitar múltiples movimientos simultáneos
    private bool isRotating = false; // Flag para evitar múltiples rotaciones simultáneas
    private bool gamepadInputProcessed = false; // Flag para evitar procesar el mismo input múltiples veces
    private Vector2 previousStickValue = Vector2.zero; // Valor anterior del stick para detectar cambios
    
    public static PlayerController Instance;
    public GameObject deadScreenImage;

    void Awake(){
        Instance = this;
    }

    void Update()
    {
        if (!canMove) return;
        // Solo procesar input si no se está moviendo o rotando
        if (isMoving || isRotating)
            return;
        
        // Resetear flag de gamepad al inicio de cada frame
        gamepadInputProcessed = false;
        
        // Procesar input de teclado
        ProcessKeyboardInput();
        
        // Procesar input de gamepad/joycon
        ProcessGamepadInput();
    }
    
    private void ProcessKeyboardInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;
        
        // Movimiento hacia adelante (W) o hacia atrás (S)
        if (keyboard.wKey.wasPressedThisFrame)
        {
            MoveForward();
        }
        else if (keyboard.sKey.wasPressedThisFrame)
        {
            MoveBackward();
        }
        
        // Rotación izquierda (A) o derecha (D)
        if (keyboard.aKey.wasPressedThisFrame)
        {
            RotateLeft();
        }
        else if (keyboard.dKey.wasPressedThisFrame)
        {
            RotateRight();
        }

    }
    
    private void ProcessGamepadInput()
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
        if (gamepad.dpad.up.wasPressedThisFrame)
        {
            MoveForward();
            gamepadInputProcessed = true;
        }
        else if (gamepad.dpad.down.wasPressedThisFrame)
        {
            MoveBackward();
            gamepadInputProcessed = true;
        }
        
        // D-pad izquierda/derecha para rotación
        if (gamepad.dpad.left.wasPressedThisFrame)
        {
            RotateLeft();
            gamepadInputProcessed = true;
        }
        else if (gamepad.dpad.right.wasPressedThisFrame)
        {
            RotateRight();
            gamepadInputProcessed = true;
        }
        
        // Si no se usó el D-pad, usar el stick izquierdo como alternativa
        if (!gamepadInputProcessed)
        {
            Vector2 leftStick = gamepad.leftStick.ReadValue();
            
            // Detectar cuando el stick cruza el umbral (movimiento discreto)
            bool stickYAboveThreshold = Mathf.Abs(leftStick.y) > stickDeadZone;
            bool stickXAboveThreshold = Mathf.Abs(leftStick.x) > stickDeadZone;
            bool previousStickYAboveThreshold = Mathf.Abs(previousStickValue.y) > stickDeadZone;
            bool previousStickXAboveThreshold = Mathf.Abs(previousStickValue.x) > stickDeadZone;
            
            // Movimiento adelante/atrás con stick vertical (solo cuando cruza el umbral)
            if (stickYAboveThreshold && !previousStickYAboveThreshold)
            {
                if (leftStick.y > 0)
                {
                    MoveForward();
                    gamepadInputProcessed = true;
                }
                else if (leftStick.y < 0)
                {
                    MoveBackward();
                    gamepadInputProcessed = true;
                }
            }
            // Rotación con stick horizontal (solo cuando cruza el umbral)
            else if (stickXAboveThreshold && !previousStickXAboveThreshold)
            {
                if (leftStick.x < 0)
                {
                    RotateLeft();
                    gamepadInputProcessed = true;
                }
                else if (leftStick.x > 0)
                {
                    RotateRight();
                    gamepadInputProcessed = true;
                }
            }
            
            // Guardar el valor actual para el próximo frame
            previousStickValue = leftStick;
        }
        else
        {
            // Si no se procesó el stick, resetear el valor anterior
            previousStickValue = gamepad.leftStick.ReadValue();
        }
    }
    
    private void MoveForward()
    {
        Vector3 targetPosition = transform.position + transform.forward * blockSize;
        if (!CanMoveTo(targetPosition)) return;
        StartCoroutine(MoveToPosition(targetPosition));
    }
    
    private void MoveBackward()
    {
        Vector3 targetPosition = transform.position - transform.forward * blockSize;
        if (!CanMoveTo(targetPosition)) return;
        StartCoroutine(MoveToPosition(targetPosition));
    }
    
    /// <summary>
    /// Verifica si se puede mover a la posición indicada en mundo.
    /// Usa WorldToGrid para convertir correctamente las coordenadas del grid.
    /// </summary>
    private bool CanMoveTo(Vector3 worldPosition)
    {
        if (GridMapController.Instance == null) return false;
        
        Vector2Int gridCell = GridMapController.Instance.WorldToGrid(worldPosition);
        return GridMapController.Instance.IsCellWalkable(gridCell.x, gridCell.y);
    }
    
    private void RotateLeft()
    {
        float targetRotation = transform.eulerAngles.y - 90f;
        StartCoroutine(RotateToAngle(targetRotation));
    }
    
    private void RotateRight()
    {
        float targetRotation = transform.eulerAngles.y + 90f;
        StartCoroutine(RotateToAngle(targetRotation));
    }
    
    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / moveSpeed;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Usar una curva suave para el movimiento
            t = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        // Asegurar que llegamos exactamente a la posición objetivo
        transform.position = targetPosition;
        isMoving = false;
    }
    
    private IEnumerator RotateToAngle(float targetAngle)
    {
        isRotating = true;
        float startAngle = transform.eulerAngles.y;
        float angleDifference = Mathf.DeltaAngle(startAngle, targetAngle);
        float duration = Mathf.Abs(angleDifference) / rotationSpeed;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t);
            float currentAngle = Mathf.LerpAngle(startAngle, targetAngle, t);
            transform.rotation = Quaternion.Euler(0f, currentAngle, 0f);
            yield return null;
        }
        
        // Asegurar que llegamos exactamente al ángulo objetivo
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        isRotating = false;
    }

    public void SetAbleWalk(bool isWalkable){
        this.canMove = isWalkable;
    }

    public void addLife(float lifeGained){
        LifebarManager.instance.addLife(lifeGained);
    }

    public void DoDamage(float damage){
        LifebarManager.instance.removeLife(damage);

        if(LifebarManager.instance.vida < 1f){
            deadScreenImage.SetActive(true);
            canMove = false;

            StartCoroutine(delayBeforeLoadingMainScreen(5f));
        }
    }

    IEnumerator delayBeforeLoadingMainScreen(float delay){
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Menu");
    }
}
