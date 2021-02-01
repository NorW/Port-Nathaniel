using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapMenuManager : MonoBehaviour
{
    public static MapMenuManager manager = null;

    public GameObject background, characterStatPanel, plotInfoPanel, mapInterfacePanel, infoTextPanel;

    public PlotItemHandler[] plotItems = new PlotItemHandler[10];

    public Texture2D itemIco;

    // Start is called before the first frame update
    void Start()
    {
        if(manager == null)
        {
            manager = this;
        }
    }

    public void AddItem(string name)
    {
        if(name == null)
        {
            return;
        }
        if(GameResourceManager.ResourceManager.GetItemData(name) == null)
        {
            return;
        }
        var data = GameResourceManager.ResourceManager.GetItemData(name);
        foreach (var obj in plotItems)
        {
            if(!obj.gameObject.activeSelf)
            {
                obj.name = name;
                obj.description = data.description;
                obj.gameObject.GetComponent<Image>().sprite = Sprite.Create(itemIco, new Rect(0, 0, itemIco.width, itemIco.height), new Vector2(0.5f, 0.5f));
                obj.gameObject.SetActive(true);
                break;
            }
        }
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        infoTextPanel.GetComponent<InfoTextMover>().HideBox();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        List<RaycastResult> res = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerData, res);
        bool found = false;
        if (res.Count > 0)
        {
            foreach(var hit in res)
            {
                if(hit.gameObject.layer == 8)
                {
                    var item = hit.gameObject.GetComponent<PlotItemHandler>();
                    if(item != null && item.plotItemName != null && item.plotItemName != "")
                    {
                        infoTextPanel.GetComponent<InfoTextMover>().ShowBox(item.plotItemName, item.description);
                        found = true;
                        break;
                    }
                }
                else if(hit.gameObject.layer == 9)
                {
                    
                }
            }
        }

        if(!found)
        {
            infoTextPanel.GetComponent<InfoTextMover>().HideBox();
        }
        /*
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            var item = hit.collider.gameObject.GetComponent<PlotItemHandler>();
            if (item != null)
            {
                infoTextPanel.GetComponent<TextMeshProUGUI>().text = item.description;
            }
        }*/
    }
}
