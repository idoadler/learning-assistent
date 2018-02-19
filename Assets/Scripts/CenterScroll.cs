using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class CenterScroll : MonoBehaviour {
    private ScrollRect scroll;
    public int sizeJump = 80;
    public ScrollRect brotherScroll;
    public bool alwaysMore;

    // Use this for initialization
    void Start () {
        scroll = GetComponent<ScrollRect>();
	}

    public void FixScrollPos()
    {
        scroll.content.localPosition = new Vector2(scroll.content.localPosition.x, Mathf.Round(scroll.content.localPosition.y / sizeJump) * sizeJump);
        if (brotherScroll != null)
        {
            if ((alwaysMore && scroll.content.localPosition.y < brotherScroll.content.localPosition.y)
                || (!alwaysMore && scroll.content.localPosition.y > brotherScroll.content.localPosition.y))
            {
                brotherScroll.content.localPosition = new Vector2(brotherScroll.content.localPosition.x, scroll.content.localPosition.y);
            }
        }
    }
}
