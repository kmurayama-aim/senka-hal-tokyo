using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    public event Action OnGet;
    MainController mainControllerSc;

    void Awake()
    {
        mainControllerSc = GameObject.Find("Main").GetComponent<MainController>();
    }

    private void FixedUpdate()
    {
        if (mainControllerSc.SuperCheatProp)
            GoToPlayer();
    }
    void GoToPlayer()
    {
        transform.LookAt(mainControllerSc.PlayerObjProp.transform);
        rb.AddForce(transform.forward * 20);
    }
    void OnTriggerEnter(Collider other)
    {
        OnGet();
    }
}
