using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 20.0f; // 速度
    [SerializeField]
    bool superCheat = false;

    public event Action<int> OnCollision;

    void Start()
    {
    }

    // 固定フレームレートで呼び出されるハンドラ
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        var rb = GetComponent<Rigidbody>();

        // 速度の設定
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        if (superCheat)
            rb.velocity = movement * speed * 3;
        else
            rb.AddForce(movement * speed);
    }

    void OnCollisionEnter(Collision collision)
    {
        var otherPlayerController = collision.gameObject.GetComponent<OtherPlayerController>();
        if (otherPlayerController != null)
        {
            OnCollision(otherPlayerController.Id);
        }
    }
}
