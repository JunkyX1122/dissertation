using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class World : MonoBehaviour
{
    public Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>();
    public List<Vector3> chunksToUpdate = new List<Vector3>();
    public Material material;
    public bool RenderChunks = true;
    private int chunkNum = 0;

    private void Start()
    {
        int chunkID = 0;
        int chunkX = 0;
        int chunkY = 0;
        int chunkZ = 0;
        while (chunkX < 5)
        {
            while (chunkZ < 5)
            {
                while (chunkY < 5)
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
                                int lifeTime = -1;
                                if (chunkX > 0  && chunkX < 4 && chunkZ > 0  && chunkZ < 4 && chunkY == 4)
                                {
                                    selected = BlockType.Sand;
                                }

                                
                                Block createdBlock = new Block(lifeTime, new Vector3(0, 0, 0), selected);
                                newChunkData.Add(new Vector3(x, y, z), createdBlock);
                            }
                        }
                    }

                    GameObject newChunk = new GameObject("chunk_" + chunkID.ToString());
                    newChunk.transform.parent = this.transform; 
                    Chunk chunkComponent = newChunk.AddComponent<Chunk>();
                    chunkComponent.InitialiseChunk(chunkWorldPosition, newChunkData, material, this, chunkID);
                    chunks.Add(chunkWorldPosition, chunkComponent);
                    chunksToUpdate.Add(chunkWorldPosition);
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
        foreach (KeyValuePair<Vector3, Chunk> chunkUpdate in chunks)
        {
            //hunkUpdate.Value.UpdateChunkRenderer();
        }
    }

    private int counter = 0;
    void Update()
    {
  
        UpdateChunks();

    }
    void OnDrawGizmosSelected()
    {
        foreach (KeyValuePair<Vector3, Chunk> chunkUpdate in chunks)
        {
            if (chunkUpdate.Value.isChunkModified || chunkUpdate.Value.blocksModified)
            {
                Gizmos.color = new Color(0, 1, 0, 0.5f);
                Gizmos.DrawCube(chunkUpdate.Value.chunkData.chunkPosition + new Vector3(1, 1, 1) * VoxelConstants.ChunkSize / 2,
                    new Vector3(1, 1, 1) * VoxelConstants.ChunkSize);
            }
        }
    }
    private void UpdateChunks()
    {
        /*
        int activeChunks = 0;

        List<Vector3> positionsCopy = new List<Vector3>(chunksToUpdate);
        
        foreach (Vector3 pos in positionsCopy)
        {
            chunks[pos].UpdateChunk();
            if (!chunks[pos].isChunkModified && !chunks[pos].blocksModified)
            {
                Debug.Log("Removed: " + chunks[pos].chunkID);
                chunksToUpdate.Remove(pos);
            }
            else
            {
                Debug.Log("Updated: " + chunks[pos].chunkID);
            }
        }
        
        foreach (Vector3 pos in positionsCopy)
        {
            chunks[pos].UpdateChunkRenderer();
        }
        */
        //*
        foreach (KeyValuePair<Vector3, Chunk> chunkUpdate in chunks)
        {
            if (chunkUpdate.Value.isChunkModified || chunkUpdate.Value.blocksModified)
            {
                chunkUpdate.Value.UpdateChunk();
                //activeChunks++;
                //chunkUpdate.Value.isChunkModified = false;
            }
        }

        if (RenderChunks)
        {
            foreach (KeyValuePair<Vector3, Chunk> chunkUpdate in chunks)
            {
                if (chunkUpdate.Value.isChunkModified || chunkUpdate.Value.blocksModified)
                {
                    chunkUpdate.Value.UpdateChunkRenderer();
                    //chunkUpdate.Value.isChunkModified = false;
                }
            }
        }
         //*/
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