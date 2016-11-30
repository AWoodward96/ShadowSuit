using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    public List<GameObject> pointLights;
    public List<GameObject> spotLights;
    private SpriteRenderer spriteRend;

    bool inLight;

    public bool lightDebug;
    public float sprintSpeed;
    public float speed;

    public Light brightnessAdjuster;

    public GameObject lightIndicator;

    CharacterController myCC;
    Vector3 Velocity;

    public int noiseLevel;

    // For Animations
    AnimatorScript myAnimatorScript;

    // Use this for initialization
    void Start () {
        spriteRend = this.GetComponent<SpriteRenderer>();   // grabs the spriteRenderer from the player object
        myCC = GetComponent<CharacterController>();

        GameManager.InitializePlayer(transform.position, this);

        myAnimatorScript = GetComponent<AnimatorScript>();

        inLight = false;
	}
	
	// Update is called once per frame
	void Update () {

        lightIndicator.SetActive(false);
        inLight = false;
        noiseLevel = 0;

        foreach (GameObject spotLight in spotLights)
        {
            int result = isLitSpot(spotLight);
            if (result > 0)
            {
                lightIndicator.SetActive(true);
                if (result == 100)
                {
                    lightIndicator.GetComponent<SpriteRenderer>().color = Color.red;
                    lightIndicator.transform.localScale = new Vector3(1, 1.2f, 1);
                }
                else
                {
                    lightIndicator.GetComponent<SpriteRenderer>().color = Color.white;
                    lightIndicator.transform.localScale = new Vector3(1, result / 100f, 1);
                }

                //spriteRend.color = Color.red;
                inLight = true;
                break;
            }
        }
        foreach (GameObject pointLight in pointLights)
        {
            int result = isLitPoint(pointLight);
            if (result > 0)
            {
                lightIndicator.SetActive(true);
                if (result == 100)
                {
                    lightIndicator.GetComponent<SpriteRenderer>().color = Color.red;
                    lightIndicator.transform.localScale = new Vector3(1, 1.2f, 1);
                }
                else
                {
                    lightIndicator.GetComponent<SpriteRenderer>().color = Color.white;
                    lightIndicator.transform.localScale = new Vector3(1, result / 100f, 1);
                }

                //spriteRend.color = Color.red;
                inLight = true;
                break;
            }
        }

        processInput();
	}

    int isLitSpot(GameObject lite) // checks to see if the light passed in as a parameter is lighting the player (assumes that it is a spot light)
    {
        Light lt = lite.GetComponent<Light>();  // grabs light component
        Vector3 lightToPlayer = this.transform.position - lite.GetComponent<Transform>().position;  // gets a vector from the light to the player
        Vector3 lightForward = lite.GetComponent<Transform>().forward;  // gets the forward vector of the light
        Vector3 lightFlattened = lite.GetComponent<Transform>().position;   // gets position vector of the light
        lightForward.y = 0;     // flattens the forward vector on the y axis 
        lightToPlayer.y = 0;    // flattens the vector to the player on the y axis
        float arc = lt.spotAngle;   // gets the spot angle of the light
        float angle = Vector3.Angle(lightForward, lightToPlayer);   // gets the angle between the forward vector and the vector to the player

        RaycastHit hit;

        if (Physics.Raycast(lightFlattened, lightToPlayer, out hit, lt.range) && angle < (arc / 2) && hit.transform == this.transform)  // checks to see if it is close enough to the player to actually cast light AND if it is within the arc of light 
        {
            if (Physics.Raycast(lightFlattened, lightToPlayer, out hit, lt.range * .4f) && angle < (arc / 2) && hit.transform == this.transform)  // checks to see if it is close enough to the player to actually cast light AND if it is within the arc of light 
            {
                //Debug.DrawRay(lightFlattened, lightToPlayer, Color.green);  // draws debug ray
                //lt.color = Color.red;
                GameObject.Find("GameManager").GetComponent<GameManager>().PlayerInLight(); //guards chase player
                return 100;
            }
            else
            {
                return (int)(((lt.range - hit.distance)) * 100 / (lt.range * .6f));
            }
        }
        else
        {
            lt.color = Color.white;
            return 0;
        }

    }

    int isLitPoint(GameObject lite)    // checks to see if the light passed in as a parameter is lighting the player (assumes that it is a point light)
    {
        Light lt = lite.GetComponent<Light>();  // grabs light component
        Vector3 lightToPlayer = this.transform.position - lite.GetComponent<Transform>().position;  // gets a vecctor from the light to the player
        Vector3 lightFlattened = lite.GetComponent<Transform>().position;   // gets position vector of the light
        lightToPlayer.y = 0;    // flattens the forward vector on the y axis
        lightFlattened.y = 0;   // flattens the vector to the player on the y axis
        RaycastHit hit;

        if (Physics.Raycast(lightFlattened, lightToPlayer, out hit, lt.range) && hit.transform == this.transform) // checks to see if it is close enough to the player to actually cast light
        {
            if (Physics.Raycast(lightFlattened, lightToPlayer, out hit, lt.range * .7f) && hit.transform == this.transform) // checks to see if it is close enough to the player to actually cast light
            {
                //Debug.DrawRay(lightFlattened, lightToPlayer, Color.green);  // draws debug ray
                // lt.color = Color.red;
                GameObject.Find("GameManager").GetComponent<GameManager>().PlayerInLight(); //guards chase player
                return 100;
            }
            else
            {
                return (int)(((lt.range - hit.distance)) * 100 / (lt.range * .3f));
            }
        }
        else
        {
            lt.color = Color.white;
            return 0;
        }

    }

    void processInput()
    {
        float usedSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            usedSpeed = sprintSpeed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (usedSpeed == sprintSpeed)
            {
                noiseLevel = 2;
            }
            else
            {
                noiseLevel = 1;
            }
            Velocity += (Vector3.forward * Time.deltaTime * usedSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (usedSpeed == sprintSpeed)
            {
                noiseLevel = 2;
            }
            else
            {
                noiseLevel = 1;
            }
            Velocity += (Vector3.left * Time.deltaTime * usedSpeed); 
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (usedSpeed == sprintSpeed)
            {
                noiseLevel = 2;
            }
            else
            {
                noiseLevel = 1;
            }
            Velocity += (Vector3.back * Time.deltaTime * usedSpeed); 
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (usedSpeed == sprintSpeed)
            {
                noiseLevel = 2;
            }
            else
            {
                noiseLevel = 1;
            }
            Velocity += (Vector3.right * Time.deltaTime * usedSpeed);
        }

        //a brightness adjuster, for unnecessarily dark computers
        if (Input.GetKey(KeyCode.R) && brightnessAdjuster)
        {
            brightnessAdjuster.intensity += .01f;
        }
        if (Input.GetKey(KeyCode.F) && brightnessAdjuster)
        {
            brightnessAdjuster.intensity -= .01f;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            GameManager.Restart();
        }


        if(myAnimatorScript)
        {
            // Send this data to the animator script
            Vector3 temp = Velocity / Time.deltaTime;
            myAnimatorScript.UpdateAnimator(temp.x, temp.z, Velocity.magnitude / (3 / usedSpeed));   
        }

        Velocity += Vector3.down;
        myCC.Move(Velocity);
        Velocity *= .8f;
    }

    public void initializeLights(List<GameObject> listOfLights)
    {
        foreach(GameObject obj in listOfLights)
        {
            Light objectLight = obj.GetComponent<Light>();
            if(objectLight.type == LightType.Spot)
            {
                spotLights.Add(obj);
            }

            if(objectLight.type == LightType.Point)
            {
                pointLights.Add(obj);
            }
        }
    }

    public bool InLight
    {
        get
        {
            return inLight;
        }
    }
}
