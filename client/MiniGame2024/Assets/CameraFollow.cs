using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform m_Target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void OnEnable()
    {
        CarController.OnLocalPlayerSpawned += SetTarget;
    }

    private void OnDisable()
    {
        CarController.OnLocalPlayerSpawned -= SetTarget;
    }

    private void SetTarget(GameObject target)
    {
        m_Target = target.transform;
    }


    void LateUpdate()
    {
        if (m_Target == null)
        {
            return;
        }
        // 计算目标位置
        Vector3 desiredPosition = m_Target.position + offset;
        // 平滑移动到目标位置
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // 更新摄像机位置
        transform.position = smoothedPosition;
    }
}
