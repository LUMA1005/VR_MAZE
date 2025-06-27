using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;

    private CharacterController controller;
    public Transform cameraTransform; // 摄像机引用

    private float xRotation = 0f;
    private float gravity = -9.81f;
    private Vector3 velocity;

    void Start() {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        MovePlayer();
        RotateView();
    }

    void MovePlayer() {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 应用重力
        if (controller.isGrounded && velocity.y < 0) {
            velocity.y = -2f; // 贴地防止持续累加
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void RotateView() {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        // 左右旋转 Player（整体绕Y轴）
        transform.Rotate(Vector3.up * mouseX);

        // 上下旋转摄像机（绕X轴）
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
