using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _minPitch = -40f;  // Min pitch angle (up/down)
    [SerializeField] private float _maxPitch = 85f;   // Max pitch angle (up/down)
    [SerializeField] private float _minYaw = -360f;   // Min yaw angle (left/right)
    [SerializeField] private float _maxYaw = 360f;    // Max yaw angle (left/right)

    [SerializeField] private GameObject _player;      // The player object to track yaw

    private float _mouseX;
    private float _mouseY;

    private float _currentPitch = 0;
    private float _currentYaw = 0;
    private float _calculatedYaw;

    [Header("Controller Sensitivity")]
    [SerializeField] private float controllerSensitivity = 1f;

    void Update()
    {
        AdjustCameraOrientation();
    }

    private void AdjustCameraOrientation()
    {
        // Mouse Input for Camera Orientation
        _mouseX = Input.GetAxis("Mouse X");
        _mouseY = Input.GetAxis("Mouse Y");

        // Adjust pitch and yaw based on mouse input
        _currentPitch = Mathf.Clamp(_currentPitch - _mouseY, _minPitch, _maxPitch);
        _currentYaw = Mathf.Clamp(_currentYaw + _mouseX, _minYaw, _maxYaw);

        // Controller Input for Camera Orientation (Right Stick)
        float controllerInputX = Input.GetAxis("RightStickHorizontal");  // Right Stick X (Yaw)
        float controllerInputY = Input.GetAxis("RightStickVertical");    // Right Stick Y (Pitch)

        // Adjust sensitivity for controller input (scale it up or down)
        _currentPitch = Mathf.Clamp(_currentPitch - controllerInputY * controllerSensitivity, _minPitch, _maxPitch);
        _currentYaw = Mathf.Clamp(_currentYaw + controllerInputX * controllerSensitivity, _minYaw, _maxYaw);

        // Combine player's rotation with calculated yaw to orient the camera relative to the player
        _calculatedYaw = _player.transform.eulerAngles.y + _currentYaw;

        // Apply the final rotation to the camera
        this.transform.eulerAngles = new Vector3(_currentPitch, _calculatedYaw, 0f);
    }
}
