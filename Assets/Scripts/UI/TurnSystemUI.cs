using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private Button EndTurnButton;

    private void Start()
    {
        EndTurn();
    }

    private void EndTurn()
    {
        EndTurnButton.onClick.AddListener(() =>
        {
            TurnManager.Instance?.NextTurn();
        });
        TurnManager.Instance.OnTurnChanged += TurnManager_OnTurnChanged;
        UpdateTurnText();
        UpdateEndTurnButtonVisibility();
    }

    private void TurnManager_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateTurnText()
    {
        turnText.text = "Turn: " + TurnManager.Instance?.GetTurnNumber().ToString();
    }

    private void UpdateEndTurnButtonVisibility()
    {
        EndTurnButton.gameObject.SetActive(TurnManager.Instance.IsPlayerTurn());
    }
}
