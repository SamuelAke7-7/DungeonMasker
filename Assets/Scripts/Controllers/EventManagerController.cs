using UnityEngine;
using TMPro; // Si usas TextMeshPro
using UnityEngine.UI;

public class MapEventManager : MonoBehaviour
{
    public GameObject panel;
    public RawImage ghostImage;
    public RawImage healedImage;
    public CanvasBlinker canvasBlinker;
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IInteractuable interactuable))
        {
            Debug.Log("Objeto interactuable detectado: " + other.name);
            interactuable.afterContact();
        }


    }
}