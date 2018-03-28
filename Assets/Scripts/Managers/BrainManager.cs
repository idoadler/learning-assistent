using System;
using System.Collections;
using UnityEngine;

public class BrainManager : MonoBehaviour {

    public TextAsset brainAsset;
    private string brainData;
    public int localBrainVer;
    private int remoteBrainVer;
    public string brainURL;
    public ConversationManager conversation;
    public ChatManager Chat;
    private void Awake()
    {
        if (remoteBrainVer > localBrainVer)
        {
            StartCoroutine(GetBrainFile());
            // TODO: Store new brain ver and data
        }
    }

    public int RemoteBrainVer
    {
        get
        {
            return remoteBrainVer;
        }

        set
        {
            remoteBrainVer = value;
            if (remoteBrainVer > localBrainVer)
            {
                StartCoroutine(GetBrainFile());
                // TODO: Store new brain ver and data
            }
        }
    }
    
    public string BrainData
    {
        get
        {
            if (string.IsNullOrEmpty(brainData))
                return brainAsset.text;
            else
                return brainData;
        }

        set
        {
            brainData = value;
        }
    }

    public void InitConversationBrain()
    {
        JsonManager.InitConversationJson(BrainData);
    }

    public string [] GetExtra()
    {
        return JsonManager.EXTRA;
    }

    IEnumerator GetBrainFile()
    {
        yield return new WaitForEndOfFrame();
        if (string.IsNullOrEmpty(brainURL))
        {
            Debug.LogError("You have no brain!");
        }
        else
        {
            using (WWW www = new WWW(brainURL))
            {
                yield return www;
                BrainData = www.text;
                Debug.Log(brainURL + ":\n" + BrainData);
                InitConversationBrain();
                localBrainVer = remoteBrainVer;
            }
        }
    }
}
