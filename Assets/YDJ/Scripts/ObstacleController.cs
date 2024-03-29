using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도

    private Rigidbody rb;




    void Start()
    {



        rb = GetComponent<Rigidbody>();


    }

    void Update()
    {
        // 입력 처리
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // 플레이어 이동
        Vector3 move = transform.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);


    }


}
