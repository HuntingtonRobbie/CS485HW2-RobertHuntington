using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Attach to the Track objects' TileExit collider.
//Set those colliders as Triggers.
public class TileExit : MonoBehaviour
{
    [SerializeField]
    GameObject TrackManager;

    //Some times 1 collider might be triggered more than once
    bool curTileFirstHit = true;

    void Start()
    {
        TrackManager = GameObject.Find("Track Manager");
    }
    //Unity will call this function when a collider hits the collider of the TileExit object
    void OnTriggerExit(Collider other)
    {
        if (TrackManager != null && other.name == "Player" && curTileFirstHit==true) //if the player hits the end of a tile
        {
            //find the TrackManager.cs script and call its public function
            TrackManager.gameObject.GetComponent<TrackManager>().TileAdjustment(this.gameObject);
            curTileFirstHit = false;
        }
    }

    //public void TileAdjustmentDone()
    //{
    //    curTileFisstHit = true;
    //}
}
