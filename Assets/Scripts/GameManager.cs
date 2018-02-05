using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public RectTransform content;
    public ScrollRect chatBack;
    public InputField input;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
       //         content.offsetMin = new Vector2(content.offsetMin.x, GetKeyboardSize());// TouchScreenKeyboard.area.height);
        /*if (TouchScreenKeyboard.visible)
        {
            if (GetKeyboardSize() > 0)
            {
                input.text = "" + GetKeyboardSize(); // TouchScreenKeyboard.area.height + "," + TouchScreenKeyboard.area.max.y + "," + TouchScreenKeyboard.area.size.y + "," + TouchScreenKeyboard.area.yMax + "," + TouchScreenKeyboard.area.y;
            }
        } else
        {
            content.offsetMin = new Vector2(content.offsetMin.x, 0);
        }*/
    }

    public void ChatToEnd()
    {
        chatBack.normalizedPosition = Vector2.zero;
        
    }

    public int GetKeyboardSize()
    {
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");

            using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
            {
                View.Call("getWindowVisibleDisplayFrame", Rct);

                return Screen.height - Rct.Call<int>("height");
            }
        }
    }
}
