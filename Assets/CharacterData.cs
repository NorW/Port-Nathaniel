using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    [SerializeField] static int startingStatValue;

    static CharacterData characterData = null;

    public static CharacterData Data { get { return characterData; } }

    Character characterStats;
    ItemDatabase items;

    void Awake()
    {
        if(characterData == null)
        {
            DontDestroyOnLoad(gameObject);
            characterData = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Character GetCharacterStats()
    {
        return characterStats;
    }

    public void CreateNewCharacter()
    {
        characterStats = new Character();
        characterStats.Muscle = new CharacterStat(startingStatValue);
        characterStats.Charm = new CharacterStat(startingStatValue);
        characterStats.Wits = new CharacterStat(startingStatValue);
        characterStats.Assets = new CharacterStat(startingStatValue);
        characterStats.Craft = new CharacterStat(startingStatValue);

        items = new ItemDatabase();
    }

    public void AddItem(string item)
    {
        if (item != null && !items.itemChecker(item))   //Add item if not null and not already added.
        {
            items.itemAdd(item); 

            var itemData = GameResourceManager.ResourceManager.GetItemData(item);   //If item has more data, check if item is a stat modifier and adjust accordingly.
            if (itemData != null)
            {
                if(itemData.type == ItemData.ItemType.Resource)
                {
                    switch(itemData.skillModified)
                    {
                        case ItemData.ModifierType.Muscle:
                            characterStats.Muscle.AddModifier(itemData.modifier);
                            break;
                        case ItemData.ModifierType.Charm:
                            characterStats.Charm.AddModifier(itemData.modifier);
                            break;
                        case ItemData.ModifierType.Wits:
                            characterStats.Wits.AddModifier(itemData.modifier);
                            break;
                        case ItemData.ModifierType.Assets:
                            characterStats.Assets.AddModifier(itemData.modifier);
                            break;
                        case ItemData.ModifierType.Craft:
                            characterStats.Craft.AddModifier(itemData.modifier);
                            break;
                        //TODO health stats
                    }
                }
            }
        }
    }

    public void RemoveItem(string item)
    {
        //If items has item, remove it and remove related modifier if applicable.
        if(items.itemChecker(item))
        {
            items.itemRemove(item);
            var itemData = GameResourceManager.ResourceManager.GetItemData(item);
            if(itemData != null && itemData.modifier != null)
            {
                switch (itemData.skillModified)
                {
                    case ItemData.ModifierType.Muscle:
                        characterStats.Muscle.RemoveModifier(itemData.modifier);
                        break;
                    case ItemData.ModifierType.Charm:
                        characterStats.Charm.RemoveModifier(itemData.modifier);
                        break;
                    case ItemData.ModifierType.Wits:
                        characterStats.Wits.RemoveModifier(itemData.modifier);
                        break;
                    case ItemData.ModifierType.Assets:
                        characterStats.Assets.RemoveModifier(itemData.modifier);
                        break;
                    case ItemData.ModifierType.Craft:
                        characterStats.Craft.RemoveModifier(itemData.modifier);
                        break;
                        //TODO health stats
                }
            }
        }
    }

    public bool ContainsItem(string item)
    {
        return items.itemChecker(item);
    }

    public bool ContainsAllItems(string[] itemsToCheck)
    {
        foreach(var item in itemsToCheck)
        {
            if(!ContainsItem(item))
            {
                return false;
            }
        }

        return true;
    }

    //TODO serialize character, probably just gonna be a list of items
    public string Serialize()
    {
        return null;
    }
}
