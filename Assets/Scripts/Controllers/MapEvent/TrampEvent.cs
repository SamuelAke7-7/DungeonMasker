using UnityEngine;
using System.Collections;
using TMPro;


public class TrampEvent : MonoBehaviour, IInteractuable
{
    public float damage = 100f;
    private string message = "AUCH!!!!! UNA TRAMPA";
    
    private MapEventType mapEventType = MapEventType.Trap;

    public void duringContact()
    {
        PlayerController.Instance.DoDamage(damage);
        Debug.Log("TrampEvent: Player lost " + damage + " life");

        MapEventManager.instance.panel.GetComponentInChildren<TextMeshProUGUI>().text = message;
        MapEventManager.instance.panel.SetActive(true);
        MapEventManager.instance.panel.GetComponentInChildren<TextMeshProUGUI>().enabled = true;

        MapEventManager.instance.canvasBlinker.TriggerBlink();

        Debug.Log("TrampEvent: duringContact");
        StartCoroutine(TimerEnd());
    }

    IEnumerator TimerEnd()
    {
        yield return new WaitForSeconds(3);
        afterContact();
    }
    
    public void afterContact()
    {
        MapEventManager.instance.panel.SetActive(false);
        PlayerController.Instance.SetAbleWalk(true);
        PlayerInteractController.Instance.SetCanInteract(true);
        Destroy(gameObject);
    }
    
    public void beforeContact()
    {
      
    }
}