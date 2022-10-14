using System.Collections.Generic;
using UnityEngine;

namespace ServerMessages
{
    [System.Serializable]
    public class ServerGameState
    {
        public List<ServerPlayerData> characters;
        public List<ServerOreData> ore;
    }

    [System.Serializable]
    public class ServerPlayerData
    {
        public int player_id;
        public Vector3 position;
        public Inventory inventory;
    }

    [System.Serializable]
    public class ServerOreData
    {
        public int ore_id;
        public string ore_type;
        public int amount;
        public Vector3 position;
    }

    [System.Serializable]
    public class Inventory
    {
        // TODO
    }

    [System.Serializable]
    public class ClientGameStateUpdate
    {
        public int player_id = 0;
        public int mine_id = 0;
        public Vector3 player_position;
    }
}

