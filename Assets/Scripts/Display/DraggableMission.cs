using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableMission : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int requiredDrag = 200;
    public MissionLine parent;
    public RectTransform data;
    private ScrollRect scroll;
    private bool vertical;

    private void Start()
    {
        scroll = FindInParents<ScrollRect>(gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        vertical = Mathf.Abs(eventData.delta.x) < Mathf.Abs(eventData.delta.y);
        if (vertical)
        {
            scroll.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (vertical)
        {
            scroll.OnDrag(eventData);
        }
        else
        {
            if (data.position.x + eventData.delta.x > 0)
            {
                data.position = new Vector2(data.position.x + eventData.delta.x, data.position.y);
            }
            else
            {
                data.position = new Vector2(0, data.position.y);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (vertical)
        {
            scroll.OnEndDrag(eventData);
        }
        else
        {
            if (data.position.x < requiredDrag)
            {
                data.position = new Vector2(0, data.position.y);
            }
            else
            {
                parent.RemoveEvent();
            }
        }
	}

    // locate and return the Canvas
    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
}
