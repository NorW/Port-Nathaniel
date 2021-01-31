using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase
{
    public List<string> items = new List<string>();//create the list

    public void itemAdd(string itemName)
    {
        itemChecker(itemName);
        items.Add(itemName);
        Debug.Log("Item succesfully added to the list.");

    }

    public bool itemChecker(string itemName)
    {
        for(int i = 0; i < items.Count; i++ )
        {
            if (itemName == items[i])
            {
                return true;
            }

        }
        return false;

    }

    public void itemRemove(string itemName)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if(itemName != items[i])
            {
                Debug.Log("The given item is not on the list.");

            }
        }
        items.Remove(itemName);
        Debug.Log("Item has been succesfully removed from the list.");
    }





}
