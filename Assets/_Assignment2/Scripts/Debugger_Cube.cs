using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger_Cube : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"))
        {
            color = Color.magenta
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
