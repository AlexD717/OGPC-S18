using UnityEngine;
using UnityEngine.InputSystem;

public class RudderController : MonoBehaviour
{
    private float rudderMoveSpeed;
    private float rudderPosition;
    
    [SerializeField] private InputActionAsset inputActions;
    private InputAction rotationAxis;
    private float rotationInput;    
    private void OnEnable()
    {
        // Get the "Rotation" action from the Input Action Asset
        InputActionMap playerControls = inputActions.FindActionMap("Player");
        rotationAxis = playerControls.FindAction("Rotation");

        // Enable the action
        rotationAxis.Enable();
    }

    private void OnDisable()
    {
        // Disable the action when no longer needed
        rotationAxis.Disable();
    }

    private void Update()
    {
        // Reads rotation value
        UpdateRudder();
    }

    private void UpdateRudder()
    {
        rotationInput = rotationAxis.ReadValue<float>();
        rudderPosition = Mathf.Lerp(rudderPosition, rotationInput * 200, rudderMoveSpeed/10f * Time.deltaTime);
    }

    public float GetRudderPosition()
    {
        return -(rudderPosition / 200f);
    }

    public void SetRudderMoveSpeed(float _rudderMoveSpeed)
    {
        rudderMoveSpeed = _rudderMoveSpeed;
    }
}
