using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent (typeof(BoxCollider))]
public class ElevatorScript : MonoBehaviour {

    public bool State;
    public string nextScene;
    bool StageFinished;
    GameObject DoorLeft;
    GameObject DoorRight;
    Light ElevatorLight;
    
  

    Vector3 ClosedPositionRight =  new Vector3( .95f, 1, .5f );
    Vector3 ClosedPositionLeft = new Vector3(.95f, 1, -.5f);

    Vector3 OpenPositionRight = new Vector3(.95f, 1, 1.5f);
    Vector3 OpenPositionLeft = new Vector3(.95f, 1, -1.5f);
    // Use this for initialization
    void Start () {

        // Get the two door game objects 
        foreach(Transform obj in transform)
        {
            if (obj.name == "DoorLeft")
                DoorLeft = obj.gameObject;

            if (obj.name == "DoorRight")
                DoorRight = obj.gameObject;

            if (obj.name == "ElevatorLight")
                ElevatorLight = obj.GetComponent<Light>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (DoorLeft && DoorRight) // Ensuring that the doors are initialized
        {
            if(!StageFinished) // If the stage isn't finished
            {
                if (State) // Move the doors based on what state we're in
                {
                    DoorLeft.transform.localPosition = Vector3.Lerp(DoorLeft.transform.localPosition, OpenPositionLeft, 3f * Time.deltaTime);
                    DoorRight.transform.localPosition = Vector3.Lerp(DoorRight.transform.localPosition, OpenPositionRight, 3f * Time.deltaTime);
                    ElevatorLight.color = Color.green;
                }
                else
                {
                    DoorLeft.transform.localPosition = Vector3.Lerp(DoorLeft.transform.localPosition, ClosedPositionLeft, 3f * Time.deltaTime);
                    DoorRight.transform.localPosition = Vector3.Lerp(DoorRight.transform.localPosition, ClosedPositionRight, 3f * Time.deltaTime);
                    ElevatorLight.color = Color.red;
                }
            }
            else
            {
                // Doors should close if the stage is finished
                ElevatorLight.color = Color.blue;
                DoorLeft.transform.localPosition = Vector3.Lerp(DoorLeft.transform.localPosition, ClosedPositionLeft, 3f * Time.deltaTime);
                DoorRight.transform.localPosition = Vector3.Lerp(DoorRight.transform.localPosition, ClosedPositionRight, 3f * Time.deltaTime);
                StartCoroutine(NextScene());

            }

        }
        else
            Debug.Log("A door isn't initialized!");
	    
	}

    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(nextScene);
    }

    void OnTriggerEnter(Collider Col)
    {
        if(Col.tag == "GamePlayer")
        {
            StageFinished = true;
        }
    }
}
