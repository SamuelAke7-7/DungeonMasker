using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealEvent : MonoBehaviour, IInteractuable
{
    private string message = "AUCH!!!!!";
    public float lifeGained = 10f;
    private MapEventType mapEventType = MapEventType.Trap;

    public void duringContact()
    {
        Debug.Log("HealEvent: Starting");

        PlayerController.Instance.addLife(lifeGained);
        MapEventManager.instance.healedImage.gameObject.SetActive(true);
        Debug.Log("HealEvent: Player gained " + lifeGained + " life");
    }
    
    public void afterContact()
    {
        MapEventManager.instance.healedImage.gameObject.SetActive(false);
    }
    
    public void beforeContact()
    {
      
    }
}