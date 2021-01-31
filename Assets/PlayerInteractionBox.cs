using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionBox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        var interactable = collision.gameObject.GetComponent<Interactable>();

        if (interactable != null)
        {
            DialogueManager.manager.OpenDialogue(interactable.InteractableName);
        }
    }
}
