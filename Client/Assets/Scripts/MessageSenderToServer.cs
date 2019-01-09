using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSenderToServer
{
    WebSocketInitializer socketInitializer = new WebSocketInitializer();
    JsonMessageFormatter jsonMessageFormatter = new JsonMessageFormatter();

    public void SendLoginMessage(string connectAddress, string playerName)
    {
        socketInitializer.Initialize(connectAddress);

        var jsonMessage = jsonMessageFormatter.FormatLoginMessage(playerName);
        Debug.Log(jsonMessage);

        socketInitializer.Send(jsonMessage);
        Debug.Log(">> Login");
    }

    public void SendPlayerOnCollisionMessage(int playerId, int otherPlayerId)
    {
        var collisionJson = jsonMessageFormatter.FormatCollisionMessage(playerId, otherPlayerId);
        socketInitializer.Send(collisionJson);
    }

    public void SendUpdatePositionMessage(Vector3 currentPlayerPosition, int playerId)
    {
        var jsonMessage = jsonMessageFormatter.FormatPlayerUpdateMessage(currentPlayerPosition, playerId);
        Debug.Log(jsonMessage);
        socketInitializer.Send(jsonMessage);
    }

    public void SendItemOnGetMessage(int itemId, int playerId)
    {
        var getItemJson = jsonMessageFormatter.FormatGetItemMessage(itemId, playerId);
        socketInitializer.Send(getItemJson);
        Debug.Log(">> GetItem");
    }
    public void SendLogoutMessage()
    {
        socketInitializer.Close();
    }
}
