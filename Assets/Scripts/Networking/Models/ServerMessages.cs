using NetworkEntityRepresentations;
using System.Collections.Generic;
using UnityEngine;

namespace ServerToClientMessages
{
    [System.Serializable] public class ServerGameState
    {
        public List<Player> characters;
        public List<Ore> ore;
    }
}

namespace ClientToServerMessages
{
    // old update style
    [System.Serializable] public class ClientGameStateUpdate
    {
        public int player_id = 0;
        public int mine_id = 0;
        public Vector3 player_position;
    }

    [System.Serializable] public class ClientPlayerStateUpdate
    {
        public int player_id = 0;
        public Vector3 position;
        public Vector3 rotation;
    }

    [System.Serializable] public class PlayerStartInteraction
    {
        // TODO
    }

    [System.Serializable] public class PlayerStopInteraction
    {
        // TODO
    }
}

namespace NetworkEntityRepresentations
{
    [System.Serializable] public class Player
    {
        public int player_id;
        public Vector3 position;
        public Inventory inventory;
    }

    [System.Serializable] public class Ore
    {
        public int ore_id;
        public string ore_type;
        public int amount;
        public Vector3 position;
        public Vector3 rotation;
    }

    [System.Serializable] public class Inventory
    {
        // TODO
    }
}
