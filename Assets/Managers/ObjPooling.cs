using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPooling : MonoBehaviour
{
    [SerializeField] PooledObj prefab;
    [SerializeField] int size;
    [SerializeField] int capacity;

    private Stack<PooledObj> objPool;


    public void CreatePool(PooledObj obj,int size, int capacity )
    {
        prefab = obj;
        this.size = size;
        this.capacity = capacity;

        objPool = new Stack<PooledObj>(capacity);

        for(int i = 0; i < size; i++)
        {
            PooledObj ins = Instantiate(prefab);
            ins.gameObject.SetActive(false);
            ins.Pooling = this;
            ins.transform.SetParent(transform,false);
            objPool.Push(ins);

        }
    }

    public PooledObj GetPool(Vector3 position,Quaternion rotation)
    {
        if(objPool.Count > 0 )
        {
            PooledObj ins = objPool.Pop();
            ins.transform.position = position;
            ins.transform.rotation = rotation;
            ins.gameObject.SetActive(true);

            return ins;
        }
        else
        {
            PooledObj ins = Instantiate(prefab);
                ins.Pooling = this;
            ins.transform.position=position;
            ins.transform.rotation=rotation;

            return ins;
        }
    }

    public void ReturnPool(PooledObj ins )
    {
        if (objPool.Count < capacity )
        {
            ins.gameObject.SetActive(false);
            ins.transform.SetParent(transform, false);
            objPool.Push(ins);
        }
        else
        {
            Destroy(ins.gameObject);
        }
    }
}
