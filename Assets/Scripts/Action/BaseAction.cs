using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    protected Unit unit;
    protected bool isActive;

    protected Action OnActionCompleted;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action OnActionComplete);
    public virtual bool IsValidActionGridPosition(GridPosition gridPosition) 
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }
    public abstract List<GridPosition> GetValidActionGridPositionList();

    public virtual int GetActionPointsCost() => 1;

    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.OnActionCompleted = onActionComplete;
    }
    protected void ActionComplete()
    {
        isActive = false;
        OnActionCompleted();
    }
}
