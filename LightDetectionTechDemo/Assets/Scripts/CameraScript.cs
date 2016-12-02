using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public enum Mode { FollowPlayer, FollowTarget }
    public Vector3 FromFollowPoint;
    public Mode CurrentMode;
    public GameObject Target;
    GameObject PlayerObject;

    Plane cursorPlane;

    public int stretchDistance = 10;

    // Use this for initialization
    void Start () {
        PlayerObject = GameObject.FindGameObjectWithTag("GamePlayer");
	}
	
	// Update is called once per frame
	void Update () {
	    switch(CurrentMode)
        {
            case Mode.FollowPlayer:

                Vector3 Additive = PlayerObject.transform.position + FromFollowPoint;
                if (!Input.GetKey(KeyCode.Space))
                {
                    Additive += HandleCursorWorldPointPosition();
                    Additive /= 2;
                }
                else
                {
                    Additive.z += 5f;
                }

                Additive.y = PlayerObject.transform.position.y + FromFollowPoint.y;
                transform.position = Vector3.Lerp(transform.position, Additive, 3f * Time.deltaTime);
                break;

            case Mode.FollowTarget:
                transform.position = Vector3.Lerp(transform.position, Target.transform.position + FromFollowPoint, 3f * Time.deltaTime);
                break;
        }
	}

    Vector3 HandleCursorWorldPointPosition()
    {
        // This creates a plane located perpendicular to the players Y axis, and parallel with the X and Z axis.
        // We raycast down to the player to find the position the cursor is pointing to in world space, and then return that position
        // Make a plane 
        cursorPlane = new Plane(Vector3.up, PlayerObject.transform.position);

        // Get the ray from the mouse position directed downwards
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Ray ray;
        ray = Camera.main.ScreenPointToRay(mousePosition); // This takes the mouse position, and starts a ray at the world position (based on where the mouse is on screen) shooting where the camera is pointed

        float dist;
        if (cursorPlane.Raycast(ray, out dist))
        {
            Vector3 point = ray.GetPoint(dist);

            if (Vector3.Distance(PlayerObject.transform.position, point) > stretchDistance)
            {
                point = PlayerObject.transform.position + (((point - PlayerObject.transform.position).normalized) * stretchDistance);
            }

            return point; // Get the point
        }
        else
        {
            //Debug.Log("We could not properly locate the world position for the cursor!");
        }


        return Vector3.zero;
    }
}
