using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    public static MouseWorld instance;
    [SerializeField] private LayerMask mousePlaneLayerMask;
    private void Awake()
    {
        instance = this;
    }
    void Update()
    {
        transform.position = MouseWorld.GetPosition();
        if (Input.GetMouseButtonDown(0)) HandleClick();
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, instance.mousePlaneLayerMask);
        return hit.point;
    }

    private void HandleClick()
    {
        Vector3 mouseWorldPos = MouseWorld.GetPosition();
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(mouseWorldPos);
        if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition))
        {
            UnitSpawnManager.instance.SpawnUnit(gridPosition);
        }
    }
}
