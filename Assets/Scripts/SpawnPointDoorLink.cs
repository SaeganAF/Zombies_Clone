using UnityEngine;

public class SpawnPointDoorLink : MonoBehaviour
{
    [Tooltip("If assigned, this spawn point will only be used when the door is open.")]
    public BuyableDoor linkedDoor;
}
