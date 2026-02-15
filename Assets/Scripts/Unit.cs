using UnityEngine;

public class Unit : MonoBehaviour
{
    private Vector3 targetPosition;
    private Animator unitAnimator;
    private GridPosition gridPosition;

    private void Start()
    {
        unitAnimator = GetComponentInChildren<Animator>();
        targetPosition = transform.position;
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance?.AddUnitAtGridPosition(gridPosition, this);
    }

    private void Update()
    {
        float stoppingDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            float rotateSpeed = 10f;
            transform.forward = Vector3.Lerp(
                transform.forward, 
                moveDirection, 
                Time.deltaTime * rotateSpeed
            ); //smoothly rotate when moving

            unitAnimator.SetBool("IsWalking", true);
        }
        else unitAnimator.SetBool("IsWalking", false);

        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        //unit changed Grid Position
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance?.UnitMoveGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }

    }
    public void Move(Vector3 targetPosition) => this.targetPosition = targetPosition;
}
