using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPC = WebSocketSample.RPC;

public class MainController : MonoBehaviour
{
    [SerializeField]
    GameObject playerPrefab;
    [SerializeField]
    GameObject otherPlayerPrefab;
    [SerializeField]
    GameObject itemPrefab;

    GameObject playerObj;
    Vector3 previousPlayerObjPosition; // 前フレームでの位置
    int playerId;
    Dictionary<int, GameObject> otherPlayerObjs = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> items = new Dictionary<int, GameObject>();

    WebSocketInitializer socketInitializer;

    void Awake()
    {
        socketInitializer = GetComponent<WebSocketInitializer>();
    }

    void Update()
    {
        UpdatePosition();
    }

    public void Login()
    {
        var jsonMessage = JsonUtility.ToJson(new RPC.Login(new RPC.LoginPayload("PlayerName")));
        Debug.Log(jsonMessage);

        socketInitializer.Send(jsonMessage);
        Debug.Log(">> Login");
    }

    public void OnLoginResponse(RPC.LoginResponsePayload response)
    {
        Debug.Log("<< LoginResponse");
        playerId = response.Id;
        Debug.Log(playerId);
        playerObj = Instantiate(playerPrefab, new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity) as GameObject;

        var playerController = playerObj.GetComponent<PlayerController>();
        playerController.OnCollision += (otherPlayerId) =>
        {
            var collisionRpc = new RPC.Collision(new RPC.CollisionPayload(playerId, otherPlayerId));
            var collisionJson = JsonUtility.ToJson(collisionRpc);
            socketInitializer.Send(collisionJson);
        };
    }

    void UpdatePosition()
    {
        if (playerObj == null) return;

        var currentPlayerPosition = playerObj.transform.position;
        if (currentPlayerPosition == previousPlayerObjPosition) return;

        Debug.Log(">> Update");

        previousPlayerObjPosition = currentPlayerPosition;

        var rpcPosition = new RPC.Position(currentPlayerPosition.x, currentPlayerPosition.y, currentPlayerPosition.z);
        var jsonMessage = JsonUtility.ToJson(new RPC.PlayerUpdate(new RPC.PlayerUpdatePayload(playerId, rpcPosition)));
        Debug.Log(jsonMessage);
        socketInitializer.Send(jsonMessage);
    }

    public void OnSync(RPC.SyncPayload payload)
    {
        Debug.Log("<< Sync");
        foreach (var rpcPlayer in payload.Players)
        {
            if (rpcPlayer.Id == playerId)
            {
                playerObj.transform.localScale = CalcPlayerScale(rpcPlayer.Score);
                continue;
            }

            var otherPlayerPoision = new Vector3(rpcPlayer.Position.X, rpcPlayer.Position.Y, rpcPlayer.Position.Z);

            if (otherPlayerObjs.ContainsKey(rpcPlayer.Id))
            {
                // 既にGameObjectがいたら更新
                otherPlayerObjs[rpcPlayer.Id].transform.position = otherPlayerPoision;
                otherPlayerObjs[rpcPlayer.Id].transform.localScale = CalcPlayerScale(rpcPlayer.Score);
            }
            else
            {
                // GameObjectがいなかったら新規作成
                var otherPlayerObj = Instantiate(otherPlayerPrefab, otherPlayerPoision, Quaternion.identity) as GameObject;
                otherPlayerObj.GetComponent<OtherPlayerController>().Id = rpcPlayer.Id;
                otherPlayerObj.name = "Other" + rpcPlayer.Id;
                otherPlayerObjs.Add(rpcPlayer.Id, otherPlayerObj);
                Debug.Log("Instantiated a new player: " + rpcPlayer.Id);
            }
        }
    }

    public void OnSpawn(RPC.SpawnPayload payload)
    {
        Debug.Log("<< OnSpawn");
        SpawnItem(payload.Item);
    }

    void SpawnItem(RPC.Item rpcItem)
    {
        var position = new Vector3(rpcItem.Position.X, rpcItem.Position.Y, rpcItem.Position.Z);
        var itemObj = Instantiate(itemPrefab, position, Quaternion.identity);
        items.Add(rpcItem.Id, itemObj);

        var item = itemObj.GetComponent<ItemController>();
        item.OnGet += () =>
        {
            items.Remove(rpcItem.Id);
            Destroy(itemObj);

            var getItemRpc = new RPC.GetItem(new RPC.GetItemPayload(rpcItem.Id, playerId));
            var getItemJson = JsonUtility.ToJson(getItemRpc);
            socketInitializer.Send(getItemJson);
            Debug.Log(">> GetItem");
        };
    }

    public void OnDeleteItem(RPC.DeleteItemPayload payload)
    {
        Debug.Log("<< DeleteItem");
        var itemId = payload.ItemId;
        if (items.ContainsKey(itemId))
        {
            Destroy(items[itemId]);
            items.Remove(itemId);
        }
    }

    public void OnEnvironment(RPC.EnvironmentPayload payload)
    {
        Debug.Log("<< Environment");

        var serverUnknownItems = new List<KeyValuePair<int, GameObject>>();
        // サーバーからのリスト(payload.Items)にないアイテムを所持していたらserverUnknownItemsに追加
        foreach (var item in items)
        {
            if (payload.Items.Exists(itemRpc => itemRpc.Id == item.Key)) continue;

            serverUnknownItems.Add(item);
        }
        // serverUnknownItemsをクライアントから削除
        foreach (var item in serverUnknownItems)
        {
            items.Remove(item.Key);
            Destroy(item.Value);
        }

        foreach (var rpcItem in payload.Items)
        {
            if (items.ContainsKey(rpcItem.Id)) continue;

            SpawnItem(rpcItem);
        }
    }

    public void OnDeletePlayer(RPC.DeletePlayerPayload payload)
    {
        if (otherPlayerObjs.ContainsKey(payload.Id))
        {
            Destroy(otherPlayerObjs[payload.Id]);
            otherPlayerObjs.Remove(payload.Id);
        }
        else if (payload.Id == playerId)
        {
            Destroy(playerObj);
            Invoke("RestartGame", 3);
        }
    }

    Vector3 CalcPlayerScale(int score)
    {
        return Vector3.one + (Vector3.one * score * 0.2f);
    }

    void RestartGame()
    {
        socketInitializer.Close();
        MainThreadExecutor.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
