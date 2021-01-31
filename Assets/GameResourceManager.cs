using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


//Class for storing dialogue parameters including conditions for dialogue to be selected and any checks required as well as items and plot points awarded or removed from the player.
public class Condition
{
    public static string[] objectAndPlotTags = new string[] {   "itemreq", "contriband",
                                                                "plotreq", "excludedplot",
                                                                "itemgiven", "itemtaken",
                                                                "plotgiven", "plottaken",
                                                                "mood", "place"};

    public static string[] numericalParamTags = new string[] {    "musclecheck", "charmcheck", "witscheck", "assetscheck", "craftcheck",
                                                                    "musclemod", "charmmod", "witsmod", "assetsmod", "craftmod",
                                                                    "healthcheck", "fortitudecheck", "spiritcheck",
                                                                    "healthmod", "fortitudemod", "spiritmod",
                                                                    "starttime", "endtime", "loop"};

    public Dictionary<string, List<string>> objectAndPlotValues;

    public Dictionary<string, int> numericalParams;

    public Condition()
    {
        objectAndPlotValues = new Dictionary<string, List<string>>();
        numericalParams = new Dictionary<string, int>();
    }
}

//Node in the dialogue tree. Stores speaker of the dialogue, name to display, conditions required to select, the dialogue itself, and branching dialogue paths.
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

    public string DisplayName { get; set; }

    public Condition condition;

    public DialogueNode(DialogueType t)
    {
        type_ = t;
        children = new List<DialogueNode>();
        parent = null;
        DisplayName = "";
    }

    public void AddNode(DialogueNode node)
    {
        children.Add(node);
    }

    public DialogueNode GetChild(uint index)
    {
        if (index < children.Count)
        {
            return children[(int)index];
        }
        return null;
    }
}

//For reading and storing dialogue data.
public class DialogueModel
{
    public static DialogueModel DialogueData { get; private set; }

    public static bool isInitialized { get; private set; }

    public Dictionary<string, DialogueNode> dialogue;   //Stores all dialogue data.

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
            if (curNode != null)
            {
                dialogue.Add(curNode.Owner, curNode);
            }
        }
    }

    private string getQuotedContent(in string str, int index)
    {
        int start, end;
        start = str.IndexOf('\"', index);
        if (start != -1)
        {
            end = str.IndexOf('\"', start + 1);
            while (end != -1 && str[end] == '\\')
            {
                end = str.IndexOf('\"', end + 1);
            }
            if (end != -1)
            {
                return str.Substring(start + 1, end - start - 1);
            }
        }
        return null;
    }

    private void ParseParams(ref Condition condition, in string parameters)
    {
        int index = 0;

        foreach (string tag in Condition.objectAndPlotTags)
        {
            index = parameters.IndexOf(tag + "=\"", System.StringComparison.OrdinalIgnoreCase);
            if (index != -1)
            {
                condition.objectAndPlotValues[tag] = new List<string>(getQuotedContent(parameters, index).Split(','));
            }
        }

        foreach (string tag in Condition.numericalParamTags)
        {
            index = parameters.IndexOf(tag + "=\"", System.StringComparison.OrdinalIgnoreCase);
            if (index != -1)
            {
                int val = 0;
                if (int.TryParse(getQuotedContent(parameters, index), out val))
                {
                    condition.numericalParams[tag] = val;
                }
            }
        }
    }

    private DialogueNode ParseTag(in string tag)
    {
        int index = tag.IndexOf(' ');
        if (index == -1)
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

        start = tag.IndexOf("name", System.StringComparison.OrdinalIgnoreCase);
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
                    node.DisplayName = tag.Substring(start + 2, end - start - 2);
                }
            }
        }
        else
        {
            node.DisplayName = owner;
        }

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

//Stores item data
public class ItemData
{
    public enum ItemType
    {
        Resource,
        Clue
    };

    public enum ModifierType
    {
        None,
        Muscle,
        Charm,
        Wits,
        Assets,
        Craft,
        Health,
        Fortitude,
        Spirit
    }

    public static string[] ValidModifiers = new string[] { "muscle", "charm", "wits", "assets", "craft", "health", "fortitude", "spirit" };

    public string name;
    public string description;
    public string image;
    public ItemType type;
    public ModifierType skillModified;
    public StatModifier modifier;

    public static ModifierType TypeFromString(string s)
    {
        ModifierType type = ModifierType.None;

        if (s == "muscle")
        {
            type = ModifierType.Muscle;
        }
        else if (s == "charm")
        {
            type = ModifierType.Charm;
        }
        else if(s == "wits")
        {
            type = ModifierType.Wits;
        }
        else if (s == "assets")
        {
            type = ModifierType.Assets;
        }
        else if (s == "craft")
        {
            type = ModifierType.Craft;
        }
        else if (s == "health")
        {
            type = ModifierType.Health;
        }
        else if (s == "foritude")
        {
            type = ModifierType.Fortitude;
        }
        else if (s == "spirit")
        {
            type = ModifierType.Spirit;
        }

        return type;
    }

    public static ItemData.ItemType TryParseType(string s)
    {
        if (s.ToLower() == "r" || s.ToLower() == "resource")
        {
            return ItemType.Resource;
        }
        else
        {
            return ItemType.Clue;
        }
    }

    public static bool TryParseModifier(string s, out string modifiedStat, out int modifierAmount)
    {
        if(s != null)
        {
            var split = s.ToLower().Split(',');
            if(split.Length > 1)
            {
                foreach(var mod in ValidModifiers)
                {
                    if (split[0] == mod)
                    {
                        modifiedStat = split[0];
                        modifierAmount = int.Parse(split[1]);   //TODO check if parse actually works
                        return true;
                    }
                }
            }
        }
        modifiedStat = null;
        modifierAmount = 0;
        return false;
    }
}

public class GameResourceManager : MonoBehaviour
{
    static GameResourceManager manager;

    public static GameResourceManager ResourceManager { get { return manager; } }

    [SerializeField] string itemDataFileLocation;
    [SerializeField] string portraitDataFileLocation;
    [SerializeField] string dialogueFileLocation;

    [SerializeField] Texture2D defaultPortrait;
    [SerializeField] Texture2D defaultItemImage;

    //Images
    Dictionary<string, Texture2D> textures;

    //Item data
    Dictionary<string, ItemData> itemData;

    //Stores NPC portrait file names by NPC and mood
    Dictionary<string, Dictionary<string, string>> portraitData;

    //NPC portraits by file name
    Dictionary<string, Texture2D> portraits;

    //Parses one line containing npc data
    void LoadNPCPortraits(string line)
    {
        if (line != null)
        {
            var segments = line.Split(';');
            portraitData.Add(segments[0], new Dictionary<string, string>());
            for (int i = 1; i < segments.Length; i++)
            {
                var moodImageSplit = segments[i].Split(',');
                if (moodImageSplit.Length > 1)
                {
                    portraitData[segments[0]].Add(moodImageSplit[0], moodImageSplit[1]);
                }
            }
        }
    }

    //Portrait data stored in the format: <npc name>;<mood>,<image location>;<mood>,<image location>... one npc per line
    void LoadPortraitData(string dataFile)
    {
        portraitData = new Dictionary<string, Dictionary<string, string>>();
        if (System.IO.File.Exists(dataFile))
        {
            var lines = System.IO.File.ReadAllLines(dataFile);
            foreach(var line in lines)
            {
                LoadNPCPortraits(line);
            }
            portraits = new Dictionary<string, Texture2D>();
            foreach(var npc in portraitData)
            {
                foreach(var mood in npc.Value)
                {
                    if (!portraits.ContainsKey(mood.Value))
                    {
                        var image = Resources.Load<Texture2D>(mood.Value);
                        if (image != null)
                        {
                            portraits.Add(mood.Value, image);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Error: Portrait Data File Not Found");
        }
    }

    void LoadTextures()
    {
        textures = new Dictionary<string, Texture2D>();

        foreach(var item in itemData)
        {
            //If item has an image and the image has not already been loaded, load the image.
            if(item.Value.image != null && !textures.ContainsKey(item.Value.image))
            {
                Texture2D image = Resources.Load<Texture2D>(item.Value.image);
                if(image != null)
                {
                    textures.Add(item.Value.image, image);
                }
            }
        }

        LoadPortraitData(portraitDataFileLocation);
    }

    //TODO test add skill modifiers based on type
    void ParseAndAddItem(string line)
    {
        var segments = line.Split(';');
        
        //A properly segmented line should have at least 4 parts
        if (segments.Length > 3)
        {
            ItemData data = new ItemData();
            int descriptionSegmentStart = 3;
            data.name = segments[0];
            data.type = ItemData.TryParseType(segments[1]);
            data.image = segments[2];

            if (data.type == ItemData.ItemType.Resource)
            {
                string modifiedStat;
                int modifiedAmount;
                if(ItemData.TryParseModifier(segments[3], out modifiedStat, out modifiedAmount))
                {
                    descriptionSegmentStart = 4;
                    data.skillModified = ItemData.TypeFromString(modifiedStat);
                    data.modifier = new StatModifier(modifiedAmount, StatModType.Flat);
                }
            }
            
            data.description = "";
            for(int i = descriptionSegmentStart; i < segments.Length; i++)
            {
                data.description += segments[i];
            }
            itemData.Add(data.name, data);
        }
    }

    //Read item data from file. Item entries are stored in the format <name>;<type>;<image file location>;<description>  each entry taking up one line
    void LoadItemData()
    {
        itemData = new Dictionary<string, ItemData>();

        if(System.IO.File.Exists(itemDataFileLocation))
        {
            var lines = System.IO.File.ReadAllLines(itemDataFileLocation);
            foreach(var line in lines)
            {
                ParseAndAddItem(line);
            }
        }
        else
        {
            Debug.LogError("Error: Item Data File Not Found");
        }
        
    }

    void LoadData()
    {
        DialogueModel.Initialize(dialogueFileLocation);
        if (!DialogueModel.isInitialized)
        {
            Debug.LogError("ERROR: Failed to initialize dialogue");
        }

        LoadItemData();
        LoadTextures();
    }

    public ItemData GetItemData(string name)
    {
        if(itemData.ContainsKey(name))
        {
            return itemData[name];
        }
        return null;
    }
    public Texture2D GetNPCPortrait(string name, string mood)
    {
        if(portraitData.ContainsKey(name))
        {
            if(mood != null &&
                portraitData[name].ContainsKey(mood) &&  
                portraits.ContainsKey(portraitData[name][mood]) && 
                portraits[portraitData[name][mood]] != null)
            {
                return portraits[portraitData[name][mood]];
            }
            else if(portraitData[name].ContainsKey("default") && portraits.ContainsKey(portraitData[name]["default"]) && portraits[portraitData[name]["default"]] != null)
            {
                return portraits[portraitData[name]["default"]];
            }
        }

        return defaultPortrait;
    }

    public Texture2D GetItemImage(string name)
    {
        if(name != null && itemData.ContainsKey(name) && itemData[name].image != null && textures.ContainsKey(itemData[name].image))
        {
            return textures[itemData[name].image];
        }
        return defaultItemImage;
    }
    void Awake()
    {
        if(manager == null)
        {
            manager = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
