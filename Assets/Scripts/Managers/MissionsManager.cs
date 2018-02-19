using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionsManager : MonoBehaviour {
    public SortedDictionary<DateTime, SortedList<DateTime, Mission>> missions;
    public SortedDictionary<DateTime, SortedList<DateTime, Mission>> tests;

    // Use this for initialization
    void Start() {
        // TODO: read and display all existing data
    }

    public void AddMission(string title, DateTime from, DateTime to)
    {

    }

    public void AddTest(string title, DateTime from, DateTime to)
    {

    }

    public struct Mission
    {
        string title;
        DateTime from;
        DateTime to;
    }

    public struct Test
    {
        string title;
        DateTime from;
        DateTime to;
    }
}
