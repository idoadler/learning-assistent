using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    // Use this for initialization
    void Start () {
        BotSay("שלום משתמש");
	}

    public void BotSay(string text)
    {
        ChatLine botLine = Instantiate(botPrefab, transform).GetComponent<ChatLine>();
        botLine.label.text = text;
        conversation.Add(botLine);
    }

    public void UserSay(string text)
    {
        ChatLine userLine = Instantiate(userPrefab, transform).GetComponent<ChatLine>();
        userLine.label.text = text;
        conversation.Add(userLine);
    }
}
