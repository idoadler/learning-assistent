using UnityEngine;

public class MissionLine : MonoBehaviour {
    public ArabicText time;
    public ArabicText desc;

    public void Finished()
    {
        Destroy(gameObject);
    }

    }
