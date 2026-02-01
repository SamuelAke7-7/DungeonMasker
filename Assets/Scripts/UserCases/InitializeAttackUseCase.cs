using UnityEngine;

public class InitializeAttackUseCase : MonoBehaviour, IInteractObject
{
    private bool isActivate = true;
    public void execute()
    {
        isActivate = false;
        PlayerController.Instance.SetAbleWalk(false);
        PlayerInteractController.Instance.SetCanInteract(false);
        StartCombatUseCase.Instance.InitializeCombat(transform.gameObject);
    }

    public bool isActivatedAlready(){
        return isActivate;
    }
}
