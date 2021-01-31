using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CharacterCreatorManager : MonoBehaviour
{
    [SerializeField] GameObject infoTextPanel;

    [SerializeField] GameObject[] resources = new GameObject[5];
    [SerializeField] GameObject[] highlights = new GameObject[5];

    int selectedIndex1, selectedIndex2;

    [SerializeField] int muscleTracker, witsTracker, charmTracker, assetsTracker, craftTracker;
    void SetResourceData(GameObject obj, string name)
    {
        var texture = GameResourceManager.ResourceManager.GetItemImage(name);
        obj.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        obj.GetComponent<PlotItemHandler>().plotItemName = name;
        obj.GetComponent<PlotItemHandler>().description = GameResourceManager.ResourceManager.GetItemData(name).description;
    }
    void Start()
    {
        SetResourceData(resources[0], "Athletic");
        SetResourceData(resources[1], "Alert");
        SetResourceData(resources[2], "Loaded");
        SetResourceData(resources[3], "Good Taste");
        SetResourceData(resources[4], "Enthusiastic");
    }

    public void CreateNewCharacter()
    {
        CharacterData.Data.CreateNewCharacter();
        selectedIndex1 = 0;
        selectedIndex2 = 1;
        highlights[0].SetActive(true);
        highlights[1].SetActive(true);
        highlights[2].SetActive(false);
        highlights[3].SetActive(false);
        highlights[4].SetActive(false);
        CharacterData.Data.AddItem(resources[0].GetComponent<PlotItemHandler>().plotItemName);
        CharacterData.Data.AddItem(resources[1].GetComponent<PlotItemHandler>().plotItemName);
    }

    void SelectResource(GameObject obj)
    {
        var newResource = obj.GetComponent<PlotItemHandler>();
        if (newResource != null)
        {
            int index = 0;
            for(int i = 0; i < resources.Length; i++)
            {
                if(resources[i] == obj)
                {
                    index = i;
                    break;
                }
            }

            if (index != selectedIndex1 && index != selectedIndex2)
            {
                highlights[selectedIndex1].SetActive(false);
                CharacterData.Data.RemoveItem(resources[selectedIndex1].GetComponent<PlotItemHandler>().plotItemName);

                highlights[index].SetActive(true);
                CharacterData.Data.AddItem(resources[index].GetComponent<PlotItemHandler>().plotItemName);
                selectedIndex1 = selectedIndex2;
                selectedIndex2 = index;
            }
        }
    }

    bool HandleContinueButton(GameObject obj)
    {
        if(Input.GetMouseButtonUp(0))
        {
            return true;
        }
        return false;
    }

    public MainMenuManager.Menu UpdateMenu()
    {
        //Raycast to item UI elements (resources in this case) and display information box if one is found.
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        List<RaycastResult> res = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerData, res);
        GameObject resource = null;
        if (res.Count > 0)
        {
            foreach (var hit in res)
            {
                if (hit.gameObject.layer == 8)
                {
                    var item = hit.gameObject.GetComponent<PlotItemHandler>();
                    if (item != null && item.plotItemName != null && item.plotItemName != "")
                    {
                        infoTextPanel.GetComponent<InfoTextMover>().ShowBox(item.plotItemName, item.description);
                        resource = hit.gameObject;
                        break;
                    }
                }
                else if(hit.gameObject.layer == 9)
                {
                    if (HandleContinueButton(hit.gameObject))
                    {
                        infoTextPanel.GetComponent<InfoTextMover>().HideBox();
                        return MainMenuManager.Menu.Game;
                    }
                }
            }
        }

        if (resource == null)
        {
            infoTextPanel.GetComponent<InfoTextMover>().HideBox();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            //TODO select  on button press
            SelectResource(resource);
        }
        /////Debug TODO REMOVE
        var stats = CharacterData.Data.GetCharacterStats();
        muscleTracker = (int)stats.Muscle.Value;
        witsTracker = (int)stats.Wits.Value;
        charmTracker = (int)stats.Charm.Value;
        assetsTracker = (int)stats.Assets.Value;
        craftTracker = (int)stats.Craft.Value;
        return MainMenuManager.Menu.CharacterCreate;
    }
}
