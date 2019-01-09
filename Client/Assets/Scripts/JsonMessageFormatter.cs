using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPC = WebSocketSample.RPC;

public class JsonMessageFormatter
{
    public string FormatLoginMessage(string message)
    {
        var jsonMessage = JsonUtility.ToJson(new RPC.Login(new RPC.LoginPayload(message)));
        return jsonMessage;
    }
    public string FormatCollisionMessage(int playerId, int otherPlayerId)
    {
        var collisionRpc = new RPC.Collision(new RPC.CollisionPayload(playerId, otherPlayerId));
        var jsonMessage = JsonUtility.ToJson(collisionRpc);
        return jsonMessage;
    }
    public string FormatPlayerUpdateMessage(Vector3 pos, int playerId)
    {
        var rpcPosition = new RPC.Position(pos.x, pos.y, pos.z);
        var jsonMessage = JsonUtility.ToJson(new RPC.PlayerUpdate(new RPC.PlayerUpdatePayload(playerId, rpcPosition)));
        return jsonMessage;
    }
    public string FormatGetItemMessage(int itemId, int playerId)
    {
        var getItemRpc = new RPC.GetItem(new RPC.GetItemPayload(itemId, playerId));
        var jsonMessage = JsonUtility.ToJson(getItemRpc);
        return jsonMessage;
    }
}
