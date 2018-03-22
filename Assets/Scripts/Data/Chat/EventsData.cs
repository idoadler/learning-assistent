using System;
using System.IO;
using UnityEngine;

public class EventsData
{
    private const string eventsDataProjectFilePath = "/StreamingAssets/events.json";

    public static AllEvents LoadEventsData()
    {
        AllEvents events;

        string filePath = Application.dataPath + eventsDataProjectFilePath;

        if (!ChatManager.IS_TESTING && File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            events = JsonUtility.FromJson<AllEvents>(dataAsJson);
        }
        else
        {
            events = new AllEvents
            {
                homeworks = new HomeworkEvent[0],
                tests = new TestEvent[0],
                others = new GeneralEvent[0]
            };
        }

        return events;
    }

    public static void SaveEventsData(AllEvents events)
    {
        string dataAsJson = JsonUtility.ToJson(events);

        string filePath = Application.dataPath + eventsDataProjectFilePath;
        File.WriteAllText(filePath, dataAsJson);
    }

    [Serializable]
    public struct AllEvents
    {
        public HomeworkEvent[] homeworks;
        public TestEvent[] tests;
        public GeneralEvent[] others;
    }

    [Serializable]
    public struct HomeworkEvent
    {
        public long utcFrom;
        public long utcTo;
        public string description;
    }

    [Serializable]
    public struct TestEvent
    {
        public long utcFrom;
        public long utcTo;
        public string description;
    }

    [Serializable]
    public struct GeneralEvent
    {
        public long utcFrom;
        public long utcTo;
        public string description;
    }
}
