using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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

    public static AnalyticsResult AddedHomeworkEvent(string desc, DateTime from, DateTime to, bool chat)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("desc", desc);
        data.Add("from", from);
        data.Add("to", to);
        data.Add("chat", chat);
        return AnalyticsEvent.Custom("AddHomework",data);
    }

    public static AnalyticsResult AddedTestEvent(string desc, DateTime from, DateTime to, bool chat)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("desc", desc);
        data.Add("from", from);
        data.Add("to", to);
        data.Add("chat", chat);
        return AnalyticsEvent.Custom("AddTest",data);
    }

    public static IEnumerator SendMail(string aSubject, string aBody)
    {
        string aFrom = "todobotapp@gmail.com";
        string aPassword = "mindcet!";
        string aTo = "todobot@lahamonim.com";

        MailMessage mail = new MailMessage();

        mail.From = new MailAddress(aFrom);
        mail.To.Add(aTo);
        mail.Subject = aSubject;
        mail.Body = aBody;

        SmtpClient smtpServer = new SmtpClient();
        smtpServer.Host = "smtp.gmail.com";
        smtpServer.Port = 587;
        smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpServer.Credentials = new NetworkCredential(aFrom, aPassword) as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        smtpServer.Send(mail);
        yield return null;
    }
}
