using System;
using System.Collections.Generic;
using UnityEngine;

public class HomeScreenManager : MonoBehaviour {
    private enum Screens { DAILY = 0, MISSIONS = 1, TESTS = 2 };

    private List<EventsData.GeneralEvent> others;
    private List<EventsData.HomeworkEvent> homeworks;
    private List<EventsData.TestEvent> tests;
    private SortedDictionary<DateTime, DateLine> missionDates = new SortedDictionary<DateTime, DateLine>();
    private SortedDictionary<DateTime, DateLine> testDates = new SortedDictionary<DateTime, DateLine>();
    private Dictionary<EventsData.HomeworkEvent, List<MissionLine>> homeworksUI = new Dictionary<EventsData.HomeworkEvent, List<MissionLine>>();
    private static Dictionary<DateTime, string> eventsDuplicates = new Dictionary<DateTime, string>();

    public MissionLine missionLinePrefab;
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
            if (e.utcTo > now)
            {
                CreateTest(e.description, DateTime.FromFileTimeUtc(e.utcFrom), DateTime.FromFileTimeUtc(e.utcTo), false);
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



    public static void StaticCreateTest(string title, DateTime from, DateTime to)
    {
        Instance.CreateTest(title, from, to, true, true);
    }

    public void CreateTest(string title, DateTime from, DateTime to, bool original = true, bool chat = false)
    {
        if (eventsDuplicates.ContainsKey(from))
        {
            Debug.Log("adding event at the same time of event: " + eventsDuplicates[from]);
        }
        else
        {
            eventsDuplicates.Add(from, title);
        }

        tests.Add(new EventsData.TestEvent { description = title, utcFrom = from.ToFileTimeUtc(), utcTo = to.ToFileTimeUtc() });

        if (!testDates.ContainsKey(from.Date))
        {
            DateLine date = Instantiate(dateLinePrefab, allTests.transform);
            date.label.Text = SetDateLabel.DateFormat(from);
            foreach (KeyValuePair<DateTime, DateLine> kvp in testDates)
            {
                if (kvp.Key > from)
                {
                    date.GetComponent<RectTransform>().SetSiblingIndex(kvp.Value.GetComponent<RectTransform>().GetSiblingIndex());
                    break;
                }
            }
            testDates.Add(from.Date, date);
        }

        MissionLine mission = Instantiate(missionLinePrefab, allTests.transform);
        mission.desc.Text = title;
        mission.time.Text = from.ToString("HH:mm") + "-" + to.ToString("HH:mm");
        mission.GetComponent<RectTransform>().SetSiblingIndex(testDates[from.Date].GetComponent<RectTransform>().GetSiblingIndex() + 1);

        if (from.Date == DateTime.Today)
        {
            // add daily mission
            MissionLine today = Instantiate(missionLinePrefab, dailyMissions.transform);
            today.desc.Text = title;
            today.time.Text = from.ToString("HH:mm") + "-" + to.ToString("HH:mm");
        }
        else
        {
            // go to mission screen
            SetScreen((int)Screens.TESTS);
        }

        if (original)
        {
            AnalyticsManager.AddedTestEvent(title, from, to, chat);

        //  set reminder
#if UNITY_EDITOR
        Debug.Log("Added notification: " + "היי,\n" + "אל תשכח להתחיל ללמוד למבחן ב" + mission.desc.Text);
        Debug.Log("Added notification: " + "היי,\n" + "סיימנו! איך היה?");
#elif UNITY_ANDROID
        int delta = (((from.Date.Day - DateTime.Now.Day) * 24 + (from.Hour - DateTime.Now.Hour)) * 60) + (from.Minute - DateTime.Now.Minute);
        int session = (to.Hour - from.Hour) * 60 + (to.Minute - from.Minute);
        NotificationManager.SendWithAppIcon(TimeSpan.FromMinutes(delta - 5), "היי", "אל תשכח להתחיל ללמוד למבחן ב" + mission.desc.Text, new Color(1, 0.8f, 1), NotificationIcon.Clock);
        NotificationManager.SendWithAppIcon(TimeSpan.FromMinutes(delta + session), "היי", "סיימנו! איך היה?", new Color(1, 0.8f, 1), NotificationIcon.Star);
#endif
        }
    }

    public static EntryPoint StaticGetEntryPoint()
    {
        return Instance.GetEntryPoint();
    }

    public EntryPoint GetEntryPoint()
    {
        return new EntryPoint {task = CurrentTask.NONE, description = "example", time = -1 };
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
