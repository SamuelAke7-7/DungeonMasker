using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasBlinker : MonoBehaviour
{
    public Image damageImage; // Arrastra aquí tu 'DamageOverlay'
    public float blinkDuration = 0.2f; // Qué tan rápido parpadea

    public void TriggerBlink()
    {
        StopAllCoroutines(); // Por si recibe varios golpes seguidos
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        // 1. Aparece el rojo (subimos el Alpha)
        damageImage.color = new Color(1f, 0f, 0f, 0.5f); 

        float elapsed = 0f;
        Color startColor = damageImage.color;
        Color endColor = new Color(1f, 0f, 0f, 0f);

        // 2. Se desvanece suavemente
        while (elapsed < blinkDuration)
        {
            elapsed += Time.deltaTime;
            damageImage.color = Color.Lerp(startColor, endColor, elapsed / blinkDuration);
            yield return null;
        }

        damageImage.color = endColor; // Aseguramos que quede en 0
    }
}