using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        var playerTagName = "Player";
        if (GameObject.FindGameObjectWithTag(playerTagName) == null)
            return;

        var playerObj = GameObject.FindGameObjectWithTag(playerTagName);
        var allObj = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach(GameObject obj in allObj)
        {
            if (obj.GetComponent<ItemController>())
                MoveItemToPlayer(obj.transform, playerObj.transform);
        }
    }
    void MoveItemToPlayer(Transform itemTrans, Transform playerTrans)
    {
        itemTrans.LookAt(playerTrans);
        itemTrans.position += itemTrans.transform.forward * Time.deltaTime * 5;
    }
}
