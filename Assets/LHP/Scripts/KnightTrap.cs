using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightTrap : MonoBehaviour
{
    Animator animator;
    bool onAttack = false;
    [SerializeField] LayerMask player;
    [SerializeField] Vector3 attackRange;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {

        if ( Manager.game.StepAction % 3 == 0 && Manager.game.StepAction != 0 && !onAttack )
        {
            onAttack = true;
            animator.Play("Attack");
            if ( Physics.BoxCast(transform.position + new Vector3(0, 1f, 0),attackRange, transform.forward, out RaycastHit hitInfo, Quaternion.identity, 2f) )
            {
                if ( player.Contain(hitInfo.collider.gameObject.layer) )
                {
                    Debug.Log("피격 - 게임오버");
                }
            }
        }
        else if ( onAttack && Manager.game.StepAction % 3 != 0 )
        {
            onAttack = false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + new Vector3(0, 0.5f, 0), new Vector3(0.1f, 0.1f, 0.1f));
    }
}
