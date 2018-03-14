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
        DateTime date = DateTime.Today.AddDays(shiftFromToday);
        if (shiftFromToday == 0)
        {
            label.Text = "היום";
        } else
        {
            label.Text = DateFormat(date);
        }
    }

    public static string DateFormat(DateTime date)
    {
        if (date.Date == DateTime.Today)
        {
            return date.ToString("היום  |  d.M", HebCulture);
        }
        else
        {
            return date.ToString("dddd  |  d.M", HebCulture);
        }
    }

    //public static DateTime Parse(string date, string hour)
    //{
    //    return DateTime.ParseExact(date + " " + hour, "dddd  |  d.M", HebCulture);
    //}
}
