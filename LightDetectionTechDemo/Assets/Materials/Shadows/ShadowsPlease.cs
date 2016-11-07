using UnityEngine;
using System.Collections;

//adds shadows to the object
[RequireComponent(typeof(SpriteRenderer))]

public class ShadowsPlease : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GetComponent<Renderer>().receiveShadows = true;
        GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

}
