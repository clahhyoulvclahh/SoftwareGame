using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("跟随参数")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("边界限制")]
    public bool useBounds = false;
    public float minX = 0.2f;
    public float maxX = 0.5f;
    public float minY = -7.99f;
    public float maxY = -3f;

    private Vector3 velocity = Vector3.zero;
    
    void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        FollowTarget();
    }

    void FollowTarget()
    {
        // 计算目标位置
        Vector3 targetPosition = target.position + offset;

        // 应用边界限制
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // 平滑移动
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
        transform.position = smoothedPosition;
    }

    // 设置摄像机边界（可在运行时调用）
    public void SetCameraBounds(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
        useBounds = true;
    }

   
}