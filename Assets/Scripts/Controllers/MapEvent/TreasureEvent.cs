using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreasureEvent : MonoBehaviour, IInteractuable
{

    private string message = "Encontraste un nuevo objeto!!!";
    private MapEventType mapEventType = MapEventType.Treasure;

    public void duringContact()
    {
        // text.text = message;
        // text.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
        MapEventManager.instance.panel.SetActive(true);
        MapEventManager.instance.panel.GetComponentInChildren<TextMeshProUGUI>().text = message;

        MapEventManager.instance.panel.GetComponentInChildren<TextMeshProUGUI>().enabled = true;

        Debug.Log("TreasureEvent: duringContact");
    }
    
    public void afterContact()
    {
        // text.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
        MapEventManager.instance.panel.SetActive(false);
        
    }
    
    public void beforeContact()
    {
        // text.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
    }
}