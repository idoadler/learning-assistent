using System.Collections.Generic;
using UnityEngine;

public class ConversationManager : MonoBehaviour {

    public GameObject botPrefab;
    public GameObject userPrefab;
    public List<ChatLine> conversation = new List<ChatLine>();
    private string lastText = "";

    private void Start()
    {
        BotSay(JsonManager.CurrentText());
    }

    public void BotSay(string text)
    {
        SayText(botPrefab, text);
        AnalyticsManager.ChatMessageSent(text, false);

    }

    public void UserSay(string text)
    {
        SayText(userPrefab, text);
        AnalyticsManager.ChatMessageSent(text, true);
    }

    private void SayText(GameObject prefab, string text)
    {
        // create prefab with the spoken text
        ChatLine chatLine = Instantiate(prefab, transform).GetComponent<ChatLine>();
        chatLine.label.Text = text;
        conversation.Add(chatLine);
    }
 }
