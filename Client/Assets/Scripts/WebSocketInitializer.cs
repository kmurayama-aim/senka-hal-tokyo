using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using RPC = WebSocketSample.RPC;

public class WebSocketInitializer
{
    WebSocket webSocket;    // WebSocketコネクション
    WebSocketSetEvents setEvents = new WebSocketSetEvents();

    public void Initialize(string connectAddress)
    {
        webSocket = new WebSocket(connectAddress);
        var jsonMessageExtractor = new JsonMessageExtractor();

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

            var header = jsonMessageExtractor.ExtractHeaderMessage(eventArgs.Data);
            switch (header.Method)
            {
                case "ping":
                    {
                        setEvents.Ping(eventArgs.Data);
                        break;
                    }
                case "login_response":
                    {
                        setEvents.OnLoginResponse(eventArgs.Data);
                        break;
                    }
                case "sync":
                    {
                        setEvents.OnSync(eventArgs.Data);
                        break;
                    }
                case "spawn":
                    {
                        setEvents.OnSpawn(eventArgs.Data);
                        break;
                    }
                case "delete_item":
                    {
                        setEvents.OnDeleteItem(eventArgs.Data);
                        break;
                    }
                case "environment":
                    {
                        setEvents.OnEnvironment(eventArgs.Data);
                        break;
                    }
                case "delete_player":
                    {
                        setEvents.OnDeletePlayer(eventArgs.Data);
                        break;
                    }
            }
        };

        webSocket.Connect();
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
