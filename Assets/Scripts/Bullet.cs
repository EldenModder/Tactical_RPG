using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Transform HitVFX;
    private TrailRenderer TrailRenderer;
    private Vector3 targetPosition;
    private void Awake()
    {
        TrailRenderer = GetComponentInChildren<TrailRenderer>();
    }
    public void Setup(Vector3 targetPos) => this.targetPosition = targetPos;

    private void Update()
    {
        Vector3 moveDir = (targetPosition - transform.position).normalized;
        float distBeforeMoving = Vector3.Distance(transform.position, targetPosition);
        float moveSpeed = 200f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        float distAfterMoving = Vector3.Distance(transform.position, targetPosition);
        if (distBeforeMoving < distAfterMoving)
        {
            transform.position = targetPosition;
            TrailRenderer.transform.parent = null;
            Destroy(gameObject);
            Instantiate(HitVFX, targetPosition, Quaternion.identity);
        }
    }
}
