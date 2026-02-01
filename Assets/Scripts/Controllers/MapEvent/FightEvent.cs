using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FightEvent : MonoBehaviour, IInteractuable
{
    public string[] sentences = {"Tienes Problemas!"};
    
    private MapEventType mapEventType = MapEventType.Fighting;

    public void duringContact()
    {
        // iniciar pelea

        Debug.Log("FightEvent: duringContact");
    }
    
    public void afterContact()
    {

    }
    
    public void beforeContact()
    {

    }
}