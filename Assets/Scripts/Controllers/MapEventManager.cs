using UnityEngine;
using TMPro; // Si usas TextMeshPro
using UnityEngine.UI;

public class MapEventManager : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel; // Arrastra aquí tu Canvas o Panel
    public GameObject panel;
    [SerializeField] private TextMeshProUGUI text;

    public static MapEventManager instance;
    private void Awake()
    {
        instance = this;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si el objeto con el que colisionamos implementa IInteractuable
        if (other.gameObject.TryGetComponent(out IInteractuable interactuable))
        {
            Debug.Log("Objeto interactuable detectado: " + other.name);
            interactuable.duringContact();
        }

        // Comprobamos si lo que entró en el trigger es el jugador
        if (other.CompareTag("Player"))
        {
            // uiPanel.SetActive(true);
            
            Debug.Log("Visuales activados");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IInteractuable interactuable))
        {
            Debug.Log("Objeto interactuable detectado: " + other.name);
            interactuable.afterContact();
        }

        // Opcional: Ocultar cuando el jugador se aleje
        if (other.CompareTag("Player"))
        {
            uiPanel.SetActive(false);
        }
    }
}