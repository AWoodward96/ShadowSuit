using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour {


    
    public GameObject[] Sliders;
    public Transform Extents;
    public Transform Reset;
    public Text BlinkingText;
    bool TextState;
    bool cdRunning;
    bool showControls;
    public string SceneToBootUp;

    public SpriteRenderer CameraTinter;
    bool next;

	// Use this for initialization
	void Start () {
        TextState = true;
	}
	
	// Update is called once per frame
	void Update () {
	    foreach(GameObject obj in Sliders)
        {
            obj.transform.position += Vector3.left / 20;

            if(Vector3.Distance(obj.transform.position, Extents.position) < 1)
            {
                obj.transform.position = Reset.position;
            }
        }

        BlinkingText.gameObject.SetActive(TextState);

        if(!cdRunning)
        {
            cdRunning = true;
            StartCoroutine(CD());
        }

        if(Input.anyKeyDown)
        {
            if (SceneToBootUp == "EXIT")
            {
                Application.Quit();
            }
            next = true;
            BlinkingText.transform.parent.gameObject.SetActive(false);
            StartCoroutine(ToNextScene());
        }

        if(next)
        {
            
            CameraTinter.color = Color.Lerp(CameraTinter.color, Color.black, Time.deltaTime * 1.2f);
        }
	}

    IEnumerator CD()
    {
        yield return new WaitForSeconds(1);
        TextState = !TextState;
        cdRunning = false;
    }

    IEnumerator ToNextScene()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(SceneToBootUp);
    }
}
