using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // There should really be assert-conditions for these components being undefined.
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] float speed = 1.0f;
    [Header("This is useless for now. Gravity is handled by the global physics engine. Maybe a TODO?")]
    [SerializeField] float gravity = 9.8f;

    [SerializeField] Camera playerCamera;
    [SerializeField] float xSensitivity = 1.0f, ySensitivity = 1.0f;
    [SerializeField] float mouseClampMin = -90.0f;
    [SerializeField] float mouseClampMax = 90.0f;

    [SerializeField] private float xRotation = 0.0f;

    [SerializeField] Vector3 desiredMovement;

    void Update()
    {
        Vector3 movementDirection = rigidbody.transform.forward * Input.GetAxisRaw("Vertical") + rigidbody.transform.right * Input.GetAxisRaw("Horizontal");
        rigidbody.AddForce(movementDirection * speed * Time.deltaTime);
        //Debug.Log("Added force of: " +  movementDirection * speed * Time.deltaTime);

        desiredMovement = movementDirection * speed * Time.deltaTime;

        // --- mouse movement ---

        float yRotation = Input.GetAxisRaw("Mouse X") * ySensitivity;
        xRotation -= Input.GetAxisRaw("Mouse Y") * xSensitivity;
        xRotation = Mathf.Clamp(xRotation, mouseClampMin, mouseClampMax);

        // rotates the player
        rigidbody.transform.Rotate(Vector3.up, yRotation);
        
        // rotates the camera
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
