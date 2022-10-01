using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerUpdateListener : Singleton<ServerUpdateListener>
{
    void ProcessNewMessage(string full_message)
    {
        // TODO: implement deserializable ServerMessage class
        var message = JsonUtility.FromJson<ServerMessage>(full_message);

        switch (message.type)
        {
            case "nearby_players":
                UpdateNearbyPlayers(message.data);
                break;

            default:
                Debug.Log("Unknown message type from server: " + message.type);
                break;
        }
    }

    #region Update Types
    void UpdateNearbyPlayers(object message_data)
    {
        foreach (object player_position in message_data)
        {
            var object_id = (int)player_position["id"];
            var player_x = (float)player_position["x"];
            var player_y = (float)player_position["y"];
            var player_z = (float)player_position["z"];
            var position = new Vector3(player_x, player_y, player_z);

            SyncManager.UpdateObjectPosition(object_id, position);
        }
    }

    #endregion
}