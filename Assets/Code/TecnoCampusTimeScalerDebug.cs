using UnityEngine;

public class TecnoCampusTimeScalerDebug : MonoBehaviour
{
    public KeyCode FastKeyCode = KeyCode.RightControl;
    public KeyCode SlowKeyCode = KeyCode.LeftControl;
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(FastKeyCode))
            Time.timeScale = 2.0f;
        if (Input.GetKeyDown(SlowKeyCode))
            Time.timeScale = 0.5f;
        if (Input.GetKeyUp(FastKeyCode))
            Time.timeScale = 1.0f;
        if (Input.GetKeyUp(SlowKeyCode))
            Time.timeScale = 1.0f;
    }
#endif
}
