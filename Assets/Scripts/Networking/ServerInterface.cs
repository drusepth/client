using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WebSocketSharp;
using UnityEngine;
using WebSocketState = System.Net.WebSockets.WebSocketState;
using WebSocket = WebSocketSharp.WebSocket;

public class ServerInterface : Singleton<ServerInterface>
{
    // TODO: handle ws/wss encryption?
    private readonly string server_address = "ws://2.tcp.ngrok.io:15248/join_game";

    public ClientWebSocket raw_socket;
    public WebSocket web_socket;

    // Process any updates from the server <3    
    //public async void FixedUpdate() {
    //    await ReadWebsocket();
    //}

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

    #region Low-level socket IO
    public async Task OpenWebsocket()
    {
        Debug.Log("Trying to open websocket...");

        // If we don't already have an open socket, open one
        if (raw_socket == null || raw_socket.State != WebSocketState.Open)
        {
            raw_socket = new ClientWebSocket();
            await raw_socket.ConnectAsync(new Uri(server_address), CancellationToken.None);
        }
    }

    private async Task ReadWebsocket()
    {
        // If we don't have an open socket, open one (or maybe error?)
        if (raw_socket == null || raw_socket.State != WebSocketState.Open)
            await OpenWebsocket();

        try
        {
            if (raw_socket.State == WebSocketState.Open)
            {
                Debug.Log("Websocket is still open :)");

                // Read from the socket until it closes
                ArraySegment<byte> buffer = new ArraySegment<byte>(new Byte[8192]);

                // Keep reading until we get an EndOfMessage flag on the buffer
                using var memstream = new MemoryStream();
                WebSocketReceiveResult result;
                do
                {
                    result = await raw_socket.ReceiveAsync(buffer, CancellationToken.None);
                    memstream.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                memstream.Seek(0, SeekOrigin.Begin);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    using var reader = new StreamReader(memstream, Encoding.UTF8);
                    var full_message = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, result.Count);
                    ServerUpdateListener.Instance.ProcessNewMessage(full_message);
                }
            } else
            {
                Debug.Log("Websocket is closed :(");
            }
        }
        catch (InvalidOperationException)
        {
            Debug.Log("[WS] Tried to receive message while already reading one.");
        }
    }

    private void SendMessageToServer(string message)
    {
        /*
        // Open a socket if we don't have one
        if (raw_socket == null || raw_socket.State != WebSocketState.Open)
            await OpenWebsocket();

        // Send the message
        var buffer = Encoding.UTF8.GetBytes(message);
        await raw_socket.SendAsync(
            new ArraySegment<byte>(buffer),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
        // Debug.Log("Data sent:");
        // Debug.Log(message);
        */
    }
    #endregion
}
