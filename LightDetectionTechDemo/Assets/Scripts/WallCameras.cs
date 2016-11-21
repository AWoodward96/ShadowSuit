using UnityEngine;
using System.Collections;

public class WallCameras : MonoBehaviour
{
    public int range;
    public GameObject player;
    public GameObject scene;
    public float angleDirection;
    public float angleRange;
    public float rotateLeft;
    public float rotateRight;
    bool direcLeft = true;

    int counter = 0;

    public int visionDetail = 200;

    // Use this for initialization
    void Start ()
    {
        createVision();

        if (rotateLeft == 0)
        {
            rotateLeft = angleDirection;
            direcLeft = false;
        }
        if(rotateRight == 0)
        {
            rotateRight = angleDirection;
            direcLeft = true;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(direcLeft && rotateLeft != angleDirection)
        {
            angleDirection += .5f;
            if(Mathf.Abs(rotateLeft - angleDirection) < 1)
            {
                direcLeft = false;
            }
        }
        if (!direcLeft && rotateRight != angleDirection)
        {
            angleDirection -= .5f;
            if (Mathf.Abs(rotateRight - angleDirection) < 1)
            {
                direcLeft = true;
            }
        }
        while(angleDirection > 360)
        {
            angleDirection -= 360;
        }
        while (angleDirection < 0)
        {
            angleDirection += 360;
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

        //attempting to control framerate issues in any way
        counter++;
        if(counter == 5)
        {
            counter = 0;
            //createVision();
        }
    }

    //THIS STILL CAUSES LOTS OF LAG
    //there needs to be a more efficient way of doing this
    public void createVision()
    {
        RaycastHit hit;

        //scaled size of each pixel, inverted (200 is max size)
        int numb = visionDetail;
        //scales the image to be the size it should be
        gameObject.transform.GetChild(0).transform.localScale = new Vector3(200/numb, 200/numb, 0);

        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        var texture = new Texture2D(numb * range, numb * range, TextureFormat.ARGB32, false);

        // set the pixel values
        for (int i = 0; i < texture.width; i++)
        {
            //the x point if the center is 0
            int iFromCent = i - (texture.width/2);
            for (int i2 = 0; i2 < texture.height; i2++)
            {
                texture.SetPixel(i, i2, Color.clear);
                //the y point if the center is 0
                int i2FromCent = i2 - (texture.height/2);
                //makes sure the point is within a certain distance range to qualify
                if (Mathf.Pow(iFromCent + iFromCent, 2) + Mathf.Pow(i2FromCent + i2FromCent, 2) < Mathf.Pow(numb * range, 2) && Mathf.Pow(iFromCent + iFromCent, 2) + Mathf.Pow(i2FromCent + i2FromCent, 2) > Mathf.Pow((numb/5) * range, 2))
                {
                    float angle;
                    //turns the points into a vector
                    Vector2 diff = new Vector2(-iFromCent, i2FromCent);
                    diff /= diff.magnitude;
                    //turns the point into an angle based on it's location relative to the center
                    if (diff.y < 0)
                    {
                        angle = 360 - (Mathf.Rad2Deg * Mathf.Acos(diff.x));
                    }
                    else
                    {
                        angle = Mathf.Rad2Deg * Mathf.Acos(diff.x);
                    }
                    //checks to see if the point is within the angle range
                    if (angle >= angleDirection - angleRange && angle <= angleDirection + angleRange)
                    {
                        //scaling number based on how detailed the vision cone is
                        int divideBy = numb / 2;
                        //checks a raycast for each point of the cone, not drawing the pixel if the walls of the scene are in the way
                        if (Physics.Raycast(transform.position, (new Vector3(iFromCent, 0, -i2FromCent) / divideBy), out hit, new Vector3(iFromCent, 0, -i2FromCent).magnitude / divideBy) && hit.collider == scene.GetComponent<Collider>())
                        {
                        }
                        else
                        {
                            texture.SetPixel(i, i2, Color.white);
                        }
                    }
                }
            }
        }

        // Apply all SetPixel calls
        texture.Apply();

        // connect texture to material of GameObject this script is attached to
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
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
        if(dist > range || dist < range/5)
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
