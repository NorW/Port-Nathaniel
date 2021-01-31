using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    public enum Menu
    {
        Start,              //Start menu
        Load,               //Load menu
        CharacterCreate,     //New game - character creation
        Game                //Transition to game
    };

    [SerializeField] GameObject infoTextPanel;
    [SerializeField] GameObject mainMenuPanel, loadMenuPanel, characterCreatePanel;

    StartMenuHandler mainMenuHandler;
    CharacterCreatorManager characterMenuManager;

    Menu currentMenu;

    // Start is called before the first frame update
    void Start()
    {
        currentMenu = Menu.Start;
        mainMenuHandler = mainMenuPanel.GetComponent<StartMenuHandler>();
        characterMenuManager = characterCreatePanel.GetComponent<CharacterCreatorManager>();
        loadMenuPanel.SetActive(false);
        characterCreatePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        Menu nextMenu = currentMenu;
        switch(currentMenu)
        {
            case Menu.Start:
                nextMenu = mainMenuHandler.UpdateMenu();
                break;
            case Menu.Load:
                //TODO
                nextMenu = Menu.Start;
                Debug.LogWarning("Warning: Load Menu Unimplemented");
                break;
            case Menu.CharacterCreate:
                nextMenu = characterMenuManager.UpdateMenu();
                break;
        }

        if(nextMenu != currentMenu) //Switch Menus
        {
            switch(currentMenu)
            {
                case Menu.Start:
                    mainMenuPanel.SetActive(false);
                    break;
                case Menu.Load:
                    loadMenuPanel.SetActive(false); //TODO reset menu??
                    break;
                case Menu.CharacterCreate:
                    characterCreatePanel.SetActive(false);
                    break;
            }

            switch(nextMenu)
            {
                case Menu.Start:
                    mainMenuPanel.SetActive(true);
                    break;
                case Menu.Load:
                    loadMenuPanel.SetActive(true);
                    break;
                case Menu.CharacterCreate:
                    characterCreatePanel.SetActive(true);
                    characterCreatePanel.GetComponent<CharacterCreatorManager>().CreateNewCharacter();  //Create new character.
                    break;
                case Menu.Game:
                    SceneManager.LoadScene("GameScene");
                    gameObject.SetActive(false);
                    break;
            }

            currentMenu = nextMenu;
        }
    }
}
