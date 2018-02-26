using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class JsonManager
{
    public static string ConversationGender = "f2f";

    private const string NODE_FIRST = "firstrun";
    private const string NODE_COMMUNICATION_ERROR = "communication-error";
    private const string NODE_GENERAL_ERROR = "general-error";
    private const string NODE_EXIT = "exit";
    private const string NODE_CONVERSATIONS = "conversations";
    private const string NODE_INTENTIONS = "intentions";
    private const string NODE_NEXT = "next";
    private const string NODE_OPTIONS = "options";
    private const string NODE_ENTITIES = "entities";
    private const string NODE_VALUE = "value";
    private const string NODE_BACK = "back";
    private const char MESSAGE_SPLIT = '\n';
    private const string SPEACH_NODE_INITIAL = "text-";
    private const string INTENT_EXIT = "exit";
    private const string INTENT_YES = "yes";
    private const string PREFS_LAST_STATE = "LASTSTATE";
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
        result.displayTexts = ((string)conversation[specialState][SPEACH_NODE_INITIAL + ConversationGender]).Split(MESSAGE_SPLIT);
        return result;
    }

    public static string CurrentText()
    {
        return currentState[SPEACH_NODE_INITIAL + ConversationGender];
    }

    public static Result RetrieveResponse(string intention, string key, bool initialBack = false)
    {
        Result result = new Result();
        result.goBack = initialBack;
        try
        {
            string state;
            if (currentState[NODE_INTENTIONS] != null && currentState[NODE_INTENTIONS][intention][key] != null)
            {
                state = currentState[NODE_INTENTIONS][intention][key].Value;
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

            result.displayTexts = /* "זוהתה כוונה\n" + intention + "," + key + "\n" + */ CurrentText().Split(MESSAGE_SPLIT);
            if (currentState[NODE_OPTIONS] != null)
            {
                result.options = new List<string>(currentState[NODE_OPTIONS]);
            }
        }
        catch (Exception e)
        {
            result.displayTexts = new string[] { "זוהתה כוונה\n" + intention + "," + key + "\n" + "Error parsing:" + e.Message };
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

        string intention = "";
        string key = "";

        // normal result
        foreach (string intent in entities.Keys)
        {
            intention = intent;
        }

        key = entities[intention][0][NODE_VALUE];
        return (key == INTENT_YES);
    }

    public static Result RetrieveOption(string option)
    {
        Result result = new Result();
        try
        {
            string state;
            if (currentState[NODE_OPTIONS] != null && currentState[NODE_OPTIONS][option] != null)
            {
                state = currentState[NODE_OPTIONS][option].Value;
            }
            else
            {
                state = currentState[NODE_NEXT].Value;
            }
            PlayerPrefs.SetString(PREFS_LAST_STATE, state);
            lastState = currentState;
            currentState = conversation[state];

            result.displayTexts = /* "זוהתה כוונה\n" + intention + "," + key + "\n" + */ CurrentText().Split(MESSAGE_SPLIT);
            if (currentState[NODE_OPTIONS] != null)
            {
                result.options = new List<string>(currentState[NODE_OPTIONS]);
            }
        }
        catch (Exception e)
        {
            result.displayTexts = new string[] { "זוהתה כוונה\n" + option + "\n" + "Error parsing:" + e.Message };
        }
        return result;
    }

    public static Result RetriveLast()
    {
        Result result = new Result();
        currentState = lastState;
        result.displayTexts = CurrentText().Split(MESSAGE_SPLIT);
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
        else if (entities.Count == 0)
        {
            // normal no result
            // return(SaySpecial("intent-unknown"));
            return (RetrieveResponse("", ""));
        }
        else if (entities.Count > 1)
        {
            // API change
            return(SaySpecial(NODE_GENERAL_ERROR));
        }
        else
        {
            string intention = "";
            string key = "";

            // normal result
            foreach (string intent in entities.Keys)
            {
                intention = intent;
            }

            key = entities[intention][0][NODE_VALUE];
            bool exit = false;
            if (intention == INTENT_EXIT && entities[intention][0][JSON_CONFIDENCE] > REQUIRED_CONFIDENCE)
            {
                exit = true;
            }
            return (RetrieveResponse(intention,key,exit));
        }
    }

    public struct Result
    {
        public string[] displayTexts;
        public List<string> options;
        public bool goBack;

        public Result(string text, bool back = false)
        {
            displayTexts = text.Split(MESSAGE_SPLIT);
            goBack = back;
            options = null;
        }
    }
}
