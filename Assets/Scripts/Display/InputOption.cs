using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputOption : MonoBehaviour {
    public ArabicText label;
    private InputComponent manager;

    public void Init(InputComponent parent, string text)
    {
        manager = parent;
        label.Text = text;
    }

    public void ChooseResult()
    {
        manager.ChooseResult(label.Text);
    }
}
