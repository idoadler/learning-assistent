using System;
using System.Collections.Generic;
using UnityEngine;

public class HomeScreenManager : MonoBehaviour {
    private enum Screens {DAILY = 0, MISSIONS = 1, TESTS = 2 };

//    private List<MissionLine> todayMissions = new List<MissionLine>();
    private SortedDictionary<DateTime, MissionList> missions = new SortedDictionary<DateTime, MissionList>();
  private SortedDictionary<DateTime, MissionList> tests = new SortedDictionary<DateTime, MissionList>();

    public MissionLine missionLinePrefab;
    public DateLine dateLinePrefab;
    public CalendarComponent calendar;
    public GameObject dailyMissions;
    public GameObject allMissions;
    public GameObject allTests;
    public GameObject[] menusToHide;
    public GameObject[] screens;
    public GameObject addMissionMenu;
    private bool isTest;

    public void SetScreen(int target)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].SetActive(i == target);
        }
        HideMenus();
    }

    private void Start()
    {
        SetScreen((int)Screens.DAILY);
        // TODO: read and display all existing data
    }

    private void HideMenus()
    {
        foreach (GameObject menu in menusToHide)
        {
            menu.SetActive(false);
        }
    }

    private void OnEnable()
    {
        HideMenus();
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

    public void CreateMission(string title, DateTime from, DateTime to)
    {
        if (!missions.ContainsKey(from.Date))
        {
            DateLine date = Instantiate(dateLinePrefab, allMissions.transform);
            date.label.Text = SetDateLabel.DateFormat(from);
            missions.Add(from.Date, new MissionList(date));
        }

        MissionLine mission = Instantiate(missionLinePrefab, allMissions.transform);
        mission.desc.Text = title;
        mission.time.Text = from.ToString("HH:mm") + "-" + to.ToString("HH:mm");
  //      try { 
  //      missions[from.Date].missions.Add(from, mission);
  //      }        catch { Debug.LogError("can't add to events at the same time"); }

        if (from.Date == DateTime.Today)
        {
            // add daily mission
            MissionLine today = Instantiate(missionLinePrefab, dailyMissions.transform);
            today.desc.Text = title;
            today.time.Text = from.ToString("HH:mm") + "-" + to.ToString("HH:mm");
//            todayMissions.Add(today);
        }
        else
        {
            // go to mission screen
            SetScreen((int)Screens.MISSIONS);
        }


        // TODO: set reminder
        //#if UNITY_ANDROID
        //        NotificationManager.SendWithAppIcon(TimeSpan.FromMinutes(5), "היי", "אל תשכח להתחיל בשיעורי הבית", new Color(1, 0.8f, 1), NotificationIcon.Clock);
        //#endif
    }

    public struct MissionList
    {
        public DateLine date;
        public SortedList<DateTime, MissionLine> missions;

        public MissionList(DateLine d)
        {
            date = d;
            missions = new SortedList<DateTime, MissionLine>();
        }
    }

    public void CreateTest(string title, DateTime from, DateTime to)
    {
        // TODO: Test screen
        if (!tests.ContainsKey(from.Date))
        {
            DateLine date = Instantiate(dateLinePrefab, allTests.transform);
            date.label.Text = SetDateLabel.DateFormat(from);
            tests.Add(from.Date, new MissionList(date));
        }

        MissionLine mission = Instantiate(missionLinePrefab, allTests.transform);
        mission.desc.Text = title;
        mission.time.Text = from.ToString("HH:mm") + "-" + to.ToString("HH:mm");
//        try        {
//            missions[from.Date].missions.Add(from, mission);
//        }        catch { Debug.LogError("can't add to events at the same time"); }

        if (from.Date == DateTime.Today)
        {
            // add daily mission
            MissionLine today = Instantiate(missionLinePrefab, dailyMissions.transform);
            today.desc.Text = title;
            today.time.Text = from.ToString("HH:mm") + "-" + to.ToString("HH:mm");
//            todayMissions.Add(today);
        }
        else
        {
            // go to mission screen
            SetScreen((int)Screens.TESTS);
        }

        // TODO: set reminder
        //#if UNITY_ANDROID
        //        NotificationManager.SendWithAppIcon(TimeSpan.FromMinutes(5), "היי", "אל תשכח להתחיל בשיעורי הבית", new Color(1, 0.8f, 1), NotificationIcon.Clock);
        //#endif
    }
}
