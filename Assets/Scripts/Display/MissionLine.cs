using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MissionLine : MonoBehaviour
{
    public ArabicText time;
    public ArabicText desc;
    private EventsData.HomeworkEvent data;

    public void Init(string title, DateTime from, DateTime to, EventsData.HomeworkEvent eventData)
    {
        desc.Text = title;
        time.Text = from.ToString("HH:mm") + "-" + to.ToString("HH:mm");
        data = eventData;
    }
    
    public void OnDrag(BaseEventData eventData)
    {
        PointerEventData data = (PointerEventData)eventData;
        Debug.Log(data.delta);
    }

    internal void RemoveEvent()
    {
        HomeScreenManager.RemoveHomeworkTask(data);
        Destroy(gameObject);
    }
}
