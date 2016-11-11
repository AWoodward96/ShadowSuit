using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    public Vector2 startingPos; //the starting position, for level resetting
    Queue<Vector3> pathFollowing = new Queue<Vector3>(); //the queue for following the path around
    public List<GameObject> pathFollowing2 = new List<GameObject>(); //a list for creating the queue, since I couldn't figure out how to make a queue manipulated in the inspector
    int guardSpeed = 3; //speed for pathing
    int chaseSpeed = 5; //speed for chasing the player
    public bool guarding = true; //a bool saying whether the enemy is pathing or chasing (true is pathing)
    public Transform target; //the enemies target, to be set to the player's x and z position
    CharacterController myCC;

    Vector3[] path;
    bool pathRequested;
    int pathIndex;

    LayerMask UnwalkableMask;

    Vector3 Velocity;

    // Use this for initialization
    void Start()
    {
        myCC = GetComponent<CharacterController>();
        startingPos = new Vector2(transform.position.x, transform.position.z); //set the starting position
        for (int i = 0; i < pathFollowing2.Count; i++) //converts the list to a queue
        {
            pathFollowing.Enqueue(returnYZeroVector3(pathFollowing2[i].transform.position));
        }

        //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));

        UnwalkableMask = LayerMask.GetMask("Ground");

    }

    // Update is called once per frame
    void Update()
    {
        if (guarding) //for pathing
        {
            Vector3 distanceVector = pathFollowing.Peek() - transform.position;
            Vector3 yZeroDistanceVector = returnYZeroVector3(distanceVector);
            float dist = Vector3.Distance(returnYZeroVector3(transform.position), pathFollowing.Peek());


            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), pathFollowing.Peek()) < .1f)
            {
                nextPath();
            }
            else
            {
                Velocity += (yZeroDistanceVector.normalized * guardSpeed);
            }

        }
        else if (target != null) //for chasing (if target is null, the enemy freezes)
        {
            Vector3 y0Target = returnYZeroVector3(target.position);
            Vector3 y0Transform = returnYZeroVector3(transform.position);

            // First thing to do is to make sure you're far enough away from the player that you need to move closer to him
            float toPlayerDist = Vector3.Distance(y0Transform, y0Target);

            // Raycast to the player object. If it's within range and there's no object in the way that could prevent movement, just move towards it
            RaycastHit hit;
            bool safe = !Physics.Raycast(y0Transform, y0Target - y0Transform, out hit, toPlayerDist, UnwalkableMask);
            if (safe)
            {
                Debug.Log("Simple");

                // First thing is to stop the a Star algorythm
                StopCoroutine("FollowPath");
                pathRequested = false; // Gotta make sure this is reset

                // Then move to that location
                Vector3 v = y0Target - y0Transform;


                v.Normalize();
                Velocity += (v * chaseSpeed);
                
            }
            else if (!pathRequested)
            {
                //Debug.Log("A Star");
                pathRequested = true;
                pathIndex = 0;
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            }
        }

        Velocity += Vector3.down;
        myCC.Move(Velocity * Time.deltaTime);
        Velocity *= 0;
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
        {
            pathRequested = false;
            Debug.Log("Path Finding failed for: " + this.gameObject.name);
        }
    }



    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            Vector3 waypointYNom = currentWaypoint; // We do this because we'll complain if the value of waypointYNom isn't set
            waypointYNom.y = transform.position.y; // Make the current y position equal to your current y position. We're syncing up the y values here
            // Otherwise the character will start to move towards 0 on the Y axis, and look like it's jumping up and down. We don't want that

            float dist = Vector3.Distance(transform.position, waypointYNom);

            if (dist < .3f)
            {
                pathIndex++;
                if (pathIndex >= path.Length)
                {
                    pathRequested = false;
                    yield break;// exit out of the coroutine
                }
                currentWaypoint = path[pathIndex];// get the next waypoint
            }

            Vector3 toVec = waypointYNom - transform.position; // Get the vector we're moving towards

            toVec.Normalize(); // Noramlize it so there's no shinanigans
                               //Vector2 yNormalized = new Vector2(toVec.x, toVec.z);
            Velocity += (toVec * chaseSpeed); // Move!
            yield return null;
        }
    }

    void nextPath() //set the next path point to the first one, while moving the first to the end
    {
        Vector3 path = pathFollowing.Dequeue(); //pops off the first element of the path...
        pathFollowing.Enqueue(path); //...and adds it to the end of the path
    }

    public void resetPath()
    {
        pathFollowing = new Queue<Vector3>(); //resets the queue path
        for (int i = 0; i < pathFollowing2.Count; i++) //converts the list to a queue
        {
            pathFollowing.Enqueue(pathFollowing2[i].transform.position);
        }
    }

    Vector3 returnYZeroVector3(Vector3 v)
    {
        return new Vector3(v.x, 0, v.z);
    }

    void OnControllerColliderHit(ControllerColliderHit col)
    {
        if (col.transform.tag == "Player")
        {
            GameManager.Restart();
        }
    }
}
