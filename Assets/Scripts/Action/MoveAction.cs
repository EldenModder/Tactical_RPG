using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    [SerializeField] private int maxMoveDistance = 4;

    private List<Vector3> PositionList;
    private int currentPositionIndex;

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;


    void Update()
    {
        if (!isActive) return;
        Vector3 targetPosition = PositionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(
            transform.forward,
            moveDirection,
            Time.deltaTime * rotateSpeed
        ); //smoothly rotate when moving
        float stoppingDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            float moveSpeed = 4f;
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= PositionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
            
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action OnActionCompleted)
    {
        List<GridPosition> pathGridPositionList =  Pathfinding.Instance?.FindPath(
            unit.GetGridPosition(), 
            gridPosition,
            out int pathLenght
        );

        currentPositionIndex = 0;
        PositionList = new List<Vector3>();
        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            PositionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
            LevelGrid.Instance.RemoveUnitAtGridPosition(pathGridPosition, UnitSpawnManager.instance.GetSelectedUnit());
        }
        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(OnActionCompleted);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        //cycle trought all potential grid position within the max move range
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue; //test if it's valid
                if (unitGridPosition == testGridPosition) continue; //same position where the unit is
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue; //position already occupied

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition)) continue;
                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition)) continue;
                int pathfindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLenght(
                        unitGridPosition, 
                        testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier
                )continue; //path lenght too low
                
                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }

    public override string GetActionName() => "Move";
}
