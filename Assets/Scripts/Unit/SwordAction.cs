using System;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    public static event EventHandler OnAnySwordHit;

    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;
    


    [SerializeField] private int Damage = 100;

    private int maxSwordDistance = 1;
    private float stateTimer;
    private Unit targetUnit;
    private enum State
    {
        SwiggingBeforeHit,
        SwiggingAfterHit
    } private State state;

    private void Update()
    {
        if (!isActive) return;
        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.SwiggingBeforeHit:
                Rotate();
                break;
            case State.SwiggingAfterHit:
                break;
        }
        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.SwiggingBeforeHit:
                state = State.SwiggingAfterHit;
                float AfterHitStateTime = 0.5f;
                stateTimer = AfterHitStateTime;
                targetUnit.Damage(Damage);
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwiggingAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                OnActionCompleted();
                break;
        }
    }
    private void Rotate()
    {
        float rotateSpeed = 10f;
        Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.transform.position).normalized;
        transform.forward = Vector3.Lerp(
            transform.forward,
            aimDir,
            Time.deltaTime * rotateSpeed
        ); //smoothly rotate toward target
    }

  

    #region Implementation
    public override string GetActionName() => "Sword";
    public int GetMaxSwordDistance() => maxSwordDistance;

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        //cycle trought all potential grid position within the max move range
        for (int x = -maxSwordDistance; x <= maxSwordDistance; x++)
        {
            for (int z = -maxSwordDistance; z <= maxSwordDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue; //test if it's valid

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue; //position empty

                Unit targetUnit = LevelGrid.Instance?.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.IsEnemy() == unit.IsEnemy()) continue; //Both unit on same team

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action OnActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        state = State.SwiggingBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;
        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(OnActionComplete);
    }

    #endregion
}
