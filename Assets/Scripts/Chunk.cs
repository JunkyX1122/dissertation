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
    
    
    public bool blocksModified
    {
        get
        {
            foreach (KeyValuePair<Vector3, Block> vectorBlock in chunkData.blocks)
            {
                if (vectorBlock.Value.NeedsUpdating)
                {
                    return true;
                }
            }
            return false;
        }
    }
    void OnDrawGizmosSelected()
    {
        if (chunkData != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawCube(chunkData.chunkPosition + new Vector3(1, 1, 1) * VoxelConstants.ChunkSize / 2, new Vector3(1, 1, 1) * VoxelConstants.ChunkSize);
        }
    }

    public void InitialiseChunk(Vector3 chunkPosition, Dictionary<Vector3, Block> blocksToAdd, 
        Material chunkMaterial, World worldReference, int chunkIdSet)
    {
        chunkData = new ChunkData(chunkPosition, blocksToAdd);
        chunkSize = chunkData.chunkSize;
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
        //Debug.Log("Chunk - UpdateChunk: Update active triangle faces based on voxels in adjacent chunks.");
        UpdateActiveFacesOnBorder();
        //Debug.Log("Chunk - UpdateChunk: Set triangles.");
        SetTriangles();
        chunkRenderer.UpdateChunkRender(chunkData);
    }
    public void ModifyBlock(Vector3 key, BlockType blockType)
    {
        chunkData.blocks[key].Type = blockType;
        chunkData.blocks[key].NeedsUpdating = true;
        chunkData.positionsToUpdate.Enqueue(key);
    }
    public void UpdateBlocks()
    {
        int updatedBlockCount = 0;
        Debug.Log("BLOCKS THAT NEED UPDATES: " + chunkData.positionsToUpdate.Count);
        int updatesThisFrame = chunkData.positionsToUpdate.Count;
        while (updatesThisFrame > 0)
        {
            updatedBlockCount += UpdateBlock(chunkData.positionsToUpdate.Dequeue());
            updatesThisFrame--;
        }
        isChunkModified = updatedBlockCount > 0;
    }

    public int UpdateBlock(Vector3 blockKey)
    {
        Block currentBlock = GetBlock(blockKey);
        if (currentBlock == null)
        {
            return 0;
        }
        
        /*
        if (currentBlock.Type == BlockType.Particle)
        {
            float slope = 0f;
            if (currentBlock.Velocity.x != 0)
            {
                //currentBlock.Velocity.y / currentBlock.Velocity.x;
            }
        }
        */
        if (currentBlock.Type == BlockType.Sand)
        {
            Vector3 nextKey = blockKey + Vector3.down;
           
            Block nextBlock = GetBlock(nextKey);
            //Debug.Log("Block Retrieved");
            if (nextBlock == null)
            {
                currentBlock.NeedsUpdating = false;
                currentBlock.Velocity = Vector3.zero;
                return 0;
            }
            if (nextBlock.Type == BlockType.Air)
            {
                EnqueVector3(nextKey);
                //Debug.Log("Next Block is Air.");
                currentBlock.Velocity = Vector3.down;
                TransferBlockData(currentBlock, nextBlock);
                BlockReset(chunkData.blocks[blockKey]);
                return 1;
            }
        }
        currentBlock.Velocity = Vector3.zero;
        currentBlock.NeedsUpdating = false;
        return 0;
    }
    
    private Block GetBlock(Vector3 blockKey)
    {
        //Debug.Log(blockKey);
        if (chunkData.blocks.ContainsKey(blockKey))
        {
            //Debug.Log("Contains block");
            return chunkData.blocks[blockKey];
        }
        
        Vector3 subtractVector = new Vector3(
            mod(Mathf.RoundToInt(blockKey.x),chunkSize),
            mod(Mathf.RoundToInt(blockKey.y),chunkSize),
            mod(Mathf.RoundToInt(blockKey.z),chunkSize));
        //Debug.Log(subtractVector);
        //*
        BlockFace checkDirection = DirectionOfOutside(blockKey);
        if (checkDirection == BlockFace.Back && chunkInBack)
        {
            Debug.Log("Checking chunk back.");
            return chunkInBack.GetBlock(subtractVector);
        }
        if (checkDirection == BlockFace.Front - 1 && chunkInFront)
        {
            Debug.Log("Checking chunk front.");
            return chunkInFront.GetBlock(subtractVector);
        }
        if (checkDirection == BlockFace.Bottom && chunkInBottom)
        {
            Debug.Log("Checking chunk bottom.");
            return chunkInBottom.GetBlock(subtractVector);
        }
        if (checkDirection == BlockFace.Top && chunkInTop)
        {
            Debug.Log("Checking chunk top.");
            return chunkInTop.GetBlock(subtractVector);
        }
        if (checkDirection == BlockFace.Left && chunkInLeft)
        {
            Debug.Log("Checking chunk left.");
            return chunkInLeft.GetBlock(subtractVector);
        }
        if (checkDirection == BlockFace.Right && chunkInRight)
        {
            Debug.Log("Checking chunk right.");
            return chunkInRight.GetBlock(subtractVector);
        }
        //*/
        return null;
    }

    private void EnqueVector3(Vector3 vector3)
    {
        BlockFace direction = DirectionOfOutside(vector3);
        if (direction == BlockFace.Null)
        {
            chunkData.positionsToUpdate.Enqueue(vector3);
        }
        Vector3 subtractVector = new Vector3(
            mod(Mathf.RoundToInt(vector3.x),chunkSize),
            mod(Mathf.RoundToInt(vector3.y),chunkSize),
            mod(Mathf.RoundToInt(vector3.z),chunkSize));
        switch (direction)
        {
            case BlockFace.Back:
                chunkInBack.chunkData.positionsToUpdate.Enqueue(subtractVector);
                break;
            case BlockFace.Front:
                chunkInFront.chunkData.positionsToUpdate.Enqueue(subtractVector);
                break;
            case BlockFace.Bottom:
                chunkInBottom.chunkData.positionsToUpdate.Enqueue(subtractVector);
                break;
            case BlockFace.Top:
                chunkInTop.chunkData.positionsToUpdate.Enqueue(subtractVector);
                break;
            case BlockFace.Left:
                chunkInLeft.chunkData.positionsToUpdate.Enqueue(subtractVector);
                break;
            case BlockFace.Right:
                chunkInRight.chunkData.positionsToUpdate.Enqueue(subtractVector);
                break;
        }
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
        blockTarget.LifeTime = blockSource.LifeTime;
        blockTarget.Velocity = blockSource.Velocity;
        blockTarget.NeedsUpdating = blockSource.NeedsUpdating;
        blockTarget.Type = blockSource.Type;
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
        if (blockKey.z < 0)
        {
            return BlockFace.Back;
        }
        if (blockKey.z > chunkSize - 1)
        {
            return BlockFace.Front;
        }
        if (blockKey.y < 0)
        {
            return BlockFace.Bottom;
        }
        if (blockKey.y > chunkSize - 1)
        {
            return BlockFace.Top;
        }
        if (blockKey.x < 0)
        {
            return BlockFace.Left;
        }
        if (blockKey.x > chunkSize - 1)
        {
            return BlockFace.Right;
        }
        return BlockFace.Null;
    }
    
    
    
    
    
    
  
    private void SetTriangles()
    {
        chunkRenderer.chunkVertecies = new List<Vector3>();
        chunkRenderer.chunkTriangles = new List<int>();
        chunkRenderer.chunkUVs = new List<Vector2>();
        //Debug.Log(chunkData.blocks.Length);

        int blockNum = 0;
        foreach (KeyValuePair<Vector3, Block> vectorBlock in chunkData.blocks)
        {
            if (vectorBlock.Value.Type != BlockType.Air)
            {
                for (int i = 0; i < VoxelConstants.CubeVertecies.Length; i++)
                {
                    chunkRenderer.chunkVertecies.Add(VoxelConstants.CubeVertecies[i] + vectorBlock.Key + chunkData.chunkPosition);
                }
            
                for (int i = 0; i < 6; i++)
                {
                    if (vectorBlock.Value.Adjacent[i])
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
        foreach (KeyValuePair<Vector3, Block> vectorBlock in chunkData.blocks)
        {
            for (int o = 0; o < 6; o++)
            {
                bool checkFace = CheckBlockAdjacencyInChunk(vectorBlock.Key + VoxelConstants.CubeVertexCheck[o]);
                vectorBlock.Value.Adjacent[o] = checkFace;
            }
        }
    }

    private void UpdateActiveFacesOnBorder()
    {
        //Debug.Log("Chunk - UpdateActiveFacesOnBorder: Test Back.");
        if (chunkInBack != null)
        {
            foreach (KeyValuePair<Vector2, Block> vectorBlock in chunkData.backChunkData)
            {
                Vector2 position = vectorBlock.Key;
                bool checkFace = CheckBlockAdjacencyInAdjacentChunk(position, chunkInBack.chunkData.frontChunkData);
                vectorBlock.Value.Adjacent[VoxelConstants.FACE_BACK] = checkFace;
            }
        }
        //Debug.Log("Chunk - UpdateActiveFacesOnBorder: Test Front.");
        if (chunkInFront != null)
        {
            foreach (KeyValuePair<Vector2, Block> vectorBlock in chunkData.frontChunkData)
            {
                Vector2 position = vectorBlock.Key;
                bool checkFace = CheckBlockAdjacencyInAdjacentChunk(position, chunkInFront.chunkData.backChunkData);
                vectorBlock.Value.Adjacent[VoxelConstants.FACE_FRONT] = checkFace;
            }
        }
        //Debug.Log("Chunk - UpdateActiveFacesOnBorder: Test Top.");
        if (chunkInTop != null)
        {
            foreach (KeyValuePair<Vector2, Block> vectorBlock in chunkData.topChunkData)
            {
                Vector2 position = vectorBlock.Key;
                bool checkFace = CheckBlockAdjacencyInAdjacentChunk(position, chunkInTop.chunkData.bottomChunkData);
                vectorBlock.Value.Adjacent[VoxelConstants.FACE_UP] = checkFace;
            }
        }
        //Debug.Log("Chunk - UpdateActiveFacesOnBorder: Test Bottom.");
        if (chunkInBottom != null)
        {
            foreach (KeyValuePair<Vector2, Block> vectorBlock in chunkData.bottomChunkData)
            {
                Vector2 position = vectorBlock.Key;
                bool checkFace = CheckBlockAdjacencyInAdjacentChunk(position, chunkInBottom.chunkData.topChunkData);
                vectorBlock.Value.Adjacent[VoxelConstants.FACE_BOTTOM] = checkFace;
            }
        }
        
        //Debug.Log("Chunk - UpdateActiveFacesOnBorder: Test Right.");
        if (chunkInRight != null)
        {
            foreach (KeyValuePair<Vector2, Block> vectorBlock in chunkData.rightChunkData)
            {
                Vector2 position = vectorBlock.Key;
                bool checkFace = CheckBlockAdjacencyInAdjacentChunk(position, chunkInRight.chunkData.leftChunkData);
                vectorBlock.Value.Adjacent[VoxelConstants.FACE_RIGHT] = checkFace;
            }
        }
        //Debug.Log("Chunk - UpdateActiveFacesOnBorder: Test Left.");
        if (chunkInLeft != null)
        {
            foreach (KeyValuePair<Vector2, Block> vectorBlock in chunkData.leftChunkData)
            {
                Vector2 position = vectorBlock.Key;
                bool checkFace = CheckBlockAdjacencyInAdjacentChunk(position, chunkInLeft.chunkData.rightChunkData);
                vectorBlock.Value.Adjacent[VoxelConstants.FACE_LEFT] = checkFace;
            }
        }
    }
    private bool CheckBlockAdjacencyInChunk(Vector3 positionToCheck)
    {
        //Debug.Log(positionBase + positionToCheck);
        if (chunkData.blocks.ContainsKey(positionToCheck))
        {
            if (chunkData.blocks[positionToCheck].Type == BlockType.Air)
            {
                return true;
            }
            return false;
        }

        return true;
    }

    private bool CheckBlockAdjacencyInAdjacentChunk(Vector2 positionToCheck, Dictionary<Vector2, Block> blockDict)
    {
        if (blockDict.ContainsKey(positionToCheck))
        {
            if (blockDict[positionToCheck].Type == BlockType.Air)
            {
                return true;
            }
            return false;
        }
        return true;
        
    }
    
    
    //*/
}
