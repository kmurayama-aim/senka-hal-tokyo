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
                        MainThreadExecutor.Enqueue(() => mainControllerSc.OnLoginResponse(loginResponse.Payload));
                        break;
                    }
                case "sync":
                    {
                        var syncMessage = JsonUtility.FromJson<RPC.Sync>(eventArgs.Data);
                        MainThreadExecutor.Enqueue(() => mainControllerSc.OnSync(syncMessage.Payload));
                        break;
                    }
                case "spawn":
                    {
                        var spawnResponse = JsonUtility.FromJson<RPC.Spawn>(eventArgs.Data);
                        MainThreadExecutor.Enqueue(() => mainControllerSc.OnSpawn(spawnResponse.Payload));
                        break;
                    }
                case "delete_item":
                    {
                        var deleteMessage = JsonUtility.FromJson<RPC.DeleteItem>(eventArgs.Data);
                        MainThreadExecutor.Enqueue(() => mainControllerSc.OnDeleteItem(deleteMessage.Payload));
                        break;
                    }
                case "environment":
                    {
                        var environmentMessage = JsonUtility.FromJson<RPC.Environment>(eventArgs.Data);
                        MainThreadExecutor.Enqueue(() => mainControllerSc.OnEnvironment(environmentMessage.Payload));
                        break;
                    }
                case "delete_player":
                    {
                        var deletePlayerMessage = JsonUtility.FromJson<RPC.DeletePlayer>(eventArgs.Data);
                        MainThreadExecutor.Enqueue(() => mainControllerSc.OnDeletePlayer(deletePlayerMessage.Payload));
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
}

///
/// 最終的にどんな実装にしたいのか,切り出そうとしたときの問題は何か？
/// １．動く保証が無い　２．MainThreadExecutorへのイベント登録を済ませた後にWebSocketConnectをしたい
/// １：動くか動かないかで分かるのでとりあえず実行してれば大丈夫
/// ２．かんたんなのはConnectやLoginもこっちで呼ぶこと
///
