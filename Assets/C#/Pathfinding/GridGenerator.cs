using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float nodeSize = 1f;
    public float nodeObstacleRadius = .4f;
    public string cuteObstacleLayer;
    public string darkObstacleLayer;
    public GameObject nodePrefab;

    public List<Node> allNodes;
    private Node[,] grid;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new Node[width, height];
        Vector3 origin = transform.position;

        var obs = LayerMask.GetMask(cuteObstacleLayer, darkObstacleLayer, "Obstacles", "Props");

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPos = origin + new Vector3(x * nodeSize, y * nodeSize, 0f);
                bool walkable = !Physics2D.OverlapCircle(worldPos, nodeSize * nodeObstacleRadius, obs);

                GameObject nodeGO = Instantiate(nodePrefab, worldPos, Quaternion.identity, transform);
                nodeGO.name = $"Node ({x},{y})";

                Node node = nodeGO.GetComponent<Node>();
                node.Initialize(x, y, worldPos, walkable, allNodes);
                grid[x, y] = node;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node current = grid[x, y];

                current.up = (y + 1 < height) ? grid[x, y + 1] : null;
                current.down = (y - 1 >= 0) ? grid[x, y - 1] : null;
                current.left = (x - 1 >= 0) ? grid[x - 1, y] : null;
                current.right = (x + 1 < width) ? grid[x + 1, y] : null;
            }
        }
    }

    public Node GetNode(int x, int y)
    {
        return grid[x, y];
    }

}
