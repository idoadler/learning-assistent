using System;
using UnityEngine;

public class randomQuotes : MonoBehaviour {

    public quote[] quotes;
    public ArabicText text;
    public ArabicText quotee;

	// Use this for initialization
	void Start () {
        int r = UnityEngine.Random.Range(0, quotes.Length);
        text.Text = quotes[r].text;
        quotee.Text = quotes[r].quotee;
	}

    [Serializable]
    public struct quote
    {
        public string text;
        public string quotee;
    }
}
