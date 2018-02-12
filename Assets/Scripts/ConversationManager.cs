using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationManager : MonoBehaviour {
    public static ConversationManager Instance;
    public GameObject botPrefab;
    public GameObject userPrefab;
    public GameObject emptyPrefab;
    public List<ChatLine> conversation = new List<ChatLine>();

    private GameObject emptyObject;

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
        SayText(botPrefab, text);
    }

    public void UserSay(string text)
    {
        SayText(userPrefab, text);
    }

    private void SayText(GameObject prefab, string text)
    {
        // hack to make the chat scrollebel to the correct hight (the scroller set this off at drag)
        GetComponent<VerticalLayoutGroup>().childControlHeight = true;

        // create prefab with the spoken text
        ChatLine chatLine = Instantiate(prefab, transform).GetComponent<ChatLine>();
        chatLine.label.Text = text;
        conversation.Add(chatLine);
    }

    public void EmptyHack()
    {
        // hack to add another empty line at the end of the chat
        if (emptyObject != null)
        {
            Destroy(emptyObject);
        }
        emptyObject = Instantiate(emptyPrefab, transform);
    }
}
