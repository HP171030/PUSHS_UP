using System.Collections;
using Unity.VisualScripting;
//using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player2Controller : MonoBehaviour
{
    public Vector3 moveDir;

    public bool moveOn;
    public bool inputKey;
    [Header("Player")]
    [SerializeField] float moveDistance;
    [SerializeField] float moveSpeed;

    [Header("Configs")]
    [SerializeField] public Animator animator;
    [SerializeField] Rigidbody rb;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask obstacleUpperLayer;
    [SerializeField] LayerMask wall;
    [SerializeField] LayerMask ground;
    [SerializeField] public bool onGround;
    [SerializeField] public bool onClimb;
    [SerializeField] public bool ontheBox;
    [SerializeField] public bool downingBox;
    [SerializeField] public bool onIce = false;
    [SerializeField] LayerMask isNotplayerLayer;
    [SerializeField] RaycastHit debug;
    [SerializeField] ScoreDown scoreDown;

    Vector3 debugVec;
    Vector3 debugVec2;

    [SerializeField] public CameraSwitch cameraSwitch;

    [Header("Sound")]
    [SerializeField] AudioClip WalkSound;
    [SerializeField] AudioClip cubeJumpSound;
    [SerializeField] AudioClip jumpDownSound;
    [SerializeField] AudioClip boomSound;




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
    private void OnTriggerExit( Collider other )
    {
        if ( obstacleUpperLayer.Contain(other.gameObject.layer) )
        {

            ontheBox = false;
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
        inputKey = true;

    }
    private void OnMove(InputValue value)
    {
       
        if (!cameraSwitch.IsPlayer1Active && !onIce&&inputKey )
        {
            Vector2 input = value.Get<Vector2>();
            moveDir = new Vector3(input.x, 0, input.y);
        }

    }

    public void OnBoom( InputValue value )
    {
        if ( value.isPressed&&!cameraSwitch.IsPlayer1Active&&Manager.game.boomAction > 0&&!onClimb)
        {
           
           
           // animator.SetTrigger("Boom");

            if ( Physics.Raycast(transform.position + new Vector3 (0,1,0), transform.forward, out BoomableHit, 2f) )                          // �� �ȸ´� ���� ���� ���������� �ٲܰ�
            {
                if ( obstacleLayer.Contain(BoomableHit.collider.gameObject.layer))
                {
                    Destroy(BoomableHit.collider.gameObject );
                    Manager.game.boomAction--;
                    scoreDown.DecreaseScore();
                    Manager.sound.PlaySFX(boomSound);
                }
            }
            else
            {
                Debug.Log("����");
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
        LayerMask layer = obstacleLayer | wall;
        Vector3 targetPos = transform.position + moveDirValue * moveDistance;
        Vector3 startPos = transform.position;

        Collider [] tiles = Physics.OverlapSphere(targetPos, 0.3f, ground);
        if ( tiles.Length > 0 )
        {
            foreach ( Collider tile in tiles )
            {
                Tile tileIns = tile.GetComponent<Tile>();
                if ( tileIns != null )
                {
                    Collider [] isBlank = Physics.OverlapSphere(tileIns.middlePoint.transform.position+ new Vector3(0,1f,0), 0.1f, wall);
                    debugVec = tileIns.middlePoint.transform.position ;
                    if (isBlank.Length == 0 )
                    {
                        targetPos = tileIns.middlePoint.position;

                       
                    }
                    else
                    {
                        foreach(Collider col in  isBlank )
                        {
                            Debug.Log($"�տ� ��ֹ��� �־� { col.name}");
                            transform.position = transform.position;
                            moveOn = false;
                            yield break;
                        }
                        
                    }
                }

            }
        }
        RaycastHit hit;
        Vector3 PreMoveDir = moveDir;
        
        if ( Physics.BoxCast(transform.position + new Vector3(0, 1f, 0), new Vector3(0.1f, 0.1f, 0.1f), moveDirValue, out hit, Quaternion.identity, 1f, layer) )
        {
            if ( !obstacleLayer.Contain(hit.collider.gameObject.layer) )                                   
            {
                Debug.Log("��ֹ��̳� ���� �ƴ�");
                Debug.Log(hit.collider.gameObject.name);
                moveOn = false;
                yield break;
            }
            else
            {
                RaycastHit [] raycastHits = Physics.RaycastAll(hit.collider.transform.position, hit.collider.transform.up, 3f,obstacleLayer);


                foreach ( RaycastHit other in raycastHits )
                {
                    if ( other.collider.gameObject != hit.collider.gameObject && !obstacleUpperLayer.Contain(other.collider.gameObject.layer) )
                    {
                        Debug.Log($"���� ��ֹ��� �ֽ��ϴ�: {other.collider.gameObject.name}");
                        moveOn = false;
                        yield break;
                    }

                }

                float time = 0;

                Vector3 ClimbPos = hit.collider.transform.position + new Vector3(0, 2, 0);
                hit.rigidbody.isKinematic = true;
                onClimb = true;
                animator.SetTrigger("ClimbStart");
                Manager.sound.PlaySFX(cubeJumpSound);
                while ( time < 1 )
                {
                    time += Time.deltaTime;
                    rb.MovePosition(Vector3.Lerp(startPos, ClimbPos, time));
                    yield return null;
                }
                Manager.game.StepAction++;
                if(hit.collider !=null)
              //  hit.rigidbody.isKinematic = false;
                animator.SetFloat("MoveSpeed", 0f);
                yield return new WaitForSeconds(0.5f);

                moveOn = false;
                onClimb = false;
            }

        }
        else if ( ontheBox )
        {
            LayerMask checkLayer = ground | obstacleLayer | wall;
            if ( Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), -transform.up, out RaycastHit onBox, 2f, obstacleLayer) )
            {
                Debug.Log($"�ö��ִ� �ڽ� : {onBox.collider.gameObject.name}");
                debug = onBox;
                Vector3 onBoxPos = onBox.collider.transform.position + new Vector3(0, 1f, 0);
                if(Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0),transform.forward,out RaycastHit isWall, 2f, wall))
                {
                    if(wall.Contain(isWall.collider.gameObject.layer))
                    {
                        Debug.Log("�տ� ��");
                        moveOn = false;
                        yield break;
                    }

                }
                else
                {
                    
                    Collider [] colliders = Physics.OverlapSphere(onBoxPos + moveDirValue*2f, 0.1f, checkLayer);
                    debugVec = onBoxPos;
                    debugVec2 = moveDirValue;
                    Debug.Log("���� ����");
                    Debug.Log(colliders.Length);
                    if ( colliders.Length > 0 )
                    {
                        foreach ( Collider collider in colliders )
                        {
                            Debug.Log($"���ִ� �ڽ� �տ� {collider.gameObject.name}");
                                
                                if ( collider.gameObject != onBox.collider.gameObject )
                                {
                                    if ( wall.Contain(collider.gameObject.layer) )
                                    {
                                        Debug.Log($"�Ʒ��� ���� �ִ� {collider.gameObject.name}");

                                        moveOn = false;
                                        yield break;
                                    }
                                    else
                                    {
                                        Debug.Log(collider.gameObject.name);
                                        float time = 0;
                                    Manager.sound.PlaySFX(WalkSound);
                                    while ( time < 1 )
                                        {
                                            time += Time.deltaTime * moveSpeed;
                                            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
                                            yield return null;
                                        }
                                    Manager.game.StepAction++;
                                        moveOn = false;
                                    }

                                }
                        }
                    }
                    else
                    {
                        Debug.Log("����?");
                        LayerMask GandObs = ground | obstacleLayer;

                        if ( Physics.Raycast(transform.position + moveDirValue * 1.5f, Vector3.down, out RaycastHit isTile, 3f, GandObs) )
                        {
                            Debug.Log("�ö󰣻����̰� �տ� �� ���� ����");
                            Tile Ank = isTile.collider.gameObject.GetComponent<Tile>();
                            if ( Ank != null )
                            {
                                    Collider [] isBlank = Physics.OverlapSphere(Ank.transform.position + new Vector3(0, 1f, 0), 1f, layer);
                                    debugVec = Ank.transform.position + new Vector3(0, 1f, 0);
                                    if ( isBlank.Length == 0 )
                                    {
                                        targetPos = Ank.middlePoint.position;
                                        Debug.Log(Ank.gameObject.name);
                                    }
                            }
                            else
                            {
                                targetPos = isTile.collider.gameObject.transform.position + new Vector3(0, 2, 0);
                            }
                            float time = 0;
                            StartCoroutine(DownAnim());
                            while ( time < 1 )
                            {
                                time += Time.deltaTime * moveSpeed;
                                rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
                                yield return null;
                            }
                            Manager.sound.PlaySFX(jumpDownSound);
                            Manager.game.StepAction++;
                            moveOn = false;
                            yield return null;
                        }
                        else
                        {
                            Debug.Log("�ʹ� ����");
                            moveOn = false;
                            yield break;
                        }
                    }
                  
                   
                }
                   

                }
            else
            {
                Debug.Log("�������� ����");
                ontheBox = false;
                moveOn = false;
                yield break;
            }

        }
        else // �տ� �ƹ��͵� ���� && ������
        {
            float time = 0;
            while ( time < 1 )
            {
                if(Physics.Raycast(transform.position + new Vector3(0,1,0),moveDirValue,out RaycastHit otherHit, 0.2f,layer) )
                {
                    Debug.Log(otherHit.collider.gameObject.name );
                    moveOn = false;
                    transform.position = transform.position;
                    yield break;
                    
                }
                time += Time.deltaTime * moveSpeed;
                rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
                yield return null;

            }
            Manager.sound.PlaySFX(WalkSound);
            Manager.game.StepAction++;
            moveOn = false;
        }
       
    }
}
