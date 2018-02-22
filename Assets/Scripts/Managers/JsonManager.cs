using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class JsonManager
{
    public static string ConversationGender = "f2f";

    private const string LAST_STATE = "LASTSTATE";
    private const string FIRST_NODE = "firstrun";
    private static JSONNode brain;
    private static JSONNode conversation;
    private static JSONNode currentState;

    public static void InitConversationJson(string json)
    {
        brain = JSON.Parse(json);
        conversation = brain["conversations"];
        string state = PlayerPrefs.GetString(LAST_STATE);

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
        string state = brain["firstrun"].Value;
        PlayerPrefs.SetString(LAST_STATE, state);
        currentState = conversation[state];
    }

    public static Result SaySpecial(string state)
    {
        Result result = new Result();
        result.goBack = true;
        string specialState = brain[state].Value;
        result.displayText = conversation[specialState]["text-" + ConversationGender];
        return result;
    }

    public static string CurrentText()
    {
        return currentState["text-" + ConversationGender];
    }

    public static Result RetrieveResponse(string intention, string key)
    {
        Result result = new Result();
        result.goBack = false;
        if (intention == "exit")
        {
            result.goBack = true;
        }
        try
        {
            string state;
            if (currentState["intentions"] != null && currentState["intentions"][intention][key] != null)
            {
                state = currentState["intentions"][intention][key].Value;
            }
            else
            {
                state = currentState["next"].Value;
            }
            PlayerPrefs.SetString(LAST_STATE, state);
            currentState = conversation[state];
            if (currentState["back"])
            {
                result.goBack = true;
            }

            result.displayText = /* "זוהתה כוונה\n" + intention + "," + key + "\n" + */ CurrentText();
            if (currentState["options"] != null)
            {
                result.options = new List<string>(currentState["options"]);
            }
        }
        catch (Exception e)
        {
            result.displayText = "זוהתה כוונה\n" + intention + "," + key + "\n" + "Error parsing:" + e.Message;
        }
        return result;
    }

    public static Result RetrieveOption(string option)
    {
        Result result = new Result();
        try
        {
            string state;
            if (currentState["options"] != null && currentState["options"][option] != null)
            {
                state = currentState["options"][option].Value;
            }
            else
            {
                state = currentState["next"].Value;
            }
            PlayerPrefs.SetString(LAST_STATE, state);
            currentState = conversation[state];

            result.displayText = /* "זוהתה כוונה\n" + intention + "," + key + "\n" + */ CurrentText();
            if (currentState["options"] != null)
            {
                result.options = new List<string>(currentState["options"]);
            }
        }
        catch (Exception e)
        {
            result.displayText = "זוהתה כוונה\n" + option + "\n" + "Error parsing:" + e.Message;
        }
        return result;
    }

    public static Result JsonToBotText(string json)
    {
        JSONNode entities = JSON.Parse(json)["entities"];

        if (entities == null)
        {
            // wrong communication
            return(SaySpecial("communication-error"));
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
            return(SaySpecial("general-error"));
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

            key = entities[intention][0]["value"];
            return(RetrieveResponse(intention,key));
        }
    }

    public struct Result
    {
        public string displayText;
        public List<string> options;
        public bool goBack;

        public Result(string text, bool back = false)
        {
            displayText = text;
            goBack = back;
            options = null;
        }
    }
}
