using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    GameObject player;

    public Vector2 startingPos; //the starting position, for level resetting
    Queue<Vector3> pathFollowing = new Queue<Vector3>(); //the queue for following the path around
    public List<GameObject> pathFollowing2 = new List<GameObject>(); //a list for creating the queue, since I couldn't figure out how to make a queue manipulated in the inspector
    int guardSpeed = 3; //speed for pathing
    int chaseSpeed = 5; //speed for chasing the player
    public GameObject target; //the enemies target, to be set to the player's x and z position
    CharacterController myCC;
    public bool busy;

    Vector3[] path;
    bool pathRequested;
    bool waitingRequested;
    int pathIndex;

    LayerMask UnwalkableMask;

    Vector3 Velocity;
    public Vector3 LastSeenPosition;

    // For animations
    AnimatorScript myAnimatorScript;

    public enum AIState { Guarding, Chasing, Inspecting };
    public AIState myState;

    // Use this for initialization
    void Start()
    {
        busy = false;
        myCC = GetComponent<CharacterController>();
        startingPos = new Vector2(transform.position.x, transform.position.z); //set the starting position
        for (int i = 0; i < pathFollowing2.Count; i++) //converts the list to a queue
        {
            pathFollowing.Enqueue(returnYZeroVector3(pathFollowing2[i].transform.position));
        }

        //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));

        UnwalkableMask = LayerMask.GetMask("Ground");

        player = GameObject.FindGameObjectWithTag("GamePlayer");

        myAnimatorScript = GetComponent<AnimatorScript>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (myState)
        {
            case AIState.Guarding:

                Guarding();
                break;

            case AIState.Chasing:
                Chasing();
                break;

            case AIState.Inspecting:
                Inspecting();
                break;
        }


        //makes sure enemies stay on the ground (no floating off elevated parts)
        Velocity += Vector3.down * 5;

        //enemy stops chasing and begins guarding if it is close to it's target
        //right now, enemy and player collide at about .9 away so checking any further will make the enemy leave before killing the player
        //if (myState != AIState.Guarding && Vector3.Distance(transform.position, target.position) < .35)
        {
            //myState = AIState.Guarding;
            //SetClosestPath();
        }
        //else
        {
            myAnimatorScript.UpdateAnimator(Velocity.x, Velocity.z);
            myCC.Move(Velocity * Time.deltaTime);
            Velocity *= 0;
        }


    }

    void Chasing()
    {
        // Ok move towards the last place you saw the players position

        //the result of the raycast check
        RaycastHit hit;
        //the distance between the player and enemy, for raycasting
        Vector3 dist = player.transform.position - transform.position;

        // Check to see if you can see the player right now
        bool safe = !Physics.Raycast(transform.position, dist, out hit, dist.magnitude, UnwalkableMask);
        if (safe)
        {
            //Debug.Log("CAN SEE PLAYER");
            // Then move towards the player
            LastSeenPosition = target.transform.position; // Update the last seen position
            
            // Now move towards that position
            // First thing is to stop the a Star algorythm
            StopCoroutine("FollowPath");
            pathRequested = false; // Gotta make sure this is reset

            // Also stop the wait coroutine
            waitingRequested = false;
            StopCoroutine("WaitAtPosition");

            // Then move to that location
            Vector3 v = returnYZeroVector3(target.transform.position) - returnYZeroVector3(transform.position);

            v.Normalize();
            Velocity += (v * chaseSpeed);
 
        }
        else
        {
            // Ok so we can't actively see the player. Lets move to the last known position

            // In the event that you just straight up can't see the last seen position --- wait
            //RaycastHit lastseenHit;
            //bool canSee = !Physics.Raycast(transform.position, LastSeenPosition - transform.position, out lastseenHit, UnwalkableMask);
            //if(!canSee)
            //{
            //    if (!waitingRequested)
            //    {
            //        waitingRequested = true;
            //        StartCoroutine(WaitAtPosition());
            //    }
            //    return;
            //}

            if (Vector3.Distance(transform.position, LastSeenPosition) > .5f)
            {
                // Move towards that last known position
                Vector3 v = returnYZeroVector3(LastSeenPosition) - returnYZeroVector3(transform.position);
                v.Normalize();
                Velocity += (v * chaseSpeed); 
            }
            else
            {
                // Ok so we're close enough to the last seen position to stop and wait for a bit
                // Request the wait
                if(!waitingRequested)
                {
                    waitingRequested = true;
                    StartCoroutine(WaitAtPosition());
                }
            }
        }
    }

    void Guarding()
    {
        Vector3 nextPoint = pathFollowing.Peek();
        Vector3 distanceVector = nextPoint - transform.position;
        Vector3 yZeroDistanceVector = returnYZeroVector3(distanceVector);

        //if the player is sprinting and nearby, the enemy will catch it
        if (player.GetComponent<PlayerController>().noiseLevel == 2 && Vector3.Distance(returnYZeroVector3(transform.position), returnYZeroVector3(player.transform.position)) < 4)
        {
            myState = AIState.Chasing;
            target = player;
        }
        //if the player is walking and VERY nearby, the enemy will catch it
        if (player.GetComponent<PlayerController>().noiseLevel == 1 && Vector3.Distance(returnYZeroVector3(transform.position), returnYZeroVector3(player.transform.position)) < 1)
        {
            myState = AIState.Chasing;
            target = player;
        }

        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), nextPoint) < .1f)
        {
            nextPath();
        }
        else
        {
            // Check to see if you can see the next point
            RaycastHit nextPointHit;
            bool safe = !Physics.Raycast(returnYZeroVector3(transform.position), returnYZeroVector3(distanceVector), out nextPointHit, distanceVector.magnitude, UnwalkableMask);
            if (safe)
            {
                StopCoroutine("FollowPath");
                pathRequested = false; // Gotta make sure this is reset
                Velocity += (yZeroDistanceVector.normalized * guardSpeed);
            }
            else
            {
                pathRequested = true;
                pathIndex = 0;
                PathRequestManager.RequestPath(transform.position, nextPoint, OnPathFound);
            }
        }

        //check if there are consoles to inspect
        //wish this used aStar to path find around walls, but i don't know aStar
        GameObject[] consoles = GameObject.FindGameObjectsWithTag("Console"); //gets all tagged consoles
        RaycastHit hit;
        for (int i = 0; i < consoles.Length; i++) //loops through all the enemies
        {
            if (Vector3.Distance(transform.position, consoles[i].transform.position) < 15 && consoles[i].GetComponent<Console>().canBeChecked)
            {
                Vector3 dist = consoles[i].transform.position - transform.position;
                if(!Physics.Raycast(transform.position, dist, out hit, dist.magnitude, UnwalkableMask))
                {
                    myState = AIState.Inspecting;
                    target = consoles[i];
                }
            }
        }
    }

    void Inspecting()
    { 
        //the distance between the player and enemy, for raycasting
        Vector3 dist = target.transform.position - transform.position;

        //PathRequestManager.RequestPath(transform.position, LastSeenPosition, OnPathFound);
        
        if (Vector3.Distance(transform.position, target.transform.position) > 2.5f)
            {
                // Move towards that last known position
                Vector3 v = returnYZeroVector3(target.transform.position) - returnYZeroVector3(transform.position);
                v.Normalize();
                Velocity += (v * chaseSpeed);
            }
            else
            {
                // Ok so we're close enough to the last seen position to stop and wait for a bit
                // Request the wait
                if (!waitingRequested)
                {
                    waitingRequested = true;
                    StartCoroutine(WaitAtConsole());
                }
            }
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
            //Debug.Log("Path Finding failed for: " + this.gameObject.name);
        }
    }

    IEnumerator FollowPath()
    {
        if (path.Length >= 1)
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
    }

    IEnumerator WaitAtPosition()
    {
        yield return new WaitForSeconds(4);
        waitingRequested = false;
        myState = AIState.Guarding;
        SetClosestPath();
    }

    IEnumerator WaitAtConsole()
    {
        busy = true;
        yield return new WaitForSeconds(3);
        busy = false;
        waitingRequested = false;
        myState = AIState.Guarding;
        SetClosestPath();
        if (target.GetComponent<Console>().currentState != target.GetComponent<Console>().defaultState)
        {
            target.GetComponent<Console>().TriggerConsole();
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
            pathFollowing.Enqueue(returnYZeroVector3(pathFollowing2[i].transform.position));
        }
    }
    //sets the queue to start at the closest point, while keeping the same order
    public void SetClosestPath()
    {
        int i = 0;
        float closest = -1;
        //loops through all points of the queue, finding the closest one
        while (i < pathFollowing.Count)
        {
            i++;
            Vector3 path = pathFollowing.Dequeue(); //pops off the first element of the path...
            pathFollowing.Enqueue(path); //...and adds it to the end of the path
            float dist = (path - transform.position).magnitude;
            if (closest == -1 || dist < closest)
            {
                closest = dist;
            }
        }
        //loops through the queue again, stopping when the closest point is the first point
        float distance = (pathFollowing.Peek() - transform.position).magnitude;
        while (distance != closest)
        {
            Vector3 path = pathFollowing.Dequeue(); //pops off the first element of the path...
            pathFollowing.Enqueue(path); //...and adds it to the end of the path

            distance = (pathFollowing.Peek() - transform.position).magnitude;
        }
    }

    Vector3 returnYZeroVector3(Vector3 v)
    {
        return new Vector3(v.x, 0, v.z);
    }

    void OnControllerColliderHit(ControllerColliderHit col)
    {
        if (col.transform.tag == "GamePlayer")
        {
            GameManager.Restart();
        }
    }
}
