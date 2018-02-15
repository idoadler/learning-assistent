using UnityEngine;

public class HomeScreenManager : MonoBehaviour {

    public GameObject missionLinePrefab;
    public GameObject dailyMissions;
    public GameObject subMenu;

    private void OnEnable()
    {
        subMenu.SetActive(false);
    }

    public void AddDailyMission()
    {
        ChatLine chatLine = Instantiate(missionLinePrefab, dailyMissions.transform).GetComponent<ChatLine>();
    }

}
