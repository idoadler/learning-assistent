using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationManager : MonoBehaviour {
    public static ConversationManager Instance;
    public GameObject botPrefab;
    public GameObject userPrefab;
    public List<ChatLine> conversation = new List<ChatLine>();

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    public void BotSay(string text)
    {
        GetComponent<VerticalLayoutGroup>().childControlHeight = true;
        ChatLine botLine = Instantiate(botPrefab, transform).GetComponent<ChatLine>();
        botLine.label.Text = text;
        conversation.Add(botLine);
    }

    public void UserSay(string text)
    {
        ChatLine userLine = Instantiate(userPrefab, transform).GetComponent<ChatLine>();
        userLine.label.Text = text;
        conversation.Add(userLine);
    }
}
