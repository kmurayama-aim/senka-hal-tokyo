﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSenderToServer
{
    WebSocketInitializer socketInitializer = new WebSocketInitializer();
    JsonMessageCreater jsonMessageCreater = new JsonMessageCreater();

    public void SendLoginMessage(string connectAddress, string playerName)
    {
        socketInitializer.Initialize(connectAddress);

        var jsonMessage = jsonMessageCreater.CreateLoginMessage(playerName);
        Debug.Log(jsonMessage);

        socketInitializer.Send(jsonMessage);
        Debug.Log(">> Login");
    }

    public void SendPlayerOnCollisionMessage(int playerId, int otherPlayerId)
    {
        var collisionJson = jsonMessageCreater.CreateCollisionMessage(playerId, otherPlayerId);
        socketInitializer.Send(collisionJson);
    }

    public void SendUpdatePositionMessage(Vector3 currentPlayerPosition, int playerId)
    {
        var jsonMessage = jsonMessageCreater.CreatePlayerUpdateMessage(currentPlayerPosition, playerId);
        Debug.Log(jsonMessage);
        socketInitializer.Send(jsonMessage);
    }

    public void SendItemOnGetMessage(int itemId, int playerId)
    {
        var getItemJson = jsonMessageCreater.CreateGetItemMessage(itemId, playerId);
        socketInitializer.Send(getItemJson);
        Debug.Log(">> GetItem");
    }
    public void SendLogoutMessage()
    {
        socketInitializer.Close();
    }
}
