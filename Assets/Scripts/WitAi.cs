using System.Collections;
using UnityEngine;

public class WitAi : MonoBehaviour {
    public static WitAi Instance;
    public static event RequestResult request_success;
    public static event RequestResult request_failure;
    public delegate void RequestResult(string result);

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        } else if (Instance != this)
        {
            Destroy(this);
        }
    }

    public void Say(string text)
    {
        string url = "https://api.wit.ai/message?q="+WWW.EscapeURL(text)+"&access_token=OLOZJWSUHECG3KKUQQZQ45DJ2WJ6L36L";
        WWW www = new WWW(url);
        StartCoroutine(WaitForRequest(www));
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            if (request_success != null)
            {
                request_success(www.text);
            }
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
            if (request_failure != null)
            {
                request_failure(www.error);
            }
        }
    }

}
