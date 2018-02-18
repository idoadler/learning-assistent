using SimpleJSON;
using System;
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
        if (string.IsNullOrEmpty(state) || conversation[state] == null)
        {
            ResetConversation();
        }
        else
        {
            currentState = conversation[state];
        }
    }

    public static void ResetConversation()
    {
        string state = brain["firstrun"].Value;
        PlayerPrefs.SetString(LAST_STATE, state);
        currentState = conversation[state];
    }

    public static string SaySpecial(string state)
    {
        string specialState = brain[state].Value;
        return conversation[specialState]["text-" + ConversationGender];
    }

    public static string CurrentText()
    {
        return currentState["text-" + ConversationGender];
    }

    public static string RetrieveResponse(string intention, string key)
    {
        try
        {
            JSONNode result = currentState["intentions"][intention][key];
            string state;
            if (result != null)
            {
                state = result.Value;
            }
            else
            {
                state = currentState["next"].Value;
            }
            PlayerPrefs.SetString(LAST_STATE, state);
            currentState = conversation[state];
            return "זוהתה כוונה\n" + intention + "," + key + "\n" + CurrentText();
        }
        catch (Exception e)
        {
            return "זוהתה כוונה\n" + intention + "," + key + "\n" + "Error parsing:" + e.Message;
        }
    }

    public static string JsonToBotText(string json)
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
            return(SaySpecial("intent-unknown"));
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

}
