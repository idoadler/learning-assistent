using System;

[Serializable]
public class BrainData
{
    public IntentData.SpecialIntents specialIntents;
    public IntentData.Intent[] defaultIntents;
    public IntentData[] conversationNodes;

    public void PrepareForDisplay()
    {
        foreach(IntentData node in conversationNodes)
        {
            node.ConvertTexts();
        }
    }

    public void PrepareForSave()
    {
        foreach (IntentData node in conversationNodes)
        {
            node.RestoreTexts();
        }
    }
}

