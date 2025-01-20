using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform followTransform;
    [SerializeField] private float smoothingSpeed = 0.5f;
    [SerializeField] private float deadZone = 0.1f;
    private Vector3 velocity = Vector3.zero;

    public void InitCamera(Transform player_transform)
    {
        followTransform = player_transform;
    }

    void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(followTransform.position.x, followTransform.position.y, this.transform.position.z);

        // Only move the camera if the player has moved significantly (beyond the dead zone)
        if (Vector3.Distance(this.transform.position, targetPosition) > deadZone)
        {
            this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPosition, ref velocity, smoothingSpeed * Time.deltaTime);
        }
    }


}