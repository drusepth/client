using UnityEngine;

public class ServerPositionReporter : MonoBehaviour
{
    public float update_frequency = 0.05f;
    private float time_until_next_update = 1f;

    public int player_id = 0;

    void FixedUpdate()
    {
        if (time_until_next_update <= 0f)
            SendPositionUpdate();
        else
            time_until_next_update -= Time.fixedDeltaTime;
    }

    void SendPositionUpdate()
    {
        // Reset our update countdown timer
        time_until_next_update = update_frequency;

        // Debug.Log("Broadcasting position update to server");
        ServerInterface.Instance.SendPlayerState(player_id, transform.position, transform.rotation);
    }
}
