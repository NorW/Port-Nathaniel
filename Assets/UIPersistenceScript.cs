using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPersistenceScript : MonoBehaviour
{
    private static GameObject UIInstance;
    // Start is called before the first frame update
    void Awake()
    {
        if(UIInstance == null)
        {
            UIInstance = gameObject;
            DontDestroyOnLoad(gameObject);
            DialogueManager.manager.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
