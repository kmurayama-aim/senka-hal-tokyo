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
    [SerializeField]
    WebSocketSetEvents setEvents;

    void Start()
    {
        webSocket = new WebSocket(connectAddress);

        // コネクションを確立したときのハンドラ
        webSocket.OnOpen += (sender, eventArgs) =>
        {
            setEvents.OnOpen();
        };

        // エラーが発生したときのハンドラ
        webSocket.OnError += (sender, eventArgs) =>
        {
            setEvents.OnError(eventArgs.Message);
        };

        // コネクションを閉じたときのハンドラ
        webSocket.OnClose += (sender, eventArgs) =>
        {
            setEvents.OnClose();
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
                        MainThreadExecutor.Enqueue(() => setEvents.OnLoginResponse(eventArgs.Data));
                        break;
                    }
                case "sync":
                    {
                        MainThreadExecutor.Enqueue(() => setEvents.OnSync(eventArgs.Data));
                        break;
                    }
                case "spawn":
                    {
                        MainThreadExecutor.Enqueue(() => setEvents.OnSpawn(eventArgs.Data));
                        break;
                    }
                case "delete_item":
                    {
                        MainThreadExecutor.Enqueue(() => setEvents.OnDeleteItem(eventArgs.Data));
                        break;
                    }
                case "environment":
                    {
                        MainThreadExecutor.Enqueue(() => setEvents.OnEnvironment(eventArgs.Data));
                        break;
                    }
                case "delete_player":
                    {
                        MainThreadExecutor.Enqueue(() => setEvents.OnDeletePlayer(eventArgs.Data));
                        break;
                    }
            }
        };

        webSocket.Connect();

        setEvents.Login();
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
