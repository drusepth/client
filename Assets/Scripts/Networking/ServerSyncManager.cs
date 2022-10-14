using ServerMessages;
using UnityEngine;

public class ServerSyncManager : Singleton<ServerSyncManager>
{
    public Transform player;
    public int local_player_id = 0;

    public GameObject nearby_player_prefab;
    public GameObject ore_prefab;

    public void Start()
    {
        local_player_id = player.GetComponent<ServerPositionReporter>().player_id;
    }

    public void ProcessServerMessage(string full_message)
    {
        // TODO we probably want to branch here on message_type:
        // * authentication response
        // * game state blend updates
        // * messages from other players
        // * items added/removed from inventory, etc

        ServerGameState game_state = JsonUtility.FromJson<ServerGameState>(full_message);
        BlendGameState(game_state);
    }

    public void ProcessServerPing()
    {
        Debug.Log("Received PING from server -- we're still connected!");
    }

    // Rather than loading a game state from scratch, this adds/updates all the data in
    // game_state into our current game state
    public void BlendGameState(ServerGameState game_state)
    {
        foreach (ServerPlayerData player_data in game_state.characters)
        {
            int player_id = player_data.player_id;
            if (player_id != local_player_id)
                UpdateNearbyPlayer(FindOrCreateOtherPlayerById(player_id), player_data);
        }

        // Update nearby objects
        foreach (ServerOreData ore_data in game_state.ore)
        {
            GameObject ore = FindOrCreateObjectById(ore_data.ore_id, ore_prefab);
            UpdateObjectPosition(ore, ore_data.position);
        }
    }

    #region Helper Functions
    public GameObject FindOrCreateOtherPlayerById(int player_id)
    {
        // Find the GameObject in the OtherPlayersPool with the matching player_id
        GameObject player_pool = GameObject.Find("OtherPlayersPool");
        foreach (var position_reporter in player_pool.GetComponentsInChildren<ServerPositionReporter>())
            if (position_reporter.player_id == player_id)
                return position_reporter.gameObject;

        // If the player doesn't exist yet, create a new one
        GameObject found_player = Instantiate(nearby_player_prefab, Vector3.zero, Quaternion.identity);
        found_player.GetComponent<ServerPositionReporter>().player_id = player_id;
        found_player.transform.parent = player_pool.transform;

        return found_player;
    }

    public GameObject FindOrCreateObjectById(int object_id, GameObject prefab)
    {
        // Find the GameObject in the ObjectPool with the matching id
        GameObject object_pool = GameObject.Find("ObjectPool");
        foreach (var uoid in object_pool.GetComponentsInChildren<UniqueObjectIdentifier>())
            if (uoid.uoid == object_id)
                return uoid.gameObject;

        // If the object doesn't exist yet, create a new one
        GameObject new_obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        new_obj.GetComponent<UniqueObjectIdentifier>().uoid = object_id;
        new_obj.transform.parent = object_pool.transform;

        return new_obj;
    }
    #endregion

    #region Update Actions
    private void UpdateNearbyPlayer(GameObject target, ServerPlayerData data)
    {
        UpdateObjectPosition(target, data.position);
    }

    private void UpdateObjectPosition(GameObject target, Vector3 position)
    {
        target.transform.position = position;
    }
    #endregion
}