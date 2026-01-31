using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TrampEvent : MonoBehaviour, IInteractuable
{

    public RawImage ghost;
    public RawImage damage_frame;
    private string message = "AUCH!!!!!";
    
    private MapEventType mapEventType = MapEventType.Tramp;

    public void duringContact()
    {
        ghost.enabled = true;
        damage_frame.enabled = true;

        MapEventManager.instance.panel.GetComponentInChildren<TextMeshProUGUI>().text = message;
        MapEventManager.instance.panel.SetActive(true);
        MapEventManager.instance.panel.GetComponentInChildren<TextMeshProUGUI>().enabled = true;


        Debug.Log("TrampEvent: duringContact");
    }
    
    public void afterContact()
    {
        ghost.enabled = false;
        damage_frame.enabled = false;
        MapEventManager.instance.panel.SetActive(false);
    }
    
    public void beforeContact()
    {
        ghost.enabled = false;
        damage_frame.enabled = false;
    }
}