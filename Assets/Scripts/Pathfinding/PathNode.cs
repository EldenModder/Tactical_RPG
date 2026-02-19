public class PathNode
{
    private GridPosition GridPosition;
    private int gCost, hCost, fCost;
    private PathNode cameFromPathNode;
    public PathNode(GridPosition GridPosition)
        { this.GridPosition = GridPosition; }

    public override string ToString()
    {
        return GridPosition.ToString();
    }
    #region Getter
    public int GetGCost() => gCost;
    public int GetHCost() => hCost;
    public int GetFCost() => fCost;
    public GridPosition GetGridPosition() => GridPosition;
    public PathNode GetCameFromPathNode() => cameFromPathNode;
    #endregion

    #region Setter
    public void SetGCost(int GCost) => this.gCost = GCost;
    public void SetHCost(int HCost) => this.hCost = HCost;
    public void SetCameFromPathNode(PathNode pathNode) => cameFromPathNode = pathNode;
    #endregion
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
    public void ResetCameFromPathNode() => cameFromPathNode = null;
}
