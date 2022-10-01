using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using UnityEngine;

public class ServerPositionReporter : MonoBehaviour
{
    private ClientWebSocket websocket;
    public string server_address = "ws://localhost:8080";

    public float update_frequency = 1f;
    private float time_until_next_update = 0f;

    public int server_client_id = 0;

    async void Start()
    {
        websocket = new ClientWebSocket();
        await websocket.ConnectAsync(new System.Uri(server_address), CancellationToken.None);
    }

    void FixedUpdate()
    {
        if (time_until_next_update <= 0f)
            SendPositionUpdate();
        else
            time_until_next_update -= Time.fixedDeltaTime;
    }

    async void SendPositionUpdate()
    {
        // Reset our update countdown timer
        time_until_next_update = update_frequency;

        var position = transform.position;
        // { id: 1234, x: 12.953, y: 1.5, z: 15.245 }
        var message = string.Format("{{\"id\":{3}, \"x\":{0},\"y\":{1},\"z\":{2}}}", position.x, position.y, position.z, server_client_id);
        var buffer = System.Text.Encoding.UTF8.GetBytes(message);

        await websocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    async void OnDestroy()
    {
        await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye", CancellationToken.None);
    }

}
