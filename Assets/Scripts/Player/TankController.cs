using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.5f, atkSpeed = 0.5f;
    private float countDownAtk;
    [SerializeField] private float rotateSpeed = 80f;
    [SerializeField] private Rigidbody2D bullet;

    public Rigidbody2D BulletPrefab => bullet;

    private float moveInput;
    private float rotateInput;

    private Rigidbody2D rg;
    void Start()
    {
        rg = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ReadInput();
        Attack();
    }

    void FixedUpdate()
    {
        Move();
        Rotate();

    }

    private void Attack()
    {
        countDownAtk -= Time.deltaTime;
        if (countDownAtk > 0)
            return;
        if (Keyboard.current.spaceKey.isPressed)
        {
            if (bullet != null)
                Instantiate<Rigidbody2D>(bullet, this.transform.position, this.transform.rotation);

            countDownAtk = atkSpeed;
        }
    }

    private void Move()
    {
        // transform.up lấy vector hướng "tiến tới" (phía trên của gameObject)
        rg.linearVelocity = this.transform.up * (moveSpeed * moveInput);
    }

    // private void Rotate()
    // {
    //     Quaternion rotation = this.transform.rotation;
    //     rotation.eulerAngles = new Vector3(0,0, rotation.eulerAngles.z + rotateSpeed * rotateInput* Time.deltaTime);

    //     this.transform.rotation = rotation;
    // }
    private void Rotate()
    {
        float rotationAmount = rotateInput * rotateSpeed * Time.fixedDeltaTime;

        rg.MoveRotation(rg.rotation + rotationAmount);
    }

    private void ReadInput()
    {
        moveInput = 0f;
        rotateInput = 0f;
        if (Keyboard.current.wKey.isPressed)
        {
            moveInput = 1f;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            moveInput = -1;
        }

        if (Keyboard.current.leftArrowKey.isPressed)
        {
            rotateInput = 1;
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            rotateInput = -1;
        }
    }


}
