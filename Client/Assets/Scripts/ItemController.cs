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

        if (Input.GetKey(KeyCode.Space))
        {
            Cheat();
        }
    }
    void Cheat()
    {
        var targetTrans = GameObject.FindGameObjectWithTag("Player").transform;
        transform.LookAt(targetTrans);
        transform.position += transform.forward * Time.fixedDeltaTime * 5;
        var distance = Vector3.Distance(transform.position, targetTrans.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
            OnGet();
    }
}
