using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectMaskController : MonoBehaviour
{
    public void ToggleObjectWithMaskEffect(TypeMask type){
        List<GameObject> listObjectWithMaskEffect = GameObject.FindGameObjectsWithTag("MaskMasked").ToList();
        listObjectWithMaskEffect.ForEach(obj => obj.GetComponent<IToggleShowObject>().execute(type));
    }

    void Update(){
        ProcessKeyboardInput();
    }
    private void ProcessKeyboardInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;
        
        // Movimiento hacia adelante (W) o hacia atrás (S)
        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            ToggleObjectWithMaskEffect(TypeMask.Slime);
        }
        else if (keyboard.digit2Key.wasPressedThisFrame)
        {
            ToggleObjectWithMaskEffect(TypeMask.Skeleton);
        }else if (keyboard.digit3Key.wasPressedThisFrame)
        {
            ToggleObjectWithMaskEffect(TypeMask.Prisor);
        }
    }
}
