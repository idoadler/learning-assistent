using Assets.SimpleAndroidNotifications;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HomeScreenManager : MonoBehaviour {
    private enum Screens { DAILY = 0, MISSIONS = 1, TESTS = 2 };
    private const int DEFALUT_STUDY_DAYS = 8;

    private List<EventsData.GeneralEvent> others;
    private List<EventsData.HomeworkEvent> homeworks;
    private List<EventsData.TestEvent> tests;
    private SortedDictionary<DateTime, DateLine> missionDates = new SortedDictionary<DateTime, DateLine>();
//    private SortedDictionary<DateTime, DateLine> testDates = new SortedDictionary<DateTime, DateLine>();
    private Dictionary<EventsData.HomeworkEvent, List<MissionLine>> homeworksUI = new Dictionary<EventsData.HomeworkEvent, List<MissionLine>>();
    private static Dictionary<DateTime, string> eventsDuplicates = new Dictionary<DateTime, string>();

    internal static void RemoveTestTask(EventsData.TestEvent data)
    {
        throw new NotImplementedException();
    }

    public MissionLine missionLinePrefab;
    public TestLine testLinePrefab;
    public DateLine dateLinePrefab;
    public CalendarComponent calendar;
    public GameObject dailyMissions;
    public GameObject allMissions;
    public GameObject allTests;
    public GameObject[] menusToHide;
    public GameObject[] screens;
    public GameObject addMissionMenu;

    private static HomeScreenManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadEvents();
        SetScreen((int)Screens.DAILY);
    }

    private void OnEnable()
    {
        HideMenus();
    }

    private void HideMenus()
    {
        foreach (GameObject menu in menusToHide)
        {
            menu.SetActive(false);
        }
    }

    public void SetScreen(int target)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].SetActive(i == target);
        }
        HideMenus();
        AnalyticsManager.ScreenVisit(screens[target].name);
    }

    private void LoadEvents()
    {
        EventsData.AllEvents allEvents = EventsData.Load();
        long now = DateTime.Now.ToFileTimeUtc();

        others = new List<EventsData.GeneralEvent>();
        foreach (EventsData.GeneralEvent e in allEvents.others)
        {
            if(e.utcTo > now)
            {
                CreateMission(e.description, DateTime.FromFileTimeUtc(e.utcFrom), DateTime.FromFileTimeUtc(e.utcTo), false);
            }
        }

        homeworks = new List<EventsData.HomeworkEvent>();
        foreach (EventsData.HomeworkEvent e in allEvents.homeworks)
        {
            if (e.utcTo > now)
            {
                CreateMission(e.description, DateTime.FromFileTimeUtc(e.utcFrom), DateTime.FromFileTimeUtc(e.utcTo), false);
            }
        }

        tests = new List<EventsData.TestEvent>();
        foreach (EventsData.TestEvent e in allEvents.tests)
        {
            if (e.utcAt > now)
            {
                CreateTest(e.description, DateTime.FromFileTimeUtc(e.utcAt), e.titles, false);
            }
        }
    }

    public void SaveEvents()
    {
        EventsData.AllEvents allEvents = new EventsData.AllEvents
        {
            others = others.ToArray(),
            homeworks = homeworks.ToArray(),
            tests = tests.ToArray()
        };
        EventsData.Save(allEvents);
    }

    public static string CheckForEventAtTime(DateTime time)
    {
        if (eventsDuplicates.ContainsKey(time))
        {
            return eventsDuplicates[time];
        }
        else
        {
            return null;
        }
    }

    public void AddMissions()
    {
        HideMenus();
        calendar.DisplayMissions();
    }

    public void AddTests()
    {
        HideMenus();
        calendar.DisplayTests();
    }

    public static void StaticCreateMission(string title, DateTime from, DateTime to)
    {
        Instance.CreateMission(title, from, to, true, true);
    }

    public void CreateMission(string title, DateTime from, DateTime to, bool original = true, bool chat = false)
    {
        if (eventsDuplicates.ContainsKey(from))
        {
            Debug.Log("adding event at the same time of event: " + eventsDuplicates[from]);
        }
        else
        {
            eventsDuplicates.Add(from, title);
        }

        if (!missionDates.ContainsKey(from.Date))
        {
            DateLine date = Instantiate(dateLinePrefab, allMissions.transform);
            date.label.Text = SetDateLabel.DateFormat(from);
            foreach (KeyValuePair<DateTime, DateLine> kvp in missionDates)
            {
                if (kvp.Key > from)
                {
                    date.GetComponent<RectTransform>().SetSiblingIndex(kvp.Value.GetComponent<RectTransform>().GetSiblingIndex());
                    break;
                }
            }
            missionDates.Add(from.Date, date);
        }

        EventsData.HomeworkEvent data = new EventsData.HomeworkEvent { description = title, utcFrom = from.ToFileTimeUtc(), utcTo = to.ToFileTimeUtc() };
        homeworks.Add(data);

        MissionLine mission = Instantiate(missionLinePrefab, allMissions.transform);
        mission.Init(title, from, to, data);
        mission.GetComponent<RectTransform>().SetSiblingIndex(missionDates[from.Date].GetComponent<RectTransform>().GetSiblingIndex() + 1);
        List<MissionLine> ui = new List<MissionLine>();
        ui.Add(mission);

        if (from.Date == DateTime.Today)
        {
            // add daily mission
            MissionLine today = Instantiate(missionLinePrefab, dailyMissions.transform);
            today.Init(title, from, to, data);
            ui.Add(today);
        }
        else
        {
            // go to mission screen
            SetScreen((int)Screens.MISSIONS);
        }
        homeworksUI.Add(data, ui);

        if (original)
        {
            AnalyticsManager.AddedHomeworkEvent(title, from, to, chat);
            //StartCoroutine(AnalyticsManager.SendMail("HW: " + title, "this is email test:\n" + SystemInfo.deviceUniqueIdentifier + "\n" + SystemInfo.deviceModel
            //    + "\n" + SystemInfo.deviceName + "\n" + SystemInfo.deviceType));

            //  set reminder
#if UNITY_EDITOR
            Debug.Log("Added notification: " + "היי,\n" + "עוד מעט מתחילים ללמוד" + mission.desc.Text);
            Debug.Log("Added notification: " + "היי,\n" + "סיימנו! איך היה?");
#elif UNITY_ANDROID
            int delta = (((from.Date.Day - DateTime.Now.Day) * 24 + (from.Hour - DateTime.Now.Hour)) * 60) + (from.Minute - DateTime.Now.Minute);
            int session = (to.Hour - from.Hour) * 60 + (to.Minute - from.Minute);
            NotificationManager.SendWithAppIcon(TimeSpan.FromMinutes(delta - 5), "היי", "עוד מעט מתחילים ללמוד" + mission.desc.Text, new Color(1, 0.8f, 1), NotificationIcon.Clock);
            NotificationManager.SendWithAppIcon(TimeSpan.FromMinutes(delta + session), "היי", "סיימנו! איך היה?", new Color(1, 0.8f, 1), NotificationIcon.Star);
#endif
        }
    }



    public static void StaticCreateTest(string title, DateTime at, string[] subjects = null)
    {
        Instance.CreateTest(title, at, subjects, true, true);
    }

    public void CreateTest(string title, DateTime at, string[] subjects, bool original = true, bool chat = false)
    {
        if (eventsDuplicates.ContainsKey(at))
        {
            Debug.Log("adding event at the same time of event: " + eventsDuplicates[at]);
        }
        else
        {
            eventsDuplicates.Add(at, title);
        }

        {
            // TODO:
            // ADDING MISSIONS DOESN'T WORK! WHYY????
            DateTime day = at.AddDays(-1);
            if (day < DateTime.Today)
            {
                CreateMission("חזרה לקראת מבחן: " + title, day, day.AddMinutes(30), false);

                if (subjects != null && subjects.Length > 0)
                {
                    for (int sub = subjects.Length - 1; sub >= 0 && day < DateTime.Today; sub--, day.AddDays(-1))
                    {
                        CreateMission("ללמוד: " + subjects[sub], day, day.AddMinutes(30), false);
                    }
                }
                else
                {
                    for (int sub = 0; sub < DEFALUT_STUDY_DAYS && day < DateTime.Today; sub++, day.AddDays(-1))
                    {
                        CreateMission("ללמוד: " + title, day, day.AddMinutes(30), false);
                    }
                }
            }
        }

        // TODO: sort by date

        EventsData.TestEvent data = new EventsData.TestEvent { description = title, utcAt = at.ToFileTimeUtc(), titles = subjects};
        tests.Add(data);

        TestLine test = Instantiate(testLinePrefab, allTests.transform);
        test.Init(title, at, 8, 0, data);
//        test.GetComponent<RectTransform>().SetSiblingIndex(testDates[at.Date].GetComponent<RectTransform>().GetSiblingIndex() + 1);

        if (at.Date == DateTime.Today)
        {
            // add daily mission
            TestLine today = Instantiate(testLinePrefab, dailyMissions.transform);
            today.Init(title, at, 8, 0, data);
        }
        else
        {
            // go to mission screen
            SetScreen((int)Screens.TESTS);
        }

        if (original)
        {
            AnalyticsManager.AddedTestEvent(title, at, subjects, chat);
        }

        //  set reminder
#if UNITY_EDITOR
        Debug.Log("Added notification: " + "היי,\n" + "בהצלחה במבחן ב" + test.desc.Text);
        Debug.Log("Added notification: " + "היי,\n" + "סיימנו! איך היה?");
#elif UNITY_ANDROID
        int delta = (((at.Date.Day - DateTime.Now.Day) * 24 + (at.Hour - DateTime.Now.Hour)) * 60) + (at.Minute - DateTime.Now.Minute);
        int session = 60;
        NotificationManager.SendWithAppIcon(TimeSpan.FromMinutes(delta - 5), "בהצלחה", "בהצלחה במבחן ב" + test.desc.Text, new Color(1, 0.8f, 1), NotificationIcon.Clock);
        NotificationManager.SendWithAppIcon(TimeSpan.FromMinutes(delta + session), "היי", "סיימנו! איך היה?", new Color(1, 0.8f, 1), NotificationIcon.Star);
#endif
    }

    public static EntryPoint StaticGetEntryPoint()
    {
        return Instance.GetEntryPoint();
    }

    public EntryPoint GetEntryPoint()
    {

        homeworks = new List<EventsData.HomeworkEvent>();
        EventsData.AllEvents allEvents = EventsData.Load();
        long now = DateTime.Now.ToFileTimeUtc();
        long flg = 6000000000*100;
        int time = -2;
        foreach (EventsData.HomeworkEvent e in allEvents.homeworks)
        {
            if ((e.utcFrom < now) && (e.utcTo > now)) time = 0;
            else
            {
                if ((0 <   e.utcFrom - now ) && (e.utcFrom - now < flg)) time = -1;
                else
                    if ((0 < now - e.utcTo)&& (now - e.utcTo < flg)) time = 1;
            }
            if (time != -2)
                return new EntryPoint { task = CurrentTask.HW, description = e.description, time = time };
        }

        foreach (EventsData.TestEvent e in allEvents.tests)
        {
            if (e.utcAt < now) time = 0;
            else
            {
                if ((0 < e.utcAt - now) && (e.utcAt - now < flg)) time = -1;
                else
                    if ((0 < now - e.utcAt) && (now - e.utcAt < flg)) time = 1;
            }
            if (time != -2)
                return new EntryPoint { task = CurrentTask.TEST, description = e.description, time = time };
        }
        return new EntryPoint { task = CurrentTask.NONE, description ="", time = time };
    }

    public static void RemoveHomeworkTask(EventsData.HomeworkEvent item)
    {
        Instance.homeworks.Remove(item);
        foreach (MissionLine mission in Instance.homeworksUI[item])
        {
            Destroy(mission.gameObject);
        }
        Instance.homeworksUI.Remove(item);
    }

    public enum CurrentTask
    {
        NONE, TEST, HW, ELSE
    }

    public struct EntryPoint
    {
        public CurrentTask task;
        public string description;
        public int time;
    }
}