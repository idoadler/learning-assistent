using UnityEngine;
using UnityEngine.EventSystems;

public class MissionLine : MonoBehaviour
{
    public ArabicText time;
    public ArabicText desc;

    public void Finished()
    {
        Destroy(gameObject);
    }

    public void OnDrag(BaseEventData eventData)
    {
        PointerEventData data = (PointerEventData)eventData;
        Debug.Log(data.delta);
    }
}
