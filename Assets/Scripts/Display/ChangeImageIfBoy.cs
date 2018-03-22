using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChangeImageIfBoy : MonoBehaviour {
    public Sprite boyImage;

    // Use this for initialization
    private void OnEnable()
    {
        if (!JsonManager.IsBotFemale)
        {
            GetComponent<Image>().sprite = boyImage;
        }
    }
}
