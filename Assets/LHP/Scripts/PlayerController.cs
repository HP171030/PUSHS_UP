using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private Vector3 moveDir;

    bool moveOn;
    bool grabOn;
    [Header("Player")]
    [SerializeField] float moveDistance;
    [SerializeField] float moveSpeed;

    [Header("Configs")]
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;
    [SerializeField] CinemachineVirtualCamera cam1;
    [SerializeField] CinemachineVirtualCamera cam2;
    [SerializeField] bool OnPlayer1On;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask wall;


    Transform obstacle;
    RaycastHit otherObs;

    RaycastHit grabHit;
    private void FixedUpdate()
    {
        OffPull();
        MoveFunc();

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

    }
    private void OnMove( InputValue value )
    {

        Vector2 input = value.Get<Vector2>();
        moveDir = new Vector3(input.x, 0, input.y);

    }

    public void OnPull( InputValue value )
    {
        if ( value.isPressed )
        {
            grabOn = true;
            Debug.Log("잡기");
            animator.SetTrigger("PullStart");

            if ( Physics.Raycast(transform.position, transform.forward, out grabHit, 1f) )
            {
                if (obstacleLayer.Contain(grabHit.collider.gameObject.layer))
                {
                    animator.SetBool("Pull", true);
                    Debug.Log("잡음");

                }

            }
        }
        else
        {
            grabOn = false;
            Debug.Log("놓기");


        }

    }
    public void OffPull()
    {
        if ( !grabOn && !moveOn )
        {
            animator.SetBool("Pull", false);
        }

    }
    public void MoveFunc()
    {
        if ( !moveOn && !grabOn )
        {
           
            animator.SetFloat("MoveSpeed", moveDir.magnitude);
            if ( Mathf.Abs(moveDir.x) != 0 && Mathf.Abs(moveDir.z) != 0 )
            {

                return;
            }
            else if ( !moveOn && moveDir.magnitude > 0 )
            {
                moveOn = true;

                StartCoroutine(MoveRoutine(moveDir));

                transform.forward = moveDir;

            }

        }
        else if ( !moveOn && grabOn && moveDir.magnitude > 0 )
        {
            if ( grabHit.collider != null )
            {
                Vector3 grabDir = ( grabHit.collider.gameObject.transform.position - transform.position ).normalized;
                if(Physics.Raycast(transform.position,-transform.forward,out RaycastHit hit, 1.5f) )
                {
                    if ( hit.collider != null )
                    {
                        Debug.Log("뒤에 벽잇음 ");
                        moveOn = false;
                        return;
                    }
                }
                moveOn = true;
                if ( grabDir.x > 0.9f && moveDir.x < 0f )
                {

                    bool X = true;
                    StartCoroutine(PullRoutine(grabDir, X));
                    Debug.Log($"{grabDir}떙겨x");
                }
                else if ( grabDir.x < -0.9f && moveDir.x > 0f )
                {
                    bool X = true;
                    StartCoroutine(PullRoutine(grabDir, X));
                    Debug.Log($"{grabDir}떙겨x");
                }
                else if ( grabDir.z > 0.9f && moveDir.z < 0f )
                {
                    bool X = false;
                    StartCoroutine(PullRoutine(grabDir, X));
                    Debug.Log($"{grabDir}떙겨z");
                }
                else if ( grabDir.z < -0.9f && moveDir.z > 0f )
                {
                    bool X = false;
                    StartCoroutine(PullRoutine(grabDir, X));
                    Debug.Log($"{grabDir}떙겨z");
                }
                else
                {
                    Debug.Log("상정외 ");
                    moveOn = false;
                }

            }


        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if ( obstacle != null && otherObs.point != null )
            // Gizmos.DrawLine(obstacle.position, otherObs.point);
            if ( grabHit.collider != null )
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(grabHit.collider.transform.position, new Vector3(0, 1, 0));

            }
    }
    private IEnumerator PullRoutine( Vector3 pullDir, bool X )
    {

        if ( Physics.Raycast(transform.position, -transform.forward, 1f) )
        {
            yield break;
        }
        Vector3 startPos = transform.position;
        Vector3 targetPos = pullDir;
        Vector3 grabStartPos = grabHit.collider.transform.position;
        Vector3 grabTargetPos = pullDir;
        if ( X )
        {
            targetPos = transform.position + new Vector3(-pullDir.x, 0, 0) * moveDistance;
            grabTargetPos = grabHit.collider.transform.position + new Vector3(-pullDir.x, 0, 0) * moveDistance;
        }
        else if ( !X )
        {
            targetPos = transform.position + new Vector3(0, 0, -pullDir.z) * moveDistance;
            grabTargetPos = grabHit.collider.transform.position + new Vector3(0, 0, -pullDir.z) * moveDistance;
        }


        List<RaycastHit> hitArray = new List<RaycastHit>(Physics.RaycastAll(grabHit.collider.transform.position, grabHit.collider.transform.up, 10f));

        hitArray.Add(grabHit);

        foreach ( RaycastHit hit in hitArray )
        {
            hit.collider.gameObject.transform.SetParent(grabHit.collider.transform, true);
            hit.rigidbody.isKinematic = true;
        }

        Debug.Log(hitArray.Count);
        float time = 0;
        float targetTime = 2;
        while ( time < 2 )
        {

            time += Time.deltaTime;
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / targetTime));
            grabHit.rigidbody.MovePosition(Vector3.Lerp(grabStartPos, grabTargetPos, time / targetTime));
            yield return null;


            if ( time >= 2 )
            {
                foreach ( RaycastHit hit in hitArray )
                {
                    hit.collider.gameObject.transform.SetParent(null, true);
                    hit.rigidbody.isKinematic = false;
                }
                moveOn = false;
            }




        }

    }
    private IEnumerator MoveRoutine( Vector3 moveDirValue )
    {
        Vector3 targetPos = transform.position + moveDirValue * moveDistance;
        Vector3 startPos = transform.position;
        RaycastHit hit;
        Vector3 PreMoveDir = moveDir;
        if ( Physics.Raycast(transform.position, moveDirValue, out hit, 1f) )
        {
           
            if (obstacleLayer.Contain(hit.collider.gameObject.layer))
            {

                obstacle = hit.transform;
                Vector3 obsStartPos = obstacle.position;
                Vector3 obsTargetPos = obstacle.position + moveDirValue * moveDistance;

                float time = 0;

                
                if ( Physics.Raycast(obstacle.position, moveDirValue, out otherObs, 1.4f)  )
                {
                   
                     if (obstacleLayer.Contain(otherObs.collider.gameObject.layer) || wall.Contain(otherObs.collider.gameObject.layer))
                     {
                         Debug.Log("방해물이든 벽이든");
                         moveOn = false;
                         if ( moveDir.magnitude < 1 || PreMoveDir != moveDir )
                         {
                             animator.SetBool("Push", false);
                         }

                     }
                    else
                    {
                        List<RaycastHit> pushHitArray = new List<RaycastHit>(Physics.RaycastAll(hit.collider.transform.position, hit.collider.transform.up, 10f));

                        

                        foreach ( RaycastHit hits in pushHitArray )
                        {
                            hits.collider.gameObject.transform.SetParent(hit.collider.transform, true);
                            hits.rigidbody.isKinematic = true;
                        }
                        while ( time < 2 )
                        {
                            animator.SetBool("Push", true);
                            time += Time.deltaTime * moveSpeed;
                            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / 2));
                            hit.rigidbody.MovePosition(Vector3.Lerp(obsStartPos, obsTargetPos, time / 2));
                            yield return null;
                        }
                        moveOn = false;
                        if ( moveDir.magnitude < 1 || PreMoveDir != moveDir )
                        {
                            animator.SetBool("Push", false);
                        }
                        foreach ( RaycastHit hits in pushHitArray )
                        {
                            hits.collider.gameObject.transform.SetParent(null, true);
                            hits.rigidbody.isKinematic = false;
                        }

                        Debug.Log("장애물도 같이 무브");
                    }
                }
                else
                {
                    List<RaycastHit> pushHitArray = new List<RaycastHit>(Physics.RaycastAll(hit.collider.transform.position, hit.collider.transform.up, 10f));

                    

                    foreach ( RaycastHit hits in pushHitArray )
                    {
                        hits.collider.gameObject.transform.SetParent(hit.collider.transform, true);
                        hits.rigidbody.isKinematic = true;
                    }
                    while ( time < 2 )
                    {
                        animator.SetBool("Push", true);
                        time += Time.deltaTime * moveSpeed;
                        rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / 2));
                        hit.rigidbody.MovePosition(Vector3.Lerp(obsStartPos, obsTargetPos, time / 2));
                        yield return null;



                    }
                    moveOn = false;
                    if ( moveDir.magnitude < 1 || PreMoveDir != moveDir )
                    {
                        animator.SetBool("Push", false);
                    }
                    foreach ( RaycastHit hits in pushHitArray )
                    {
                        hits.collider.gameObject.transform.SetParent(null, true);
                        hits.rigidbody.isKinematic = false;
                    }

                    Debug.Log("장애물도 같이 무브");
                }

            }
            else if (wall.Contain(hit.collider.gameObject.layer)) 
            {
                Debug.Log("앞에 벽");
                    moveOn = false;
                    yield return null;
                
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
            yield return null;
            moveOn = false;
            if ( moveDir.magnitude < 1 || PreMoveDir != moveDir )
            {
                animator.SetBool("Push", false);
            }

            Debug.Log("무브");
        }




    }


}
