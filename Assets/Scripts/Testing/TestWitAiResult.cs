using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

[RequireComponent(typeof(WitAi))]
[RequireComponent(typeof(Text))]
public class TestWitAiResult : MonoBehaviour {

    private Text Label;

	// Use this for initialization
	void Start () {
        Label = GetComponent<Text>();
        WitAi.request_success += SetLabel;
        GetComponent<WitAi>().Say("הי");
	}
	
    private void SetLabel(string text)
    {
        JSONNode entities = JSON.Parse(text)["entities"];
        string key = "";
        string value = "";

        if (entities == null)
        {
            // wrong communication
            Label.text = "no entities";
        }
        else if (entities.Count == 0)
        {
            // normal no result
            Label.text = "no keys";
        }
        else if (entities.Count > 1)
        {
            // API change
            Label.text = "too many keys";
        }
        else
        {
            // normal result
            foreach (string k in entities.Keys)
            {
                key = k;
            }

            value = entities[key][0]["value"];
            Label.text = key + ": " + value;
        }
    }
}
