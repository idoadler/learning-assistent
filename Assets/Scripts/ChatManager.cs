using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {
    public ScrollRect chatScroll;
    public InputField input;

    private void Awake()
    {
        WitAi.request_success += SetBotTextJSON;
        WitAi.request_failure += SetBotErrorMsg;
        input.Select();
        input.ActivateInputField();
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
            WitAi.Instance.Say(input.text);
            chatScroll.normalizedPosition = Vector2.zero;
            input.text = "";
        }
        input.Select();
        input.ActivateInputField();
    }

    private void SetBotText(string text)
    {
        ConversationManager.Instance.BotSay(text);
    }

    private void SetBotErrorMsg(string error)
    {
        SetBotText("התקבלה שגיאה:\n" + error);
    }

    private void SetBotTextJSON(string json)
    {
        JSONNode entities = JSON.Parse(json)["entities"];
        string key = "";
        string value = "";

        if (entities == null)
        {
            // wrong communication
            SetBotText("שגיאה: בעית תקשורת");
        }
        else if (entities.Count == 0)
        {
            // normal no result
            SetBotText("אין לי מושג למה התכוונת");
        }
        else if (entities.Count > 1)
        {
            // API change
            SetBotText("שגיאה: קלט לא מזוהה");
        }
        else
        {
            // normal result
            foreach (string k in entities.Keys)
            {
                key = k;
            }

            value = entities[key][0]["value"];
            SetBotText("זוהתה כוונה\n"+key + ": " + value);
        }
    }
}
