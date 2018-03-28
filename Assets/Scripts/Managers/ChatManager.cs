using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WitAi))]
[RequireComponent(typeof(BrainManager))]
public class ChatManager : MonoBehaviour {
    public static bool IS_TESTING = false;

    public GameObject[] screens;
    private int currentScreen;
    public ScrollRect chatScroll;
    public InputField input;
    public ConversationManager conversation;
    private WitAi witai;

    private JsonManager.Result lastBeforeBack = new JsonManager.Result();
    private static string PREFS_USED_SCREEN = "USED_SCREEN";

    private void Awake()
    {
        if(IS_TESTING)
        {
            Debug.LogWarning("TESTING MODE");
            PlayerPrefs.DeleteAll();
        }

        witai = GetComponent<WitAi>();
        WitAi.request_success += SetBotTextJSON;
        WitAi.request_failure += SetBotErrorMsg;
    }

    private void Start()
    {
        InitConversation();

        // reset screens
        foreach (GameObject g in screens)
        {
            g.SetActive(false);
        }
        currentScreen = PlayerPrefs.GetInt(PREFS_USED_SCREEN, 0);
        screens[currentScreen].SetActive(true);
    }

    private void InitConversation()
    {
        GetComponent<BrainManager>().InitConversationBrain();
        
        ChatHistoryData.Chat chatData = ChatHistoryData.Load();
        if (chatData.texts.Length > 0)
        {
            foreach (ChatHistoryData.ChatText chatLine in chatData.texts)
            {
                if (chatLine.isBot)
                {
                    conversation.BotSay(chatLine.text);
                }
                else
                {
                    conversation.UserSay(chatLine.text, false);
                }
            }
        }
        //SetBotTextByErr(GetComponent<BrainManager>().GetExtra());
        ScrollToBottom();
   }

    public void NextScreen()
    {
        if (currentScreen < screens.Length)
        {
            screens[currentScreen].SetActive(false);
            currentScreen++;
            screens[currentScreen].SetActive(true);
            PlayerPrefs.SetInt(PREFS_USED_SCREEN, currentScreen);
        }
    }

    public void LastScreen()
    {
        if (currentScreen > 0)
        {
            screens[currentScreen].SetActive(false);
            currentScreen--;
            screens[currentScreen].SetActive(true);
            PlayerPrefs.SetInt(PREFS_USED_SCREEN, currentScreen);
        }
    }

    // Update is called once per frame
    void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Application.Quit();
            // TODO: show quit question
        }
        else if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter) /*|| !input.isFocused*/)
        {
            SendText();
        }

        // TODO: adjust UI height by TouchScreenKeyboard.area.height
    }

    public void SetAsistentGender(bool isGirl)
    {
        JsonManager.IsBotFemale = isGirl;

        // assistent introduction
        foreach (string text in JsonManager.CurrentTexts())
        {
            SetBotText(text);
        }
    }


    public void SetBotTextByErr(string [] texts)
    {
        foreach (string text in texts)
        {
            SetBotText(text);
        }
    }


    public void SendText()
    {
        if (input.text != "")
        {
            conversation.UserSay(input.text);
            ScrollToBottom();
            witai.Say(input.text);
            input.text = "";
        }
    }

    private void SetBotText(string text)
    {
        conversation.BotSay(text);
        ScrollToBottom();
    }

    private void SetBotErrorMsg(string error)
    {
        SetBotText("התקבלה שגיאה:\n" + error);
    }

    private void SetBotTextJSON(string json)
    {
        // Testing for exit attempt
        if (lastBeforeBack.goBack)
        {
            bool exit = JsonManager.VerifyJsonExit(json);
            SetBotText(JsonManager.RetriveLast().displayTexts[JsonManager.RetriveLast().displayTexts.Length-1]);
            lastBeforeBack = new JsonManager.Result();
            if (exit)
            {
                NextScreen();
            }
        }
        else
        {
            JsonManager.Result result = JsonManager.JsonToBotText(json);
            if (result.goBack)
            {
                lastBeforeBack = result;
                foreach (string text in JsonManager.AskForExit().displayTexts)
                {
                    SetBotText(text);
                }
            }
            else
            {
                // Main chat behavior
                foreach (string text in result.displayTexts)
                {
                    SetBotText(text);
                }
            }
        }
    }

    private void ScrollToBottom()
    {
        StartCoroutine(DelayedExecute(() => {
            chatScroll.verticalNormalizedPosition = 0;
        }));
    }

    private IEnumerator DelayedExecute(Action f)
    {
        yield return new WaitForEndOfFrame();
        f();
    }
}
