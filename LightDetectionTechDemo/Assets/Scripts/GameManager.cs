using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    //static GameManager instance;

    static Vector3 playerStartingPosition;
    bool canvasTextUsed;
    GameObject firstInteract;

	// Use this for initialization
	void Start ()
    {
        canvasTextUsed = false;
        //instance = this;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); //gets all tagged enemies
        for (int i = 0; i < enemies.Length; i++) //loops through all the enemies
        {
            for (int i2 = 0; i2 < enemies.Length; i2++) //loops through all the enemies
            {
                Physics.IgnoreCollision(enemies[i].GetComponent<Collider>(), enemies[i2].GetComponent<Collider>());
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (canvasTextUsed)
        {
            if (firstInteract.GetComponent<Console>().Dist > firstInteract.GetComponent<Console>().interactRange)
            {
                firstInteract.GetComponent<Console>().interact.gameObject.SetActive(false);
                canvasTextUsed = false;
            }
        }
    }

    public void PlayerInLight() //run this function if the player is lit up
    {
        //Debug.Log("IN LIGHT");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); //gets all tagged enemies
        GameObject player = GameObject.FindGameObjectWithTag("GamePlayer"); //gets the tagged player
        for (int i = 0; i < enemies.Length; i++) //loops through all the enemies
        {
            //the result of the raycast check
            RaycastHit hit;
            //the distance between the player and enemy, for raycasting
            Vector3 dist = player.transform.position - enemies[i].transform.position;

            //checks to see if the enemy can see the player(if a raycast between them results in colliding with the player's collider)
            if (Physics.Raycast(enemies[i].transform.position, dist, out hit) && hit.collider != player.GetComponent<Collider>())
            {
            }
            //only chase the player if the enemy can see them
            else if (!enemies[i].GetComponent<EnemyController>().busy)
            {
                enemies[i].GetComponent<EnemyController>().myState = EnemyController.AIState.Chasing; //sets this enemy to chase the player
                enemies[i].GetComponent<EnemyController>().target = player; //sets the enemies target to the player's position
            }
        }
        /*GameObject[] cameras = GameObject.FindGameObjectsWithTag("WallCamera"); //gets all tagged wall cameras
        for (int i = 0; i < cameras.Length; i++) //loops through all the cameras
        {
            //check if the camera can see the player
            if(cameras[i].GetComponent<WallCameras>().CanSee(player))
            {
                for (int i2 = 0; i2 < enemies.Length; i2++) //loops through all the enemies
                {
                    //only change enemies that don't presently know where the player is
                    if (enemies[i2].GetComponent<EnemyController>().myState != EnemyController.AIState.Chasing && !enemies[i2].GetComponent<EnemyController>().busy)
                    {
                        //enemies go to where the player was last seen
                        enemies[i2].GetComponent<EnemyController>().myState = EnemyController.AIState.Chasing;
                        enemies[i2].GetComponent<EnemyController>().target = player;
                    }
                }
            }
        }*/
    }

    public static void Restart() //resets the level to it's original state
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); //gets all tagged enemies
        for (int i = 0; i < enemies.Length; i++) //loops through all the enemies
        {
            enemies[i].transform.position = new Vector3(enemies[i].GetComponent<EnemyController>().startingPos.x, enemies[i].transform.position.y, enemies[i].GetComponent<EnemyController>().startingPos.y); //sets this enemy to it's starting position
            enemies[i].GetComponent<EnemyController>().myState = EnemyController.AIState.Guarding; //sets this enemy to follow it's path
            enemies[i].GetComponent<EnemyController>().resetPath(); //makes sure this enemies path is followed in the right order
        }
        GameObject player = GameObject.FindGameObjectWithTag("GamePlayer"); //gets the tagged player
        player.transform.position = playerStartingPosition; //sets the player to it's starting position
        GameObject[] console = GameObject.FindGameObjectsWithTag("Console"); //gets all tagged consoles
        for (int i = 0; i < console.Length; i++) //loops through all the consoles
        {
            //resets the console and all its attached objects to their default state
            if (console[i].GetComponent<Console>().currentState != console[i].GetComponent<Console>().defaultState)
            {
                console[i].GetComponent<Console>().TriggerConsole();
            }
        }
    }

    public static void InitializePlayer(Vector3 startingPosition, PlayerController pc)
    {
        // Set up the player reset poisition
        playerStartingPosition = startingPosition;

        // Get a list of all objects with the layer mask 'Light'
        GameObject[] Arr = FindObjectsOfType<GameObject>();
        List<GameObject> Lights = new List<GameObject>();
        int mask = LayerMask.NameToLayer("Light");

        Light tester;

        foreach(GameObject obj in Arr)
        {
            // Check to see if it's in the layer mask
            if(obj.layer == mask)
            {
                // Ensure there's actually a light on it
                tester = obj.GetComponent<Light>();
                if(tester)
                {
                    // Add the light
                    Lights.Add(obj);
                }

                tester = null;
            }
        }

        pc.initializeLights(Lights);
    }

    public bool CanvasTextUsed
    {
        get
        {
            return canvasTextUsed;
        }
    }
    public void UseText()
    {
        canvasTextUsed = true;
    }

    public void SetFirstInteract(GameObject first)
    {
        firstInteract = first;
    }
}
