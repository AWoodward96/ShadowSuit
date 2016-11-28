using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    List<TexPoint> visionPath = new List<TexPoint>();
    Texture2D blankTexture;

    // Use this for initialization
    void Start ()
    {
        //basic blank texture to base the actual camera texture on
        blankTexture = new Texture2D(visionDetail * range, visionDetail * range, TextureFormat.ARGB32, false);
        for (int i = 0; i < visionDetail * range; i++)
        {
            for (int i2 = 0; i2 < visionDetail * range; i2++)
            {
                blankTexture.SetPixel(i, i2, Color.clear);
            }
        }
        // Apply all SetPixel calls
        blankTexture.Apply();

        createVisionPath();
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
    void Update()
    {
        if (direcLeft && rotateLeft != angleDirection)
        {
            //disabling rotation for now
            //angleDirection += .5f;
            if (Mathf.Abs(rotateLeft - angleDirection) < 1)
            {
                direcLeft = false;
            }
        }
        if (!direcLeft && rotateRight != angleDirection)
        {
            //disabling rotation for now
            //angleDirection -= .5f;
            if (Mathf.Abs(rotateRight - angleDirection) < 1)
            {
                direcLeft = true;
            }
        }
        while (angleDirection > 360)
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
        if (counter == 3)
        {
            counter = 0;
        }
        createVision();
    }

    //THIS STILL CAUSES LOTS OF LAG
    //there needs to be a more efficient way of doing this
    public void createVision()
    {
        RaycastHit hit;

        //scaled size of each pixel, inverted (200 is max size)
        int numb = visionDetail;

        float angleDetail = .25f;

        //creates a set of raycasts at incrementing angles. These angles are used to check sets of pixels instead of each pixel raycasting
        float[] rayDist = new float[(int)(((angleDirection + angleRange) - (angleDirection - angleRange)) / angleDetail) + 1];
        for (int i = 0; i <= ((angleDirection + angleRange) - (angleDirection - angleRange)) / angleDetail; i++)
        {
            if (Physics.Raycast(transform.position, new Vector3(-Mathf.Cos(Mathf.Deg2Rad * ((angleDirection - angleRange) + (i * angleDetail))), transform.position.y, -Mathf.Sin(Mathf.Deg2Rad * ((angleDirection - angleRange) + (i * angleDetail)))), out hit, range) && hit.collider == scene.GetComponent<Collider>())
            { 
                rayDist[i] = hit.distance;
                //Debug.DrawLine(transform.position, transform.position - new Vector3(hit.distance * Mathf.Cos(Mathf.Deg2Rad * ((angleDirection - angleRange) + (i * angleDetail))), transform.position.y, hit.distance * Mathf.Sin(Mathf.Deg2Rad * ((angleDirection - angleRange) + (i * angleDetail)))), Color.green);  // draws debug ray
            }
            else
            {
                rayDist[i] = 10;
                //Debug.DrawLine(transform.position, transform.position - new Vector3(range * Mathf.Cos(Mathf.Deg2Rad * ((angleDirection - angleRange) + (i * angleDetail))), transform.position.y, range * Mathf.Sin(Mathf.Deg2Rad * ((angleDirection - angleRange) + (i * angleDetail)))), Color.green);  // draws debug ray
            }
        }

        //scales the image to be the size it should be
        gameObject.transform.GetChild(0).transform.localScale = new Vector3(200 / numb, 200 / numb, 0);

        // Create a new texture ARGB32 (32 bit with alpha) and no mipmaps
        var texture = new Texture2D(visionDetail * range, visionDetail * range, TextureFormat.ARGB32, false);
        //var texture = blankTexture;
        texture.SetPixels(blankTexture.GetPixels());
        
        //loops through the set pixels that are a part of the camera's view
        for (int i = 0; i < visionPath.Count; i++)
        {
            float iFromCent = (visionPath[i].pos.x - (texture.width / 2));
            float i2FromCent = (visionPath[i].pos.y - (texture.height / 2));
            //for rotations whenever that happens
            //float iFromCentRot = ((iFromCent * Mathf.Cos(Mathf.Deg2Rad * (90 - angleDirection))) - (i2FromCent * Mathf.Sin(Mathf.Deg2Rad * (90 - angleDirection))));
            //float i2FromCentRot = ((i2FromCent * Mathf.Cos(Mathf.Deg2Rad * (90 - angleDirection))) + (iFromCent * Mathf.Sin(Mathf.Deg2Rad * (90 - angleDirection))));
            int arrayCheck = (int)(Mathf.Round((visionPath[i].angle + (90 - angleDirection)) / angleDetail) - ((angleDirection - angleRange) / angleDetail));
            if (new Vector2(iFromCent * 2, i2FromCent * 2).magnitude  < (numb * (rayDist[arrayCheck])))
            {
                //sets the individual pixel's color
                texture.SetPixel((int)visionPath[i].pos.x, (int)visionPath[i].pos.y, Color.white);
                //for rotations whenever that happens
                //texture.SetPixel((int)(iFromCentRot + (texture.width / 2)), (int)(i2FromCentRot + (texture.height / 2)), Color.white);
            }
        }

        // Apply all SetPixel calls
        texture.Apply();

        // connect texture to material of GameObject this script is attached to
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    //creates the list of pixels that are included in the camera texture
    public void createVisionPath()
    {
        int numb = visionDetail;

        //loops through every possible pixel, only adding the pixels included in the camera's range
        for (int i = 0; i < numb * range; i++)
        {
            int iFromCent = i - (numb * range / 2);

            for (int i2 = 0; i2 < numb * range; i2++)
            {
                int i2FromCent = i2 - (numb * range / 2);
                if (Mathf.Pow(iFromCent + iFromCent, 2) + Mathf.Pow(i2FromCent + i2FromCent, 2) < Mathf.Pow(numb * range, 2) && Mathf.Pow(iFromCent + iFromCent, 2) + Mathf.Pow(i2FromCent + i2FromCent, 2) > Mathf.Pow((numb / 5) * range, 2))
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
                        visionPath.Add(new TexPoint(i, i2, angle));
                    }
                }
            }
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

public class TexPoint
{
    public Vector2 pos;
    public float angle;

    public TexPoint(int x, int y, float a)
    {
        pos = new Vector2(x, y);
        angle = a;
    }
}