using UnityEngine;

public class MaskIUController : MonoBehaviour
{
    public Transform objectMaskColor;
    public Transform objectMaskmiddle;

    public Color colorSlime = new Color()
    public static MaskIUController Instance;

    void Start(){
        Instance = this;
    }

    public TypeMask GetTypeMask(){
        return TypeMask.Slime;
    }

    public void ChangeMask(){
        switch (GetTypeMask())
        {
            case TypeMask.Slime:
            break;

            default:
            objectMaskColor.gameObject.SetActive(false);
            objectMaskmiddle.gameObject.SetActive(false);
            break;
        }
    }
}
