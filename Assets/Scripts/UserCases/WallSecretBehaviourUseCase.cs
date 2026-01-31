using UnityEngine;

public class WallSecretBehaviourUseCase : MonoBehaviour, IToggleShowObject
{
    public TypeMask type;
    public void execute(TypeMask type)
    {
        Vector2Int gridCell = GridMapController.Instance.WorldToGrid(transform.position);
        if (type == this.type){
            GetComponent<MeshRenderer>().enabled = false;
            GridMapController.Instance.ChangeTypeCell(gridCell.x,gridCell.y,CellType.Path);
        } else {
            GetComponent<MeshRenderer>().enabled = true;
            GridMapController.Instance.ChangeTypeCell(gridCell.x,gridCell.y,CellType.WallChanger);
        }
    }
}
