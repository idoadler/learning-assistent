﻿using Assets.SimpleAndroidNotifications;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {
    public GameObject[] screens;
    private int currentScreen = 0;
    public ScrollRect chatScroll;
    public InputField input;
    public TextAsset brain;
    public ConversationManager conversation;

    private void Awake()
    {
        WitAi.request_success += SetBotTextJSON;
        WitAi.request_failure += SetBotErrorMsg;
    }

    private void Start()
    {
        conversation.BotSay(JsonManager.InitConversationJson(brain.text));
        NotificationManager.SendWithAppIcon(TimeSpan.FromMinutes(5), "היי", "אל תשכח להתחיל בשיעורי הבית", new Color(1, 0.8f, 1), NotificationIcon.Clock);

        // reset screens
        foreach(GameObject g in screens)
        {
            g.SetActive(false);
        }
        screens[currentScreen].SetActive(true);
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

    public void SendText()
    {
        if (input.text != "")
        {
            conversation.UserSay(input.text);
            StartCoroutine(ScrollToBottom());
            WitAi.Instance.Say(input.text);
            input.text = "";
        }
    }

    private void SetBotText(string text)
    {
        conversation.BotSay(text);
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
        //conversation.EmptyHack();
        yield return new WaitForEndOfFrame();
        chatScroll.verticalNormalizedPosition = 0;
    }
}
