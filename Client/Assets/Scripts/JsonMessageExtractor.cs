using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPC = WebSocketSample.RPC;

public class JsonMessageExtractor {

    public RPC.Header ExtractHeaderMessage(string jsonMessage)
    {
        var header = JsonUtility.FromJson<RPC.Header>(jsonMessage);
        return header;
    }

    public RPC.Ping ExtractPingMessage(string jsonMessage)
    {
        var pong = JsonUtility.FromJson<RPC.Ping>(jsonMessage);
        return pong;
    }

    public RPC.LoginResponse ExtractOnLoginResponseMessage(string jsonMessage)
    {
        var loginResponse = JsonUtility.FromJson<RPC.LoginResponse>(jsonMessage);
        return loginResponse;
    }

    public RPC.Sync ExtractOnSyncMessage(string jsonMessage)
    {
        var syncMessage = JsonUtility.FromJson<RPC.Sync>(jsonMessage);
        return syncMessage;
    }

    public RPC.Spawn ExtractOnSpawnMessage(string jsonMessage)
    {
        var spawnResponse = JsonUtility.FromJson<RPC.Spawn>(jsonMessage);
        return spawnResponse;
    }

    public RPC.DeleteItem ExtractOnDeleteItemMessage(string jsonMessage)
    {
        var deleteMessage = JsonUtility.FromJson<RPC.DeleteItem>(jsonMessage);
        return deleteMessage;
    }

    public RPC.Environment ExtractOnEnvironmentMessage(string jsonMessage)
    {
        var environmentMessage = JsonUtility.FromJson<RPC.Environment>(jsonMessage);
        return environmentMessage;
    }

    public RPC.DeletePlayer ExtractOnDeletePlayerMessage(string jsonMessage)
    {
        var deletePlayerMessage = JsonUtility.FromJson<RPC.DeletePlayer>(jsonMessage);
        return deletePlayerMessage;
    }
}
