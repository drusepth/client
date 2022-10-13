using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClientMessage : MonoBehaviour
{
    public int player_id = 0;
    public int mine_id = 0;
    public Vector3 player_position;
}
