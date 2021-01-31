using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    public GameObject dialoguePanel, mapMenuPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            if(dialoguePanel.activeSelf)
            {
                dialoguePanel.GetComponent<DialogueManager>().CloseDialogue();
            }
            else
            {
                dialoguePanel.GetComponent<DialogueManager>().OpenDialogue("a");
            }
        }

        if(Input.GetKeyDown(KeyCode.M))
        {

        }
    }
}
