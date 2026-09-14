using System.Collections.Generic;
using UnityEngine;
using TMPro;                                   

// Function: keep an endless stream of collectables ahead of the player.
//   HIT  — reported by CollectableHit on each collectable when the Player enters its trigger.
//          The collectable is destroyed on contact and scores a point.
//   MISS — detected here: the player's Z has passed the nearest collectable's Z by missMargin.
//          The collectable stays visible and is only destroyed once it is despawnBehind behind the
//          player — the same way TrackManager drops tiles only after they are out of view.
public class CollectableManager : MonoBehaviour
{
    // ---- Assign in the Inspector ----
    public GameObject collectablePrefab;
    public GameObject player;
    public TMP_Text scoreText;                 // drag the ScoreText UI object here

    // ---- Tuning (public = editable in the Inspector; Inspector values override these defaults) ----
    public int   countInFront  = 3;     // collectables kept ahead of the player at all times (HW needs >= 2)
    public float minGap        = 3f;    // random Z distance between consecutive collectables
    public float maxGap        = 4f;    // keep countInFront * maxGap <= 14 so spawns always land on laid track
    public float firstZ        = 4f;    // Z of the very first collectable (player starts at Z = 0)
    public float spawnY        = 0.25f; // overlaps the player's sphere collider (Y 0.10 - 0.40)
    public float missMargin    = 0.5f;  // player must be this far past a collectable's Z to count as a miss
                                        // (contact is still possible up to ~0.31; keep this above that)
    public float despawnBehind = 4f;    // a missed collectable is destroyed this far behind the player
                                        //         (camera sits 3 behind; tiles are dropped ~4 behind)
    public int   pointsPerHit  = 1;     

    // ---- Runtime state ([SerializeField] = visible in the Inspector for debugging) ----
    [SerializeField] private List<GameObject> collectables       = new List<GameObject>(); // ahead of player, nearest first
    [SerializeField] private List<GameObject> missedCollectables = new List<GameObject>(); // behind player, nearest first
    [SerializeField] private int numCollectables = 0; // running counter; doubles as each collectable's "No."
    [SerializeField] private int hits   = 0;
    [SerializeField] private int misses = 0;
    [SerializeField] private int score  = 0;          

    private float nextSpawnZ;

    void Start()
    {
        if (collectablePrefab == null) Debug.LogError("Assign the collectable prefab to the Collectable Manager");
        if (player == null)            Debug.LogError("Assign the Player obj to the Collectable Manager");

        nextSpawnZ = firstZ;
        for (int i = 0; i < countInFront; i++)
            SpawnCollectable();

        UpdateScoreText();                     
    }

    void Update()
    {
        float playerZ = player.transform.position.z;   // shared by both loops

        // Miss check. The list is ordered by Z, so only the nearest collectable can be passed.
        // 'while' instead of 'if' so a frame hitch can never skip one.
        while (collectables.Count > 0 &&
               playerZ > collectables[0].transform.position.z + missMargin)
        {
            GameObject m = collectables[0];
            collectables.RemoveAt(0);
            misses++;
            Debug.Log("MISS No. " + m.GetComponent<CollectableHit>().index + " collectable");
            missedCollectables.Add(m);         
            SpawnCollectable();                
            UpdateScoreText();                 
        }

        // destroy missed collectables only once they are fully behind the camera
        while (missedCollectables.Count > 0 &&
               playerZ > missedCollectables[0].transform.position.z + despawnBehind)
        {
            Destroy(missedCollectables[0]);
            missedCollectables.RemoveAt(0);
        }
    }

    // Called by CollectableHit when the Player enters a collectable's trigger.
    public void RegisterHit(GameObject collectable, int index)
    {
        if (!collectables.Remove(collectable)) return; 
        hits++;
        score += pointsPerHit;                 // score counter
        Debug.Log("HIT No. " + index + " collectable");
        Replace(collectable);                  // hit collectables vanish on contact
        UpdateScoreText();                     // update score for hit collectables
    }

    
    void SpawnCollectable()
    {
        numCollectables++;

        // Random start X inside the limits CollectableMove already knows about
        CollectableMove prefabMove = collectablePrefab.GetComponent<CollectableMove>();
        float x = Random.Range(prefabMove.leftLimitX, prefabMove.rightLimitX);

        Vector3 pos = new Vector3(x, spawnY, nextSpawnZ);
        GameObject c = Instantiate(collectablePrefab, pos, Quaternion.identity);
        c.transform.parent = this.transform;
        c.name = "Collectable " + numCollectables;

        // Hand the new collectable its number and a reference back to this manager
        CollectableHit hit = c.GetComponent<CollectableHit>();
        hit.index   = numCollectables;
        hit.manager = this;

        // Random starting direction (Start() on CollectableMove has not run yet, so this takes effect)
        c.GetComponent<CollectableMove>().startDirection = (Random.value < 0.5f) ? 1 : -1;

        collectables.Add(c);
        nextSpawnZ += Random.Range(minGap, maxGap);
    }

    // Destroy hit collectables
    void Replace(GameObject c)
    {
        Destroy(c);
        SpawnCollectable();
    }

    // Hit Counter
    void UpdateScoreText()
    {
        if (scoreText == null) return;         // UI is optional; the game runs without it
        scoreText.text = "Score: " + score + "   Missed: " + misses;   // drop the Missed part if you only want the score
    }
}