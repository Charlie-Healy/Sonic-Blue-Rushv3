using UnityEngine;

public abstract class PlayerAction : MonoBehaviour
{
    [SerializeField] protected PlayerPhysics playerPhysics;

    protected Rigidbody rb => playerPhysics.rb;

    protected PlayerPhysics.GroundInfo groundInfo => playerPhysics.groundInfo;
}
