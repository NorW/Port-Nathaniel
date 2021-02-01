using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


//Manages dialogue data and display
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager manager;

    public string currentSpeaker;
    public DialogueNode currentNode;

    public GameObject dialogueBox, speakerTag, speakerImage, dialoguePanel;
    private int currentChoice;

    private int currentDialoguePosition = 0;
    private bool dialogueEndReached = false;

    //Activates dialogue UI and begins dialogue conversation.
    public void OpenDialogue(string name)
    {
        currentNode = null;
        if (DialogueModel.DialogueData.dialogue.ContainsKey(name))
        {
            currentNode = DialogueModel.DialogueData.dialogue[name];
            if(!ProgressDialogue())
            {
                CloseDialogue();
            }
        }
        else
        {
            Debug.LogWarning("Warning unable to find dialogue.");
            speakerTag.GetComponent<TextMeshProUGUI>().text = "??????";
            dialogueBox.GetComponent<TextMeshProUGUI>().text = "";
        }

        dialoguePanel.SetActive(true);
    }

    //TODO check if dialogue is already closing or opening and block close and open calls while animating.
    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    //TODO
    bool CheckCondition(Condition condition)
    {
        if(condition.objectAndPlotValues.ContainsKey("itemreq") && condition.objectAndPlotValues["itemreq"].Count > 0)
        {
            foreach(var c in condition.objectAndPlotValues["itemreq"])
            {
                if (!CharacterData.Data.ContainsItem(c))
                {
                    return false;
                }
            }
        }

        if(condition.objectAndPlotValues.ContainsKey("contriband") && condition.objectAndPlotValues["contriband"].Count > 0)
        {
            foreach(var c in condition.objectAndPlotValues["contriband"])
            {
                if(CharacterData.Data.ContainsItem(c))
                {
                    return false;
                }
            }
        }

        if (condition.objectAndPlotValues.ContainsKey("plotreq") && condition.objectAndPlotValues["plotreq"].Count > 0)
        {
            foreach (var c in condition.objectAndPlotValues["plotreq"])
            {
                if (!CharacterData.Data.ContainsItem(c))
                {
                    return false;
                }
            }
        }

        if (condition.objectAndPlotValues.ContainsKey("excludedplot") && condition.objectAndPlotValues["excludedplot"].Count > 0)
        {
            foreach (var c in condition.objectAndPlotValues["excludedplot"])
            {
                if (CharacterData.Data.ContainsItem(c))
                {
                    return false;
                }
            }
        }

        return true;
    }

    bool RollSkillChecks(Condition condition)
    {
        int mod = Random.Range(1, 12);
        if(condition.numericalParams.ContainsKey("musclecheck"))
        {
            mod += (int) CharacterData.Data.GetCharacterStats().Muscle.Value;
            return mod >= 7;
        }
        else if(condition.numericalParams.ContainsKey("witscheck"))
        {
            mod += (int)CharacterData.Data.GetCharacterStats().Wits.Value;
            return mod >= 7;
        }
        else if (condition.numericalParams.ContainsKey("charmcheck"))
        {
            mod += (int)CharacterData.Data.GetCharacterStats().Charm.Value;
            return mod >= 7;
        }
        else if (condition.numericalParams.ContainsKey("assetscheck"))
        {
            mod += (int)CharacterData.Data.GetCharacterStats().Assets.Value;
            return mod >= 7;
        }
        else if (condition.numericalParams.ContainsKey("craftcheck"))
        {
            mod += (int)CharacterData.Data.GetCharacterStats().Craft.Value;
            return mod >= 7;
        }

        return true;
    }

    bool ContainsSkillChecks(Condition condition)
    {
        return condition.numericalParams.ContainsKey("musclecheck") || condition.numericalParams.ContainsKey("charmcheck") ||
            condition.numericalParams.ContainsKey("witscheck") || condition.numericalParams.ContainsKey("assetscheck") ||
            condition.numericalParams.ContainsKey("craftcheck");
    }

    DialogueNode SelectNextNode()
    {
        DialogueNode nextNode = null;
        if(currentNode == null)
        {
            return null;
        }

        if(currentNode.Type == DialogueNode.DialogueType.Choice)
        {
            var selectedNode = currentNode.parent.GetChild((uint)currentChoice);
            
            //If choice can be chosen based on condition check, attempt skillcheck and return next node. Otherwise do nothing.
            if (CheckCondition(selectedNode.condition))
            {
                if (ContainsSkillChecks(selectedNode.condition))
                {
                    bool pass = RollSkillChecks(selectedNode.condition);
                    var type = pass ? DialogueNode.DialogueType.Success : DialogueNode.DialogueType.Failure;

                    if (!pass)
                    {
                        //TODO spirit prompt
                    }

                    foreach (var node in selectedNode.children)
                    {
                        if (node.Type == type && CheckCondition(node.condition))
                        {
                            nextNode = node;
                            break;
                        }
                    }
                }
                else
                {
                    foreach(DialogueNode node in selectedNode.children)
                    {
                        if(CheckCondition(node.condition))
                        {
                            nextNode = node;
                            break;
                        }
                    }
                }
            }
            else
            {
                return currentNode;
            }
        }
        else
        {
            if(currentNode.children.Count > 0)
            {
                switch(currentNode.GetChild(0).Type)
                {
                    case DialogueNode.DialogueType.Dialogue:
                        foreach(DialogueNode node in currentNode.children)
                        {
                            if(CheckCondition(node.condition))
                            {
                                nextNode = node;
                                break;
                            }
                        }
                        break;
                    case DialogueNode.DialogueType.Failure:
                        Debug.LogWarning("Warning: Malformed dialogue tree, failure node under non-choice");
                        nextNode = currentNode.GetChild(0);
                        break;
                    case DialogueNode.DialogueType.Success:
                        Debug.LogWarning("Warning: Malformed dialogue tree, success node under non-choice");
                        nextNode = currentNode.GetChild(0);
                        break;
                    case DialogueNode.DialogueType.Choice:
                        nextNode = currentNode.GetChild(0);
                        currentChoice = 0;
                        break;
                }
            }
            else if(currentNode.condition.numericalParams.ContainsKey("loop"))
            {
                DialogueNode priorChoice = currentNode;
                for(int i = 0; i < currentNode.condition.numericalParams["loop"]; i++)
                {
                    do
                    {
                        priorChoice = priorChoice.parent;
                    } while (priorChoice != null && priorChoice.Type != DialogueNode.DialogueType.Choice);
                }
                nextNode = priorChoice;
            }
        }

        HandleDialogueItemChanges(nextNode);

        return nextNode;
    }

    void HandleDialogueItemChanges(DialogueNode node)
    {
        if(node != null)
        {
            if(node.condition.objectAndPlotValues.ContainsKey("itemgiven"))
            {
                foreach(var item in node.condition.objectAndPlotValues["itemgiven"])
                {
                    CharacterData.Data.AddItem(item);
                    if(GameResourceManager.ResourceManager.GetItemData(item) != null)
                    {
                        MapMenuManager.manager.AddItem(item);
                    }
                }
            }

            if (node.condition.objectAndPlotValues.ContainsKey("itemtaken"))
            {
                foreach (var item in node.condition.objectAndPlotValues["itemtaken"])
                {
                    CharacterData.Data.RemoveItem(item);
                }
            }

            if (node.condition.objectAndPlotValues.ContainsKey("plotgiven"))
            {
                foreach (var item in node.condition.objectAndPlotValues["plotgiven"])
                {
                    CharacterData.Data.AddItem(item);
                    if (GameResourceManager.ResourceManager.GetItemData(item) != null)
                    {
                        MapMenuManager.manager.AddItem(item);
                    }
                }
            }

            if (node.condition.objectAndPlotValues.ContainsKey("plottaken"))
            {
                foreach (var item in node.condition.objectAndPlotValues["plottaken"])
                {
                    CharacterData.Data.RemoveItem(item);
                }
            }
        }
    }

    void DisplayChoices()
    {
        var display = dialogueBox.GetComponent<TextMeshProUGUI>();
        display.text = "";
        for(int i = 0; i < currentNode.parent.children.Count; i++)
        {
            string prefix = "", suffix = "\n";
            if (i == currentChoice)
            {
                prefix += "<b>>";
                suffix = "</b>" + suffix;
            }

            if(!CheckCondition(currentNode.parent.GetChild((uint)i).condition))
            {
                prefix += "<color=#777777>";
                suffix = "</color>" + suffix;
            }
           
            display.text += prefix + currentNode.parent.GetChild((uint)i).Dialogue + suffix;
        }
    }

    //TODO access data
    void SetPortrait(string owner, string mood)
    {
        var texture = GameResourceManager.ResourceManager.GetNPCPortrait(owner, mood);
        if (texture != null)
        {
            speakerImage.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }

    //Progresses the dialogue. Returns true if dialogue progresses successfully, false if progressing past end of dialogue.
    bool ProgressDialogue()
    {
        var nextNode = SelectNextNode();    //Get next node
        if (nextNode == null)
        {
            return false;
        }
        else if(nextNode != currentNode)    //If node changes, adjust text
        {
            currentNode = nextNode;
            speakerTag.GetComponent<TextMeshProUGUI>().text = currentNode.DisplayName;
            
            if (currentNode.Type == DialogueNode.DialogueType.Choice)
            {
                dialogueEndReached = true;  //Don't animate text scrawl
                DisplayChoices();
            }
            else
            {
                var display = dialogueBox.GetComponent<TextMeshProUGUI>();
                display.richText = true;
                display.text = "";

                currentDialoguePosition = 0;
                dialogueEndReached = false;
            }
            string mood = null;
            if(currentNode.condition.objectAndPlotValues.ContainsKey("mood"))
            {
                mood = currentNode.condition.objectAndPlotValues["mood"][0];
            }
            SetPortrait(currentNode.Owner, mood);
        }

        return true;
    }


    //Shifts current choice selected up or down if there is a choice required. Else does nothing.
    void ChangeChoice(bool up)
    {
        //If node and next node exists and the next node is a choice selection
        if(currentNode != null && currentNode.parent.children.Count > 0 && currentNode.Type == DialogueNode.DialogueType.Choice)
        {
            if(up)
            {
                currentChoice--;
                if(currentChoice < 0)
                {
                    currentChoice = currentNode.parent.children.Count - 1;
                }
            }
            else
            {
                currentChoice++;
                if(currentChoice >= currentNode.parent.children.Count)
                {
                    currentChoice = 0;
                }
            }
            DisplayChoices();
        }
    }

    private void Awake()
    {
        if(manager == null)
        {
            manager = this;
        }
    }

    //Draws text letter by letter. If forced, draws it all at once.
    private void ProgressText(bool force)
    {
        if(!dialogueEndReached)
        {
            if(currentNode.Dialogue == null || currentNode.Dialogue.Length == 0)
            {
                dialogueEndReached = true;
                return;
            }

            var display = dialogueBox.GetComponent<TextMeshProUGUI>();
            if(force)
            {
                display.text = currentNode.Dialogue;
                dialogueEndReached = true;
            }
            else
            {
                currentDialoguePosition++;
                if (currentDialoguePosition < currentNode.Dialogue.Length + 1)
                {
                    display.text = currentNode.Dialogue.Substring(0, currentDialoguePosition);
                }
                else
                {
                    dialogueEndReached = true;
                }
            }
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(currentNode != null && !dialogueEndReached)
            {
                ProgressText(true);
            }
            else if(!ProgressDialogue())
            {
                CloseDialogue();
            }
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            ChangeChoice(true);
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            ChangeChoice(false);
        }

        if (currentNode != null && !dialogueEndReached)
        {
            ProgressText(false);
        }
    }
}