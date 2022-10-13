using UnityEngine;

public class UniqueObjectIdentifier : MonoBehaviour
{
    public enum IdentifierScope
    {
        Global,
        Player,
        Ore
    };

    public IdentifierScope scope;
    public int uoid;
}
