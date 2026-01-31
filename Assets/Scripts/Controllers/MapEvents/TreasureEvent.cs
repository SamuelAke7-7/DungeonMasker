using UnityEngine;
using TMPro;

public class TreasureEvent : MonoBehaviour, IInteractuable
{

    private string message = "Encontraste un tesoro!!!";
    public TextMeshProUGUI text;
    private MapEventType mapEventType = MapEventType.Treasure;

    public void duringContact()
    {
        text.text = message;
        text.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;

        Debug.Log("TreasureEvent: duringContact");
    }
    
    public void afterContact()
    {
        text.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;

    }
    
    public void beforeContact()
    {
        // text.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
    }
}