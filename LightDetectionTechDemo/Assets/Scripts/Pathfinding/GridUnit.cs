using UnityEngine;
using System.Collections;

public class GridUnit : MonoBehaviour
{

    public Transform target;
    public float speed = 1;
    public Vector3[] path;
    int targetIndex;

    void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
        else
            Debug.Log("Path Finding failed for: " + this.gameObject.name);
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;// exit out of the coroutine
                }
                currentWaypoint = path[targetIndex];// get the next waypoint
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }
}
