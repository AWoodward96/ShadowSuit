using UnityEngine;
using System.Collections;

using System.Collections.Generic;
public class Grid : MonoBehaviour
{

    // public Transform player;
    public LayerMask unwalkableMask; // The layer that is registered as unwalkable
    public Vector2 gridWorldSize;
    public float nodeRadius; // how much space each individual node covers
    Nodes[,] grid;

    public bool updateGridFlag;

    public bool displayGridGizmos;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
   

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); // Can't have half of an integer
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter); // Can't have half of an integer
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void Update()
    {
        if (updateGridFlag)
        {
            updateGridFlag = false;
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); // Can't have half of an integer
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter); // Can't have half of an integer
            CreateGrid();
        }
 
    }

    void CreateGrid()
    {
        grid = new Nodes[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2; // bottom left coordinate of the world

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                grid[x, y] = new Nodes(walkable, worldPoint, x, y);// Populate our variable
            }
        }
    }

    public List<Nodes> getNeighbors(Nodes n, bool diagonals)
    {
        List<Nodes> neighbors = new List<Nodes>();

        if (diagonals)
        {

            // Search in a 3 x 3 block around the node
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) // It's the same node. continue
                        continue;

                    int checkX = n.gridX + x;
                    int checkY = n.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbors.Add(grid[checkX, checkY]);
                    }

                }
            }

        }
        else
        {
            // We're doing the plus check 
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0) // It's the same node. continue
                    continue;

                int checkX = Mathf.RoundToInt(n.gridX) + x;
                int checkY = Mathf.RoundToInt(n.gridY);

                if (checkX >= 0 && checkX < gridSizeX)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }

            for (int y = -1; y <= 1; y++)
            {
                if (y == 0)
                    continue;

                int checkX = Mathf.RoundToInt(n.gridX);
                int checkY = Mathf.RoundToInt(n.gridY) + y;

                if (checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }

            }
        }


        return neighbors;

    }

    public Nodes NodeFromWorldPoint(Vector3 worldPosition)
    {
        // Convert the world position to a percentage
        // Far left = 0%, middle - .5%, Far Right = 1%
        float percentX = (worldPosition.x + gridWorldSize.x / 2 + (-transform.position.x)) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2 + (-transform.position.z)) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }



    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null && displayGridGizmos)
        {
            foreach (Nodes val in grid)
            {

                Gizmos.color = (val.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(val.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }

    }
}
