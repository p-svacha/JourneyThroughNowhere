using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float Gravity = -9.81f;

    public Transform Player;
    public CharacterController Controller;
    public Transform Camera;
    public Transform GroundCheck;

    public float MouseSensitivity = 300f;
    public float MovementSpeed = 12f;
    public float JumpHeight = 3f;

    public float GroundDistance = 0.2f;
    public LayerMask GroudMask;
    public bool IsGrounded;

    public Vector3 Velocity;
    public float RotationX = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroudMask);
        if (IsGrounded && Velocity.y < 0) Velocity.y = -2f;

        // Looking
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        RotationX -= mouseY;
        RotationX = Mathf.Clamp(RotationX, -90f, 90f);

        Camera.localRotation = Quaternion.Euler(RotationX, 0f, 0f);
        Player.Rotate(Vector3.up * mouseX);

        // Movement
        float movementX = Input.GetAxis("Horizontal");
        float movementZ = Input.GetAxis("Vertical");
        Vector3 move = Player.transform.right * movementX + Player.transform.forward * movementZ;
        Controller.Move(move * MovementSpeed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && IsGrounded)
        {
            Velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        Velocity.y += Gravity * Time.deltaTime;

        Controller.Move(Velocity * Time.deltaTime);
    }
}
