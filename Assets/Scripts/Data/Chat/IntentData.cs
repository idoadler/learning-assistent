using System;

[Serializable]
public class IntentData
{
	public string nodeName;
    public Texts texts;
    public Intent[] intents;
    public SaveVar[] dataToSave;
    public InjectToNext[] dataToTransferNext;
    public string defaultNextNode;

    public void ConvertTexts()
    {
        texts.f2f = texts.femaleToFemale;
        texts.femaleToFemale = ArabicSupport.ArabicFixer.Fix(texts.f2f);
    }

    public void RestoreTexts()
    {
        texts.femaleToFemale = texts.f2f;
    }

    public string GetText(bool isBotMale, bool isUserMale)
    {
        if (isBotMale)
        {
            return (isUserMale ? texts.maleToMale : texts.maleToFemale);
        }
        else
        {
            return (isUserMale ? texts.femaleToMale : texts.femaleToFemale);
        }
    }

    [Serializable]
    public struct SpecialIntents
    {
        public string exit;
        public string firstrun;
        public string general_error;
        public string communication_error;
        public string intent_unknown;
    }

    [Serializable]
    public struct Texts
    {
        public string femaleToFemale;
        public string femaleToMale;
        public string maleToFemale;
        public string maleToMale;

        internal string f2f;
        private string f2m;
        private string m2f;
        private string m2m;
    }

    [Serializable]
    public struct SaveVar
    {
        public string var;
        public string expectedIntention;
    }

    [Serializable]
    public struct InjectToNext
    {
        public string backAddress;
        public string[] options;
        public Intent[] addedIntents;
    }

    [Serializable]
    public struct Intent
    {
        public string name;
        public IntentPair[] data;
    }

    [Serializable]
    public struct IntentPair
    {
        public string value;
        public string nextNode;
    }
}