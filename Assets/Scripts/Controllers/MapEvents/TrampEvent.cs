using UnityEngine;
using UnityEngine.UI;


public class TrampEvent : MonoBehaviour, IInteractuable
{

    public RawImage ghost;
    public RawImage damage_frame;
    private MapEventType mapEventType = MapEventType.Tramp;

    public void duringContact()
    {
        ghost.enabled = true;
        damage_frame.enabled = true;

        Debug.Log("TrampEvent: duringContact");
    }
    
    public void afterContact()
    {
        ghost.enabled = false;
        damage_frame.enabled = false;
    }
    
    public void beforeContact()
    {
        ghost.enabled = false;
        damage_frame.enabled = false;
    }
}