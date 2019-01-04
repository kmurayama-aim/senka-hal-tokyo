using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using RPC = WebSocketSample.RPC;

public class WebSocketInitializer : MonoBehaviour
{
    WebSocket webSocket;    // WebSocketコネクション
    [SerializeField]
    string connectAddress;

    void Start()
    {
        webSocket = new WebSocket(connectAddress);
        var mainControllerSc = GameObject.Find("Main").GetComponent<MainController>();

        // コネクションを確立したときのハンドラ
        webSocket.OnOpen += (sender, eventArgs) =>
        {
            Debug.Log("WebSocket Opened");
        };

        // エラーが発生したときのハンドラ
        webSocket.OnError += (sender, eventArgs) =>
        {
            Debug.Log("WebSocket Error Message: " + eventArgs.Message);
        };

        // コネクションを閉じたときのハンドラ
        webSocket.OnClose += (sender, eventArgs) =>
        {
            Debug.Log("WebSocket Closed");
        };

        // メッセージを受信したときのハンドラ
        webSocket.OnMessage += (sender, eventArgs) =>
        {
            Debug.Log("WebSocket Message: " + eventArgs.Data);

            var header = JsonUtility.FromJson<RPC.Header>(eventArgs.Data);
            switch (header.Method)
            {
                case "ping":
                    {
                        var pong = JsonUtility.FromJson<RPC.Ping>(eventArgs.Data);
                        Debug.Log(pong.Payload.Message);
                        break;
                    }
                case "login_response":
                    {
                        var loginResponse = JsonUtility.FromJson<RPC.LoginResponse>(eventArgs.Data);
                        MainThreadExecutor.Enqueue(() => mainControllerSc.OnLoginResponse(loginResponse.Payload.Id));
                        break;
                    }
                case "sync":
                    {
                        var syncMessage = JsonUtility.FromJson<RPC.Sync>(eventArgs.Data);
                        var players = new List<Player>();
                        foreach (var rpcPlayer in syncMessage.Payload.Players)//Linqチャンス！！
                        {
                            var pos = new Vector3(rpcPlayer.Position.X, rpcPlayer.Position.Y, rpcPlayer.Position.Z);
                            var player = new Player(rpcPlayer.Id, rpcPlayer.Score, pos);
                            players.Add(player);
                        }
                        MainThreadExecutor.Enqueue(() => mainControllerSc.OnSync(players));
                        break;
                    }
                case "spawn":
                    {
                        var spawnResponse = JsonUtility.FromJson<RPC.Spawn>(eventArgs.Data);

                        var pos = new Vector3(spawnResponse.Payload.Item.Position.X, spawnResponse.Payload.Item.Position.Y, spawnResponse.Payload.Item.Position.Z);
                        var item = new Item(spawnResponse.Payload.Item.Id, pos);
                        MainThreadExecutor.Enqueue(() => mainControllerSc.OnSpawn(item));
                        break;
                    }
                case "delete_item":
                    {
                        var deleteMessage = JsonUtility.FromJson<RPC.DeleteItem>(eventArgs.Data);
                        var deleteItem = new DeleteItem(deleteMessage.Payload.ItemId);
                        MainThreadExecutor.Enqueue(() => mainControllerSc.OnDeleteItem(deleteItem));
                        break;
                    }
                case "environment":
                    {
                        var environmentMessage = JsonUtility.FromJson<RPC.Environment>(eventArgs.Data);
                        var items = new List<Item>();
                        foreach (var rpcItem in environmentMessage.Payload.Items)
                        {
                            var pos = new Vector3(rpcItem.Position.X, rpcItem.Position.Y, rpcItem.Position.Z);
                            var item = new Item(rpcItem.Id, pos);
                            items.Add(item);
                        }
                        var environment = new Environment(items);
                        MainThreadExecutor.Enqueue(() => mainControllerSc.OnEnvironment(environment));
                        break;
                    }
                case "delete_player":
                    {
                        var deletePlayerMessage = JsonUtility.FromJson<RPC.DeletePlayer>(eventArgs.Data);
                        MainThreadExecutor.Enqueue(() => mainControllerSc.OnDeletePlayer(deletePlayerMessage.Payload.Id));
                        break;
                    }
            }
        };

        webSocket.Connect();

        mainControllerSc.Login();
    }
    void OnDestroy()
    {
        Close();
    }
    public void Send(string message)
    {
        webSocket.Send(message);
    }
    public void Close()
    {
        webSocket.Close();
    }

    public string CreateLoginMessage(string message)
    {
        var jsonMessage = JsonUtility.ToJson(new RPC.Login(new RPC.LoginPayload(message)));
        return jsonMessage;
    }
    public string CreateCollisionMessage(int playerId, int otherPlayerId)
    {
        var collisionRpc = new RPC.Collision(new RPC.CollisionPayload(playerId, otherPlayerId));
        var jsonMessage = JsonUtility.ToJson(collisionRpc);
        return jsonMessage;
    }
    public string CreatePlayerUpdateMessage(Vector3 pos, int playerId)
    {
        var rpcPosition = new RPC.Position(pos.x, pos.y, pos.z);
        var jsonMessage = JsonUtility.ToJson(new RPC.PlayerUpdate(new RPC.PlayerUpdatePayload(playerId, rpcPosition)));
        return jsonMessage;
    }
    public string CreateGetItemMessage(int itemId, int playerId)
    {
        var getItemRpc = new RPC.GetItem(new RPC.GetItemPayload(itemId, playerId));
        var jsonMessage = JsonUtility.ToJson(getItemRpc);
        return jsonMessage;
    }
}
