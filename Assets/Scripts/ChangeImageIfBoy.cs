using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChangeImageIfBoy : MonoBehaviour {
    public Sprite boyImage;

    // Use this for initialization
    private void OnEnable()
    {
        if (!ChatManager.IsAssistentGirl)
        {
            GetComponent<Image>().sprite = boyImage;
        }
    }
}
