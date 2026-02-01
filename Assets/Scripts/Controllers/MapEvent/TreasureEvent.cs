using UnityEngine;
using System.Collections;
using TMPro;

public class TreasureEvent : MonoBehaviour, IInteractuable
{

    private string message = "Encontraste un nuevo objeto!!!";
    private MapEventType mapEventType = MapEventType.Treasure;

    public TypeMask typeMask;

    public void duringContact()
    {
        // text.text = message;
        // text.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
        MapEventManager.instance.panel.SetActive(true);

        if (typeMask == TypeMask.vacio){
            message = $"Oh, el cofre estaba vacio";
        } else {
            if (typeMask == TypeMask.Hungus){
                message = $"Oh, encontraste un orbe extraño, tiene el reflejo de una llave en su interior";
            } else {
                string description = typeMask.GetDescription();
                message = $"Encontraste la mascara de {description}";
                Debug.Log( message );
            }
            
        }
        

        MapEventManager.instance.panel.GetComponentInChildren<TextMeshProUGUI>().text = message;

        MapEventManager.instance.panel.GetComponentInChildren<TextMeshProUGUI>().enabled = true;

        StartCoroutine(TimerEnd());
    }

    IEnumerator TimerEnd()
    {
        yield return new WaitForSeconds(3);
        afterContact();
    }
    
    public void afterContact()
    {
        // text.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
        MapEventManager.instance.panel.SetActive(false);
        PlayerController.Instance.SetAbleWalk(true);
        PlayerInteractController.Instance.SetCanInteract(true);

        if (typeMask != TypeMask.vacio){
           if (typeMask == TypeMask.Hungus){
                InventoryController.Instance.hasPurpleOrb = true;
            } else {
                switch (typeMask)
                {
                    case TypeMask.Slime:
                    InventoryController.Instance.hasSlimeMask = true;
                    break;
                    case TypeMask.Kobold:
                    InventoryController.Instance.hasKoboldMask = true;
                    break;
                    case TypeMask.Skeleton:
                    InventoryController.Instance.hasSkeletonMask = true;
                    break;
                }
            }
        } 

        Destroy(gameObject);
    }
    
    public void beforeContact()
    {
        // text.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
    }
}