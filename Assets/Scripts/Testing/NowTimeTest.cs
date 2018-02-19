using System;
using System.Globalization;
using UnityEngine;

[RequireComponent(typeof(ArabicText))]
public class NowTimeTest : MonoBehaviour {
	// Use this for initialization
	void Start () {
        CultureInfo culture = new CultureInfo("he-IL");
        ArabicText label = GetComponent<ArabicText>();
        //label.Text = DateTime.Now.ToString(culture.DateTimeFormat.LongDatePattern, culture);
    }
}
