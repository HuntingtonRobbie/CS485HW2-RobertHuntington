using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to: the TrackManagerScipt
//Function: position tiles to form the track

//For simplicity, the player will be at x=0, y=0 upon starts and each tile is 2 meter's long along z axis

public class TrackManager : MonoBehaviour
{
    //the list that saves all the tile prefabs for generating endless path
    public List<GameObject> TilePrefabs;

    //how many tiles you want to have that lays in front of the player
    public int tileCountInFront = 5;
    //how many tiles you want to have that lays in front of the player
    public int tileCountBehind = 2;

    //saves the player object
    public GameObject player;

    //the list that saves all a pool of tiles to reuse.
    [SerializeField]
    private List<GameObject> TilePool = new List<GameObject>();

    //the list that saves all the existing tiles.
    [SerializeField]//this tag makes your private variable also viewable in the editor for easier debugging
    private List<GameObject> Tiles=new List<GameObject>();

    //saves the total number of tiles generated
    [SerializeField] 
    private int numTiles=0;
    //saves the tile has the player on it
    [SerializeField]
    private int curTileWithPlayer;

    //saves the tile length on z direction
    //for easier coding, it is important to make all the parent/manager objects with scale (1,1,1) so that the child obj's scale does not get modified
    float tileLenZ;
    float nextTilePosZ;


    // Start is called before the first frame update
    void Start()
    {
        //init all your variables
        if (TilePrefabs.Count == 0)
            Debug.LogError("No Tile Prefabs assigned to the Track Manger");
        if (player==null)
            Debug.LogError("Assign player obj to the TackManager obj");
        tileLenZ = TilePrefabs[0].transform.Find("Ground").localScale.z;

        LayTilesAtBeginning();
        curTileWithPlayer = PlayerOnTileIndex();

    }

    void GenerateTilePool()
    {

    }
    //position tileCountBehind+tileCountInFront number of tiles to form the initial path
    void LayTilesAtBeginning()
    {
        int playerTileLocationIndex = PlayerOnTileIndex();

        nextTilePosZ = -tileLenZ/2-(tileCountBehind-1)*tileLenZ;//location of the farest tile behind the player

        //Lay tiles  the player object
        for(int tileCn=0; tileCn<tileCountBehind+ tileCountInFront; tileCn++)
        {
            AddATile();
        }
       

    }

    //Find out which tile the player is on
    //Each tile is 2m long in Z. The 1st tile will be at z=0-2,  2nd tile be at z=2-4...
    int PlayerOnTileIndex ()
    {
        int index = 0;
        float playerZpos = player.transform.position.z;
        

        index = Mathf.FloorToInt(playerZpos / tileLenZ);
        return index;
    }

    //delete a tile at the end of the list
    void DeleteEnd()
    {
        GameObject TileToRemove = Tiles[0];
        Tiles.RemoveAt(0);
        GameObject.Destroy(TileToRemove);
    }

    void AddATile()
    {
        //get a random tile prefab index
        int randomeTileIndexToInit = Random.Range(0, TilePrefabs.Count);
        //crate a new tile by making a copy of the saved prefab object
        GameObject newTile = Instantiate(TilePrefabs[randomeTileIndexToInit]);
        //position the tile
        newTile.transform.position = new Vector3(0f, 0f, nextTilePosZ);
        //make the tile object the child object of the track manager
        newTile.transform.parent = this.transform;
        numTiles++;

        newTile.transform.name = numTiles.ToString() + newTile.transform.name;
        //add this tile to the list of tiles
        Tiles.Add(newTile);
        
        nextTilePosZ += tileLenZ;
    }

    public void TileAdjustment(GameObject ExitTile)
    {
        int curTile = PlayerOnTileIndex();
        Debug.Log("Player is on Tile Number: "+curTile.ToString());
        AddATile();
        DeleteEnd();
    }
}
