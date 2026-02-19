using System;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private int maxThrowDistance = 7;
    [SerializeField] private Transform GrenadePrefab;

    private void Update()
    {
        if (!isActive) return;
    }

    public override string GetActionName() => "Grenade";

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        //cycle trought all potential grid position within the max move range
        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue; //test if it's valid
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxThrowDistance) continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action OnActionComplete)
    {
        Transform grenadeProjectileTransform = Instantiate(GrenadePrefab, unit.GetWorldPosition(), Quaternion.identity);
        Grenade grenade = grenadeProjectileTransform.GetComponent<Grenade>();
        grenade.Setup(gridPosition, OnGrenadeExplode);
        ActionStart(OnActionComplete);
    }

    private void OnGrenadeExplode() => ActionComplete();
}
