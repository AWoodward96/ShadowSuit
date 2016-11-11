using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


 
public class PathRequestManager : MonoBehaviour
{

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest CurrentPathRequest;

    static PathRequestManager instance;
    PathFinding pathfinding;
    bool isProcessingPath;

 

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<PathFinding>();
        
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) // call back is weather or not it was successful
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        // if we're not already processing a path start another process
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            CurrentPathRequest = pathRequestQueue.Dequeue();// get first queue and dequeueueueueue it
            isProcessingPath = true;
            pathfinding.StartFindPath(CurrentPathRequest.pathStart, CurrentPathRequest.pathEnd);
        }

    }

    // Called by the path finding script when it's finished processing a path
    public void FinishedProcessingPath(Vector3[] path, bool success)
    {

        CurrentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            callback = _callback;
            pathStart = _start;
            pathEnd = _end;
        }
    }
 
}
