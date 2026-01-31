using UnityEngine;

/// <summary>
/// Hace que el objeto siempre mire en la dirección del personaje/jugador.
/// Útil para iconos, señales, o efectos que deben orientarse hacia el jugador.
/// </summary>
public class LookAtPlayerBehaviour : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool onlyYAxis = true; // Solo rotar en Y (horizontal) - ideal para top-down
    [SerializeField] private float smoothSpeed = 10f; // 0 = instantáneo
    
    private Transform playerTransform;
    
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning($"[LookAtPlayerBehaviour] No se encontró objeto con tag '{playerTag}'");
        }
    }
    
    void LateUpdate()
    {
        if (playerTransform == null) return;
        
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        
        if (onlyYAxis)
        {
            directionToPlayer.y = 0; // Ignorar diferencia en altura
            if (directionToPlayer.sqrMagnitude < 0.001f) return; // Evitar rotación cuando está muy cerca
        }
        
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        
        if (smoothSpeed > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }
}
