using UnityEngine;
using System.Collections;
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
        Destroy(gameObject);
    }
    
    public void beforeContact()
    {
        // text.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
    }
}