using UnityEngine;

public class ServerPositionReporter : MonoBehaviour
{
    public float update_frequency = 1f;
    private float time_until_next_update = 0f;

    public int player_id = 0;

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

        Debug.Log("Broadcasting position update to server");
        await ServerManager.Instance.ReportPlayerPosition(
            player_id,
            transform.position.x, transform.position.y, transform.position.z
        ).ConfigureAwait(false);
    }
}
