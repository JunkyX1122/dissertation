using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class World : MonoBehaviour
{
    public Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>();
    public Material material;
    private int chunkNum = 0;

    private void Start()
    {
        int chunkID = 0;
        int chunkX = 0;
        int chunkY = 0;
        int chunkZ = 0;
        while (chunkX < 1)
        {
            while (chunkZ < 1)
            {
                while (chunkY < 11)
                {
                    Vector3 chunkWorldPosition = new Vector3(chunkX, chunkY, chunkZ) * VoxelConstants.ChunkSize;
                    
                    
                    Dictionary<Vector3, Block> newChunkData = new Dictionary<Vector3, Block>();
                    for (int x = 0; x < VoxelConstants.ChunkSize; x++)
                    {
                        for (int y = 0; y < VoxelConstants.ChunkSize; y++)
                        {
                            for (int z = 0; z < VoxelConstants.ChunkSize; z++)
                            {
                                BlockType selected = BlockType.Air;
                
                                if (y==15 && chunkY == 10)
                                {
                                    selected = BlockType.Sand;
                                }
                                Block createdBlock = new Block(-1, new Vector3(4, 3, 0), selected);
                                newChunkData.Add(new Vector3(x, y, z), createdBlock);
                            }
                        }
                    }

                    GameObject newChunk = new GameObject("chunk_" + chunkID.ToString());
                    newChunk.transform.parent = this.transform; 
                    Chunk chunkComponent = newChunk.AddComponent<Chunk>();
                    chunkComponent.InitialiseChunk(chunkWorldPosition, newChunkData, material, this, chunkID);
                    chunks.Add(chunkWorldPosition, chunkComponent);
                    UpdateAdjactentChunkStore(chunkWorldPosition, chunkComponent);
                    
                    chunkID++;
                    chunkY++;
                }

                chunkY = 0;
                chunkZ++;
            }

            chunkZ = 0;
            chunkX++;
        }

        UpdateChunks();
    }

    private int counter = 0;
    void Update()
    {
        if (counter % 10 == 0)
        {
            int randomX = Random.Range(0, 15);
            int randomZ = Random.Range(0, 15);

            chunks[new Vector3(0, 16 * 10,0)]
                .ModifyBlock(new Vector3(randomX, 15, randomZ), BlockType.Sand);
        }
        counter++;
        UpdateChunks();

    }

    private void UpdateChunks()
    {
        int activeChunks = 0;
        foreach (KeyValuePair<Vector3, Chunk> chunkUpdate in chunks)
        {
            if (chunkUpdate.Value.isChunkModified || chunkUpdate.Value.blocksModified)
            {
                chunkUpdate.Value.UpdateChunk();
                activeChunks++;
                //chunkUpdate.Value.isChunkModified = false;
            }
        }
        foreach (KeyValuePair<Vector3, Chunk> chunkUpdate in chunks)
        {
            if (chunkUpdate.Value.isChunkModified || chunkUpdate.Value.blocksModified)
            {
                chunkUpdate.Value.UpdateChunkRenderer();
                //chunkUpdate.Value.isChunkModified = false;
            }
        }
        //Debug.Log(activeChunks);
    }
 
    private void UpdateAdjactentChunkStore(Vector3 pos, Chunk chunkTest)
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
    
}