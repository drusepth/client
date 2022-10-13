using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerUpdateListener : Singleton<ServerUpdateListener>
{
    public Transform player;
    public string local_player_id = null;

    public GameObject nearby_player_prefab;

    public void Start()
    {
        local_player_id = player.GetComponent<ServerPositionReporter>().player_id.ToString();
    }

    public void ProcessNewMessage(string full_message)
    {

        full_message = "{\"characters\":[{\"player_id\":2,\"position\":{\"x\":31.319707165172915,\"y\":72.94538403401285},\"inventory\":{}},{\"player_id\":1,\"position\":{\"x\":1.0,\"y\":1.0},\"inventory\":{}}]}";

        ServerGameState game_state = JsonUtility.FromJson<ServerGameState>(full_message);

        #region Example game state API response
        /*
            Example game state:
            {
               "characters":{
                  "2":{
                     "player_id":2,
                     "position":{
                        "x_coordinate":31.319707165172915,
                        "y_coordinate":72.94538403401285
                     },
                     "inventory":{
            
                     }
                  },
                  "1":{
                     "player_id":1,
                     "position":{
                        "x_coordinate":1.0,
                        "y_coordinate":1.0
                     },
                     "inventory":{
            
                     }
                  }
               },
               "ore":{
                  "3":{
                     "ore_id":3,
                     "ore_type":"SANDSTONE",
                     "amount":1,
                     "position":{
                        "x_coordinate":49.75192387745273,
                        "y_coordinate":48.914714319784004
                     }
                  },
                  "2":{
                     "ore_id":2,
                     "ore_type":"CRYSTAL",
                     "amount":1,
                     "position":{
                        "x_coordinate":50.3452697248149,
                        "y_coordinate":49.35169625717146
                     }
                  },
                  "1":{
                     "ore_id":1,
                     "ore_type":"CRYSTAL",
                     "amount":2,
                     "position":{
                        "x_coordinate":50.0,
                        "y_coordinate":50.0
                     }
                  },
                  "4":{
                     "ore_id":4,
                     "ore_type":"IRON",
                     "amount":2,
                     "position":{
                        "x_coordinate":49.804023924743056,
                        "y_coordinate":48.945807143160344
                     }
                  },
                  "5":{
                     "ore_id":5,
                     "ore_type":"DRAGONHIDE",
                     "amount":1,
                     "position":{
                        "x_coordinate":50.70805107137224,
                        "y_coordinate":48.65652471457309
                     }
                  }
               }
            }
        */
        #endregion

        // Update nearby players
        foreach (ServerPlayerData player_data in game_state.characters)
        {
            string player_id = player_data.player_id.ToString();

            if (player_id != local_player_id)
                UpdateNearbyPlayer(FindOrCreateOtherPlayerById(player_id), player_data);
        }
        
        // Update nearby objects
        // TODO
    }

    #region Helper Functions
    public GameObject FindOrCreateOtherPlayerById(string player_id)
    {
        // Find the GameObject in the OtherPlayersPool with the matching player_id
        GameObject found_player = null;
        GameObject player_pool = GameObject.Find("OtherPlayersPool");
        foreach (var position_reporter in player_pool.GetComponentsInChildren<ServerPositionReporter>())
            if (position_reporter.player_id.ToString() == player_id)
                return position_reporter.gameObject;

        // If the player doesn't exist yet, create a new one
        if (found_player == null)
        {
            found_player = Instantiate(nearby_player_prefab, Vector3.zero, Quaternion.identity);
            found_player.GetComponent<ServerPositionReporter>().player_id = int.Parse(player_id);
            found_player.transform.parent = player_pool.transform;
        }

        return found_player;
    }
    #endregion

    #region Update Types
    private void UpdateNearbyPlayer(GameObject target, ServerPlayerData data)
    {
        UpdateObjectPosition(target, data.position);
    }

    private void UpdateNearbyObject(GameObject target, ServerOreData data)
    {
        UpdateObjectPosition(target, data.position);
    }

    private void UpdateObjectPosition(GameObject target, Vector3 position)
    {
        target.transform.position = position;
    }
    #endregion
}