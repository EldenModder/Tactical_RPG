using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] private Transform GridSystemVisualSinglePrefab;

    private void Start()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for(int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Instantiate(GridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
            }
        }
    }
}
