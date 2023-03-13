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
            if (chunkData.positionsToUpdate.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
    void OnDrawGizmosSelected()
    {
        if (chunkData != null && isChunkModified)
        {
            Gizmos.color = new Color(1, 0, 0, 0.15f);
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
        chunkData.positionsToUpdate.Add(key);
    }
    public void UpdateBlocks()
    {
        isChunkModified = false;
        //Debug.Log("BLOCKS THAT NEED UPDATES: " + chunkData.positionsToUpdate.Count);
        List<Vector3> positionsCopy = new List<Vector3>(chunkData.positionsToUpdate);
        foreach (Vector3 pos in positionsCopy)
        {
            chunkData.positionsToUpdate.Remove(pos);
            UpdateBlock(pos);
        }

    }

    public void UpdateBlock(Vector3 blockKey)
    {
        
        Block currentBlock = GetBlock(blockKey);
        if (currentBlock == null)
        {
            return;
        }
        /*
        
        if (currentBlock.Type == BlockType.Particle)
        {
            //Debug.Log(currentBlock.Velocity);
            
            if (currentBlock.LifeTime > 0)
            {
                currentBlock.LifeTime--;
                
                Vector3 nextKey = blockKey + currentBlock.Velocity;
                Block nextBlock = GetBlock(nextKey);
                if (nextBlock == null)
                {
                    EnqueVector3(blockKey);
                    return;
                }
                
                if (nextBlock.Type == BlockType.Air)
                {
                    //Debug.Log("Movement!");
                    EnqueVector3(nextKey);
                    TransferBlockData(currentBlock, nextBlock);
                    BlockReset(chunkData.blocks[blockKey]);
                    return;
                }
                EnqueVector3(blockKey);
            }
            else
            {
                BlockReset(chunkData.blocks[blockKey]);
            }
            return;
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
                return;
            }
            if (nextBlock.Type == BlockType.Air)
            {
                EnqueVector3(blockKey);
                EnqueVector3(nextKey);
                //Debug.Log("Next Block is Air.");
                currentBlock.Velocity = Vector3.down;
                TransferBlockData(currentBlock, nextBlock);
                return;
            }
        }
        return;
    }
    
    private Block GetBlock(Vector3 blockKey)
    {
        //Debug.Log(blockKey);
        if (chunkData.blocks.ContainsKey(blockKey))
        {
            //Debug.Log("Contains block");
            return chunkData.blocks[blockKey];
        }
        BlockFace checkDirection = DirectionOfOutside(blockKey);
        if (checkDirection == BlockFace.Back && chunkInBack)
        {
            blockKey.z = mod(Mathf.RoundToInt(blockKey.z), chunkSize);
            //Debug.Log("Checking chunk back.");
            return chunkInBack.GetBlock(blockKey);
        }
        if (checkDirection == BlockFace.Front - 1 && chunkInFront)
        {
            blockKey.z = mod(Mathf.RoundToInt(blockKey.z), chunkSize);
            //Debug.Log("Checking chunk front.");
            return chunkInFront.GetBlock(blockKey);
        }
        if (checkDirection == BlockFace.Bottom && chunkInBottom)
        {
            blockKey.y = mod(Mathf.RoundToInt(blockKey.y), chunkSize);
            //Debug.Log("Checking chunk bottom.");
            return chunkInBottom.GetBlock(blockKey);
        }
        if (checkDirection == BlockFace.Top && chunkInTop)
        {
            blockKey.y = mod(Mathf.RoundToInt(blockKey.y), chunkSize);
            //Debug.Log("Checking chunk top.");
            return chunkInTop.GetBlock(blockKey);
        }
        if (checkDirection == BlockFace.Left && chunkInLeft)
        {
            blockKey.x = mod(Mathf.RoundToInt(blockKey.x), chunkSize);
            //Debug.Log("Checking chunk left.");
            return chunkInLeft.GetBlock(blockKey);
        }
        if (checkDirection == BlockFace.Right && chunkInRight)
        {
            blockKey.x = mod(Mathf.RoundToInt(blockKey.x), chunkSize);
            //Debug.Log("Checking chunk right.");
            return chunkInRight.GetBlock(blockKey);
        }
        //*/
        return null;
    }

    public void EnqueVector3(Vector3 vector3)
    {
        BlockFace direction = DirectionOfOutside(vector3);
        switch (direction)
        {
            case BlockFace.Back:
                vector3.z = mod(Mathf.RoundToInt(vector3.z),chunkSize);
                chunkInBack.EnqueVector3(vector3);
                break;
            case BlockFace.Front:
                vector3.z = mod(Mathf.RoundToInt(vector3.z),chunkSize);
                chunkInFront.EnqueVector3(vector3);
                break;
            case BlockFace.Bottom:
                vector3.y = mod(Mathf.RoundToInt(vector3.y),chunkSize);
                chunkInBottom.EnqueVector3(vector3);
                break;
            case BlockFace.Top:
                vector3.y = mod(Mathf.RoundToInt(vector3.y),chunkSize);
                chunkInTop.EnqueVector3(vector3);
                break;
            case BlockFace.Left:
                vector3.x = mod(Mathf.RoundToInt(vector3.x),chunkSize);
                chunkInLeft.EnqueVector3(vector3);
                break;
            case BlockFace.Right:
                vector3.x = mod(Mathf.RoundToInt(vector3.x),chunkSize);
                chunkInRight.EnqueVector3(vector3);
                break;
            case BlockFace.Inside:
                chunkData.positionsToUpdate.Add(vector3);
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
