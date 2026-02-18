using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 2;

    public static event EventHandler OnAnyActionPointChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;
    private BaseAction[] baseActionArray;
    private HealthSystem healthSystem;


    private int actionPoint = ACTION_POINTS_MAX;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance?.AddUnitAtGridPosition(gridPosition, this);
        TurnManager.Instance.OnTurnChanged += TurnManager_OnTurnChanged;
        healthSystem.OnDeath += HealthSystem_OnDeath;
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
       GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        //unit changed Grid Position
        if (newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance?.UnitMoveGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    //Generics
    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction  in baseActionArray)
        {
            if (baseAction is T) return (T)baseAction;
        }
        return null;
    }

    #region Getter
    public GridPosition GetGridPosition() => gridPosition; 

    public Vector3 GetWorldPosition() => transform.position;

    public BaseAction[] GetBaseActionsArray() => baseActionArray;
    
    public int GetActionPoint() => actionPoint;

    public bool IsEnemy() => isEnemy;
    public void Damage(int amount) => healthSystem.Damage(amount);
    public float GetHealthNormalized() => healthSystem.GetNormalizedHealth();

    #endregion
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

    private void HealthSystem_OnDeath(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }
}
