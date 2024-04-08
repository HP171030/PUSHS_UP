using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Pool;

public class PooledObj : MonoBehaviour
{
    [SerializeField] bool autoRealease;
    [SerializeField] float releaseTime;

    private ObjPooling pool;
    public ObjPooling Pooling { get { return pool; } set { pool = value; } }

        private void OnEnable()
    {
        if ( autoRealease )
        {
            StartCoroutine(ReleaseRoutine());
        }
    }

    IEnumerator ReleaseRoutine()
    {
        yield return new WaitForSeconds( releaseTime );
        Release();
    }

    public void Release()
    {
        if (pool != null )
        {
            pool.ReturnPool(this);

        }
        else
        {
            Destroy(gameObject);
        }
    }
}
