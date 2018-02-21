using System;
using System.Collections.Generic;
using UnityEngine;

public class HomeScreenManager : MonoBehaviour {
    private List<MissionLine> todayMissions = new List<MissionLine>();
    private SortedDictionary<DateTime, MissionList> missions = new SortedDictionary<DateTime, MissionList>();
  //  private SortedDictionary<DateTime, SortedList<DateTime, Test>> tests = new SortedDictionary<DateTime, SortedList<DateTime, Test>>();

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
        SetScreen(0);
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
        calendar.Display(false);
    }

    public void AddTests()
    {
        HideMenus();
        calendar.Display(true);
    }

    public void CreateMission(string title, DateTime from, DateTime to)
    {
        if (!missions.ContainsKey(from.Date))
        {
            DateLine date = Instantiate(dateLinePrefab, allMissions.transform);
            date.label.Text = SetDateLabel.DateFormat(from);
            missions.Add(from.Date, new MissionList(date));
        }

        if (from.Date == DateTime.Today)
        {
            MissionLine today = Instantiate(missionLinePrefab, dailyMissions.transform);
            today.desc.Text = title;
            today.time.Text = from.ToString("HH:mm") + "-" + to.ToString("HH:mm");
            todayMissions.Add(today);
        }

        MissionLine mission = Instantiate(missionLinePrefab, allMissions.transform);
        mission.desc.Text = title;
        mission.time.Text = from.ToString("HH:mm") + "-" + to.ToString("HH:mm");
        missions[from.Date].missions.Add(from, mission);
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
        if (!missions.ContainsKey(from.Date))
        {
            DateLine date = Instantiate(dateLinePrefab, allTests.transform);
            date.label.Text = SetDateLabel.DateFormat(from);
            missions.Add(from.Date, new MissionList(date));
        }

        if (from.Date == DateTime.Today)
        {
            MissionLine today = Instantiate(missionLinePrefab, allTests.transform);
            today.desc.Text = title;
            today.time.Text = from.ToString("HH:mm") + "-" + to.ToString("HH:mm");
            todayMissions.Add(today);
        }

        MissionLine mission = Instantiate(missionLinePrefab, allTests.transform);
        mission.desc.Text = title;
        mission.time.Text = from.ToString("HH:mm") + "-" + to.ToString("HH:mm");
        missions[from.Date].missions.Add(from, mission);
// TODO: set reminder
//#if UNITY_ANDROID
//        NotificationManager.SendWithAppIcon(TimeSpan.FromMinutes(5), "היי", "אל תשכח להתחיל בשיעורי הבית", new Color(1, 0.8f, 1), NotificationIcon.Clock);
//#endif
    }
}
