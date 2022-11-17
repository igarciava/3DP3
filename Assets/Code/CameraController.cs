using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform LookAtTransform;
    public float MinDistance = 3.0f;
    public float MaxDistance = 15.0f;
    public float YawRotationalSpeed = 720.0f;
    public float PitchRotationalSpeed = 360.0f;

    float Pitch = 0.0f;
    public float MinPitch = -60.0f;
    public float MaxPitch = 20.0f;

    [Header("AvoidObjects")]
    public LayerMask AvoidObjectsLayerMask;
    public float AvoidObjectsOffset= 0.1f;

    [Header("Debug")]
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    bool m_AngleLocked = false;
    bool m_AimLocked = true;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
    }

#if UNITY_EDITOR
    void UpdateInputDebug()
    {
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
        {
            m_AngleLocked = !m_AngleLocked;
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        }
    }
#endif

    private void LateUpdate()
    {
#if UNITY_EDITOR
        UpdateInputDebug();
#endif

        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");
#if UNITY_EDITOR
        if(m_AngleLocked)
        {
            l_MouseX = 0.0f;
            l_MouseY = 0.0f;
        }
#endif
        transform.LookAt(LookAtTransform.position);
        float l_Distance = Vector3.Distance(transform.position, LookAtTransform.position);
        l_Distance = Mathf.Clamp(l_Distance, MinDistance, MaxDistance);
        Vector3 l_EulerAngles = transform.rotation.eulerAngles;
        float l_Yaw = l_EulerAngles.y;

        l_Yaw += l_MouseX * YawRotationalSpeed * Time.deltaTime;
        Pitch += l_MouseY * PitchRotationalSpeed * Time.deltaTime;
        Pitch = Mathf.Clamp(Pitch, MinPitch, MaxPitch);

        Vector3 l_ForwardCamera = new Vector3(Mathf.Sin(l_Yaw * Mathf.Deg2Rad)* Mathf.Cos(Pitch * Mathf.Deg2Rad),
            Mathf.Sin(Pitch * Mathf.Deg2Rad), Mathf.Cos(l_Yaw * Mathf.Deg2Rad) * Mathf.Cos(Pitch * Mathf.Deg2Rad));
        Vector3 l_DesiredPosition = LookAtTransform.position - l_ForwardCamera * l_Distance;

        Ray l_Ray = new Ray(LookAtTransform.position, -l_ForwardCamera);
        RaycastHit l_RaycastHit;
        if(Physics.Raycast(l_Ray, out l_RaycastHit, l_Distance, AvoidObjectsLayerMask.value))
        {
            l_DesiredPosition = l_RaycastHit.point + l_ForwardCamera * AvoidObjectsOffset;
        }

        transform.position = l_DesiredPosition;
        transform.LookAt(LookAtTransform.position);
    }
}
