using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TrampEvent : MonoBehaviour, IInteractuable
{
    public float damage = 100f;
    private string message = "AUCH!!!!!";
    
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
    }
    
    public void afterContact()
    {
        MapEventManager.instance.panel.SetActive(false);
    }
    
    public void beforeContact()
    {
      
    }
}