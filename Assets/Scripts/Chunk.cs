using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof (ChunkRenderer))]
public class Chunk : MonoBehaviour
{
    
    
    public ChunkRenderer chunkRenderer;
    public ChunkData chunkData = null;
    
    public Chunk chunkInBack = null;
    public Chunk chunkInTop = null;
    public Chunk chunkInRight = null;
    public Chunk chunkInLeft = null;
    public Chunk chunkInFront = null;
    public Chunk chunkInBottom = null;

    public int testChunkType;
    public Material material;
    public World world;
    public int chunkID;

    private int chunkSize = 0;
    public bool isChunkModified = true;

    public bool blocksModified = true;
    void OnDrawGizmosSelected()
    {
        if (chunkData != null && isChunkModified)
        {
            Gizmos.color = new Color(1, 0, 0, 0.15f);
            Gizmos.DrawCube(chunkData.chunkPosition + new Vector3(1, 1, 1) * VoxelConstants.ChunkSize / 2, new Vector3(1, 1, 1) * VoxelConstants.ChunkSize);
        }
    }

    public void InitialiseChunk(Vector3 chunkPosition, List<Vector3> blocksToAdd, 
        Material chunkMaterial, World worldReference, int chunkIdSet)
    {
        chunkData = new ChunkData(chunkPosition, blocksToAdd);
        chunkID = chunkIdSet;
        world = worldReference;
        chunkRenderer = GetComponent<ChunkRenderer>();
        
        chunkRenderer.InitialiseChunkData(chunkMaterial);
        
        //SetTriangles();

        chunkRenderer.UpdateChunkRender(chunkData);
    }

    
    
    
    
    
    
    
    
    
    public void UpdateChunk()
    {
        UpdateBlocks();
    }

    public void UpdateChunkRenderer()
    {
        //Debug.Log("Chunk - UpdateChunk: Update active triangle faces based on voxels in chunk.");
        UpdateActiveFaces();
        //Debug.Log("Chunk - UpdateChunk: Set triangles.");
        SetTriangles();
        chunkRenderer.UpdateChunkRender(chunkData);
    }
    
    public void UpdateBlocks()
    {
        
    }
    
    private Vector3 Bresenham(int x1, int y1, int x2, int y2)
    {
        int diff_x = x2 - x1;
        int diff_y = y2 - y1;
        int increm_x = Math.Sign(diff_x);
        int increm_y = Math.Sign(diff_y);
        diff_x = Math.Abs(diff_x);
        diff_y = Math.Abs(diff_y);
        if (diff_y == 0)
        {
            for (int x = x1; x != x2 + increm_x; x += increm_x)
            {
                
            }
        }

        return Vector3.zero;
    }
    
    int mod(int x, int m) {
        return (x%m + m)%m;
    }
    private void TransferBlockData(Block blockSource, Block blockTarget)
    {
        int l = blockTarget.LifeTime;
        Vector3 v = blockTarget.Velocity;
        BlockType t = blockTarget.Type;
        
        blockTarget.LifeTime = blockSource.LifeTime;
        blockTarget.Velocity = blockSource.Velocity;
        blockTarget.Type = blockSource.Type;

        blockSource.LifeTime = l;
        blockSource.Velocity = v;
        blockSource.Type = t;
        
    }

    private void BlockReset(Block block)
    {
        block.LifeTime = -1;
        block.Velocity = Vector3.zero;
        block.NeedsUpdating = false;
        block.Type = BlockType.Air;
    }

    public BlockFace DirectionOfOutside(Vector3 blockKey)
    {
        if (blockKey.x < 0)
        {
            return BlockFace.Left;
        }
        if (blockKey.x > chunkSize - 1)
        {
            return BlockFace.Right;
        }
        if (blockKey.y < 0)
        {
            return BlockFace.Bottom;
        }
        if (blockKey.y > chunkSize - 1)
        {
            return BlockFace.Top;
        }
        if (blockKey.z < 0)
        {
            return BlockFace.Back;
        }
        if (blockKey.z > chunkSize - 1)
        {
            return BlockFace.Front;
        }
        return BlockFace.Inside;
    }
    
    
    
    
    
    
  
    private void SetTriangles()
    {
        chunkRenderer.chunkVertecies = new List<Vector3>();
        chunkRenderer.chunkTriangles = new List<int>();
        chunkRenderer.chunkUVs = new List<Vector2>();
        //Debug.Log(chunkData.blocks.Length);

        int blockNum = 0;
        foreach (Vector3 vectorBlock in chunkData.blockKeysInChunk)
        {
            int worldIndTest = world.CalculateArrayIndex(vectorBlock);
            
            if (world.indivCellDatas[worldIndTest].Type != 0)
            {
                for (int i = 0; i < VoxelConstants.CubeVertecies.Length; i++)
                {
                    chunkRenderer.chunkVertecies.Add(VoxelConstants.CubeVertecies[i] + vectorBlock);
                }
            
                for (int i = 0; i < 6; i++)
                {
                    if (world.worldBlocks[vectorBlock].Adjacent[i])
                    {
                        for (int o = 0; o < 6; o++)
                        {
                            chunkRenderer.chunkTriangles.Add(VoxelConstants.Triangles[i, o] + blockNum * 8);
                        }
                    }
                }

                for (int i = 0; i < VoxelConstants.UV.Length; i++)
                {
                    chunkRenderer.chunkUVs.Add(VoxelConstants.UV[i]);
                }
                
                blockNum++;
            }
        }
        //Debug.Log(chunkRenderer.chunkVertecies.Count);
        //Debug.Log(chunkRenderer.chunkTriangles.Count);
        //Debug.Log(chunkRenderer.chunkUVs.Count);
    }
    
    private void UpdateActiveFaces()
    {
        foreach (Vector3 blockPos in chunkData.blockKeysInChunk)
        {
            int worldIndTest = world.CalculateArrayIndex(blockPos);
            if (world.indivCellDatas[worldIndTest].Type != 0)
            {
                for (int o = 0; o < 6; o++)
                {
                    bool checkFace =
                        CheckBlockAdjacency(world.CalculateArrayIndex(blockPos + VoxelConstants.CubeVertexCheck[o]));
                    world.worldBlocks[blockPos].Adjacent[o] = checkFace;
                }
            }
            else
            {
                for (int o = 0; o < 6; o++)
                {
                    world.worldBlocks[blockPos].Adjacent[o] = false;
                }
            }
        }
    }
    private bool CheckBlockAdjacency(int worldIndTest)
    {
        //Debug.Log(positionBase + positionToCheck);
        
        //Debug.Log("TEST AT: " + worldIndTest);
        if (worldIndTest == -1)
        {
            return true;
        }
        if (world.indivCellDatas[worldIndTest].Type == 0)
        {
            return true;
        }

        return false;
    }
    
    
    
    //*/
}
