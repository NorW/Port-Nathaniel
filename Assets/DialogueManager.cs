using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Condition
{
    public static string[] objectAndPlotTags = new string[] {   "itemreq", "contriband",
                                                                "plotreq", "excludedplot",
                                                                "itemgiven", "itemtaken",
                                                                "plotgiven", "plottaken",
                                                                "mood", "place"};

    public static string[] skillAndResourceTags = new string[] {    "musclecheck", "charmcheck", "witscheck", "assetscheck", "craftcheck",
                                                                    "musclemod", "charmmod", "witsmod", "assetsmod", "craftmod",
                                                                    "healthcheck", "fortitudecheck", "spiritcheck",
                                                                    "healthmod", "fortitudemod", "spiritmod"};

    public int startTime, endTime;

    public Dictionary<string, List<string>> objectAndPlotValues;

    public Dictionary<string, int> skillAndResourceValues;

    public Condition()
    {
        objectAndPlotValues = new Dictionary<string, List<string>>();
        skillAndResourceValues = new Dictionary<string, int>();
        startTime = -1;
        endTime = -1;
    }
}

public class DialogueNode
{
    public enum DialogueType
    {
        Dialogue,   //Normal dialogue
        Choice,     //Choice
        Failure,    //Failure dialogue
        Success,    //Success dialogue
        Root
    };

    private DialogueType type_;

    public string Owner { get; set; }//Name of the person or object delivering the dialogue

    public string Dialogue { get; set; }

    public DialogueNode parent;

    public DialogueType Type { get { return type_; } }

    public List<DialogueNode> children;

    public Condition condition;

    public DialogueNode(DialogueType t)
    {
        type_ = t;
        children = new List<DialogueNode>();
        parent = null;
    }

    public void AddNode(DialogueNode node)
    {
        children.Add(node);
    }

    public DialogueNode GetChild(uint index)
    {
        if(index < children.Count)
        {
            return children[(int)index];
        }
        return null;
    }
}


public class DialogueModel
{
    public static DialogueModel DialogueData { get; private set; }

    public static bool isInitialized { get; private set; }

    public Dictionary<string, DialogueNode> dialogue;

    public static bool Initialize(string dialogueFileLocation)
    {
        if (!isInitialized)
        {
            DialogueData = new DialogueModel();
            isInitialized = DialogueData.initialize(dialogueFileLocation);
        }
        return isInitialized;
    }

    private void ParseDialogueFile(string dialogueFileLocation)
    {
        string rawDialogue = System.IO.File.ReadAllText(dialogueFileLocation);
        Stack<DialogueNode> nodes = new Stack<DialogueNode>();
        int index = rawDialogue.IndexOf('<');
        DialogueNode curNode;

        while (index != -1)
        {
            curNode = ParseNode(ref index, ref rawDialogue, ref nodes);
            index = rawDialogue.IndexOf('<', index);
            if(curNode != null)
            {
                dialogue.Add(curNode.Owner, curNode);
            }
        }
    }

    private string getQuotedContent(in string str, int index)
    {
        int start, end;
        start = str.IndexOf('\"', index);
        if(start != -1)
        {
            end = str.IndexOf('\"', start + 1);
            while(end != -1 && str[end] == '\\')
            {
                end = str.IndexOf('\"', end + 1);
            }
            if(end != -1)
            {
                return str.Substring(start + 1, end - start - 1);
            }
        }
        return null;
    }

    private void ParseParams(ref Condition condition, in string parameters)
    {
        int index = 0;

        foreach(string tag in Condition.objectAndPlotTags)
        {
            index = parameters.IndexOf(tag + "=\"", System.StringComparison.OrdinalIgnoreCase);
            if (index != -1)
            {
                condition.objectAndPlotValues[tag] = new List<string>(getQuotedContent(parameters, index).Split(','));
            }
        }

        foreach (string tag in Condition.skillAndResourceTags)
        {
            index = parameters.IndexOf(tag + "=\"", System.StringComparison.OrdinalIgnoreCase);
            if (index != -1)
            {
                int val = 0;
                if (int.TryParse(getQuotedContent(parameters, index), out val))
                {
                    condition.skillAndResourceValues[tag] = val;
                }
            }
        }
    }

    private DialogueNode ParseTag(in string tag)
    {
        int index = tag.IndexOf(' ');
        if(index == -1)
        {
            index = tag.Length;
        }

        int start, end;
        string part = tag.Substring(0, index).ToLower();
        DialogueNode.DialogueType type;

        //Determine dialoge type
        if (part.Equals("d"))    //Dialogue
        {
            type = DialogueNode.DialogueType.Dialogue;
        }
        else if (part.Equals("c"))   //Choice
        {
            type = DialogueNode.DialogueType.Choice;
        }
        else if (part.Equals("f"))   //Failure
        {
            type = DialogueNode.DialogueType.Failure;
        }
        else if (part.Equals("s"))   //Success
        {
            type = DialogueNode.DialogueType.Success;
        }
        else if (part.Equals("r"))   //Root
        {
            type = DialogueNode.DialogueType.Root;
        }
        else //Undefined or closing
        {
            return null;
        }

        Condition condition = new Condition();
        string owner = null;

        start = tag.IndexOf("owner", System.StringComparison.OrdinalIgnoreCase);
        if(start != -1)
        {
            start = tag.IndexOf("=\"", start);
            if(start != -1)
            {
                end = tag.IndexOf("\"", start + 2);
                while(end != -1 && tag[end -1] == '\\')
                {
                    end = tag.IndexOf("\"", end + 1);
                }

                if(end != -1)
                {
                    owner = tag.Substring(start + 2, end - start - 2);
                    index = end + 1;
                }
            }
        }
        else
        {
            owner = "";
        }

        DialogueNode node = new DialogueNode(type);
        node.Owner = owner;

        ParseParams(ref condition, in tag);

        node.condition = condition;

        start = tag.IndexOf("dialogue", System.StringComparison.OrdinalIgnoreCase);
        if (start != -1)
        {
            start = tag.IndexOf("=\"", start);
            if (start != -1)
            {
                end = tag.IndexOf("\"", start + 2);
                while (end != -1 && tag[end - 1] == '\\')
                {
                    end = tag.IndexOf("\"", end + 1);
                }

                if (end != -1)
                {
                    node.Dialogue = tag.Substring(start + 2, end - start - 2);
                    index = end + 1;
                }
            }
        }

        return node;
    }

    private DialogueNode ParseNode(ref int index, ref string rawDialogue, ref Stack<DialogueNode> nodes)
    {
        int tagStart = rawDialogue.IndexOf('<', index);
        int tagEnd = rawDialogue.IndexOf('>', index);
        index = tagEnd;

        if (tagStart != -1 && tagEnd != -1)
        {
            index = tagEnd + 1;
            string tag = rawDialogue.Substring(tagStart + 1, tagEnd - tagStart - 1).Trim();
            if (tag.Length != 0)
            {
                DialogueNode node = ParseTag(in tag);
                if (node == null)
                {
                    nodes.Pop();    //close node
                    return null;
                }
                else
                {
                    if (nodes.Count > 0)
                    {
                        nodes.Peek().AddNode(node);
                        node.parent = nodes.Peek();
                    }
                    nodes.Push(node);
                    while (ParseNode(ref index, ref rawDialogue, ref nodes) != null) ;
                    return node;
                }
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }


    private DialogueModel()
    {
        
    }

    private bool initialize(string dialogueFileLocation)
    {
        dialogue = new Dictionary<string, DialogueNode>();

        if (System.IO.File.Exists(dialogueFileLocation))    //Load dialogue
        {
            ParseDialogueFile(dialogueFileLocation);
            return true;
        }
        else
        {
            Debug.LogError("ERROR: Dialogue File Not Found");
            return false;
        }
    }
}

public class DialogueManager : MonoBehaviour
{
    public string currentSpeaker;
    public DialogueNode currentNode;

    public GameObject dialogueBox, speakerTag, speakerImage, canvas;
    public string dialogueFileLocation;
    private int currentChoice;

    private int currentDialoguePosition = 0;
    private bool dialogueEndReached = false;

    void Start()
    {
        DialogueModel.Initialize(dialogueFileLocation);
        if(!DialogueModel.isInitialized)
        {
            Debug.LogError("ERROR: Failed to initialize dialogue");
        }
        gameObject.SetActive(false);
    }

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
            speakerTag.GetComponent<TextMeshProUGUI>().text = "??????";
            dialogueBox.GetComponent<TextMeshProUGUI>().text = "";
        }

        canvas.SetActive(true);
    }

    public void CloseDialogue()
    {
        canvas.SetActive(false);
    }

    //TODO
    bool CheckCondition(Condition condition)
    {
        
        return true;
    }

    bool RollSkillChecks(Condition condition)
    {
        int rollValue = Random.Range(1, 12);
        if (rollValue < 7)
        {
            return false;
        }
        else
        {
            return true;
        }
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
                bool pass = RollSkillChecks(selectedNode.condition);
                var type = pass ? DialogueNode.DialogueType.Success : DialogueNode.DialogueType.Failure;
                
                if(!pass)
                {
                    //TODO spirit prompt
                }

                foreach(var node in selectedNode.children)
                {
                    if(node.Type == type && CheckCondition(node.condition))
                    {
                        return node;
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
        }

        return nextNode;
    }

    void DisplayChoices()
    {
        var display = dialogueBox.GetComponent<TextMeshProUGUI>();
        display.text = "";
        for(int i = 0; i < currentNode.parent.children.Count; i++)
        {
            if (i == currentChoice)
            {
                display.text += "<b><color=\"yellow\">" + currentNode.parent.GetChild((uint)i).Dialogue + "</color></b>\n";
            }
            else
            {
                display.text += currentNode.parent.GetChild((uint)i).Dialogue + "\n";
            }
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
            speakerTag.GetComponent<TextMeshProUGUI>().text = currentNode.Owner;
            
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

    private void ProgressText(bool force)
    {
        if(!dialogueEndReached)
        {
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