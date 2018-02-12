using Assets.SimpleAndroidNotifications;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {
    public ScrollRect chatScroll;
    public InputField input;
    public TextAsset brain;

    private void Awake()
    {
        WitAi.request_success += SetBotTextJSON;
        WitAi.request_failure += SetBotErrorMsg;
    }

    private void Start()
    {
        ConversationManager.Instance.BotSay(JsonManager.InitConversationJson(brain.text));
        NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds(5), "היי", "אל תשכח להתחיל בשיעורי הבית", new Color(1, 0.8f, 1), NotificationIcon.Clock);
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

    public void SendText()
    {
        if (input.text != "")
        {
            ConversationManager.Instance.UserSay(input.text);
            StartCoroutine(ScrollToBottom());
            WitAi.Instance.Say(input.text);
            input.text = "";
        }
    }

    private void SetBotText(string text)
    {
        ConversationManager.Instance.BotSay(text);
        StartCoroutine(ScrollToBottom());
    }

    private void SetBotErrorMsg(string error)
    {
        SetBotText("התקבלה שגיאה:\n" + error);
    }

    private void SetBotTextJSON(string json)
    {
        SetBotText(JsonManager.JsonToBotText(json));
    }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        chatScroll.verticalNormalizedPosition = 0;
        ConversationManager.Instance.EmptyHack();
        yield return new WaitForEndOfFrame();
        chatScroll.verticalNormalizedPosition = 0;
    }
}
