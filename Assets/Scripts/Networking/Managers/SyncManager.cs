using System.Collections;
using UnityEngine;

public class SyncManager : Singleton<SyncManager>
{
    public static void UpdateObjectPosition(int object_id, Vector3 position)
    {
        // TODO: find object by object_id
        GameObject found_object = null;

        if (found_object != null)
            found_object.transform.position = position;
    }
}