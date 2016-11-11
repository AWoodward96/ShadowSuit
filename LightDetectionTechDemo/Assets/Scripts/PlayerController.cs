using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public GameObject[] pointLights;
    public GameObject[] spotLights;
    private SpriteRenderer spriteRend;

    public bool lightDebug;
    public float speed;

    public Light brightnessAdjuster;

    CharacterController myCC;
    Vector3 Velocity;

	// Use this for initialization
	void Start () {
        spriteRend = this.GetComponent<SpriteRenderer>();   // grabs the spriteRenderer from the player object
        myCC = GetComponent<CharacterController>();

        GameManager.InitializePlayer(transform.position);
	}
	
	// Update is called once per frame
	void Update () {
        foreach (GameObject spotLight in spotLights)
        {
            if (isLitSpot(spotLight))
            {
                spriteRend.color = Color.blue;
                break;
            }
        }
        foreach (GameObject pointLight in pointLights)
        {
            if (isLitPoint(pointLight))
            {
                spriteRend.color = Color.red;
                break;
            }
        }

        processInput();
	}

    bool isLitSpot(GameObject lite) // checks to see if the light passed in as a parameter is lighting the player (assumes that it is a spot light)
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

        if (Physics.Raycast(lightFlattened, lightToPlayer, out hit, lt.range * .4f) && angle < (arc/2) && hit.transform == this.transform)  // checks to see if it is close enough to the player to actually cast light AND if it is within the arc of light 
        {
            Debug.DrawRay(lightFlattened, lightToPlayer, Color.green);  // draws debug ray
            lt.color = Color.red;
            GameObject.Find("GameManager").GetComponent<GameManager>().PlayerInLight(); //guards chase player
            return true;
        }
        else
        {
            lt.color = Color.white;
            return false;
        }

    }

    bool isLitPoint(GameObject lite)    // checks to see if the light passed in as a parameter is lighting the player (assumes that it is a point light)
    {
        Light lt = lite.GetComponent<Light>();  // grabs light component
        Vector3 lightToPlayer = this.transform.position - lite.GetComponent<Transform>().position;  // gets a vecctor from the light to the player
        Vector3 lightFlattened = lite.GetComponent<Transform>().position;   // gets position vector of the light
        lightToPlayer.y = 0;    // flattens the forward vector on the y axis
        lightFlattened.y = 0;   // flattens the vector to the player on the y axis


        RaycastHit hit;

        if (Physics.Raycast(lightFlattened, lightToPlayer, out hit, lt.range * .7f) && hit.transform == this.transform) // checks to see if it is close enough to the player to actually cast light
        {
            Debug.DrawRay(lightFlattened, lightToPlayer, Color.green);  // draws debug ray
            lt.color = Color.red;
            GameObject.Find("GameManager").GetComponent<GameManager>().PlayerInLight(); //guards chase player
            return true;
        }
        else
        {
            lt.color = Color.white;
            return false;
        }

    }

    void processInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Velocity += (Vector3.forward * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Velocity += (Vector3.left * Time.deltaTime * speed); 
        }
        if (Input.GetKey(KeyCode.S))
        {
            Velocity += (Vector3.back * Time.deltaTime * speed); 
        }
        if (Input.GetKey(KeyCode.D))
        {
            Velocity += (Vector3.right * Time.deltaTime * speed);
        }
        //a brightness adjuster, for unnecessarily dark computers
        if (Input.GetKey(KeyCode.R))
        {
            brightnessAdjuster.intensity += .01f;
        }
        if (Input.GetKey(KeyCode.F))
        {
            brightnessAdjuster.intensity -= .01f;
        }

        Velocity += Vector3.down;

        myCC.Move(Velocity);
        Velocity *= .8f;
    }
}
