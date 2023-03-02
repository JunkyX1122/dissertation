using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>();
    public Material material;
    private int chunkNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        int testSize = 16;
        for (int x = 0; x < testSize; x++)
        {
            for (int z = 0; z < testSize; z++)
            {
                for (int y = 0; y < testSize; y++)
                {
                    SpawnChunk(new Vector3(x * 16, y * 16, z * 16), chunkNum);
                    chunkNum++;
                }
            }
        }

        foreach (KeyValuePair<Vector3, Chunk> chunkUpdate in chunks)
        {
            chunkUpdate.Value.UpdateChunk();
        }
        
    }

    private void SpawnChunk(Vector3 pos, int id)
    {
        GameObject newChunk = new GameObject("chunk_" + id.ToString());
        newChunk.transform.parent = this.transform; 
        Chunk chunkTest = newChunk.AddComponent<Chunk>();
        chunkTest.InitialiseChunk(0, pos, material, this, id);
        chunks.Add(pos, chunkTest);
        UpdateAdjactentChunkStore(pos, id, chunkTest);
        


    }

    private void UpdateAdjactentChunkStore(Vector3 pos, int id, Chunk chunkTest)
    {
        Debug.Log("CURRENT CHUNK ID: " + chunkTest.chunkID);
        
        Vector3 chunkKeyCheck = pos + VoxelConstants.FaceFront * VoxelConstants.ChunkSize;
        Debug.Log("TEST IF CHUNK EXISTS AT " + chunkKeyCheck);
        if (chunks.ContainsKey(chunkKeyCheck))
        {
            chunkTest.chunkInFront = chunks[chunkKeyCheck];
            chunks[chunkKeyCheck].chunkInBack = chunkTest;
            Debug.Log("CURRENT FRONT CHUNK ID: " + chunkTest.chunkInFront.chunkID);
        }

        chunkKeyCheck = pos + VoxelConstants.FaceBack * VoxelConstants.ChunkSize;
        Debug.Log("TEST IF CHUNK EXISTS AT " + chunkKeyCheck);
        if (chunks.ContainsKey(chunkKeyCheck))
        {
            chunkTest.chunkInBack = chunks[chunkKeyCheck];
            chunks[chunkKeyCheck].chunkInFront = chunkTest;
            Debug.Log("CURRENT BACK CHUNK ID: " + chunkTest.chunkInBack.chunkID);
        }
        
        
        chunkKeyCheck = pos + VoxelConstants.FaceTop * VoxelConstants.ChunkSize;
        Debug.Log("TEST IF CHUNK EXISTS AT " + chunkKeyCheck);
        if (chunks.ContainsKey(chunkKeyCheck))
        {
            chunkTest.chunkInTop = chunks[chunkKeyCheck];
            chunks[chunkKeyCheck].chunkInBottom = chunkTest;
            Debug.Log("CURRENT TOP CHUNK ID: " + chunkTest.chunkInTop.chunkID);
        }

        chunkKeyCheck = pos + VoxelConstants.FaceBottom * VoxelConstants.ChunkSize;
        Debug.Log("TEST IF CHUNK EXISTS AT " + chunkKeyCheck);
        if (chunks.ContainsKey(chunkKeyCheck))
        {
            chunkTest.chunkInBottom = chunks[chunkKeyCheck];
            chunks[chunkKeyCheck].chunkInTop = chunkTest;
            Debug.Log("CURRENT BACK CHUNK ID: " + chunkTest.chunkInBottom.chunkID);
        }
        
        
        chunkKeyCheck = pos + VoxelConstants.FaceRight * VoxelConstants.ChunkSize;
        Debug.Log("TEST IF CHUNK EXISTS AT " + chunkKeyCheck);
        if (chunks.ContainsKey(chunkKeyCheck))
        {
            chunkTest.chunkInRight = chunks[chunkKeyCheck];
            chunks[chunkKeyCheck].chunkInLeft = chunkTest;
            Debug.Log("CURRENT RIGHT CHUNK ID: " + chunkTest.chunkInRight.chunkID);
        }

        chunkKeyCheck = pos + VoxelConstants.FaceLeft * VoxelConstants.ChunkSize;
        Debug.Log("TEST IF CHUNK EXISTS AT " + chunkKeyCheck);
        if (chunks.ContainsKey(chunkKeyCheck))
        {
            chunkTest.chunkInLeft = chunks[chunkKeyCheck];
            chunks[chunkKeyCheck].chunkInRight = chunkTest;
            Debug.Log("CURRENT BACK CHUNK ID: " + chunkTest.chunkInLeft.chunkID);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}