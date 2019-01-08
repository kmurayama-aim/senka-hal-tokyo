using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

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

    [SerializeField]
    MessageSenderToServer messageSenderToServer; //WebSocketInitializerがStartでイベントを登録する前に参照が無いとエラー

    void Start()
    {
        Login();
    }

    void Update()
    {
        UpdatePosition();
    }

    public void Login()
    {
        WebSocketInitializer socketInitializer = GetComponent<WebSocketInitializer>();
        socketInitializer.Initialize();
        messageSenderToServer.SendLoginMessage("PlayerName");
    }

    public void OnLoginResponse(int playerId)
    {
        Debug.Log("<< LoginResponse");
        this.playerId = playerId;
        Debug.Log(playerId);
        playerObj = Instantiate(playerPrefab, new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity) as GameObject;
        var playerController = playerObj.GetComponent<PlayerController>();

        //PlayerControllerに任せたい
        playerController.OnCollision += (otherPlayerId) =>
        {
            messageSenderToServer.SendPlayerOnCollisionMessage(playerId, otherPlayerId);
        };
    }

    void UpdatePosition()
    {
        if (playerObj == null) return;

        var currentPlayerPosition = playerObj.transform.position;
        if (currentPlayerPosition == previousPlayerObjPosition) return;

        Debug.Log(">> Update");

        previousPlayerObjPosition = currentPlayerPosition;

        messageSenderToServer.SendUpdatePositionMessage(currentPlayerPosition, playerId);
    }

    public void OnSync(List<Player> players)
    {
        Debug.Log("<< Sync");
        foreach (var player in players)
        {
            if (player.Id == playerId)
            {
                playerObj.transform.localScale = CalcPlayerScale(player.Score);
                continue;
            }

            var otherPlayerPoision = player.Position;
            Debug.Log(player.Id);

            if (otherPlayerObjs.ContainsKey(player.Id))
            {
                // 既にGameObjectがいたら更新
                otherPlayerObjs[player.Id].transform.position = otherPlayerPoision;
                otherPlayerObjs[player.Id].transform.localScale = CalcPlayerScale(player.Score);
            }
            else
            {
                // GameObjectがいなかったら新規作成
                var otherPlayerObj = Instantiate(otherPlayerPrefab, otherPlayerPoision, Quaternion.identity) as GameObject;
                otherPlayerObj.GetComponent<OtherPlayerController>().Id = player.Id;
                otherPlayerObj.name = "Other" + player.Id;
                otherPlayerObjs.Add(player.Id, otherPlayerObj);
                Debug.Log("Instantiated a new player: " + player.Id);
            }
        }
    }

    public void OnSpawn(Item item)
    {
        Debug.Log("<< OnSpawn");
        SpawnItem(item);
    }

    void SpawnItem(Item item)
    {
        var position = item.Position;
        var itemObj = Instantiate(itemPrefab, position, Quaternion.identity);
        items.Add(item.Id, itemObj);

        var itemSc = itemObj.GetComponent<ItemController>();

        //ItemScに任せたい
        itemSc.OnGet += () =>
        {
            items.Remove(item.Id);
            Destroy(itemObj);

            messageSenderToServer.SendItemOnGetMessage(item.Id, playerId);
        };
    }

    public void OnDeleteItem(DeleteItem deleteItem)
    {
        Debug.Log("<< DeleteItem");
        var itemId = deleteItem.Id;
        if (items.ContainsKey(itemId))
        {
            Destroy(items[itemId]);
            items.Remove(itemId);
        }
    }

    public void OnEnvironment(Environment environment)
    {
        Debug.Log("<< Environment");

        var serverUnknownItems = new List<KeyValuePair<int, GameObject>>();
        // サーバーからのリスト(payload.Items)にないアイテムを所持していたらserverUnknownItemsに追加
        foreach (var item in items)
        {
            if (environment.Items.Any(itemRpc => itemRpc.Id == item.Key)) continue;

            serverUnknownItems.Add(item);
        }
        // serverUnknownItemsをクライアントから削除
        foreach (var item in serverUnknownItems)
        {
            items.Remove(item.Key);
            Destroy(item.Value);
        }

        foreach (var rpcItem in environment.Items)
        {
            if (items.ContainsKey(rpcItem.Id)) continue;

            SpawnItem(rpcItem);
        }
    }

    public void OnDeletePlayer(int id)
    {
        if (otherPlayerObjs.ContainsKey(id))
        {
            Destroy(otherPlayerObjs[id]);
            otherPlayerObjs.Remove(id);
        }
        else if (id == playerId)
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
        messageSenderToServer.SendLogoutMessage();
        MainThreadExecutor.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
