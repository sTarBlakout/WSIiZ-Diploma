using UnityEngine;

public class GameArea : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Transform pawnsHolder;

    private void Start()
    {
        CenterPawns();
    }

    [ContextMenu("Center Pawns")]
    public void CenterPawns()
    {
        foreach (Transform pawn in pawnsHolder)
        {
            var cellPosition = grid.WorldToCell(pawn.position);
            pawn.position = grid.GetCellCenterWorld(cellPosition);
        }
    }
}
