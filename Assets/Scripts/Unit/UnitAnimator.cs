using System;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private Animator unitAnimator;
    private void Awake()
    {
        unitAnimator = GetComponentInChildren<Animator>();
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }
        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += shootAction_OnShoot;
        }
    }
    
    private void MoveAction_OnStartMoving(object sender, EventArgs e)
        => unitAnimator.SetBool("IsWalking", true);
    private void MoveAction_OnStopMoving(object sender, EventArgs e)
        => unitAnimator.SetBool("IsWalking", false);

    private void shootAction_OnShoot(object sender, EventArgs e)
        => unitAnimator.SetTrigger("Shoot");

}
