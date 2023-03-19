using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public struct Cell
{
    public int Type;
    public Vector3 Velocity;
}
public class World : MonoBehaviour
{
    public Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>();
    public List<Vector3> chunksToUpdate = new List<Vector3>();
    public Material material;
    public bool RenderChunks = true;
    private int chunkNum = 0;

    public Dictionary<Vector3, Block> worldBlocks = new Dictionary<Vector3, Block>();
    public Cell[] indivCellDatas;
    
    private int chunkSize;

    public ComputeShader computeShader;
    
    
    private void Start()
    {
        chunkSize = VoxelConstants.ChunkSize;
        int chunkID = 0;
        int chunkX = 0;
        int chunkY = 0;
        int chunkZ = 0;
        int totalBlocks = (chunkSize * chunkSize * chunkSize) * (3 * 3 * 3);
        indivCellDatas = new Cell[totalBlocks];
        //Debug.Log("TOTAL BLOCKS: "+ indivCellDatas.Length);
        
        while (chunkX < 3)
        {
            while (chunkZ < 3)
            {
                while (chunkY < 3)
                {
                    Vector3 chunkWorldPosition = new Vector3(chunkX, chunkY, chunkZ) * chunkSize;
                    
                    
                    List<Vector3> newChunkData = new List<Vector3>();
                    for (int x = 0; x < chunkSize; x++)
                    {
                        for (int y = 0; y < chunkSize; y++)
                        {
                            for (int z = 0; z < chunkSize; z++)
                            {
                                BlockType selected = BlockType.Air;
                                int lifeTime = -1;
                                if (chunkX == 1 && chunkZ == 1 && chunkY == 2)
                                {
                                    selected = BlockType.Sand;
                                }

                                Block createdBlock = new Block(lifeTime, new Vector3(0, 0, 0), selected);
                                newChunkData.Add(new Vector3(x, y, z) + chunkWorldPosition);
                                worldBlocks.Add(new Vector3(x, y, z) + chunkWorldPosition, createdBlock);

                                Cell cellData = new Cell();
                                cellData.Type = selected == BlockType.Air ? 0 : 1;
                                cellData.Velocity = Vector3.zero;
                                Vector3 pos = new Vector3(x, y, z) + chunkWorldPosition;
                                int posInd = CalculateArrayIndex(pos);
                                //Debug.Log(pos + " : " + posInd );
                                indivCellDatas[posInd] =
                                    cellData;
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

        TestUpdate();
        
        //s
        UpdateChunks();

    }

    private void TestUpdate()
    {
        /*
        int intSize = sizeof(int);
        int vector3Size = sizeof(float) * 3;
        int totalSize = intSize + vector3Size;
        ComputeBuffer computeBuffer = new ComputeBuffer(indivCellDatas.Length, totalSize);
        computeBuffer.SetData(indivCellDatas);
        computeShader.SetBuffer(0, "cells", computeBuffer);
        computeShader.SetInt("worldWidth", chunkSize * 3);
        computeShader.SetInt("worldHeight", chunkSize * 3);
        computeShader.SetInt("worldLength", chunkSize * 3);
        */
        
    }
    
    public int CalculateArrayIndex(Vector3 ind)
    {
        Vector3Int indInt = Vector3Int.FloorToInt(ind);
        int x = indInt.x;
        int y = indInt.y;
        int z = indInt.z;
        int indexer = x + y * (chunkSize * 3) + z * ((chunkSize * 3) * (chunkSize * 3));
        if(indexer < 0 || indexer >= ((chunkSize * 3) * (chunkSize * 3) * (chunkSize * 3)))
        {
            return -1;
        }
        return indexer;
    }
    
    private int counter = 0;
    void Update()
    {
  
        //UpdateChunks();

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