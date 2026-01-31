using UnityEngine;

public class InitializeAttackUseCase : MonoBehaviour, IInteractObject
{
    public void execute()
    {
        PlayerController.Instance.SetAbleWalk(false);
    }

    public bool isActivatedAlready(){
        return true;
    }
}
