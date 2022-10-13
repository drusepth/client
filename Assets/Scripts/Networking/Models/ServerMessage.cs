using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class ServerGameState
{
    public Dictionary<string, ServerPlayerData> characters;
    public Dictionary<string, ServerOreData> ore;
}

[System.Serializable]
class ServerPlayerData
{
    public int player_id;
    public Vector3 position;
    public Inventory inventory;
}

[System.Serializable]
class ServerOreData
{
    public int ore_id;
    public string ore_type;
    public int amount;
    public Vector3 position;
}

[System.Serializable]
class Inventory
{
    // TODO
}