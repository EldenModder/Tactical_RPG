using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            GridPosition currentUnitPosition = UnitActionSystem.Instance.GetSelectedUnit().GetGridPosition(); 
            List<GridPosition> gridPositionList = Pathfinding.Instance.FindPath(currentUnitPosition, mouseGridPosition);
            if (gridPositionList == null) Debug.Log("gridPositionList null");
            if (gridPositionList != null && gridPositionList.Count <= 0) Debug.Log("gridPositionList empty");
            if (gridPositionList != null && gridPositionList.Count > 0)
            {
                for (int i = 0; i < gridPositionList.Count - 1; i++) 
                {
                    Debug.DrawLine(
                        LevelGrid.Instance.GetWorldPosition(gridPositionList[i]),
                        LevelGrid.Instance.GetWorldPosition(gridPositionList[i + 1]),
                        Color.white,
                        10f
                    );
                };
            }
            
        }
    }
}
