using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameScript : MonoBehaviour
{
    PlayerInteractionBox interactionBox;
    float interactCooldown = 0;

    // Start is called before the first frame update
    void Start()
    {
        interactionBox = GetComponentInChildren<PlayerInteractionBox>();
        interactionBox.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!DialogueManager.manager.gameObject.activeSelf)
        {
            if (interactCooldown > 0)
            {
                interactCooldown -= Time.deltaTime;
                if (interactCooldown <= 0)
                {
                    interactionBox.gameObject.SetActive(false);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    interactCooldown = 0.2f;
                    interactionBox.gameObject.SetActive(true);
                }
            }
        }
    }
}
