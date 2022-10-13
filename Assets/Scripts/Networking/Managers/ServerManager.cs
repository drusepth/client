using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ServerManager : Singleton<ServerManager>
{
    // TODO: handle ws/wss encryption?
    private readonly string server_address = "wss://demo.piesocket.com/v3/channel_1?api_key=VCXCEuvhGcBDP7XhiJJUDvR1e1D3eiVjgZ9VRiaV&notify_self";

    public ClientWebSocket raw_socket;

    public void FixedUpdate()
    {
        Debug.Log("Updating game state from server");
        // ReadWebsocket();

        ServerUpdateListener.Instance.ProcessNewMessage("");
    }

    #region Low-level socket IO
    public async Task OpenWebsocket()
    {
        // If we don't already have an open socket, open one
        if (raw_socket == null || raw_socket.State != WebSocketState.Open)
        {
            raw_socket = new ClientWebSocket();
            await raw_socket.ConnectAsync(new System.Uri(server_address), CancellationToken.None);
        }
    }

    private async Task ReadWebsocket()
    {
        // If we don't have an open socket, open one (or maybe error?)
        if (raw_socket == null || raw_socket.State != WebSocketState.Open)
            await OpenWebsocket();

        // Read from the socket until it closes
        ArraySegment<byte> buffer = new ArraySegment<byte>(new Byte[8192]);
        WebSocketReceiveResult result = null;

        // Keep reading until we get an EndOfMessage flag on the buffer
        using var memstream = new MemoryStream();
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
    }

    private async Task SendData(string message)
    {
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
        Debug.Log("Data sent:");
        Debug.Log(message);
    }
    #endregion

    #region Public API methods for the game to interact with the server
    public async Task ReportPlayerPosition(int player_id, float x, float y, float z)
    {
        ClientMessage state = new ClientMessage();
        state.player_id = player_id;
        state.mine_id = 0;
        state.player_position = new Vector3(x, y, z);

        string json_state = JsonUtility.ToJson(state);
        Debug.Log(json_state);
        await SendData(json_state).ConfigureAwait(false);
    }
    #endregion
}
