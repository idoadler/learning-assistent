using System;
using System.Collections.Generic;
using UnityEngine.Analytics;

public static class AnalyticsManager {
    private static string lastMessage = "";
    public static AnalyticsResult ChatMessageSent(string text, bool isUser)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("from", isUser ? "user" : "bot");
        data.Add("text", text);
        data.Add("previous", lastMessage);
        lastMessage = text;
        return AnalyticsEvent.ChatMessageSent(data);
    }

    public static AnalyticsResult UserSignup()
    {
        return AnalyticsEvent.UserSignup(AuthorizationNetwork.None);
    }

    private static string lastScreen = "";
    public static AnalyticsResult ScreenVisit(string screen)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("previous", lastScreen);
        return AnalyticsEvent.ScreenVisit(screen, data);
    }

    public static AnalyticsResult AddedHomeworkEvent(string desc, DateTime from, DateTime to)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("desc", desc);
        data.Add("from", from);
        data.Add("to", to);
        return AnalyticsEvent.Custom("AddHomework",data);
    }

    public static AnalyticsResult AddedTestEvent(string desc, DateTime from, DateTime to)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("desc", desc);
        data.Add("from", from);
        data.Add("to", to);
        return AnalyticsEvent.Custom("AddTest",data);
    }
}
