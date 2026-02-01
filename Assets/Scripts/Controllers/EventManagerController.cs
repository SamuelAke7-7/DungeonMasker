using UnityEngine;
using TMPro; // Si usas TextMeshPro
using UnityEngine.UI;

public class MapEventManager : MonoBehaviour
{
    public GameObject panel;
    public GameObject ghostImage;
    public RawImage healedImage;
    public CanvasBlinker canvasBlinker;
    [SerializeField] private TextMeshProUGUI text;
    public static MapEventManager instance;

    private void Awake()
    {
        instance = this;
    }
}