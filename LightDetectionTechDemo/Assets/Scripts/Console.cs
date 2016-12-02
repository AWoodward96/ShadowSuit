using UnityEngine;
using UnityEngine.Events;
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

    public bool canBeChecked;
    int countdownTimer;

    public enum Purpose
    {
        lights,
        camera,
        exit,
    };

    public Purpose action;

    // Use this for initialization
    void Start()
    {
        canBeChecked = false;
        //find player
        player = GameObject.FindGameObjectWithTag("GamePlayer");
        currentState = false;
        defaultState = false;
        if (action == Purpose.lights)
        {
            gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
        }
        if (action == Purpose.camera)
        {
            gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
        }
        if (action == Purpose.exit)
        {
            gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(countdownTimer >= 0)
        {
            if(countdownTimer == 0)
            {
                canBeChecked = true;
            }
            else
            {
                countdownTimer--;
            }
        }
        if(currentState == defaultState)
        {
            canBeChecked = false;
        }

        //dist between player and console
        dist = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(this.transform.position.x, this.transform.position.z));
        

        //if in range and key pressed
        if (dist <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            //Physics.Raycast(this.transform.position, (player.transform.position - this.transform.position), out hit);

            //if (hit.collider == player.GetComponent<Collider>())
            {
                //toggle associated lights
                if (action == Purpose.lights)
                {
                    ToggleLights();
                    countdownTimer = 120;
                }
                if (action == Purpose.camera)
                {
                    DisableCamera();
                    countdownTimer = 240;
                }
                if (action == Purpose.exit)
                {
                    OpenExit();
                    countdownTimer = -1;
                }
                currentState = !currentState;
                gameObject.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(!currentState);
                gameObject.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(currentState);
            }
        }
    }

    void ToggleLights()
    {
        foreach (GameObject light in lights)
        {
            //light.enabled = !light.enabled;
            light.SetActive(!light.activeSelf);
        }
    }
    void OpenExit()
    {
        foreach (GameObject exit in lights)
        {
            exit.GetComponent<ElevatorScript>().State = !exit.GetComponent<ElevatorScript>().State;
        }
    }
    void DisableCamera()
    {
        foreach (GameObject camera in lights)
        {
            //light.enabled = !light.enabled;
            camera.GetComponent<WallCameras>().deActivated = !camera.GetComponent<WallCameras>().deActivated;
        }
    }
    public void TriggerConsole()
    {
        if (action == Purpose.lights)
        {
            ToggleLights();
            countdownTimer = 120;
        }
        if (action == Purpose.camera)
        {
            DisableCamera();
            countdownTimer = 240;
        }
        if (action == Purpose.exit)
        {
            OpenExit();
            countdownTimer = -1;
        }
        currentState = !currentState;
        gameObject.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(!currentState);
        gameObject.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(currentState);
    }
}
