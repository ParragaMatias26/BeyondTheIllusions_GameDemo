using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public GridGenerator gridGen;

    private void Start()
    {
        gridGen = GameManager.Instance.GridGen;
    }

    public List<Node> GetPath(Vector2 startWorldPos, Vector2 targetWorldPos)
    {
        Node startNode = CalculateNearNode(startWorldPos);
        Node targetNode = CalculateNearNode(targetWorldPos);

        if (startNode == null || targetNode == null || !targetNode.walkable)
        {
            Debug.LogWarning("Pathfinder: nodo inválido o no caminable.");
            return null;
        }

        List<Node> path = AStar(startNode, targetNode);

        if (path == null) 
        {
            Debug.LogWarning($"Pathfinder: AStar no encontro camino");
            return null;
        }

        foreach (Node node in path)
        {
            node.SetColor(Color.green);
        }

        return AStar(startNode, targetNode);
    }

    List<Node> AStar(Node start, Node goal)
    {
        List<Node> openSet = new List<Node> { start };
        HashSet<Node> closedSet = new HashSet<Node>();

        start.gCost = 0;
        start.hCost = Vector2.Distance(start.worldPosition, goal.worldPosition);
        start.parent = null;

        while (openSet.Count > 0)
        {
            Node current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
                if (openSet[i].fCost < current.fCost || (openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost))
                    current = openSet[i];

            if (current == goal)
                return RetracePath(start, goal);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Node neighbor in GetNeighbors(current))
            {
                if (neighbor == null || !neighbor.walkable || closedSet.Contains(neighbor)) continue;

                float newG = current.gCost + Vector2.Distance(current.worldPosition, neighbor.worldPosition);
                if (newG < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newG;
                    neighbor.hCost = Vector2.Distance(neighbor.worldPosition, goal.worldPosition);
                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    List<Node> RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node current = end;

        while (current != start)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    List<Node> GetNeighbors(Node node)
    {
        return new List<Node> { node.up, node.down, node.left, node.right };
    }
    public Node CalculateNearNode(Vector3 position)
    {
        float distance = Mathf.Infinity; Node nearNode = default;

        foreach (Node item in gridGen.allNodes)
        {
            if (Vector3.Distance(item.transform.position, position) < distance && item.walkable)
            {
                nearNode = item;
                distance = Vector3.Distance(item.transform.position, position);
            }
        }

        return nearNode;
    }
}


