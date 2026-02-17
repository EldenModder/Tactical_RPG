using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Waiting,
        TakingTurn,
        Busy
    } private State state;

    private float timer;

    private void Awake()
    {
        state = State.Waiting;
    }

    private void Start()
    {
        TurnManager.Instance.OnTurnChanged += TurnManager_OnTurnChanged;
    }
    private void Update()
    {
        if (TurnManager.Instance.IsPlayerTurn()) return;
        switch(state)
        {
            case State.Waiting:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    state = State.Busy;
                    if (TryTakeAction(SetStateTakingTurn)) state = State.Busy;
                    else TurnManager.Instance?.NextTurn(); //no more enemy have action
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnManager_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnManager.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;
        }
    }

    private bool TryTakeAction(Action OnActionComplete)
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if (TryTakeAction(enemyUnit, OnActionComplete)) return true;
        }
        return false;
    }

    private bool TryTakeAction(Unit enemyUnit, Action onActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;
        foreach (BaseAction baseAction in enemyUnit.GetBaseActionsArray())
        {
            //enemy can't afford action
            if (!enemyUnit.CanSpendActionPointToTakeAction(baseAction)) continue;
            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }
        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPoint(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onActionComplete);
            return true;
        }
        else return false;
    }
}
