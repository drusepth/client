using System;
using WebSocketSharp;
using UnityEngine;

public class ServerInterface : Singleton<ServerInterface>
{
    private readonly string server_address = "ws://2.tcp.ngrok.io:15248/join_game";

    public WebSocket web_socket;

    public void Start()
    {
        SetUpWebsocket();
    }

    public void SetUpWebsocket()
    {
        web_socket = new WebSocket(server_address);

        web_socket.OnMessage += (sender, d) =>
        {
            Debug.Log("Received message from server: " + d.Data);
            ServerGameState game_state = JsonUtility.FromJson<ServerGameState>(d.Data);
            ServerSyncManager.Instance.BlendGameState(game_state);
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
}
