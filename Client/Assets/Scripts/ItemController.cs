using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public event Action OnGet;
    public event Action OnMove;

    private void FixedUpdate()
    {
        if(OnMove != null)
            OnMove();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
            OnGet();
    }
}
