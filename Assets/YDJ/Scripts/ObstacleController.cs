using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵� �ӵ�

    private Rigidbody rb;




    void Start()
    {



        rb = GetComponent<Rigidbody>();


    }

    void Update()
    {
        // �Է� ó��
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // �÷��̾� �̵�
        Vector3 move = transform.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);


    }


}
