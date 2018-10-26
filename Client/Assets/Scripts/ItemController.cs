using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    public event Action OnGet;
    public event Action<Rigidbody> MoveToPlayer;

    void Awake()
    {
    }

    private void FixedUpdate()
    {
        MoveToPlayer(rb);
    }
    void OnTriggerEnter(Collider other)
    {
        OnGet();
    }
}
