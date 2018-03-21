using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WitAi))]
[RequireComponent(typeof(BrainManager))]
public class ChatManager : MonoBehaviour {
    public static bool IS_TESTING = false;

    public static bool IsAssistentGirl = true;
    public static bool IsUserGirl = true;

    public GameObject[] screens;
    private int currentScreen = 0;
    public ScrollRect chatScroll;
    public InputField input;
    public ConversationManager conversation;

    private WitAi witai;

    private void Awake()
    {
        witai = GetComponent<WitAi>();
        WitAi.request_success += SetBotTextJSON;
        WitAi.request_failure += SetBotErrorMsg;

        // reset screens
        foreach (GameObject g in screens)
        {
            g.SetActive(false);
        }
        screens[0].SetActive(true);
    }

    private void Start()
    {
        GetComponent<BrainManager>().InitConversationBrain();
        foreach (string text in JsonManager.CurrentTexts())
        {
            SetBotText(text);
        }
    }

    public void NextScreen()
    {
        if (currentScreen < screens.Length)
        {
            screens[currentScreen].SetActive(false);
            currentScreen++;
            screens[currentScreen].SetActive(true);
        }
    }

    public void LastScreen()
    {
        if (currentScreen > 0)
        {
            screens[currentScreen].SetActive(false);
            currentScreen--;
            screens[currentScreen].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter) /*|| !input.isFocused*/)
        {
            SendText();
        }

        // TODO: adjust UI height by TouchScreenKeyboard.area.height
    }

    public void SetAsistentGender(bool isGirl)
    {
        IsAssistentGirl = isGirl;
        if (!isGirl)
        {
            JsonManager.ConversationGender = "m2f";
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

    private JsonManager.Result lastBeforeBack = new JsonManager.Result();
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

    internal void ChooseResult(string text)
    {
        throw new NotImplementedException();
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
