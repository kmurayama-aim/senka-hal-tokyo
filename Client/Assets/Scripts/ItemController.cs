using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public event Action OnGet;
    public event Action OnMove;

    private void FixedUpdate()
    {
        if(OnMove != null)
            OnMove();//名前これでいいのかな…？そもそもRigidBodyつけてないし…RigidBodyを普通につけた方が良さそう
    }

    void OnTriggerEnter(Collider other)
    {
        OnGet();
    }
}
