using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VersionLabel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = Application.version;
	}
}
