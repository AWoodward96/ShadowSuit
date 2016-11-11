using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Grid))]

public class PathFinding : MonoBehaviour
{
    Grid grid;
    PathRequestManager requestManager;

    void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        // Get their actual position
        Nodes startNode = grid.NodeFromWorldPoint(startPos);
        Nodes targetNode = grid.NodeFromWorldPoint(targetPos);

        if(!startNode.walkable)
        {
            print("Will fail because the start node is on an unwalkable node");
            float lowest = 90000000000000;
            Nodes newStart = startNode;
            foreach (Nodes neighbour in grid.getNeighbors(startNode, true))
            {
                if (!neighbour.walkable)
                    continue;

                float dist = Vector3.Distance(neighbour.worldPosition, targetPos);
                if(lowest > dist)
                {
                    newStart = neighbour;
                    lowest = dist;
                }
            }

            startNode = newStart;
            if(!startNode.walkable)
            {
                Debug.Log("We've failed boiz");
            }

        }

        if (!targetNode.walkable)
        {
            print("Will fail because the target is on an unwalkable node");

            float lowest = 90000000000000;
            Nodes newEnd = targetNode;
            foreach (Nodes neighbour in grid.getNeighbors(targetNode, true))
            {
                if (!neighbour.walkable)
                    continue;

                float dist = Vector3.Distance(neighbour.worldPosition, startPos);
                if (lowest > dist)
                {
                    newEnd = neighbour;
                    lowest = dist;
                }
            }

            targetNode = newEnd;
            if (!startNode.walkable)
            {
                Debug.Log("We've failed (end) boiz");
            }
        }

        if (startNode.walkable && targetNode.walkable)
        {
            // Start the process
            // We'll be using lists
            Heap<Nodes> openSet = new Heap<Nodes>(grid.MaxSize);
            HashSet<Nodes> closedSet = new HashSet<Nodes>();

            // Start at the start
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                // Find the node in the open set with the lowest fCost
                Nodes currentNode = openSet.RemoveFirst();


                closedSet.Add(currentNode);

                // Have we found the exit yet?
                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                // Start comparing
                foreach (Nodes neighbour in grid.getNeighbors(currentNode, true))
                {
                    // if the neighbor isn't walkable or is already in the closed set then ignore it
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;


                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    // if the new movement cost is less then the cost to actually move towards the exit or if the open set dooesn't already have the neighbor
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        // Add the neighbor
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);

                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }

                    }
                }

            }
        }

        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Nodes startNode, Nodes endNode)
    {
        List<Nodes> path = new List<Nodes>();
        Nodes currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Add(startNode);

        // Simplify the path
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    // Simplify the paths
    Vector3[] SimplifyPath(List<Nodes> _path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < _path.Count; i++)
        {
            Vector2 directionNew = new Vector2(_path[i - 1].gridY - _path[i].gridX, _path[i - 1].gridY - _path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(_path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Nodes nodeA, Nodes nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);

        return 14 * distX + 10 * (distY - distX);
    }
}
