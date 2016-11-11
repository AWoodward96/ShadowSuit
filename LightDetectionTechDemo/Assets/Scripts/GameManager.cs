using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    static GameManager instance;

    static Vector3 playerStartingPosition;

	// Use this for initialization
	void Start ()
    {
        instance = this;
	}
	
	// Update is called once per frame
	void Update ()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); //gets all tagged enemies
        GameObject player = GameObject.FindGameObjectWithTag("Player"); //gets the tagged player
        for (int i = 0; i < enemies.Length; i++) //loops through all the enemies
        {
            if(!enemies[i].GetComponent<EnemyController>().guarding) //if this enemy is chasing the player...
            {
                enemies[i].GetComponent<EnemyController>().target = player.transform; //...update it's target position
            }
        }
    }

    public void PlayerInLight() //run this function if the player is lit up
    {
        //Debug.Log("IN LIGHT");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); //gets all tagged enemies
        GameObject player = GameObject.FindGameObjectWithTag("Player"); //gets the tagged player
        for (int i = 0; i < enemies.Length; i++) //loops through all the enemies
        {
            enemies[i].GetComponent<EnemyController>().guarding = false; //sets this enemy to chase the player
            enemies[i].GetComponent<EnemyController>().target = player.transform; //sets the enemies target to the player's position
        }
    }

    public static void Restart() //resets the level to it's original state
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); //gets all tagged enemies
        for (int i = 0; i < enemies.Length; i++) //loops through all the enemies
        {
            enemies[i].transform.position = new Vector3(enemies[i].GetComponent<EnemyController>().startingPos.x, enemies[i].transform.position.y, enemies[i].GetComponent<EnemyController>().startingPos.y); //sets this enemy to it's starting position
            enemies[i].GetComponent<EnemyController>().guarding = true; //sets this enemy to follow it's path
            enemies[i].GetComponent<EnemyController>().resetPath(); //makes sure this enemies path is followed in the right order
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player"); //gets the tagged player
        player.transform.position =playerStartingPosition; //sets the player to it's starting position
    }

    public static void InitializePlayer( Vector3 startingPosition)
    {
        playerStartingPosition = startingPosition;
    }
}
