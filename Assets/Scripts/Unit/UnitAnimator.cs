using System;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform ShootPoint;
    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform swordTransform;
    private Animator unitAnimator;

    private void Start()
    {
        EquipRifle();
    }

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
        if (TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += swordAction_OnSwordActionStarted;
            swordAction.OnSwordActionCompleted += swordAction_OnSwordActionCompleted;
        }
    }
    private void swordAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();
        unitAnimator.SetTrigger("SwordSlash");
    }
    private void swordAction_OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
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

    private void EquipSword()
    {
        swordTransform.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
    }

    private void EquipRifle()
    {
        swordTransform.gameObject.SetActive(false);
        rifleTransform.gameObject.SetActive(true);
    }

}
