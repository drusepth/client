using System;
using WebSocketSharp;
using UnityEngine;

public class ServerInterface : Singleton<ServerInterface>
{
    private readonly string server_address = "ws://2.tcp.ngrok.io:15248/join_game";

    public WebSocket web_socket;

    public void Start() => SetUpWebsocket();

    public void SetUpWebsocket()
    {
        web_socket = new WebSocket(server_address);
        web_socket.EmitOnPing = true; // triggers OnMessage for ping events

        web_socket.OnOpen += (sender, e) =>
        {
            AttemptAuthenticationAs("player name");
        };

        web_socket.OnMessage += (sender, e) =>
        {
            Debug.Log("Received message from server: " + e.Data);
            if (e.IsPing)
                ServerSyncManager.Instance.ProcessServerPing();
            else
                ServerSyncManager.Instance.ProcessServerMessage(e.Data);
        };

        web_socket.OnError += (sender, e) =>
        {
            Debug.Log("Error received from server:");
            Debug.Log(e.Message);
            Debug.Log(e.Exception);
        };

        web_socket.OnClose += (sender, e) =>
        {
            Debug.Log("Websocket has been closed!");
            Debug.Log("Code: " + e.Code);
            Debug.Log("Reason: " + e.Reason);
        };

        web_socket.Connect();
    }

    #region Public API methods for the game to interact with the server
    public void SendClientState(int player_id, float x, float y, float z)
    {
        ClientMessage state = new()
        {
            player_id = player_id,
            mine_id = 0,
            player_position = new Vector3(x, y, z)
        };

        string json_state = JsonUtility.ToJson(state);
        try
        {
            // TODO we should probably switch to SendAsync since we don't need an ACK here
            web_socket.Send(json_state);
        }
        catch (NullReferenceException)
        {
            Debug.Log("null reference on sending state");
            Debug.Log(json_state);
        }        
        Debug.Log("game state sent successfully!");
    }
    #endregion

    #region Private API methods for the game to interact with the server
    private void AttemptAuthenticationAs(string username)
    {
        // TODO send our username (and password?) to server
        // TODO get back a player_id to set (handled by message-receive tho)
    }
    #endregion
}
