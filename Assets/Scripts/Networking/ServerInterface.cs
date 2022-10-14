using ServerMessages;
using WebSocketSharp;
using UnityEngine;
using System.Collections.Concurrent;

public class ServerInterface : Singleton<ServerInterface>
{
    private readonly string server_address = "ws://2.tcp.ngrok.io:16769/join_game";

    public WebSocket web_socket;
    public LogLevel socket_loglevel = LogLevel.Trace;

    public ConcurrentQueue<string> incoming_server_messages;

    public void SetUpWebsocket()
    {
        incoming_server_messages = new ConcurrentQueue<string>();
        
        web_socket = new WebSocket(server_address);
        web_socket.EmitOnPing = true; // triggers OnMessage for ping events
        web_socket.Log.Level = socket_loglevel;

        web_socket.OnOpen += (sender, e) =>
        {
            AttemptAuthenticationAs("player name");
        };

        web_socket.OnMessage += (sender, e) =>
        {
            Debug.Log("Received message from server: " + e.Data);
            if (e.IsPing)
            {
                Debug.Log("got ping");
                ServerSyncManager.Instance.ProcessServerPing();
            }
            else
            {
                Debug.Log("got game message");
                Debug.Log(e.Data);
                incoming_server_messages.Enqueue(e.Data);
                Debug.Log(incoming_server_messages.Count + " messages to process");
                // ServerSyncManager.Instance.ProcessServerMessage(e.Data);
                Debug.Log("found singleton instance");
            }
        };

        /*
        web_socket.OnError += (sender, e) =>
        {
            Debug.Log($"#{e.Exception} error received from server: {e.Message}");
        };
        */

        web_socket.OnClose += (sender, e) =>
        {
            Debug.Log($"Websocket has been closed! {e.Reason} ({e.Code})");
        };

        // TODO we probably want to call this on game start instead of right before
        // sending client state, so we can use ConnectAsync (and not lock the main thread)
        // and just bank on it not being used immediately
        web_socket.Connect();
    }

    #region Public API methods for the game to interact with the server
    public void SendClientState(int player_id, float x, float y, float z)
    {
        if (web_socket == null)
            SetUpWebsocket();

        if (web_socket.ReadyState == WebSocketState.Open)
        {
            ClientGameStateUpdate state = new()
            {
                player_id = player_id,
                mine_id = 0,
                player_position = new Vector3(x, y, z)
            };

            string json_state = JsonUtility.ToJson(state);
            // TODO we should probably switch to SendAsync since we don't need an ACK here
            web_socket.Send(json_state);
            Debug.Log("game state sent successfully!");
        }
        else
        {
            // TODO queue a most recent state to send if there are internet hiccups?
            Debug.Log("Websocket state is not open, so we cannot send state.");
        }
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
