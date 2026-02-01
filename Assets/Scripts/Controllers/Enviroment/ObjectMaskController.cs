using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectMaskController : MonoBehaviour
{
    public TypeMask actualMask;
    public int contadorMask;

    public static ObjectMaskController Instance;
    public void ToggleObjectWithMaskEffect(TypeMask type){
        List<GameObject> listObjectWithMaskEffect = GameObject.FindGameObjectsWithTag("MaskMasked").ToList();
        listObjectWithMaskEffect.ForEach(obj => obj.GetComponent<IToggleShowObject>().execute(type));
    }

    void Awake(){
        Instance = this;
    }

    void Start() {
        actualMask = TypeMask.Prisor;
        contadorMask = 4;
        ChangeMask();
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
        if (keyboard.qKey.wasPressedThisFrame)
        {
            ChangeMask();
        }
    }

    private void FindActualMask(){
        bool isFinding = false;
        while(!isFinding){

            if (contadorMask == 4){
                isFinding = true;
                actualMask = TypeMask.Prisor;
                Debug.Log("Regreso a prisor mask");
            } else {
                
                TypeMask cellType = (TypeMask)contadorMask;
                string description = cellType.GetDescription();
                Debug.Log($"No encontro mask {description}" );
                contadorMask++;
            }

            if(contadorMask > 4){
                contadorMask = 1;
            }
        }
    }

    private void ChangeMask(){
        contadorMask++;
        FindActualMask();
        ToggleObjectWithMaskEffect(actualMask);
        MaskIUController.Instance.ChangeMask();
    }
}
