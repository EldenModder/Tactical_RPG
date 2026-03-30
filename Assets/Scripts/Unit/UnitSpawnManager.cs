using UnityEngine;

public class UnitSpawnManager : MonoBehaviour
{
    public static UnitSpawnManager instance;

    [SerializeField] private Transform selectedUnitPrefab;
    private Unit UnitToGet;

    [SerializeField] private int availableUnitCount = 1;

    private void Awake()
    {
        instance = this;
    }
    
    public void SetSelectedUnit(Transform UnitPrefab) => selectedUnitPrefab = UnitPrefab;
    public Unit GetSelectedUnit() => UnitToGet;

    public void SpawnUnit(GridPosition gridPosition)
    {
        if (selectedUnitPrefab == null || availableUnitCount <= 0)
        {
            Debug.LogWarning("No Unit Selected");
            return;
        }
        availableUnitCount--;
        Vector3 worldPos = LevelGrid.Instance.GetWorldPosition(gridPosition);
        Transform Unit = Instantiate(selectedUnitPrefab, worldPos, Quaternion.identity);
        Unit unitComponent = Unit.GetComponent<Unit>();
        UnitToGet = unitComponent;
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, unitComponent);
        UnitActionSystem.Instance.SetSelectedUnit(unitComponent);
    }
}
