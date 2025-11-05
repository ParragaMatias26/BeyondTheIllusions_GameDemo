using System.Collections.Generic;
using UnityEngine;

public class CustomPathfinder : MonoBehaviour
{
    [SerializeField]CustomNode[] customNodes;
    public List<CustomNode> FindPath(Vector3 startPos, Vector3 finalPos)
    {
        CustomNode startNode = CalculateNearNode(startPos);
        CustomNode targetNode = CalculateNearNode(finalPos);

        if (startNode == null || targetNode == null)
        {
            Debug.LogWarning("Start or target node not found.");
            return null;
        }

        Queue<CustomNode> queue = new Queue<CustomNode>();
        Dictionary<CustomNode, CustomNode> cameFrom = new Dictionary<CustomNode, CustomNode>();
        HashSet<CustomNode> visited = new HashSet<CustomNode>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            CustomNode current = queue.Dequeue();

            if (current == targetNode)
                return RebuildPath(cameFrom, current);

            foreach (CustomNode neighbor in current.neighbors)
            {
                if (neighbor == null || visited.Contains(neighbor)) continue;

                visited.Add(neighbor);
                cameFrom[neighbor] = current;
                queue.Enqueue(neighbor);
            }
        }

        Debug.LogWarning("No path found.");
        return null;
    }

    List<CustomNode> RebuildPath(Dictionary<CustomNode, CustomNode> cameFrom, CustomNode actual)
    {
        List<CustomNode> totalPath = new List<CustomNode> { actual };
        while (cameFrom.ContainsKey(actual))
        {
            actual = cameFrom[actual];
            totalPath.Insert(0, actual);
        }
        return totalPath;
    }

    public CustomNode CalculateNearNode(Vector3 position)
    {
        float distance = Mathf.Infinity; CustomNode nearNode = default;

        foreach (CustomNode item in customNodes)
        {
            if (GameManager.Instance.InLineOfSight(item.transform.position, position))
            {
                if (Vector3.Distance(item.transform.position, position) < distance)
                {
                    nearNode = item;
                    distance = Vector3.Distance(item.transform.position, position);
                }
            }
        }

        return nearNode;
    }
}
