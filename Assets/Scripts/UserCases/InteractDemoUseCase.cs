using UnityEngine;

public class InteractDemoUseCase : MonoBehaviour, IInteractObject
{
    public void execute()
    {
        Debug.Log("Esta recogiendo un cofre");
    }

    public bool isActivatedAlready()
    {
        return false;
    }
}
