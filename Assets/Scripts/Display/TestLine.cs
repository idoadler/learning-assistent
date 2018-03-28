using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestLine : MonoBehaviour
{
    public ArabicText desc;
    public ArabicText time;
    public ArabicText daysLeft;
    public ArabicText subjectsLeft;
    private EventsData.TestEvent data;

    public void Init(string title, DateTime date, int subjectsNum, int subjectsStudied, EventsData.TestEvent eventData)
    {
        desc.Text = title;
        time.Text = date.ToString("d.M.yy");
        daysLeft.Text = "עוד "+(date-DateTime.Today).Days+" ימים";
        subjectsLeft.Text = "למדת " + subjectsStudied + " נושאים מתוך " + subjectsNum;
        data = eventData;
    }

    public void OnDrag(BaseEventData eventData)
    { 
        PointerEventData data = (PointerEventData)eventData;
        Debug.Log(data.delta);
    }

    internal void RemoveEvent()
    {
        Destroy(gameObject);
        // HomeScreenManager.RemoveTestTask(data);
    }
}
