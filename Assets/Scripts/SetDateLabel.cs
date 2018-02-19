using System;
using System.Globalization;
using UnityEngine;

[RequireComponent(typeof(ArabicText))]
public class SetDateLabel : MonoBehaviour {
    private static CultureInfo HebCulture = new CultureInfo("he-IL");

    public int shiftFromToday;
    private ArabicText label;

	// Use this for initialization
	void Start ()
    {
        label = GetComponent<ArabicText>();
        if (shiftFromToday == 0)
        {
            label.Text = "היום";
        } else
        {
            DateTime date = DateTime.Today.AddDays(shiftFromToday);
            label.Text = date.ToString("dddd  |  d.M", HebCulture);
        }
    }
}
