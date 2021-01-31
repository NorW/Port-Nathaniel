using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartMenuHandler : MonoBehaviour
{
    [SerializeField] GameObject newGameButton, exitButton;

    [SerializeField] Texture2D buttonUp, buttonHover, buttonDown;

    GameObject mouseOverButton;


    void Start()
    {
        mouseOverButton = null;
    }

    //Returns null if no buttons are pressed. Returns button name if a button is pressed.
    string UpdateButtonStates()
    {
        //Ray cast to button layer and store whether a successful hit was made
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        List<RaycastResult> res = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, res);
        GameObject nextOver = null;

        if (res.Count > 0)
        {
            foreach (var hit in res)
            {
                if (hit.gameObject.layer == 9)
                {
                    nextOver = hit.gameObject;
                }
            }
        }

        //If mouse is now hovering over something else, if the mouse was hovering over a button prior, change that button state to up.
        if(mouseOverButton != nextOver)
        {
            if(mouseOverButton != null)
            {
                mouseOverButton.GetComponent<Image>().sprite = Sprite.Create(buttonUp, new Rect(0, 0, buttonUp.width, buttonUp.height), new Vector2(0.5f, 0.5f));
                mouseOverButton.GetComponent<ButtonInfo>().state = ButtonInfo.ButtonState.Up;
            }
        }

        //If mouse over a button
        if(nextOver != null)
        {
            var buttonInfo = nextOver.GetComponent<ButtonInfo>();

            if(Input.GetMouseButtonUp(0))   //Button pressed
            {
                nextOver.GetComponent<Image>().sprite = Sprite.Create(buttonUp, new Rect(0, 0, buttonUp.width, buttonUp.height), new Vector2(0.5f, 0.5f));
                nextOver.GetComponent<ButtonInfo>().state = ButtonInfo.ButtonState.Up;
                return buttonInfo.buttonName;
            }
            else if(Input.GetMouseButton(0))    //Button down
            {
                if(buttonInfo.state != ButtonInfo.ButtonState.Down) //State change
                {
                    nextOver.GetComponent<Image>().sprite = Sprite.Create(buttonDown, new Rect(0, 0, buttonDown.width, buttonDown.height), new Vector2(0.5f, 0.5f));
                    buttonInfo.state = ButtonInfo.ButtonState.Down;
                }
            }
            else    //Butto over
            {
                if (buttonInfo.state != ButtonInfo.ButtonState.Over) //State change
                {
                    nextOver.GetComponent<Image>().sprite = Sprite.Create(buttonHover, new Rect(0, 0, buttonHover.width, buttonHover.height), new Vector2(0.5f, 0.5f));
                    buttonInfo.state = ButtonInfo.ButtonState.Over;
                }
            }
        }

        mouseOverButton = nextOver;

        return null;
    }

    public MainMenuManager.Menu UpdateMenu()
    {
        var buttonPressed = UpdateButtonStates();
        if(buttonPressed == "New Game")
        {
            return MainMenuManager.Menu.CharacterCreate;
        }
        else if(buttonPressed == "Load")
        {
            return MainMenuManager.Menu.Load;
        }
        else if(buttonPressed == "Exit")
        {
            Application.Quit();
        }

        return MainMenuManager.Menu.Start;
    }
}
