using System;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;


    [SerializeField] private int damage = 30;
    [SerializeField] private Transform ExplosionVFX;
    [SerializeField] private TrailRenderer Trail;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    private Vector3 targetPosition;
    private Vector3 positionXZ;
    private Action onGrenadeExplode;
    private float totalDistance;
    public void Setup(GridPosition targetGridPosition, Action onGrenadeExplode)
    { 
        this.onGrenadeExplode = onGrenadeExplode;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }

    private void Update()
    {
        Vector3 moveDir = (targetPosition - positionXZ).normalized;

        float moveSpeed = 15f;
        positionXZ += moveDir * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;

        float maxHeight = totalDistance / 4f;
        float posY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, posY, positionXZ.z);

        float reachTargetDistance = .2f;
        if (Vector3.Distance(positionXZ, targetPosition) < reachTargetDistance)
        {
            float damageRadius = 4f;
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(damage);
                }
            }
            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);
            Trail.transform.parent = null;
            Instantiate(ExplosionVFX, targetPosition + Vector3.up, Quaternion.identity);
            Destroy(gameObject);
            onGrenadeExplode();
        }
    }
}
