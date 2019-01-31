using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPC = WebSocketSample.RPC;

public class WebSocketSetEvents
{
    MainController mainControllerSc;
    JsonMessageExtractor jsonMessageExtractor = new JsonMessageExtractor();

    public WebSocketSetEvents()
    {
        MainThreadExecutor.Enqueue(() => mainControllerSc = GameObject.Find("Main").GetComponent<MainController>());
    }

    public void OnOpen()
    {
        Debug.Log("WebSocket Opened");
    }
    public void OnError(string message)
    {
        Debug.Log("WebSocket Error Message: " + message);
    }

    public void OnClose()
    {
        Debug.Log("WebSocket Closed");
    }

    public void Ping(string message)
    {
        var pong = jsonMessageExtractor.ExtractPingMessage(message);
        Debug.Log(pong.Payload.Message);
    }

    public void OnLoginResponse(string message)
    {
        var loginResponse = jsonMessageExtractor.ExtractOnLoginResponseMessage(message);
        MainThreadExecutor.Enqueue(() => mainControllerSc.OnLoginResponse(loginResponse.Payload.Id));
    }

    public void OnSync(string message)
    {
        var syncMessage = jsonMessageExtractor.ExtractOnSyncMessage(message);
        var players = new List<Player>();
        foreach (var rpcPlayer in syncMessage.Payload.Players)
        {
            var pos = new Vector3(rpcPlayer.Position.X, rpcPlayer.Position.Y, rpcPlayer.Position.Z);
            var player = new Player(rpcPlayer.Id, rpcPlayer.Score, pos);
            players.Add(player);
        }
        MainThreadExecutor.Enqueue(() => mainControllerSc.OnSync(players));
    }

    public void OnSpawn(string message)
    {
        var spawnResponse = jsonMessageExtractor.ExtractOnSpawnMessage(message);

        var pos = new Vector3(spawnResponse.Payload.Item.Position.X, spawnResponse.Payload.Item.Position.Y, spawnResponse.Payload.Item.Position.Z);
        var item = new Item(spawnResponse.Payload.Item.Id, pos);
        MainThreadExecutor.Enqueue(() => mainControllerSc.OnSpawn(item));
    }

    public void OnDeleteItem(string message)
    {
        var deleteMessage = jsonMessageExtractor.ExtractOnDeleteItemMessage(message);
        var deleteItem = new DeleteItem(deleteMessage.Payload.ItemId);
        MainThreadExecutor.Enqueue(() => mainControllerSc.OnDeleteItem(deleteItem));
    }

    public void OnEnvironment(string message)
    {
        var environmentMessage = jsonMessageExtractor.ExtractOnEnvironmentMessage(message);
        var items = new List<Item>();
        foreach (var rpcItem in environmentMessage.Payload.Items)
        {
            var pos = new Vector3(rpcItem.Position.X, rpcItem.Position.Y, rpcItem.Position.Z);
            var item = new Item(rpcItem.Id, pos);
            items.Add(item);
        }
        var environment = new Environment(items);
        MainThreadExecutor.Enqueue(() => mainControllerSc.OnEnvironment(environment));
    }

    public void OnDeletePlayer(string message)
    {
        var deletePlayerMessage = jsonMessageExtractor.ExtractOnDeletePlayerMessage(message);
        MainThreadExecutor.Enqueue(() => mainControllerSc.OnDeletePlayer(deletePlayerMessage.Payload.Id));
    }
}
