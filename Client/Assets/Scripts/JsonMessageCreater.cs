using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPC = WebSocketSample.RPC;

public class JsonMessageCreater
{
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
