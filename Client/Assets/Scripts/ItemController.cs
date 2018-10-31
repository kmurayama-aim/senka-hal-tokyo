using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public event Action OnGet;
    public event Action OnMove;

    private void FixedUpdate()
    {
        if(OnMove != null)
            OnMove();//���O����ł����̂��ȁc�H��������RigidBody���ĂȂ����cRigidBody�𕁒ʂɂ��������ǂ�����
    }

    void OnTriggerEnter(Collider other)
    {
        OnGet();
    }
}
