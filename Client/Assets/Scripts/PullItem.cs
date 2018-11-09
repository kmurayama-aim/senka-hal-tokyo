using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PullItem : MonoBehaviour
{
    bool playCheat = false;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playCheat = !playCheat;
        }
        if (playCheat)
            Cheat();
    }
    void Cheat()
    {
        const string playerTagName = "Player";
        var player = GameObject.FindGameObjectWithTag(playerTagName);
        if (player == null)
            return;

        var items = GameObject.FindObjectsOfType<ItemController>();
        foreach (var item in items)
            MoveItemToPlayer(item.transform, player.transform);
    }
    void MoveItemToPlayer(Transform itemTrans, Transform playerTrans)
    {
        itemTrans.LookAt(playerTrans);
        itemTrans.position += itemTrans.transform.forward * Time.deltaTime * 5;
    }
}
