using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    public event Action OnGet;
    public event Action<Rigidbody> MoveToPlayer;
    MainController mainControllerSc;

    void Awake()
    {
        mainControllerSc = GameObject.Find("Main").GetComponent<MainController>();
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
