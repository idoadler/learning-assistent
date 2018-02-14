using UnityEngine;

public class HomeScreenManager : MonoBehaviour {

    public GameObject missionLinePrefab;
    public GameObject dailyMissions;

    public void AddDailyMission()
    {
        ChatLine chatLine = Instantiate(missionLinePrefab, dailyMissions.transform).GetComponent<ChatLine>();
    }

}
