using UnityEngine;

public class InteractDemoUseCase : MonoBehaviour, IInteractObject
{
    public bool isActivate = false;
    public void execute()
    {
        isActivate = false;
        PlayerController.Instance.SetAbleWalk(false);
        PlayerInteractController.Instance.SetCanInteract(false);
        GetComponent<IInteractuable>().duringContact();
    }

    public bool isActivatedAlready()
    {
        return isActivate;
    }
}
