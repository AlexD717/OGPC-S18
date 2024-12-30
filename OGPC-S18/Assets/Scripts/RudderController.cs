using UnityEngine;
using UnityEngine.InputSystem;

public class RudderController : MonoBehaviour
{
    private float rudderMoveSpeed;
    private float rudderPosition;
    
    [SerializeField] private InputActionAsset inputActions;
    private InputAction rotationAxis;
    private float rotationInput;

    [SerializeField] private RectTransform rudderPositionIndicator;
    
    private void OnEnable()
    {
        // Get the "Move" action from the Input Action Asset
        var playerControls = inputActions.FindActionMap("Player");
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
        rotationInput = rotationAxis.ReadValue<float>();

        UpdateUI();
    }

    private void UpdateUI()
    {
        rudderPosition = Mathf.Lerp(rudderPositionIndicator.localPosition.x, rotationInput * 200, rudderMoveSpeed/10f * Time.deltaTime);
        rudderPositionIndicator.localPosition = new Vector2(rudderPosition, rudderPositionIndicator.localPosition.y);
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
