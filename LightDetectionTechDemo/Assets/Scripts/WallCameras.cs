using UnityEngine;
using System.Collections;

public class WallCameras : MonoBehaviour
{
    public int range;
    public GameObject player;
    public float angleDirection;
    public float angleRange;
    public float rotateLeft;
    public float rotateRight;
    bool direcLeft = true;

	// Use this for initialization
	void Start ()
    {
        if (rotateLeft == 0)
        {
            rotateLeft = angleDirection;
        }
        if(rotateRight == 0)
        {
            rotateRight = angleDirection;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(direcLeft && rotateLeft != angleDirection)
        {
            angleDirection += .5f;
            gameObject.transform.GetChild(0).transform.Rotate(new Vector3(0, 0, -.5f));
            if(Mathf.Abs(rotateLeft - angleDirection) < 1)
            {
                direcLeft = false;
            }
        }
        if (!direcLeft && rotateRight != angleDirection)
        {
            angleDirection -= .5f;
            gameObject.transform.GetChild(0).transform.Rotate(new Vector3(0, 0, .5f));
            if (Mathf.Abs(rotateRight - angleDirection) < 1)
            {
                direcLeft = true;
            }
        }
        if (CanSee(player))
        {
            if (player.GetComponent<PlayerController>().InLight)
            {
                gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
            }
        }
        else
        {
            gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.white;
        }
	}

    public bool CanSee(GameObject target)
    {
        RaycastHit hit;
        
        //line of sight check
        if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit) && hit.collider != target.GetComponent<Collider>())
        {
            //Debug.Log("Hit Something");
            //Debug.DrawLine(transform.position, target.transform.position);
            return false;
        }
        //distance check
        float dist = (transform.position - target.transform.position).magnitude;
        if(dist > range || dist < 2)
        {
            return false;
        }
        //angle check
        float angle;
        Vector2 diff = (new Vector2(transform.position.x, transform.position.z) - new Vector2(target.transform.position.x, target.transform.position.z));
        diff /= diff.magnitude;
        if(diff.y < 0)
        {
            angle = 360 - (Mathf.Rad2Deg * Mathf.Acos(diff.x));
        }
        else
        {
            angle = Mathf.Rad2Deg * Mathf.Acos(diff.x);
        }
        //Debug.Log("Success - " + angle);
        if(angle >= angleDirection - angleRange && angle <= angleDirection + angleRange)
        {
            return true;
        }
        return false;
    }
}
