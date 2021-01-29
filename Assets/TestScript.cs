using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    public GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            if(canvas.activeSelf)
            {
                canvas.GetComponent<DialogueManager>().CloseDialogue();
            }
            else
            {
                canvas.GetComponent<DialogueManager>().OpenDialogue("a");
            }
        }
    }
}
