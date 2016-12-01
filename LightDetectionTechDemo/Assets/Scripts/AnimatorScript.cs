using UnityEngine;
using System.Collections;

public class AnimatorScript : MonoBehaviour {

    public bool DEBUG;
    public bool isMoving;
    public Direction myDirection;
    public enum Direction { Right,Up,Left,Down }
    Animator myAnimator;
    public float speed = 1;

    public void UpdateAnimator(float horez, float vert)
    {  
        // We'll need these for science
        float horezAbs = Mathf.Abs(horez);
        float vertAbs = Mathf.Abs(vert);

        // Assumes that if both horez and vert are 0 that we're not moving otherwise we are
        if (horezAbs <= .5 && vertAbs <= .5)
            isMoving = false;
        else
            //Debug.Log(horez + " - " + vert);
            isMoving = true;

        // Only update the direction if we're moving
        if(isMoving)
        {
            //Now get the direction out of the horez and verticle
            if (vertAbs > horezAbs) // We are primarily moving up and down
            {
                if (vert >= 0)
                    myDirection = Direction.Up;
                else
                    myDirection = Direction.Down;
            }
            else // We are primarily moving left to right
            {
                if (horez >= 0)
                    myDirection = Direction.Right;
                else
                    myDirection = Direction.Left;
            }
        }


        // Call send to animator
        SendToAnimator();
    }

    public void UpdateAnimator(float horez, float vert, float _speed)
    {
        // We'll need these for science
        float horezAbs = Mathf.Abs(horez);
        float vertAbs = Mathf.Abs(vert);

        // Assumes that if both horez and vert are 0 that we're not moving otherwise we are
        if (horezAbs <= .5 && vertAbs <= .5)
            isMoving = false;
        else
            isMoving = true;

        // Only update the direction if we're moving
        if(isMoving)
        {
            //Now get the direction out of the horez and verticle
            if (vertAbs > horezAbs) // We are primarily moving up and down
            {
                if (vert >= 0)
                    myDirection = Direction.Up;
                else
                    myDirection = Direction.Down;
            }
            else // We are primarily moving left to right
            {
                if (horez >= 0)
                    myDirection = Direction.Right;
                else
                    myDirection = Direction.Left;
            }
        }

        speed = _speed;

        // Call send to animator
        SendToAnimator();
    }

    public void UpdateAnimator(int direction, bool moving)
    {
        // I'm not sure if there's a better way to go about this
        myDirection = ReturnDirectionFromInt(direction);

        // Assign the moving variable
        isMoving = moving;

        SendToAnimator();
    }

    public void UpdateAnimator(int direction)
    {
        // Assuming that the moving isn't changing
        myDirection = ReturnDirectionFromInt(direction);

        SendToAnimator();
    }

    // Will actually update the variables within the animator
    void SendToAnimator()
    {
        if(!myAnimator)
        {
            myAnimator = GetComponent<Animator>();
        }

        if(myAnimator)
        {
            // These values are hard coded in. isMoving and Direction should be two parameters in any animator this script is attached to 
            myAnimator.SetBool("isMoving", isMoving);
            myAnimator.SetFloat("Direction", (float)myDirection); // This has to be a float because branch tree's cant be based off of an int variable. It works, dw about it
            myAnimator.SetFloat("Speed", speed);
        }
        else
        {
            if(DEBUG)
                Debug.Log("There is no animator attached to this game object! Animator script is failing!"); // So we don't crash our executable
        }
    }

    public Direction ReturnDirectionFromInt(int direction)
    {
        switch (direction) // But here we are
        {
            case 0:
                return Direction.Right;
 
            case 1:
                return Direction.Up;
 
            case 2:
                return Direction.Left;
 
            case 3:
                return Direction.Down;
            default: 
                return Direction.Down; // Hopefully this will never be called
        }
    }
}
