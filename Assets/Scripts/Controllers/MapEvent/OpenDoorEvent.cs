using UnityEngine;
using System.Collections;
using TMPro;

public class OpenDoorEvent : MonoBehaviour, IInteractuable
{
    private string messagePositivo = "Has Logrado abrir la puerta";
    private string messageNegativo = "No tienes la llave para abrir la puerta";
    private MapEventType mapEventType = MapEventType.Treasure;
    public int llaveNecesaria;

    public void duringContact()
    {
        // text.text = message;
        // text.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
        MapEventManager.instance.panel.SetActive(true);
        MapEventManager.instance.panel.GetComponentInChildren<TextMeshProUGUI>().text = messagePositivo;

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
        Vector2Int gridCell = GridMapController.Instance.WorldToGrid(transform.position);
        GridMapController.Instance.ChangeTypeCell(gridCell.x,gridCell.y,CellType.Path);
        Destroy(gameObject);
    }
    
    public void beforeContact()
    {
        // text.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
    }
}
