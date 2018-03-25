using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChatHistoryData
{
    private const string PREF_HISTORY_DATA = "HISTORY";
    private const int MAX_CHAT_HISTORY = 10;
//    private const string eventsDataProjectFilePath = "/StreamingAssets/history.json";

    public static Chat Load()
    {
        Chat conversation;
        string dataAsJson = PlayerPrefs.GetString(PREF_HISTORY_DATA, "");
//        string filePath = eventsDataProjectFilePath.FullPath();

        if (!ChatManager.IS_TESTING && !string.IsNullOrEmpty(dataAsJson))
        {
//            string dataAsJson = File.ReadAllText(filePath);
            conversation = JsonUtility.FromJson<Chat>(dataAsJson);
        }
        else
        {
            conversation = new Chat
            {
                texts = new ChatText[0]
            };
        }

        return conversation;
    }

    public static void Save(List<ChatHistoryData.ChatText> chatTexts)
    {
        Chat conversation = new Chat();
        if (chatTexts.Count <= MAX_CHAT_HISTORY)
        {
            conversation.texts = chatTexts.ToArray();
        } else
        {
            conversation.texts = chatTexts.GetRange(chatTexts.Count - MAX_CHAT_HISTORY, MAX_CHAT_HISTORY).ToArray();
        }

        string dataAsJson = JsonUtility.ToJson(conversation);

        PlayerPrefs.SetString(PREF_HISTORY_DATA, dataAsJson);
//        string filePath = eventsDataProjectFilePath.FullPath();
//        File.WriteAllText(filePath, dataAsJson);
    }

    [Serializable]
    public struct Chat
    {
        public ChatText[] texts;
    }

    [Serializable]
    public struct ChatText
    {
        public bool isBot;
        public string text;
    }
}
