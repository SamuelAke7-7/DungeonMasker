using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuIUController : MonoBehaviour
{
    public Transform panelMainMenu;
    public Transform panelCredits;

    public void StartGame(){
        SceneManager.LoadScene("LevelDesing");
    }

    public void RenderCredits(){
        panelMainMenu.gameObject.SetActive(false);
        panelCredits.gameObject.SetActive(true);
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void ReturnMenu(){
        panelMainMenu.gameObject.SetActive(true);
        panelCredits.gameObject.SetActive(false);
    }
}
