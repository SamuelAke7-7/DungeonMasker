using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DeadEvent : MonoBehaviour, IInteractuable
{

    public string[] sentences = {"Tienes Problemas!"};
    
    private MapEventType mapEventType = MapEventType.Fighting;
    public RawImage deathScreen;
    public static DeadEvent instance;

    private void Awake()
    {
        instance = this;
    }

    public void duringContact()
    {


        Debug.Log("DeadEvent: duringContact");
    }

    public void showDeathScreen()
    {
        deathScreen.enabled = true;
    }
    
    public void afterContact()
    {
        
    }
    
    public void beforeContact()
    {
        
    }
}