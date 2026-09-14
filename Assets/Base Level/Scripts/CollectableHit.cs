using UnityEngine;

// Function: report a HIT to the CollectableManager when the Player enters this object's trigger.
// The manager fills in 'index' and 'manager' when it spawns each collectable.
public class CollectableHit : MonoBehaviour
{
    public int index;                              // this collectable's "No." (visible in the Inspector)
    [HideInInspector] public CollectableManager manager;

    // A trigger can fire more than once; report only the first 
    private bool reported = false;

    void OnTriggerEnter(Collider other)
    {
        if (reported || other.name != "Player") return;
        reported = true;

        if (manager != null)
            manager.RegisterHit(this.gameObject, index);
        else
            Debug.LogError("CollectableHit on " + name + " has no manager assigned");
    }
}
