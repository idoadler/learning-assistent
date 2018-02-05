using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {
    public ScrollRect chatScroll;
    public InputField input;

    private void Start()
    {
        WitAi.request_success += SetBotTextJSON;
        WitAi.request_failure += SetBotText;
    }

    // Update is called once per frame
    void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (input.isFocused && Input.GetKey(KeyCode.Return))
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
    }

    private void SetBotText(string text)
    {
        ConversationManager.Instance.BotSay(text);
    }

    private void SetBotTextJSON(string json)
    {
        JSONNode entities = JSON.Parse(json)["entities"];
        string key = "";
        string value = "";

        if (entities == null)
        {
            // wrong communication
            SetBotText("no entities");
        }
        else if (entities.Count == 0)
        {
            // normal no result
            SetBotText("no keys");
        }
        else if (entities.Count > 1)
        {
            // API change
            SetBotText("too many keys");
        }
        else
        {
            // normal result
            foreach (string k in entities.Keys)
            {
                key = k;
            }

            value = entities[key][0]["value"];
            SetBotText(key + ": " + value);
        }
    }
}
