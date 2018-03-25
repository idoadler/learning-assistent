using System.Collections.Generic;
using UnityEngine;

public class ConversationManager : MonoBehaviour {
    public GameObject botPrefab;
    public GameObject userPrefab;
    public List<ChatHistoryData.ChatText> conversation = new List<ChatHistoryData.ChatText>();

    public void BotSay(string text)
    {
        SayText(botPrefab, text);
        conversation.Add(new ChatHistoryData.ChatText { isBot = true, text = text });
        ChatHistoryData.Save(conversation);
        //AnalyticsManager.ChatMessageSent(text, false);
    }

    public void UserSay(string text, bool sendAnalytics = true)
    {
        SayText(userPrefab, text);
        conversation.Add(new ChatHistoryData.ChatText { isBot = false, text = text });
        ChatHistoryData.Save(conversation);
        if (sendAnalytics)
        {
            AnalyticsManager.ChatMessageSent(text, true);
        }
    }

    private void SayText(GameObject prefab, string text)
    {
        // create prefab with the spoken text
        ChatLine chatLine = Instantiate(prefab, transform).GetComponent<ChatLine>();
        chatLine.label.Text = text;
    }
 }
