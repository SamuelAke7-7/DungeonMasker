using UnityEngine;

public class ObjectMaskBehaviourUseCase : MonoBehaviour
{
    public TypeMask type;
    public void execute(TypeMask type)
    {
        if (type == this.type){
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
        } else {
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<Collider>().enabled = false;
        }
    }
}
