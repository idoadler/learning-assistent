using Assets.SimpleAndroidNotifications;
using System.Collections.Generic;
using UnityEngine;

public class ConversationManager : MonoBehaviour {

    public GameObject botPrefab;
    public GameObject userPrefab;
    public List<ChatLine> conversation = new List<ChatLine>();

    private void Start()
    {
       // if miss
     //   JsonManager.ChgCnvNode("task-ver");
        BotSay(JsonManager.CurrentText());
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
        // create prefab with the spoken text
        ChatLine chatLine = Instantiate(prefab, transform).GetComponent<ChatLine>();
        chatLine.label.Text = text;
        conversation.Add(chatLine);
    }
 }
