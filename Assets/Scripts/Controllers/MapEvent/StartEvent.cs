using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;


public class StartEvent : MonoBehaviour, IInteractuable
{
    public Sprite sprite;
    public float timeToShowText = 3f;
    public bool isMainDialogue = false;
    public string[] sentences = {
        "OH! otro maldito humano!",
    "¡Ay! ¡Ay! ¡Ay! YO TE MALDIGO!!!",
    "Y te sentencio a usar esta mascara hasta tu muerte!!!!"};
    
    private MapEventType mapEventType = MapEventType.Start_floor;

    public Color colorClaro = new Color(1f, 0f, 0f); // Rojo puro
    public Color colorOscuro = new Color(0.3f, 0f, 0f); // Rojo sangre oscura

    public void duringContact()
    {
        Debug.Log("StartEvent: duringContact");
        if(MapEventManager.instance.ghostImage != null){
            MapEventManager.instance.panel.GetComponentInChildren<TextMeshProUGUI>().text = "";
            MapEventManager.instance.ghostImage.gameObject.GetComponent<Image>().sprite = sprite;
            MapEventManager.instance.ghostImage.gameObject.SetActive(true);
            MapEventManager.instance.panel.SetActive(true);
            if (isMainDialogue){
                StartCoroutine(ShowText(MapEventManager.instance.panel.GetComponentInChildren<TextMeshProUGUI>(), 0.05f));
            } else {
                NormalShowText(0.05f);
            }
            
        }else{
            Debug.Log("StartEvent: duringContact not executed due to null ghostImage");
        }
        
    }
    
    public void afterContact()
    {
        if(MapEventManager.instance.ghostImage.gameObject != null){
            MapEventManager.instance.panel.SetActive(false);
            MapEventManager.instance.ghostImage.gameObject.SetActive(false);
            //Destroy(MapEventManager.instance.ghostImage.gameObject);
        }
        PlayerController.Instance.SetAbleWalk(true);
        PlayerInteractController.Instance.SetCanInteract(true);
    }
    
    public void beforeContact()
    {
        
    }

    IEnumerator TimerEnd()
    {
        yield return new WaitForSeconds(timeToShowText);
        afterContact();
    }

    IEnumerator ShowText(TextMeshProUGUI textComponent, float delay)
    {
        Debug.Log("ShowText");
        foreach(string s in sentences)
        {
            Debug.Log("ShowText: " + s);
            string hexColor = ColorUtility.ToHtmlStringRGB(colorOscuro);
            foreach (char c in s)
            {

                if( c == ' '){
                    Color randomRed = Color.Lerp(colorOscuro, colorClaro, Random.value);
                    hexColor = ColorUtility.ToHtmlStringRGB(randomRed);
                }
                
                textComponent.text += $"<color=#{hexColor}>{c}</color>";

                yield return new WaitForSeconds(delay);
            }
        }
        StartCoroutine(TimerEnd());
    }

    void NormalShowText(float delay)
    {
        MapEventManager.instance.panel.GetComponentInChildren<TextMeshProUGUI>().text = sentences[0];
        StartCoroutine(TimerEnd());
    }
}