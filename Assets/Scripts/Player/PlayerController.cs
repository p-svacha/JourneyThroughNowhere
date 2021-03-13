using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float Gravity = -9.81f;

    public Transform Player;
    public Transform Camera;
    public Transform GroundCheck;
    public CharacterController Controller;

    public float MouseSensitivity = 300f;
    public float MovementSpeed = 12f;
    public float JumpHeight = 3f;

    public float GroundDistance = 0.1f;
    public LayerMask GroudMask;
    public bool IsOnGround;

    public LayerMask TrainMask;
    public bool IsOnTrain;
    public Wagon CurrentWagon;

    public bool IsGrounded;

    public Vector3 Velocity;
    public float RotationX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Player position (on ground / on train)
        bool wasOnGround = IsOnGround;

        IsOnGround = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroudMask);
        IsOnTrain = Physics.CheckSphere(GroundCheck.position, GroundDistance, TrainMask);
        Collider[] trainCol = Physics.OverlapSphere(GroundCheck.position, GroundDistance, TrainMask);

        if (CurrentWagon != null && !wasOnGround && IsOnGround)
        {
            CurrentWagon = null;
        }
        
        foreach (Collider c in trainCol)
        {
            if (c.gameObject.GetComponent<WagonFloor>() != null)
            {
                Wagon collisionWagon = c.gameObject.GetComponent<WagonFloor>().Wagon;
                if (collisionWagon != CurrentWagon && collisionWagon != null)
                {
                    CurrentWagon = collisionWagon;
                }
            }
        }

        IsGrounded = IsOnGround || IsOnTrain;

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
        bool isMoving = movementX != 0f || movementZ != 0f;
        if(isMoving)
        {
            Vector3 move = Player.transform.right * movementX + Player.transform.forward * movementZ;
            Controller.Move(move * MovementSpeed * Time.deltaTime);
        }

        if(CurrentWagon != null) Controller.Move(CurrentWagon.FrameMoveVector);

        // Train collision (when outside of train)
        TrainPart colPart = null;
        Collider[] playerCollision = Physics.OverlapBox(transform.position, new Vector3(1f, 1.9f, 1f));
        foreach (Collider c in playerCollision)
        {
            if (c.gameObject.layer == 9 && c.GetComponent<TrainPart>() != null)
            {
                colPart = c.GetComponent<TrainPart>();
                break;
            }
        }
        if (colPart != null && CurrentWagon == null)
        {
            Controller.Move(colPart.Wagon.FrameMoveVector);
        }

        // Gravity & Jumping
        if (!IsGrounded) Velocity.y += Gravity * Time.deltaTime;
        else Velocity.y = 0f;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            Velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        // Train control
        if(Input.GetKeyDown(KeyCode.UpArrow) && CurrentWagon != null)
        {
            CurrentWagon.Train.VelocityKph += 5;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && CurrentWagon != null)
        {
            CurrentWagon.Train.VelocityKph -= 5;
        }

        transform.position += (Velocity * Time.deltaTime);

        
       
        
    }
}
