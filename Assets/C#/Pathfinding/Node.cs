using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int x, y;
    public Vector3 worldPosition;
    public bool walkable;

    public Node up, down, left, right;

    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost;
    public Node parent;

    public void Initialize(int x, int y, Vector3 worldPos, bool isWalkable, List<Node> allNodes)
    {
        this.x = x;
        this.y = y;
        this.worldPosition = worldPos;
        this.walkable = isWalkable;

        allNodes.Add(this);

        GetComponent<Renderer>().material.color = isWalkable ? Color.white : Color.red;
    }

    public void SetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }
}
