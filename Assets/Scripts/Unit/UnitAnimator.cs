using System;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform ShootPoint;
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

    private void shootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        unitAnimator.SetTrigger("Shoot");
        Transform bulletProjectileTransform = 
            Instantiate(bulletProjectilePrefab, ShootPoint.transform.position, Quaternion.identity);
        Bullet bullet = bulletProjectileTransform.GetComponent<Bullet>();
        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        targetUnitShootAtPosition.y = ShootPoint.transform.position.y;
        bullet.Setup(targetUnitShootAtPosition);
    }

}
