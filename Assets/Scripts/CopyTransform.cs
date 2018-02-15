using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CopyTransform : MonoBehaviour {
    public RectTransform target;
    public Vector2 sizeModifier = Vector2.zero;
    public Vector3 positionModifier = Vector2.zero;

    // Use this for initialization
    void Start () {
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = target.sizeDelta + sizeModifier;
        rect.position = target.position + positionModifier;
    }
}
