using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public ScrollRect s;
    //public RectTransform chatScroll;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChatToEnd()
    {
        s.normalizedPosition = Vector2.zero;
        //chatScroll.localPosition =  new Vector2(chatScroll.localPosition.x, chatScroll.sizeDelta.y);
    }
}
