using System;
using UnityEngine;
using UnityEngine.UI;

public class CalendarComponent : MonoBehaviour {
    public HomeScreenManager home;
    public InputField input;
    public RectTransform dates;
    public RectTransform hoursFrom;
    public RectTransform hoursTo;

    private bool firstTime = true;
    private float datesHeight;
    private float hoursToHeight;
    private float hoursFromHeight;
    private bool isTest;
    private bool isHomework;

    private void Init()
    {
        firstTime = false;
        datesHeight = dates.localPosition.y;
        hoursFromHeight = hoursFrom.localPosition.y;
        hoursToHeight = hoursTo.localPosition.y;
    }

    public void DisplayMissions()
    {
        Display(false,true);
    }

    public void DisplayTests()
    {
        Display(true, false);
    }

    public void DisplayGeneral()
    {
        Display(false, false);
    }

    private void Display(bool test, bool homework)
    {
        if (firstTime)
        {
            Init();
        }
        else
        {
            dates.localPosition = new Vector2(dates.localPosition.x, datesHeight);
            hoursFrom.localPosition = new Vector2(hoursFrom.localPosition.x, hoursFromHeight);
            hoursTo.localPosition = new Vector2(hoursTo.localPosition.x, hoursToHeight);
        }
        isTest = test;
        isHomework = homework;
        input.text = "";
        gameObject.SetActive(true);
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    public void Submit()
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            DateTime from = DateTime.Today.AddDays((dates.localPosition.y - datesHeight) / 80).AddHours(16 + (Math.Round((hoursFrom.localPosition.y - hoursFromHeight) / 80))/2);
            DateTime to = DateTime.Today.AddDays((dates.localPosition.y - datesHeight) / 80).AddHours(16.5 + (Math.Round((hoursTo.localPosition.y - hoursToHeight) / 80))/2);

            if (isTest)
            {
                home.CreateTest(input.text, from, new string[] { });
            }
            else if(isHomework)
            {
                home.CreateMission("שיעורים ב" + input.text, from, to);
            }
            else
            {
                home.CreateMission(input.text, from, to);
            }
            home.SaveEvents();
            gameObject.SetActive(false);
        }
    }
}
