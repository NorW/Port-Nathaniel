using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class MapMenuManager : MonoBehaviour
{

    public GameObject background, characterStatPanel, plotInfoPanel, mapInterfacePanel, infoTextPanel;
    // Start is called before the first frame update
    void Start()
    {
        
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
