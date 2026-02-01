using UnityEngine;
using UnityEngine.UI; // Necesario para el componente Slider

public class LifebarManager : MonoBehaviour
{
    public float vida = 100f;
    public Slider sliderVida;
    public static LifebarManager instance;
    public RawImage deathScreen;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Configuramos el valor máximo del slider al iniciar
        if (sliderVida != null)
        {
            sliderVida.maxValue = 100f;
            sliderVida.value = vida;
        }
    }

    void Update()
    {
        // Mantiene el visual del Slider sincronizado con la variable vida
        if (sliderVida != null)
        {
            sliderVida.value = vida;
        }
    }

    // Método útil para llamar desde otros scripts (ej. al recibir daño o curarse)
    public void addLife(float cantidad)
    {
        vida += cantidad;
        // Mantenemos la vida siempre entre 0 y 100
        vida = Mathf.Clamp(vida, 0f, 100f);
        
    }

    public void removeLife(float cantidad)
    {
        vida -= cantidad;
        // Mantenemos la vida siempre entre 0 y 100
        vida = Mathf.Clamp(vida, 0f, 100f);
    }
}