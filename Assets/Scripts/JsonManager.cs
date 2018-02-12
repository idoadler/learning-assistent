using SimpleJSON;
using UnityEngine;

public static class JsonManager
{
    private const string LAST_STATE = "LASTSTATE";
    private const string FIRST_NODE = "firstrun";
    private static JSONNode brain;
    private static JSONNode conversation;
    private static JSONNode currentState;

    public static string InitConversationJson(string json)
    {
        brain = JSON.Parse(json);
        conversation = brain["conversations"];
        string state = PlayerPrefs.GetString(LAST_STATE);
        if (string.IsNullOrEmpty(state))
            return (ResetConversation());
        else
        {
            currentState = conversation[state];
            return currentState["text"][0];
        }
    }

    public static string ResetConversation()
    {
        string state = brain["firstrun"].Value;
        PlayerPrefs.SetString(LAST_STATE, state);
        currentState = conversation[state];
        return currentState["text"][0];
    }

    public static string RetrieveResponse(string intention, string key)
    {
        JSONNode result = currentState["intentions"][intention][key];
        if (result != null)
        {
            currentState = conversation[result["next"].Value];
        } else
        {
            currentState = conversation[currentState["retry"].Value];
        }
        return "זוהתה כוונה\n" + intention + "," + key + "\n" + currentState["text"][0];
    }

    public static string JsonToBotText(string json)
    {
        JSONNode entities = JSON.Parse(json)["entities"];

        if (entities == null)
        {
            // wrong communication
            return("שגיאה: בעית תקשורת");
        }
        else if (entities.Count == 0)
        {
            // normal no result
            return("אין לי מושג למה התכוונת");
        }
        else if (entities.Count > 1)
        {
            // API change
            return("שגיאה: קלט לא מזוהה");
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
