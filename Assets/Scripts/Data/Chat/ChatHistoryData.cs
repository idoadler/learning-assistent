using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChatHistoryData
{
    private const int MAX_CHAT_HISTORY = 10;
    private const string eventsDataProjectFilePath = "/StreamingAssets/history.json";

    public static Chat Load()
    {
        Chat conversation;

        string filePath = Application.dataPath + eventsDataProjectFilePath;

        if (!ChatManager.IS_TESTING && File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
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
            conversation.texts = chatTexts.GetRange(chatTexts.Count - MAX_CHAT_HISTORY - 1, MAX_CHAT_HISTORY).ToArray();
        }

        string dataAsJson = JsonUtility.ToJson(conversation);

        string filePath = Application.dataPath + eventsDataProjectFilePath;
        File.WriteAllText(filePath, dataAsJson);
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
