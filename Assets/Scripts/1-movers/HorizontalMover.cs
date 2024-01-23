using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HorizontalMover : MonoBehaviour
{
    public float screenEdge = 10f;
    [Tooltip("Speed of movement, in meters per second")]
    [SerializeField] float speed = 10f;

    [SerializeField] InputAction moveHorizontal = new InputAction(type: InputActionType.Button);
    [SerializeField] InputAction moveVertical = new InputAction(type: InputActionType.Button);


    void OnEnable()
    {
        moveHorizontal.Enable();
        moveVertical.Enable();
    }

    void OnDisable()
    {
        moveHorizontal.Disable();
        moveVertical.Disable();
    }

    void Update()
    {
        float horizontal = moveHorizontal.ReadValue<float>();
        float vertical = moveVertical.ReadValue<float>();
        Vector3 movementVector = new Vector3(horizontal, vertical, 0) * speed * Time.deltaTime;
        transform.position += movementVector;

        // Get the current position
        Vector3 currentPosition = transform.position;

        // Check if the object is beyond the screen edge
        if (Mathf.Abs(currentPosition.x) > screenEdge)
        {
            // Move the object to the other side
            currentPosition.x = -currentPosition.x;
            transform.position = currentPosition;
        }
    }

    public void IncreaseSpeed(float factor)
    {
        speed *= factor;
       
    }

    public void ResetSpeed()
    {
        speed = 10f; 
    }
}
