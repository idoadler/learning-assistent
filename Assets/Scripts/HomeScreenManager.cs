using UnityEngine;

public class HomeScreenManager : MonoBehaviour {

    public GameObject missionLinePrefab;
    public GameObject dateLinePrefab;
    public GameObject dailyMissions;
    public GameObject allMissions;
    public GameObject allTests;
    public GameObject subMenu;
    public GameObject[] screens;

    public void SetScreen(int target)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].SetActive(i == target);
        }
        subMenu.SetActive(false);
    }

    private void Start()
    {
        SetScreen(0);
    }

    private void OnEnable()
    {
        subMenu.SetActive(false);
    }

    public void AddDailyMission()
    {
        // TODO: STUB
        ChatLine chatLine = Instantiate(missionLinePrefab, dailyMissions.transform).GetComponent<ChatLine>();
    }

    public void AddMissions()
    {
        // TODO: STUB
        Instantiate(dateLinePrefab, allMissions.transform).GetComponent<ChatLine>();
        Instantiate(missionLinePrefab, allMissions.transform).GetComponent<ChatLine>();
        Instantiate(missionLinePrefab, allMissions.transform).GetComponent<ChatLine>();
    }

    public void AddTests()
    {
        // TODO: STUB
        Instantiate(dateLinePrefab, allTests.transform).GetComponent<ChatLine>();
        Instantiate(missionLinePrefab, allTests.transform).GetComponent<ChatLine>();
        Instantiate(missionLinePrefab, allTests.transform).GetComponent<ChatLine>();
    }
}
