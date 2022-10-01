using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

public class ServerManager : Singleton<ServerManager>
{    
    private readonly string server_address = "ws://localhost:8080";

    public ClientWebSocket raw_socket;

    public async Task OpenWebsocket()
    {
        // If we don't already have an open socket, open one
        if (raw_socket == null || raw_socket.State != WebSocketState.Open)
        {
            raw_socket = new ClientWebSocket();
            await raw_socket.ConnectAsync(new System.Uri(server_address), CancellationToken.None);
        }
    }

    public async Task ReportPlayerPosition(int player_id, float x, float y, float z)
    {
        var message = string.Format(
            "{{\"id\":{0}, \"x\":{1},\"y\":{2},\"z\":{3}}}",
            player_id, x, y, z
        );
        await SendData(message).ConfigureAwait(false);
    }
    
    private async Task SendData(string message)
    {
        // Open a socket if we don't have one
        if (raw_socket == null || raw_socket.State != WebSocketState.Open)
            await OpenWebsocket();

        // Send the message
        var buffer = System.Text.Encoding.UTF8.GetBytes(message);
        await raw_socket.SendAsync(
            new ArraySegment<byte>(buffer), 
            WebSocketMessageType.Text, 
            true,
            CancellationToken.None
        );
    }
}
