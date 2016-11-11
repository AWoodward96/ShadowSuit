using UnityEngine;
using System.Collections;

public class Nodes : IHeapItem<Nodes>
{

    public bool walkable;
    public Vector3 worldPosition;
    public int gridX; /// Grid location x
    public int gridY; /// Grid location y

    public int gCost;
    public int hCost;
    public Nodes parent;
    int heapIndex;

    public Nodes(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Nodes nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }

    //public bool walkable;
    //public Vector3 worldPosition;
    //public int gCost;
    //public int hCost;
    //public Vector2 gridLocation;

    //public Nodes parent;

    //int heapIndex;

    //public Nodes(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
    //{
    //    walkable = _walkable;
    //    worldPosition = _worldPosition;
    //    gridLocation = new Vector2(_gridX, _gridY);
    //}

    //public int fCost // Will never be assigned
    //{
    //    get
    //    {
    //        return gCost + hCost;
    //    }
    //}

    //public int HeapIndex
    //{
    //    get
    //    {
    //        return heapIndex;
    //    }
    //    set
    //    {
    //        heapIndex = value;
    //    }
    //}

    //// Comparing the f and h values of two nodes
    //public int CompareTo(Nodes nodeToCompare)
    //{
    //    int compare = fCost.CompareTo(nodeToCompare.fCost);
    //    if(compare == 0)
    //    {
    //        compare = hCost.CompareTo(nodeToCompare.hCost);
    //    }

    //    return -compare;
    //}
}
