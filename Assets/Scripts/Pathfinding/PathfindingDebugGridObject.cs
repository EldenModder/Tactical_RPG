using TMPro;
using UnityEngine;

public class PathfindingDebugGridObject : GridDebugObject
{
    [SerializeField] private TextMeshProUGUI gCostText;
    [SerializeField] private TextMeshProUGUI hCostText;
    [SerializeField] private TextMeshProUGUI fCostText;
    [SerializeField] private SpriteRenderer IsWalkableSpriteRenderer;

    private PathNode pathNode;

    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        pathNode = (PathNode)gridObject;
    }

    protected override void Update()
    {
        base.Update();
        gCostText.text = pathNode.GetGCost().ToString();
        hCostText.text = pathNode.GetHCost().ToString();
        fCostText.text = pathNode.GetFCost().ToString();
        IsWalkableSpriteRenderer.color = pathNode.IsWalkable() ? Color.green : Color.red;
    }
}
