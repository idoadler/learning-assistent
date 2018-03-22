using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputComponent : MonoBehaviour {
    public InputOption inputPrefab;
    public ChatManager manager;
    private List<InputOption> inputs = new List<InputOption>();

    public void Display(List<string> options)
    {
        foreach (string option in options)
        {
            InputOption input = Instantiate(inputPrefab, gameObject.transform);
            input.Init(this, option);
            inputs.Add(input);
        }
        gameObject.SetActive(true);
    }

    public void ChooseResult(string text)
    {
        //manager.ChooseResult(text);
        foreach (InputOption input in inputs)
        {
            Destroy(input.gameObject);
        }
        inputs.Clear();
        gameObject.SetActive(false);
    }
}
