using System.Collections;
using UnityEngine;

public class BrainManager : MonoBehaviour {

    public TextAsset brainAsset;
    private string brainData;
    public int localBrainVer;
    private int remoteBrainVer;
    public string brainURL;

    private void Awake()
    {
        if (remoteBrainVer > localBrainVer)
        {
            GetBrainFile();
            // TODO: Store new brain ver
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
                GetBrainFile();
                // TODO: Store new brain ver
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

    public void UpdateBrain()
    {

        JsonManager.InitConversationJson(BrainData);
    }

    IEnumerator GetBrainFile()
    {
        using (WWW www = new WWW(brainURL))
        {
            yield return www;
            BrainData = www.text;
            Debug.Log(BrainData);
            UpdateBrain();
        }
    }
}
