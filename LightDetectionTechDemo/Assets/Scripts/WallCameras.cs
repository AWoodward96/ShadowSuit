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
    public int alertRange;
    Rect alertRect;
    public bool deActivated;

    public int visionDetail = 200;
    Texture2D blankTexture;

    // Use this for initialization
    void Start ()
    {
        //basic blank texture to base the actual camera texture on
        blankTexture = new Texture2D((visionDetail * range)+1, (visionDetail * range)+1, TextureFormat.ARGB32, false);
        for (int i = 0; i < (visionDetail * range)+1; i++)
        {
            for (int i2 = 0; i2 < (visionDetail * range)+1; i2++)
            {
                blankTexture.SetPixel(i, i2, Color.clear);
            }
        }
        // Apply all SetPixel calls
        blankTexture.Apply();

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

        alertRect = new Rect(transform.position.x - alertRange, transform.position.z - alertRange, alertRange * 2, alertRange * 2);
        deActivated = false;
	}

    // Update is called once per frame
    void Update()
    {
        if (!deActivated)
        {


            if (direcLeft && rotateLeft != angleDirection)
            {
                //disabling rotation for now
                angleDirection += .5f;
                if (Mathf.Abs(rotateLeft - angleDirection) < 1)
                {
                    direcLeft = false;
                }
            }
            if (!direcLeft && rotateRight != angleDirection)
            {
                //disabling rotation for now
                angleDirection -= .5f;
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
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); //gets all tagged enemies
                    for (int i = 0; i < enemies.Length; i++) //loops through all the enemies
                    {
                        //Debug.Log("Any - " + Vector2.Distance(alertRect.position, new Vector2(enemies[i].transform.position.x, enemies[i].transform.position.z)));
                        if (alertRect.Contains(new Vector2(enemies[i].transform.position.x, enemies[i].transform.position.z)) && !enemies[i].GetComponent<EnemyController>().busy)
                        {
                            //enemies go to where the player was last seen
                            enemies[i].GetComponent<EnemyController>().myState = EnemyController.AIState.Chasing;
                            enemies[i].GetComponent<EnemyController>().target = player;
                        }
                    }
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

            createNewVision();
        }
        else
        {
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Sprite.Create(blankTexture, new Rect(0, 0, blankTexture.width, blankTexture.height), new Vector2(0.5f, 0.5f));
        }
    }

    //new way of creating camera vision
    //only creates outline of vision
    //can still be laggy with multiple cameras
    public void createNewVision()
    {
        // Create a new texture ARGB32 (32 bit with alpha) and no mipmaps
        var texture = new Texture2D((visionDetail * range) + 1, (visionDetail * range) + 1, TextureFormat.ARGB32, false);
        //var texture = blankTexture;
        texture.SetPixels(blankTexture.GetPixels());

        RaycastHit hit;

        //scaled size of each pixel, inverted (200 is max size)
        int numb = visionDetail;

        //scales the image to be the size it should be
        gameObject.transform.GetChild(0).transform.localScale = new Vector3(200 / numb, 200 / numb, 0);
        //gameObject.transform.GetChild(0).transform.eulerAngles = new Vector3(0, 8)

        float angleDetail = .2f;

        //creates a set of raycasts at incrementing angles. These angles are used to check sets of pixels instead of each pixel raycasting
        float[] rayDist = new float[(int)(2 * angleRange / angleDetail) + 1];
        for (int i = 0; i <=  2 * angleRange / angleDetail; i++)
        {
            //checks if the raycast hits anything in the way
            if (Physics.Raycast(transform.position, new Vector3(-Mathf.Cos(Mathf.Deg2Rad * ((angleDirection - angleRange) + (i * angleDetail))), transform.position.y, -Mathf.Sin(Mathf.Deg2Rad * ((angleDirection - angleRange) + (i * angleDetail)))), out hit, range) && hit.collider == scene.GetComponent<Collider>())
            {
                rayDist[i] = hit.distance;
                //Debug.DrawLine(transform.position, transform.position - new Vector3(hit.distance * Mathf.Cos(Mathf.Deg2Rad * ((angleDirection - angleRange) + (i * angleDetail))), transform.position.y, hit.distance * Mathf.Sin(Mathf.Deg2Rad * ((angleDirection - angleRange) + (i * angleDetail)))), Color.green);  // draws debug ray
            }
            else
            {
                rayDist[i] = range;
                //Debug.DrawLine(transform.position, transform.position - new Vector3(range * Mathf.Cos(Mathf.Deg2Rad * ((angleDirection - angleRange) + (i * angleDetail))), transform.position.y, range * Mathf.Sin(Mathf.Deg2Rad * ((angleDirection - angleRange) + (i * angleDetail)))), Color.green);  // draws debug ray
            }
        }
        //checks each raycast set and creates a pixel at its max length
        for (int i = 0; i < rayDist.Length; i++)
        {
            //assuming the far left of the angle is 0, this angle puts it back where it is supposed to be
            float angleOffset = (angleDirection - angleRange);

            float percent = rayDist[i] * texture.width / 20f;
            texture.SetPixel((texture.width / 2) + (int)((percent) * -Mathf.Cos(Mathf.Deg2Rad * ((angleDirection - angleRange) + (i * angleDetail)))), (texture.height / 2) + (int)((percent) * Mathf.Sin(Mathf.Deg2Rad * ((angleDirection - angleRange) + (i * angleDetail)))), Color.white);
            //checks each set with the sets next to them and draws a line if the adjacent set does not reach as far
            float shorter = -1;
            if(i == 0 || i == rayDist.Length -1)
            {
                shorter = 0;
            }
            else if(rayDist[i - 1] < rayDist[i])
            {
                shorter = rayDist[i - 1];
            }
            else if (rayDist[i + 1] < rayDist[i] && (shorter == -1 || rayDist[i + 1] < shorter))
            {
                shorter = rayDist[i + 1];
            }
            if (shorter != -1)
            {
                for (float i2 = shorter * texture.width / 20; i2 < percent; i2++)
                {
                    texture.SetPixel((texture.width / 2) + (int)(i2 * -Mathf.Cos(Mathf.Deg2Rad * ((angleOffset) + (i * angleDetail)))), (texture.height / 2) + (int)((i2) * Mathf.Sin(Mathf.Deg2Rad * ((angleOffset) + (i  * angleDetail)))), Color.white);
                }
            }
        }

        // Apply all SetPixel calls
        texture.Apply();

        // connect texture to material of GameObject this script is attached to
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.texture = texture;
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
        if(dist > range/* || dist < range/5*/)
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