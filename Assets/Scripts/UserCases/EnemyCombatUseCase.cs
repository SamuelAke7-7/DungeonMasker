using UnityEngine;

public class EnemyCombatUseCase : MonoBehaviour
{
    public float life = 100;
    public float strength = 25;
    public Animator animManager;

    void Start(){
        animManager = transform.GetChild(0).GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void InitCombat(){
        animManager.SetTrigger("InitAttack");
    }

    public void DoDamage(){
        animManager.SetTrigger("IsAttacking");
    }

    public void TakeDamage(float amount){
        life = Mathf.Max(0, life - amount);
    }

    public void Die(){
        Destroy(gameObject);
    }
}
