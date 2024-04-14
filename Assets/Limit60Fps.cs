using UnityEngine;

public class Limit60Fps : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        double vSyncFactor = new RefreshRate().value / 60.0f;
        QualitySettings.vSyncCount = Mathf.Clamp(Mathf.RoundToInt((float)vSyncFactor), 1, 4);
        //QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
