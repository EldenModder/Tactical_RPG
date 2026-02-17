using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        Yellow,
        RedSoft
    }


    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
    [SerializeField] private Transform GridSystemVisualSinglePrefab;

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple Grid System Visual Instance ! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for(int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform =
                    Instantiate(
                        GridSystemVisualSinglePrefab, 
                        LevelGrid.Instance.GetWorldPosition(gridPosition), 
                        Quaternion.identity
                    );
                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }
        UpdateGridVisual();
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMoved += LevelGrid_OnAnyUnitMoved;
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e) => UpdateGridVisual();
    private void LevelGrid_OnAnyUnitMoved(object sender, EventArgs e) => UpdateGridVisual();

    public void HideallGridPosition()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }
    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType type)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for(int x = -range; x <= range; x++)
        {
            for( int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range) continue;
                gridPositionList.Add(testGridPosition);
            }
        }
        ShowGridPositionList(gridPositionList, type);
    }
    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType type)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(type));
        }
    }

    private void UpdateGridVisual()
    {
        HideallGridPosition();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance?.GetSelectedAction();
        GridVisualType gridVisualType;
        switch(selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRange(
                    selectedUnit.GetGridPosition(), 
                    shootAction.GetMaxShootDistance(), 
                    GridVisualType.RedSoft);
                break;
        }
        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridMaterial in gridVisualTypeMaterialList)
        {
            if (gridMaterial.gridVisualType == gridVisualType) return gridMaterial.material;
        }
        Debug.LogError("Could not find GridMaterial for GridVisualType" + gridVisualType);
        return null;
    }
}
