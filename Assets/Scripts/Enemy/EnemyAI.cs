using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private float timer;

    private void Start()
    {
        TurnManager.Instance.OnTurnChanged += TurnManager_OnTurnChanged;
    }
    private void Update()
    {
        if (TurnManager.Instance.IsPlayerTurn()) return;

        timer -= Time.deltaTime;
        if (timer <= 0f) TurnManager.Instance?.NextTurn();
    }

    private void TurnManager_OnTurnChanged(object sender, EventArgs e)
    {
        timer = 2f;
    }
}
