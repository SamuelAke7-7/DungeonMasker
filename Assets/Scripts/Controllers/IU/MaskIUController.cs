using UnityEngine;
using UnityEngine.UI;

public class MaskIUController : MonoBehaviour
{
    public Transform objectMaskColor;
    public Transform objectMaskmiddle;

    public Color colorSlime = new Color(0f, 1f, 0.1294117f, 0.1058824f);
    public Color colorKobold = new Color(1f, 0f, 0.04082775f, 0.1058824f);
    public static MaskIUController Instance;

    void Start(){
        Instance = this;
    }

    public TypeMask GetTypeMask(){
        return TypeMask.Prisor;
    }

    public void ChangeMask(){
        switch (GetTypeMask())
        {
            case TypeMask.Slime:
                objectMaskColor.gameObject.SetActive(true);
                objectMaskmiddle.gameObject.SetActive(false);
                objectMaskColor.gameObject.GetComponent<Image>().color = colorSlime;
            break;
            case TypeMask.Kobold:
                objectMaskColor.gameObject.SetActive(true);
                objectMaskmiddle.gameObject.SetActive(false);
                objectMaskColor.gameObject.GetComponent<Image>().color = colorKobold;
                break;
            case TypeMask.Prisor:
                objectMaskColor.gameObject.SetActive(false);
                objectMaskmiddle.gameObject.SetActive(true);
                break;
            default:
            objectMaskColor.gameObject.SetActive(false);
            objectMaskmiddle.gameObject.SetActive(false);
            break;
        }
    }
}
