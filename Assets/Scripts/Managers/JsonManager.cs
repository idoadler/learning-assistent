using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class JsonManager
{
    private static bool isUserFemale = true;
    private static bool isBotFemale = true;

    private const string NODE_FIRST = "firstrun";
    private const string NODE_COMMUNICATION_ERROR = "communication-error";
    private const string NODE_EXIT = "exit";
    private const string NODE_CONVERSATIONS = "conversations";
    private const string NODE_INTENTIONS = "intentions";
    private const string NODE_NEXT = "next";
    private const string NODE_OPTIONS = "options";
    private const string NODE_ENTITIES = "entities";
    private const string NODE_CTX = "ctx";
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
    private const string PREFS_USER_MALE = "USER_MALE";
    private const string PREFS_BOT_MALE = "BOT_MALE";
    private const string DEFAULT_GENDER = "f2f";
    private const string JSON_CONFIDENCE = "confidence";
    private const string INTENT_DATE = "datetime";
    private const string INTENT_DUP_DATE = "DupMissionerr";
    private const string INTENT_WRONG_DATE = "MissionDateerr";
    private const string INTENT_SAVE_TSK = "homework-scdual";
    private const string INTENT_TSK = "task";
    private const string INTENT_TIME = "TIME";
    private const string INTENT_TEST_TSK = "test-task-2";
    private const string INTENT_HW_TSK = "homework-task-2";
    private const float REQUIRED_CONFIDENCE = 0.8f;

    private static string TSK_NAME ;
    private static string TSK_TYPE;
    private static DateTime TSK_DATE ; 
    private static JSONNode ctx;
    private static JSONNode brain;
    private static JSONNode conversation;
    private static JSONNode lastState;
    private static JSONNode currentState;

    public static bool IsUserFemale
    {
        get
        {
            return isUserFemale;
        }

        set
        {
            isUserFemale = value;
            if (value)
            {
                PlayerPrefs.DeleteKey(PREFS_USER_MALE);
            }
            else
            {
                PlayerPrefs.SetInt(PREFS_USER_MALE, 0);
            }
        }
    }

    public static bool IsBotFemale
    {
        get
        {
            return isBotFemale;
        }

        set
        {
            isBotFemale = value;
            if (value)
            {
                PlayerPrefs.DeleteKey(PREFS_BOT_MALE);
            }
            else
            {
                PlayerPrefs.SetInt(PREFS_BOT_MALE, 0);
            }
        }
    }

    public static void InitConversationJson(string json)
    {
        if (ChatManager.IS_TESTING)
        {
            PlayerPrefs.DeleteAll();
        }

        brain = JSON.Parse(json);
        conversation = brain[NODE_CONVERSATIONS];
        string state = PlayerPrefs.GetString(PREFS_LAST_STATE);
        IsBotFemale = !PlayerPrefs.HasKey(PREFS_BOT_MALE);
        IsUserFemale = !PlayerPrefs.HasKey(PREFS_USER_MALE);
        if (string.IsNullOrEmpty(state))
        {
            ResetConversation();
        }
        else
        {
            currentState = conversation[state];
            lastState = currentState;
        }
    }

    public static void ResetConversation()
    {
        string state = brain[NODE_FIRST].Value;
        PlayerPrefs.SetString(PREFS_LAST_STATE, state);
        currentState = conversation[state];
        lastState = currentState;
        AnalyticsManager.UserSignup();
    }

    private static string TextGender()
    {
        return SPEACH_NODE_INITIAL + (isBotFemale ? "f" : "m") + "2" + (isUserFemale ? "f" : "m");
    }

    public static Result SaySpecial(string state)
    {
        Result result = new Result();
        result.goBack = true;
        string specialState = brain[state].Value;
        result.displayTexts = FormatBotText(conversation[specialState][TextGender()]);
        return result;
    }

    public static string[] CurrentTexts()
    {
        return FormatBotText(currentState[TextGender()]);
    }

    public static Result RetrieveResponse(JSONNode entities, bool initialBack = false)
    {
        Result result = new Result();
        result.goBack = initialBack;

        // save results to vars
        if (currentState[NODE_SAVE_DATA][NODE_CTX][0])
            ctx = currentState[NODE_SAVE_DATA][NODE_CTX];
        foreach (string i in ctx.Keys)
        {
            PlayerPrefs.SetString(i, ctx[i]);
        }

        JSONNode intentions = currentState[NODE_SAVE_DATA][NODE_INTENTIONS];
        KeyValuePair<string, string> intent = MatchBestIntent(entities, intentions);
        if (!string.IsNullOrEmpty(intent.Key))
        {
            PlayerPrefs.SetString(currentState[NODE_SAVE_DATA][NODE_SAVE_VAR_NAME], intent.Value);
            // TODO: hack, move to someplace that make sense
            if (intent.Key == "sex" && intent.Value == "boy")
            {
                IsUserFemale = false;
            }
        }

        // check next state
        string state;
        intentions = currentState[NODE_INTENTIONS];
        intent = MatchBestIntent(entities, intentions);
        if ((!string.IsNullOrEmpty(intent.Key) && (!string.IsNullOrEmpty(intentions[intent.Key][intent.Value]))) || (intent.Key == INTENT_DATE))
        {
            if (intent.Key == INTENT_DATE)
            {

                string tempValue = CheckMissionDate(DateTime.Parse(intent.Value));
                if (tempValue == null)
                    intentions[intent.Key][INTENT_DATE].Value = GetParam(intentions[intent.Key][INTENT_DATE].Value, ctx);
                else intentions[intent.Key][INTENT_DATE].Value = tempValue;
                state = intentions[intent.Key][INTENT_DATE].Value;
            }
            else
            {
                intentions[intent.Key][intent.Value].Value = GetParam(intentions[intent.Key][intent.Value].Value, ctx);
                state = intentions[intent.Key][intent.Value].Value;
            }
        }
        else
        {
            currentState[NODE_NEXT].Value = GetParam(currentState[NODE_NEXT].Value, ctx);
            state = currentState[NODE_NEXT].Value;
        }

        if ((state == INTENT_TEST_TSK) || (state == INTENT_HW_TSK))
        {
            TSK_NAME = PlayerPrefs.GetString(INTENT_TSK);
            if (state == INTENT_HW_TSK)
                TSK_TYPE = "TST";
            else TSK_TYPE = "HW";

        }
             if (state == INTENT_SAVE_TSK )
           {
            TSK_DATE = DateTime.Parse(PlayerPrefs.GetString(INTENT_TIME));
            DateTime to = TSK_DATE;
            to.AddMinutes(30.0f);
            if (TSK_TYPE == "TST")
                HomeScreenManager.StaticCreateMission(TSK_NAME, TSK_DATE, to);
            else
                HomeScreenManager.StaticCreateTest(TSK_NAME, TSK_DATE, to);
        }

        PlayerPrefs.SetString(PREFS_LAST_STATE, state);
        lastState = currentState;
        currentState = conversation[state];
        if (currentState[NODE_BACK])
        {
            result.goBack = true;
        }

        result.displayTexts = CurrentTexts();

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

        return (JSON.Parse(json)[NODE_ENTITIES][INTENT_YESNO][0][NODE_VALUE] == INTENT_YES);
    }

    public static Result RetriveLast()
    {
        Result result = new Result();
        currentState = lastState;
        result.displayTexts = CurrentTexts();
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




    private static string GetParam(string value, JSONNode ctx)
    {
        if (value[0] == '#')
        {
            string val = value.Substring(1, value.Length - 2);
            if (ctx[val])
            return PlayerPrefs.GetString(val);
            else return  "re-focuse" ;
        }
        return value;
    }



    private static string CheckMissionDate(DateTime date)
    {
        if (DateTime.Compare(DateTime.Now, date) > 0)
            return INTENT_WRONG_DATE;
        string temp = HomeScreenManager.CheckForEventAtTime(date);
        if (temp == null)
            return null;
        else {
            PlayerPrefs.SetString("INTENT_DUP_DATE", temp);
            return INTENT_DUP_DATE;
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
