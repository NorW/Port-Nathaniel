using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_control : MonoBehaviour
{
    public static float speed = 2.5f;

    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.Translate(Vector3.left * speed * Time.deltaTime);
            animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
        }

        if (Input.GetKey(KeyCode.S))
        {
            gameObject.transform.Translate(Vector3.down * speed * Time.deltaTime);
            animator.SetFloat("Vertical", Input.GetAxis("Vertical"));
        }
        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.Translate(Vector3.right * speed * Time.deltaTime);
            animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));

        }
        if (Input.GetKey(KeyCode.W))
        {
            gameObject.transform.Translate(Vector3.up * speed * Time.deltaTime);
            animator.SetFloat("Vertical", Input.GetAxis("Vertical"));
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



}
