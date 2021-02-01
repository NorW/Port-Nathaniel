using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_control : MonoBehaviour
{
    public static float speed = 1.5f;

    public AnimationScript animator;

    PlayerInteractionBox interactionBox;
    float interactCooldown = 0;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<AnimationScript>();
        interactionBox = GetComponentInChildren<PlayerInteractionBox>();
        interactionBox.gameObject.SetActive(false);
    }

    void UpdateMovement()
    {
        bool moving = false;
        int newDirection = -1;

        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.Translate(Vector3.left * speed * Time.deltaTime);
            newDirection = AnimationScript.LEFT;
            moving = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.Translate(Vector3.right * speed * Time.deltaTime);
            newDirection = AnimationScript.RIGHT;
            moving = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            gameObject.transform.Translate(Vector3.down * speed * Time.deltaTime);
            newDirection = AnimationScript.DOWN;
            moving = true;

        }

        if (Input.GetKey(KeyCode.W))
        {
            gameObject.transform.Translate(Vector3.up * speed * Time.deltaTime);
            newDirection = AnimationScript.UP;
            moving = true;

        }

        if (!moving)
        {
            animator.StopWalk();
        }
        if (newDirection != -1)
        {
            animator.StartWalk(newDirection);
        }
    }

    void HandleInteraction()
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                interactCooldown = 0.2f;
                interactionBox.gameObject.SetActive(true);
            }
        }
    }

    private void UpdateMap()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            if(MapMenuManager.manager.gameObject.activeSelf)
            {
                MapMenuManager.manager.CloseMenu();
            }
            else
            {
                MapMenuManager.manager.OpenMenu();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!DialogueManager.manager.gameObject.activeSelf)
        {
            if (!MapMenuManager.manager.gameObject.activeSelf)
            {
                UpdateMovement();
                HandleInteraction();
                if (!DialogueManager.manager.gameObject.activeSelf)
                {
                    UpdateMap();
                }
            }
            else
            {
                UpdateMap();
            }
        }
       

        // if(Input.GetKey(KeyCode.M)){
        // OpenMap();
        // }

    }

    //    private void CheckInteraction()
    //   {
    //     RaycastHit2D hit = Physics2D.BoxCast()
    // }

    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //   if (collision.CompareTag("Player"))
    //    {
    //    if (Input.GetKey(KeyCode.E))
    //       {
    //OpenDialogue();

    //        }
    //    }
    //  }
    //asdasdasd



}
