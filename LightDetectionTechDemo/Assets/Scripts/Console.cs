using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Console : MonoBehaviour {

    //list of associated lights
    public List<GameObject> lights = new List<GameObject>();

    float dist;
    public float interactRange;
    GameObject player;

    public bool currentState;
    public bool defaultState;

    // Use this for initialization
    void Start()
    {
        //find player
        player = GameObject.FindGameObjectWithTag("GamePlayer");
        currentState = false;
        defaultState = false;
    }

    // Update is called once per frame
    void Update()
    {
        //dist between player and console
        dist = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(this.transform.position.x, this.transform.position.z));

        //if in range and key pressed
        if (dist <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            Physics.Raycast(this.transform.position, (player.transform.position - this.transform.position), out hit);

            if (hit.collider == player.GetComponent<Collider>())
            {
                //toggle associated lights
                ToggleLights();
            }
        }
    }

    public void ToggleLights()
    {
        foreach (GameObject light in lights)
        {
            //light.enabled = !light.enabled;
            light.SetActive(!light.activeSelf);
            currentState = !currentState;
        }
    }
}
