    using System.Collections;
    using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
using UnityEngine.Windows;

    public class PlayerController : MonoBehaviour
    {
      private Vector3 moveDir;

        bool moveOn;

    [Header("Player")]
        [SerializeField] float moveDistance;
    [SerializeField] float moveSpeed;

    [Header("Configs")]
    [SerializeField] Animator animator;
      [SerializeField] Rigidbody rb;

    Transform obstacle;
    RaycastHit otherObs;

    private void Update()
    {
        MoveFunc();
    }

    private void Start()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();

        }
        private void OnMove(InputValue value)
        {
        
            
                Vector2 input = value.Get<Vector2>();
        moveDir = new Vector3(input.x, 0, input.y);
      
        }
    public void MoveFunc()
    {
        if ( Mathf.Abs(moveDir.x) != 0 && Mathf.Abs(moveDir.z) != 0 )
        {
            Debug.Log("대각");
            return;
        }
        else if ( !moveOn && moveDir.magnitude > 0 )
        {
            moveOn = true;
            Debug.Log("CO");
            StartCoroutine(MoveRoutine(moveDir));
            transform.forward = moveDir;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if(obstacle != null &&otherObs.point!=null)
        Gizmos.DrawLine(obstacle.position, otherObs.point);
    }
    private IEnumerator MoveRoutine(Vector3 moveDir)
        {
           Vector3 targetPos = transform.position + moveDir * moveDistance;
        Vector3 startPos = transform.position;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, moveDir,out hit, 1f) )
        {
            if ( hit.collider.CompareTag("Obstacle") )
            {
                Debug.Log("isObs");
                obstacle = hit.transform;
                Vector3 obsStartPos = obstacle.position;
                Vector3 obsTargetPos = obstacle.position + moveDir * moveDistance;
                Rigidbody obsRb = obstacle.GetComponent<Rigidbody>();
                float time = 0;

                if ( Physics.Raycast(obstacle.position, moveDir, out otherObs, 1f) )
                {
                    if ( otherObs.collider.CompareTag("Obstacle") )
                    {
                    Debug.Log("장애물 뒤에 장애물이 있어용");
                    moveOn = false;
                    }
                    else
                    {
                        while ( time < 1 )
                        {

                            time += Time.deltaTime * moveSpeed;
                            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
                            obsRb.MovePosition(Vector3.Lerp(obsStartPos, obsTargetPos, time));
                            yield return null;
                        }
                        moveOn = false;
                        Debug.Log("장애물도 같이 무브");
                    }
                }
                else
                {
                    while ( time < 1 )
                    {

                        time += Time.deltaTime * moveSpeed;
                        rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
                        obsRb.MovePosition(Vector3.Lerp(obsStartPos, obsTargetPos, time));
                        yield return null;
                    }
                    moveOn = false;
                    Debug.Log("장애물도 같이 무브");
                }

            }




        }
        else
        {
            float time = 0;
            while ( time < 1 )
            {

                time += Time.deltaTime * moveSpeed;
                rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
                yield return null;
            }
            moveOn = false;
            Debug.Log("무브");
        }
               
            
            
     
        }

   
    }
