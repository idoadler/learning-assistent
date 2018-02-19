using System;
using UnityEngine;

public class HomeScreenManager : MonoBehaviour {

    public GameObject missionLinePrefab;
    public GameObject dateLinePrefab;
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

    public void AddDailyMission()
    {
        ChatLine chatLine = Instantiate(missionLinePrefab, dailyMissions.transform).GetComponent<ChatLine>();
        // TODO: STUB
        HideMenus();
        addMissionMenu.SetActive(true);
    }

    public void AddMissions()
    {
        Instantiate(dateLinePrefab, allMissions.transform).GetComponent<ChatLine>();
        Instantiate(missionLinePrefab, allMissions.transform).GetComponent<ChatLine>();
        Instantiate(missionLinePrefab, allMissions.transform).GetComponent<ChatLine>();
        // TODO: STUB
        HideMenus();
        addMissionMenu.SetActive(true);
        isTest = false;
    }

    public void AddTests()
    {
        Instantiate(dateLinePrefab, allTests.transform).GetComponent<ChatLine>();
        Instantiate(missionLinePrefab, allTests.transform).GetComponent<ChatLine>();
        Instantiate(missionLinePrefab, allTests.transform).GetComponent<ChatLine>();
        // TODO: STUB
        HideMenus();
        addMissionMenu.SetActive(true);
        isTest = true;
    }

    public void CreateMission(DateTime from, DateTime to, string mission)
    {

    }
}
