using System;

[Serializable]
public class DialogueLine
{
    public string id;
    public string eventId;
    public int? day;
    public string type;      // "line"
    public string speaker;
    //public string portrait;
    public string emotion;
    public string text;
    public string nextId;
    public string choiceGroup;
}

[Serializable]
public class DialogueChoice
{
    public string groupId;
    public int choiceId;
    public string text;
    public string nextId;
}