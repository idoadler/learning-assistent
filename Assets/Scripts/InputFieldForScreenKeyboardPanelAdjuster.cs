using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class InputFieldForScreenKeyboardPanelAdjuster : MonoBehaviour
{
    // Assign panel here in order to adjust its height when TouchScreenKeyboard is shown
    public RectTransform panelRectTrans;
    public ScrollRect chatScrollRect;

    private InputField inputField;
    private Vector2 panelOffsetMinOriginal;
    private float panelHeightOriginal;
    private float currentKeyboardHeightRatio;

    public void Start()
    {
        TouchScreenKeyboard.hideInput = true;
        inputField = transform.GetComponent<InputField>();
        panelOffsetMinOriginal = panelRectTrans.offsetMin;
        panelHeightOriginal = panelRectTrans.rect.height;
    }

    public void LateUpdate()
    {
        // TODO: happen on focus/unfocus event
        // TODO: hide on disable
        TouchScreenKeyboard.hideInput = true;
        if (inputField.isFocused)
        {
            float newKeyboardHeightRatio = GetKeyboardHeightRatio();
            if (currentKeyboardHeightRatio != newKeyboardHeightRatio)
            {
                currentKeyboardHeightRatio = newKeyboardHeightRatio;
                panelRectTrans.offsetMin = new Vector2(panelOffsetMinOriginal.x, panelHeightOriginal * currentKeyboardHeightRatio);
                StartCoroutine(DelayedExecute(() => {
                    chatScrollRect.verticalNormalizedPosition = 0;
                }));
            }
        }
        else if (currentKeyboardHeightRatio != 0f)
        {
            if (panelRectTrans.offsetMin != panelOffsetMinOriginal)
            {
                StartCoroutine( DelayedExecute(() => {
                    panelRectTrans.offsetMin = panelOffsetMinOriginal;
                }));
            }
            currentKeyboardHeightRatio = 0f;
        }
    }

    private IEnumerator DelayedExecute(Action f)
    {
        yield return new WaitForEndOfFrame();
        f();
    }

    private float GetKeyboardHeightRatio()
    {
        if (Application.isEditor)
        {
            return 0.4f; // fake TouchScreenKeyboard height ratio for debug in editor        
        }

#if UNITY_ANDROID        
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
            using (AndroidJavaObject rect = new AndroidJavaObject("android.graphics.Rect"))
            {
                View.Call("getWindowVisibleDisplayFrame", rect);
                return (float)(Screen.height - rect.Call<int>("height")) / Screen.height;
            }
        }
#else
        return (float)TouchScreenKeyboard.area.height / Screen.height;
#endif
    }

}
