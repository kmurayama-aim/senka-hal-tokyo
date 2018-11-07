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
        var allObjs = GameObject.FindObjectsOfType(typeof(GameObject)) as IEnumerable<GameObject>;
        var playerTagName = "Player";
        if (!allObjs.Any(obj => obj.tag == playerTagName))
            return;

        var playerObj = allObjs.First(obj => obj.tag == playerTagName);
        var itemObjs = allObjs.Where(obj => obj.GetComponent<ItemController>());
        foreach(var itemObj in itemObjs)
        {
            MoveItemToPlayer(itemObj.transform, playerObj.transform);
        }
    }
    void MoveItemToPlayer(Transform itemTrans, Transform playerTrans)
    {
        itemTrans.LookAt(playerTrans);
        itemTrans.position += itemTrans.transform.forward * Time.deltaTime * 5;
    }
}
