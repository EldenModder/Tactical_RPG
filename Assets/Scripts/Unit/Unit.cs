using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 2;

    public static event EventHandler OnAnyActionPointChanged;

    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;
    private MoveAction moveAction;
    private BaseAction[] baseActionArray;
    private int actionPoint = ACTION_POINTS_MAX;

    private void Awake()
    {
        moveAction = GetComponent<MoveAction>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance?.AddUnitAtGridPosition(gridPosition, this);
        TurnManager.Instance.OnTurnChanged += TurnManager_OnTurnChanged;
    }

    private void Update()
    {
       GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        //unit changed Grid Position
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance?.UnitMoveGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }

    public MoveAction GetMoveAction() => moveAction;

    public GridPosition GetGridPosition() => gridPosition; 

    public Vector3 GetWorldPosition() => transform.position;

    public BaseAction[] GetBaseActionsArray() => baseActionArray;

    public bool CanSpendActionPointToTakeAction(BaseAction baseAction)
    {
        if (actionPoint >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TrySpendActionPoint(BaseAction baseAction)
    {
        if (CanSpendActionPointToTakeAction(baseAction))
        {
            SpendActionPoint(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }
    private void SpendActionPoint(int amount)
    {
        actionPoint -= amount;
        OnAnyActionPointChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoint() => actionPoint;

    private void TurnManager_OnTurnChanged(object sender, EventArgs e)
    {
        //if it a enemy and it's enemy turn or if it's not a enemy and it's player turn 
        // refresh the action point
        if ((IsEnemy() && !TurnManager.Instance.IsPlayerTurn()) ||
            (!IsEnemy() && TurnManager.Instance.IsPlayerTurn()))
        {
            actionPoint = ACTION_POINTS_MAX;
            OnAnyActionPointChanged?.Invoke(this, EventArgs.Empty);
        }
        
    }

    public bool IsEnemy() => isEnemy;

    public void Damage() { }
}
