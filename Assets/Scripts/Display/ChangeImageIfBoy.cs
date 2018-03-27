using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChangeImageIfBoy : MonoBehaviour {
    public Sprite boyImage;

    private void Start()
    {
        if (!JsonManager.IsBotFemale)
        {
            GetComponent<Image>().sprite = boyImage;
        }
    }

    // Use this for initialization
    private void OnEnable()
    {
        if (!JsonManager.IsBotFemale)
        {
            GetComponent<Image>().sprite = boyImage;
        }
    }
}
