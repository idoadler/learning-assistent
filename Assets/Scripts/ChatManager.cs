using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {
    public ScrollRect chatScroll;
    public InputField input;
    public TextAsset conversation;

    private void Awake()
    {
        WitAi.request_success += SetBotTextJSON;
        WitAi.request_failure += SetBotErrorMsg;
        input.Select();
        input.ActivateInputField();
    }

    private void Start()
    {
        ConversationManager.Instance.BotSay(JsonManager.InitConversationJson(conversation.text));
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
    }

    public void SendText()
    {
        if (input.text != "")
        {
            ConversationManager.Instance.UserSay(input.text);
            //TODO: scroll to bottom
            WitAi.Instance.Say(input.text);
            input.text = "";
        }
        input.Select();
        input.ActivateInputField();
    }

    private void SetBotText(string text)
    {
        ConversationManager.Instance.BotSay(text);
        //TODO: scroll to bottom
    }

    private void SetBotErrorMsg(string error)
    {
        SetBotText("התקבלה שגיאה:\n" + error);
    }

    private void SetBotTextJSON(string json)
    {
        SetBotText(JsonManager.JsonToBotText(json));
    }
}
