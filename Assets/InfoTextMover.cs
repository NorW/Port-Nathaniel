using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoTextMover : MonoBehaviour
{
    public GameObject mapMenuPanel, infoTextBox;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void HideBox()
    {
        gameObject.SetActive(false);
    }

    public void ShowBox(string name, string info)
    {
        SetText(name, info);
        UpdatePosition();
        gameObject.SetActive(true);
    }

    //TODO consider adjusting text box based on how much info is being set
    public void SetText(string name, string info)
    {
        infoTextBox.GetComponent<TextMeshProUGUI>().text = "<b>"+name+"</b>\n" + info;
    }

    public void UpdatePosition()
    {
        //Set textbox's top left corner to mouse position
        var mousePos = Input.mousePosition;
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;
        var size = gameObject.GetComponent<RectTransform>().sizeDelta;

        Vector3 offset = new Vector3();

        offset.x -= size.x / 2 + 10;
        offset.y += size.y / 2 + 20;
        var newPos = mousePos - offset;

        //TODO check if flip method works better
        //Check if textbox will be off screen to the left or bottom
        if ((newPos.x + size.x / 2) > screenWidth)
        {
            newPos.x -= (newPos.x + size.x / 2) - screenWidth;
        }

        if ((newPos.y - size.y / 2) < 0)
        {
            newPos.y += size.y / 2 - newPos.y;
        }

        gameObject.GetComponent<RectTransform>().anchoredPosition = newPos;
    }

    void Update()
    {
        UpdatePosition();
    }
}
