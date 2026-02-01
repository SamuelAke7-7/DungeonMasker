using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class StartCombatUseCase : MonoBehaviour
{
    public Transform iuCombat;
    public static StartCombatUseCase Instance;
    private readonly float filedOfViewCombat = 85;
    private readonly float filedOfViewOutCombat = 60;
    [SerializeField] private float fovTransitionDuration = 0.5f;

    [Header("Minijuego combate")]
    [SerializeField] private float barWidth = 900f;
    [SerializeField] private float barHeight = 88f;
    [SerializeField] private float cursorSpeed = 0.8f;
    [SerializeField] private float zoneStartNormalized = 0.4f;
    [SerializeField] private float zoneEndNormalized = 0.6f;
    [SerializeField] private float damageToEnemy = 25f;
    [SerializeField] private float barOffsetFromBottom = 80f;
    [Tooltip("Cuánto aumenta la velocidad del cursor tras cada intento.")]
    [SerializeField] private float speedIncrementPerAttempt = 0.15f;
    [Tooltip("Velocidad máxima del cursor (0 = sin límite).")]
    [SerializeField] private float maxCursorSpeed = 3.5f;
    [Tooltip("Cuánto se reduce el ancho de la zona segura tras cada intento (en valor normalizado 0-1).")]
    [SerializeField] private float zoneWidthDecrementPerAttempt = 0.03f;
    [Tooltip("Ancho mínimo de la zona segura (0-1). Evita que sea imposible.")]
    [SerializeField] private float minZoneWidthNormalized = 0.03f;

    private GameObject _minigameCanvasRoot;
    private RectTransform _cursorRect;
    private RectTransform _zoneRect;
    private bool _combatActive;
    private float _currentZoneStart;
    private float _currentZoneEnd;

    void Awake(){
        Instance = this;
    }

    public void InitializeCombat(GameObject enemy){
        GameObject cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        Camera cam = cameraObj.GetComponent<Camera>();
        StartCoroutine(SmoothFieldOfView(cam, cam.fieldOfView, filedOfViewCombat));

        enemy.GetComponent<EnemyCombatUseCase>().InitCombat();
        _combatActive = true;
        RenderIuCombat(enemy);
    }

    /// <summary>Llama cuando quieras terminar el combate (ej: muerte del jugador o del enemigo).</summary>
    public void EndCombat(){
        _combatActive = false;
    }

    private IEnumerator SmoothFieldOfView(Camera cam, float fromFov, float toFov){
        float elapsed = 0f;
        while (elapsed < fovTransitionDuration){
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fovTransitionDuration);
            float smoothT = t * t * (3f - 2f * t); // SmoothStep para transición más suave
            cam.fieldOfView = Mathf.Lerp(fromFov, toFov, smoothT);
            yield return null;
        }
        cam.fieldOfView = toFov;
    }

    private void RenderIuCombat(GameObject enemy){
        if (_minigameCanvasRoot != null){
            Destroy(_minigameCanvasRoot);
        }
        CreateCombatMinigameUI();
        StartCoroutine(RunCombatMinigame(enemy));
    }

    private void CreateCombatMinigameUI(){
        _minigameCanvasRoot = new GameObject("CombatMinigameCanvas");
        var canvas = _minigameCanvasRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _minigameCanvasRoot.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _minigameCanvasRoot.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        _minigameCanvasRoot.AddComponent<GraphicRaycaster>();

        var panel = new GameObject("Panel");
        panel.transform.SetParent(_minigameCanvasRoot.transform, false);
        var panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0f);
        panelRect.anchorMax = new Vector2(0.5f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.sizeDelta = new Vector2(barWidth + 60, 140);
        panelRect.anchoredPosition = new Vector2(0, barOffsetFromBottom);

        float halfBar = barWidth * 0.5f;
        float barLocalY = 50f;

        var horizontalBar = new GameObject("HorizontalBar");
        horizontalBar.transform.SetParent(panel.transform, false);
        var barRect = horizontalBar.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0.5f, 0f);
        barRect.anchorMax = new Vector2(0.5f, 0f);
        barRect.pivot = new Vector2(0.5f, 0.5f);
        barRect.sizeDelta = new Vector2(barWidth, barHeight);
        barRect.anchoredPosition = new Vector2(0, barLocalY);
        horizontalBar.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

        var zoneWidth = (zoneEndNormalized - zoneStartNormalized) * barWidth;
        var zoneCenterX = (zoneStartNormalized + zoneEndNormalized - 1f) * halfBar;
        var sweetSpot = new GameObject("SweetSpotZone");
        sweetSpot.transform.SetParent(panel.transform, false);
        _zoneRect = sweetSpot.AddComponent<RectTransform>();
        _zoneRect.anchorMin = new Vector2(0.5f, 0f);
        _zoneRect.anchorMax = new Vector2(0.5f, 0f);
        _zoneRect.pivot = new Vector2(0.5f, 0.5f);
        _zoneRect.sizeDelta = new Vector2(zoneWidth, barHeight);
        _zoneRect.anchoredPosition = new Vector2(zoneCenterX, barLocalY);
        sweetSpot.AddComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f, 0.9f);

        var cursor = new GameObject("VerticalCursor");
        cursor.transform.SetParent(panel.transform, false);
        _cursorRect = cursor.AddComponent<RectTransform>();
        _cursorRect.anchorMin = new Vector2(0.5f, 0f);
        _cursorRect.anchorMax = new Vector2(0.5f, 0f);
        _cursorRect.pivot = new Vector2(0.5f, 0.5f);
        _cursorRect.sizeDelta = new Vector2(10, barHeight + 16);
        _cursorRect.anchoredPosition = new Vector2(-halfBar, barLocalY);
        cursor.AddComponent<Image>().color = Color.white;
    }

    private void RandomizeZone(float barLocalY, float currentZoneWidthNormalized){
        float halfWidth = currentZoneWidthNormalized * 0.5f;
        float center = Random.Range(halfWidth, 1f - halfWidth);
        _currentZoneStart = center - halfWidth;
        _currentZoneEnd = center + halfWidth;

        float halfBar = barWidth * 0.5f;
        float zoneWidthPx = currentZoneWidthNormalized * barWidth;
        float zoneCenterX = (_currentZoneStart + _currentZoneEnd - 1f) * halfBar;
        _zoneRect.sizeDelta = new Vector2(zoneWidthPx, barHeight);
        _zoneRect.anchoredPosition = new Vector2(zoneCenterX, barLocalY);
    }

    private IEnumerator RunCombatMinigame(GameObject enemy){
        float halfBar = barWidth * 0.5f;
        float barLocalY = 50f;
        float currentSpeed = cursorSpeed;
        float currentZoneWidth = Mathf.Clamp01(zoneEndNormalized - zoneStartNormalized);
        while (_combatActive && enemy != null){
            var enemyCombat = enemy.GetComponent<EnemyCombatUseCase>();
            if (enemyCombat == null || enemyCombat.life <= 0f){
                break;
            }

            currentZoneWidth = Mathf.Max(minZoneWidthNormalized, currentZoneWidth);
            RandomizeZone(barLocalY, currentZoneWidth);
            float cursorT = 0f;
            int direction = 1;
            bool attemptResolved = false;

            while (!attemptResolved && _combatActive && enemy != null){
                cursorT += direction * currentSpeed * Time.deltaTime;
                if (cursorT >= 1f){ cursorT = 1f; direction = -1; }
                if (cursorT <= 0f){ cursorT = 0f; direction = 1; }

                float cursorX = Mathf.Lerp(-halfBar, halfBar, cursorT);
                _cursorRect.anchoredPosition = new Vector2(cursorX, barLocalY);

                bool pressed = (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                    || (Gamepad.current != null && Gamepad.current.xButton.wasPressedThisFrame);

                if (pressed){
                    if (cursorT >= _currentZoneStart && cursorT <= _currentZoneEnd){
                        enemyCombat.TakeDamage(damageToEnemy);
                        attemptResolved = true;
                    }
                    else{
                        Debug.Log("Jugador recibió daño");
                        enemyCombat.DoDamage();
                        attemptResolved = true;
                    }
                }

                yield return null;
            }

            currentSpeed += speedIncrementPerAttempt;
            if (maxCursorSpeed > 0f){
                currentSpeed = Mathf.Min(currentSpeed, maxCursorSpeed);
            }

            currentZoneWidth -= zoneWidthDecrementPerAttempt;
            currentZoneWidth = Mathf.Max(minZoneWidthNormalized, currentZoneWidth);

            if (enemyCombat != null && enemyCombat.life <= 0f){
                enemyCombat.Die();
                break;
            }
        }

        if (_minigameCanvasRoot != null){
            Destroy(_minigameCanvasRoot);
            _minigameCanvasRoot = null;
        }
        PlayerController.Instance.SetAbleWalk(true);
        PlayerInteractController.Instance.SetCanInteract(true);
        GameObject cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        Camera cam = cameraObj.GetComponent<Camera>();
        StartCoroutine(SmoothFieldOfView(cam, cam.fieldOfView, filedOfViewOutCombat));
    }
}
