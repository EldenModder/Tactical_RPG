using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }
    private const int MOVE_STRAIGHT_COST = 10;
    //private const int MOVE_DIAGONAL_COST = 14;

    private int width, height;
    private float cellSize;
    private GridSystem<PathNode> GridSystem;

    [SerializeField] private Transform gridDebugObject;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple Pathfinding Instance ! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        GridSystem = new GridSystem<PathNode>(10, 10, 2f,
            (GridSystem<PathNode> g, GridPosition GridPosition) => new PathNode(GridPosition));
        GridSystem.CreateDebugObjects(gridDebugObject);
        width = GridSystem.GetWidth();
        height = GridSystem.GetHeight();
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closeList = new List<PathNode>();

        PathNode startNode = GridSystem.GetGridObject(startGridPosition);
        PathNode endNode = GridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < GridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < GridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = GridSystem.GetGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();
        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                //Reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closeList.Contains(neighbourNode)) continue;
                int tentativeGCost = 
                    currentNode.GetGCost() +
                    CalculateDistance(
                        currentNode.GetGridPosition(),
                        neighbourNode.GetGridPosition()
                    );
                if (tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(),endGridPosition));
                    neighbourNode.CalculateFCost();
                    if (!openList.Contains(neighbourNode)) openList.Add(neighbourNode);
                }
            }
        }
        //could not find a path
        Debug.Log("could not find a path");
        return null;
    }

    public int CalculateDistance(GridPosition GridPositionA, GridPosition GridPositionB)
    {
        GridPosition gridPositionDistance = GridPositionA - GridPositionB;
        int xdistance = Mathf.Abs(gridPositionDistance.x);
        int zdistance = Mathf.Abs(gridPositionDistance.z);
        return (xdistance + zdistance) * MOVE_STRAIGHT_COST;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() <  lowestFCostPathNode.GetFCost()) lowestFCostPathNode = pathNodeList[i];
        }
        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z) => GridSystem.GetGridObject(new GridPosition(x, z));

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();
        
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                if (dx == 0 && dz == 0) continue; //self is not a neighbour
                if ((gridPosition.x + dx) < 0 || (gridPosition.x + dx) >= width) continue;//Out of width bound
                if ((gridPosition.z + dz) < 0 || (gridPosition.z + dz) >= height) continue; //Out of height bound
                if (Mathf.Abs(dx) + Mathf.Abs(dz) > 1) continue;
                PathNode neighbourNode = GetNode(gridPosition.x + dx, gridPosition.z + dz);
                neighbourList.Add(neighbourNode);
            }
        }
        return neighbourList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new() { endNode };
        PathNode currentNode = endNode;
        while (currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }
        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }
        return gridPositionList;
    }
}
