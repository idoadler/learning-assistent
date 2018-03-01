using SimpleJSON;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class JsonManager
{
    public static string ConversationGender = "f2f";
 
    private const string NODE_FIRST = "firstrun";
    private const string NODE_COMMUNICATION_ERROR = "communication-error";
//    private const string NODE_GENERAL_ERROR = "general-error";
    private const string NODE_EXIT = "exit";
    private const string NODE_CONVERSATIONS = "conversations";
    private const string NODE_INTENTIONS = "intentions";
    private const string NODE_NEXT = "next";
    private const string NODE_OPTIONS = "options";
    private const string NODE_ENTITIES = "entities";
    private const string NODE_VALUE = "value";
    private const string NODE_BACK = "back";
    private const string NODE_SAVE_DATA = "save";
    private const string NODE_SAVE_VAR_NAME = "var";
    private const char MESSAGE_SPLIT = '\n';
    private const string SPEACH_NODE_INITIAL = "text-";
    private const string INTENT_EXIT = "exit";
    private const string INTENT_YESNO = "yes_no";
    private const string INTENT_YES = "yes";
    private const string PREFS_LAST_STATE = "LASTSTATE";
    private const string PREFS_USER_SEX = "SEX";
    private const string JSON_CONFIDENCE = "confidence";
    private const float REQUIRED_CONFIDENCE = 0.8f;

    private static JSONNode brain;
    private static JSONNode conversation;
    private static JSONNode lastState;
    private static JSONNode currentState;

    public static void InitConversationJson(string json)
    {
        brain = JSON.Parse(json);
        conversation = brain[NODE_CONVERSATIONS];
        string state = PlayerPrefs.GetString(PREFS_LAST_STATE);

        if (ChatManager.IS_TESTING)
        {
            ResetConversation(); // TODO: Remove, this is for testing
        }
        else
        {
            if (string.IsNullOrEmpty(state) || conversation[state] == null)
            {
                ResetConversation();
            }
            else
            {
                currentState = conversation[state];
            }
        }
    }

    public static void ResetConversation()
    {
        string state = brain[NODE_FIRST].Value;
        PlayerPrefs.SetString(PREFS_LAST_STATE, state);
        currentState = conversation[state];
        lastState = currentState;
    }

    public static Result SaySpecial(string state)
    {
        Result result = new Result();
        result.goBack = true;
        string specialState = brain[state].Value;
        result.displayTexts = FormatBotText(conversation[specialState][SPEACH_NODE_INITIAL + ConversationGender]);
        return result;
    }

    public static string CurrentText()
    {
        return currentState[SPEACH_NODE_INITIAL + ConversationGender];
    }

    public static Result RetrieveResponse(JSONNode entities, bool initialBack = false)
    {
        Result result = new Result();
        result.goBack = initialBack;

        // save results to vars
        JSONNode intentions = currentState[NODE_SAVE_DATA][NODE_INTENTIONS];
        KeyValuePair<string, string> intent = MatchBestIntent(entities, intentions);
        if (!string.IsNullOrEmpty(intent.Key))
        {
            PlayerPrefs.SetString(currentState[NODE_SAVE_DATA][NODE_SAVE_VAR_NAME], intent.Value);
            // TODO: hack, move to someplace that make sense
            if (intent.Key == "sex" && intent.Value == "boy")
            {
                ConversationGender = ConversationGender.Substring(0, ConversationGender.Length - 1) + 'm';
            }
        }

        // check next state
        string state;
        intentions = currentState[NODE_INTENTIONS];
        intent = MatchBestIntent(entities, intentions);
        if (!string.IsNullOrEmpty(intent.Key) && intentions[intent.Key][intent.Value])
        {
            state = intentions[intent.Key][intent.Value].Value;
        }
        else
        {
            state = currentState[NODE_NEXT].Value;
        }
        PlayerPrefs.SetString(PREFS_LAST_STATE, state);
        lastState = currentState;
        currentState = conversation[state];
        if (currentState[NODE_BACK])
        {
            result.goBack = true;
        }

        result.displayTexts = /* "זוהתה כוונה\n" + intention + "," + key + "\n" + */ FormatBotText(CurrentText());
        //if (currentState[NODE_OPTIONS] != null)
        //{
        //    result.options = new List<string>(currentState[NODE_OPTIONS]);
        //}

        return result;
    }

    private static KeyValuePair<string, string> MatchBestIntent(JSONNode entities, JSONNode intentions)
    {
        KeyValuePair<string, string> result = new KeyValuePair<string, string>();
        int bestConfidence = -1;
        if (intentions != null)
        {
            foreach (string intent in entities.Keys)
            {
                if ((intentions[intent] != null) && entities[intent][0][JSON_CONFIDENCE] > bestConfidence)
                {
                    bestConfidence = entities[intent][0][JSON_CONFIDENCE];
                    result = new KeyValuePair<string, string>(intent, entities[intent][0][NODE_VALUE]);
                }
            }
        }

        return result;
    }

    internal static Result AskForExit()
    {
        return (SaySpecial(NODE_EXIT));
    }

    internal static bool VerifyJsonExit(string json)
    {
        JSONNode entities = JSON.Parse(json)[NODE_ENTITIES];

        return (JSON.Parse(json)[NODE_ENTITIES][INTENT_YESNO] == INTENT_YES);
/*
        string intention = "";
        string key = "";

        // normal result
        foreach (string intent in entities.Keys)
        {
            intention = intent;
        }

        key = entities[intention][0][NODE_VALUE];
        return (key == INTENT_YES);*/
    }

    //public static Result RetrieveOption(string option)
    //{
    //    Result result = new Result();
    //    try
    //    {
    //        string state;
    //        if (currentState[NODE_OPTIONS] != null && currentState[NODE_OPTIONS][option] != null)
    //        {
    //            state = currentState[NODE_OPTIONS][option].Value;
    //        }
    //        else
    //        {
    //            state = currentState[NODE_NEXT].Value;
    //        }
    //        PlayerPrefs.SetString(PREFS_LAST_STATE, state);
    //        lastState = currentState;
    //        currentState = conversation[state];

    //        result.displayTexts = /* "זוהתה כוונה\n" + intention + "," + key + "\n" + */ CurrentText().Split(MESSAGE_SPLIT);
    //        if (currentState[NODE_OPTIONS] != null)
    //        {
    //            result.options = new List<string>(currentState[NODE_OPTIONS]);
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        result.displayTexts = new string[] { "זוהתה כוונה\n" + option + "\n" + "Error parsing:" + e.Message };
    //    }
    //    return result;
    //}

    public static Result RetriveLast()
    {
        Result result = new Result();
        currentState = lastState;
        result.displayTexts = FormatBotText(CurrentText());
        result.goBack = currentState[NODE_BACK];
        return result;
    }

    public static Result JsonToBotText(string json)
    {
        JSONNode entities = JSON.Parse(json)[NODE_ENTITIES];

        if (entities == null)
        {
            // wrong communication
            return(SaySpecial(NODE_COMMUNICATION_ERROR));
        }
        else
        {
            bool exit = (entities[INTENT_EXIT] != null && entities[INTENT_EXIT][0][JSON_CONFIDENCE] > REQUIRED_CONFIDENCE);
            return (RetrieveResponse(entities,exit));
        }
    }

    private static string[] FormatBotText(string botText)
    {
        string result = botText;
        foreach (Match match in Regex.Matches(botText, "#([^#]*)#"))
        {
            string val = match.ToString();
            result = botText.Replace(val, PlayerPrefs.GetString(val.Substring(1, val.Length - 2)));
        }

        return result.Split(MESSAGE_SPLIT);
    }

    public struct Result
    {
        public string[] displayTexts;
        //public List<string> options;
        public bool goBack;

        public Result(string text, bool back = false)
        {
            displayTexts = FormatBotText(text);
            goBack = back;
            //options = null;
        }
    }
}
