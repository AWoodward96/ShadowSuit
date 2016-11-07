using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    public Vector2 startingPos; //the starting position, for level resetting
    Queue<Vector2> pathFollowing = new Queue<Vector2>(); //the queue for following the path around
    public List<Vector2> pathFollowing2 = new List<Vector2>(); //a list for creating the queue, since I couldn't figure out how to make a queue manipulated in the inspector
    int guardSpeed = 3; //speed for pathing
    int chaseSpeed = 5; //speed for chasing the player
    public bool guarding = true; //a bool saying whether the enemy is pathing or chasing (true is pathing)
    public Vector2 target; //the enemies target, to be set to the player's x and z position

    // Use this for initialization
    void Start ()
    {
        startingPos = new Vector2(transform.position.x, transform.position.z); //set the starting position
        for(int i = 0; i < pathFollowing2.Count; i++) //converts the list to a queue
        {
            pathFollowing.Enqueue(pathFollowing2[i]);
        }
	}
	
	// Update is called once per frame
	void Update ()
    { 
        if (guarding) //for pathing
        {
            float velocity = guardSpeed * Time.deltaTime; //distance traveled this frame
            Vector2 direction = (pathFollowing.Peek() - new Vector2(transform.position.x, transform.position.z)).normalized; //direction moved in this frame
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), pathFollowing.Peek()) < velocity) //check if the enemy is close to the next point
            {
                nextPath(); //move to the next part of the path if it is
            }
            else
            {
                transform.position = new Vector3(transform.position.x + (velocity * direction.x), transform.position.y, transform.position.z + (velocity * direction.y)); //if not, move the enemy closer to its path point
            }
        }
        else if (target != null) //for chasing (if target is null, the enemy freezes)
        {
            float velocity = chaseSpeed * Time.deltaTime; //distance traveled this frame
            Vector2 direction = (target - new Vector2(transform.position.x, transform.position.z)).normalized; //direction moved in this frame
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), target) < 1) //check if the enemy is close to it's target, currently assumed to be the player's position
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().Restart(); //run the gameManager's restart function (to reset the level) if it is
            }
            else
            {
                transform.position = new Vector3(transform.position.x + (velocity * direction.x), transform.position.y, transform.position.z + (velocity * direction.y)); //if not, move the enemy closer to it's target
            }
        }
    }

    void nextPath() //set the next path point to the first one, while moving the first to the end
    {
        Vector2 path = pathFollowing.Dequeue(); //pops off the first element of the path...
        pathFollowing.Enqueue(path); //...and adds it to the end of the path
    }

    public void resetPath()
    {
        for (int i = 0; i < pathFollowing2.Count; i++) //converts the list to a queue
        {
            pathFollowing.Enqueue(pathFollowing2[i]);
        }
    }
}
