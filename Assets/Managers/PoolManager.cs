using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    private Dictionary<int,ObjPooling> poolDic = new Dictionary<int, ObjPooling>();

    public void CreatePool( PooledObj prefab, int size, int capacity )
    {
        GameObject obj = new GameObject();
        obj.name = $"Pool_{prefab.name}";

        ObjPooling pooledObj = gameObject.AddComponent<ObjPooling>();
        pooledObj.CreatePool(prefab, size, capacity);

        poolDic.Add(prefab.GetInstanceID(), pooledObj);
    }

    public void DestroyPool(PooledObj prefab)
    {
        ObjPooling obj = poolDic [prefab.GetInstanceID()];
        Destroy(obj.gameObject);

        poolDic.Remove(prefab.GetInstanceID());
    }

    public void ClearPool()
    {
        foreach(ObjPooling objPool in poolDic.Values )
        {
            Destroy(objPool.gameObject);
        }

        poolDic.Clear();    
    }

    public PooledObj GetPool( PooledObj prefab,Vector3 position,Quaternion rotation )
    {
        return poolDic [prefab.GetInstanceID()].GetPool(position,rotation);
    }

    public void ReturnPool( PooledObj prefab )
    {
        poolDic [prefab.GetInstanceID()].ReturnPool(prefab);
    }


}
