using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public GameObject PointLight;
    public GameObject SpotLight;
    private SpriteRenderer spriteRend;

	// Use this for initialization
	void Start () {
        spriteRend = this.GetComponent<SpriteRenderer>();   // grabs the spriteRenderer from the player object
	
	}
	
	// Update is called once per frame
	void Update () {
        if(isLitSpot(SpotLight))
            spriteRend.color = Color.blue;  // if in a point light, turns blue
        else if(isLitPoint(PointLight))
            spriteRend.color = Color.red;   // if in a spot light, turns red
        else
            spriteRend.color = Color.white; // if in neither, turns white
	}

    bool isLitSpot(GameObject lite) // checks to see if the light passed in as a parameter is lighting the player (assumes that it is a spot light)
    {
        Light lt = lite.GetComponent<Light>();  // grabs light component
        Vector3 lightToPlayer = this.transform.position - lite.GetComponent<Transform>().position;  // gets a vector from the light to the player
        Vector3 lightForward = lite.GetComponent<Transform>().forward;  // gets the forward vector of the light
        Vector3 lightFlattened = lite.GetComponent<Transform>().position;   // gets position vector of the light
        lightForward.z = 0;     // flattens the forward vector on the z axis 
        lightToPlayer.z = 0;    // flattens the vector to the player on the z axis
        float arc = lt.spotAngle;   // gets the spot angle of the light
        float angle = Vector3.Angle(lightForward, lightToPlayer);   // gets the angle between the forward vector and the vector to the player

        Debug.DrawRay(lightFlattened, lightToPlayer, Color.green);  // draws debug ray
        if (Physics.Raycast(lightFlattened, lightToPlayer, lt.range * .4f) && angle < (arc/2))  // checks to see if it is close enough to the player to actually cast light AND if it is within the arc of light 
            return true;
        else
            return false;
    }

    bool isLitPoint(GameObject lite)    // checks to see if the light passed in as a parameter is lighting the player (assumes that it is a point light)
    {
        Light lt = lite.GetComponent<Light>();  // grabs light component
        Vector3 lightToPlayer = this.transform.position - lite.GetComponent<Transform>().position;  // gets a vecctor from the light to the player
        Vector3 lightFlattened = lite.GetComponent<Transform>().position;   // gets position vector of the light
        lightToPlayer.z = 0;    // flattens the forward vector on the z axis
        lightFlattened.z = 0;   // flattens the vector to the player on the z axis

        Debug.DrawRay(lightFlattened, lightToPlayer, Color.green);  // draws debug ray
        if (Physics.Raycast(lightFlattened, lightToPlayer, lt.range * .7f)) // checks to see if it is close enough to the player to actually cast light
            return true;
        else
            return false;
    }
}
