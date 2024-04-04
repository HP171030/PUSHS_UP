using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player2Controller : MonoBehaviour
{
    public Vector3 moveDir;

    bool moveOn;
    [Header("Player")]
    [SerializeField] float moveDistance;
    [SerializeField] float moveSpeed;

    [Header("Configs")]
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask obstacleUpperLayer;
    [SerializeField] LayerMask wall;
    [SerializeField] public bool onGround;
    [SerializeField] public bool onClimb;
    [SerializeField] public bool ontheBox;
    [SerializeField] public bool downingBox;
    [SerializeField] public bool onIce = false;

    [SerializeField] CameraSwitch cameraSwitch;


    RaycastHit BoomableHit;


    private void FixedUpdate()
    {
        if(onGround&&!downingBox)
            MoveFunc();
        
    }
    private void OnTriggerStay( Collider other )
    {
        if ( obstacleUpperLayer.Contain(other.gameObject.layer) )
        {
           
            ontheBox = true;
        }
    }
    public IEnumerator DownAnim()
    {
        downingBox = true;
        animator.SetFloat("MoveSpeed", 0f);
        animator.Play("Jumping Down");
        yield return new WaitForSeconds(0.8f);
        downingBox = false;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

    }
    private void OnMove(InputValue value)
    {

        if (!cameraSwitch.IsPlayer1Active && !onIce )
        {
            Vector2 input = value.Get<Vector2>();
            moveDir = new Vector3(input.x, 0, input.y);
        }

    }

    public void OnBoom( InputValue value )
    {
        if ( value.isPressed&&!cameraSwitch.IsPlayer1Active&&Manager.game.boomAction > 0)
        {
            
           
           // animator.SetTrigger("Boom");

            if ( Physics.Raycast(transform.position, transform.forward, out BoomableHit, 1f) )
            {
                if ( obstacleLayer.Contain(BoomableHit.collider.gameObject.layer))
                {
                    Destroy(BoomableHit.collider.gameObject );
                    Manager.game.boomAction--;

                }
            }
        }
    }
    public void MoveFunc()
    {
        if ( !moveOn && !onIce )
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
    }


    private IEnumerator MoveRoutine( Vector3 moveDirValue )
    {
        Vector3 targetPos = transform.position + moveDirValue * moveDistance;
        Vector3 startPos = transform.position;
        RaycastHit hit;
        Vector3 PreMoveDir = moveDir;
        LayerMask layer = obstacleLayer | wall;
        if ( Physics.BoxCast(transform.position+new Vector3(0,0.5f,0),new Vector3(0.1f,0.1f,0.1f), moveDirValue, out hit, Quaternion.identity,1f,layer) )
        {
            if ( !obstacleLayer.Contain(hit.collider.gameObject.layer) )
            {
                Debug.Log(hit.collider.gameObject.name);
                moveOn = false;
                yield break;
            }
            else
            {
                RaycastHit [] raycastHits = Physics.RaycastAll(hit.collider.transform.position, hit.collider.transform.up, 3f);


                foreach ( RaycastHit other in raycastHits )
                {
                    if ( other.collider.gameObject != hit.collider.gameObject && !obstacleUpperLayer.Contain(other.collider.gameObject.layer) )
                    {
                        Debug.Log($"위에 장애물이 있습니다: {other.collider.gameObject.name}");
                        moveOn = false;
                        yield break;
                    }

                }

                float time = 0;

                Vector3 ClimbPos = hit.collider.transform.position + new Vector3(0, 2, 0);
                hit.rigidbody.isKinematic = true;
                onClimb = true;
                animator.SetTrigger("ClimbStart");
                while ( time < 1 )
                {
                    time += Time.deltaTime;
                    rb.MovePosition(Vector3.Lerp(startPos, ClimbPos, time));
                    yield return null;
                }
                Manager.game.StepAction++;
                hit.rigidbody.isKinematic = false;
                animator.SetFloat("MoveSpeed", 0f);
                yield return new WaitForSeconds(0.5f);
                
                moveOn = false;
                onClimb = false;
            }

        }
        else
        {
            if ( Physics.Raycast(transform.position + transform.forward, -transform.up, out RaycastHit wallHit, 3f) )
            {
                if ( wall.Contain(wallHit.collider.gameObject.layer) )
                {
                    transform.position = transform.position;
                    Debug.Log("밑에 벽이 잇음");
                    moveOn = false;
                }
                else
                {
                    float time = 0;
                    while ( time < 1 )
                    {
                        if(Physics.Raycast(transform.position + transform.forward, -transform.up, out RaycastHit hitinfo, 3f))
                        {
                            if ( wall.Contain(hitinfo.collider.gameObject.layer) )
                            {
                                Debug.Log("걸어가는 중에 밑에 벽");
                                transform.position = transform.position;
                                Manager.game.StepAction++;
                                moveOn = false;
                                yield break;
                            }  
                        }
                        time += Time.deltaTime * moveSpeed;
                        rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
                        yield return null;

                    }
                    Manager.game.StepAction++;
                    moveOn = false;
                }
            }
            else
            {
               
                moveOn = false;
            }
            
        }
    }
}
