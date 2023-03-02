using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ChunkData
{
    public int chunkWidth { get; private set;  }
    public int chunkHeight { get; private set;  }
    public int chunkLength { get; private set;  }

    public Vector3 chunkPosition;
    public Dictionary<Vector3, Block> blocks = new Dictionary<Vector3, Block>();
    public bool isChunkModified;

    
    
    public Dictionary<Vector2, Block> frontChunkData = new Dictionary<Vector2, Block>();
    public Dictionary<Vector2, Block> backChunkData = new Dictionary<Vector2, Block>();
    
    public Dictionary<Vector2, Block> topChunkData = new Dictionary<Vector2, Block>();
    public Dictionary<Vector2, Block> bottomChunkData = new Dictionary<Vector2, Block>();
    
    public Dictionary<Vector2, Block> rightChunkData = new Dictionary<Vector2, Block>();
    public Dictionary<Vector2, Block> leftChunkData = new Dictionary<Vector2, Block>();
    // For chunk rendering
    //public List<Vector3> chunkVertecies = new List<Vector3>();
    //public List<int> chunkTriangles = new List<int>();
    //public List<Vector2> chunkUVs = new List<Vector2>();


    public ChunkData(Vector3 chunkPosition, int type)
    {
        chunkWidth = VoxelConstants.ChunkSize;
        chunkHeight = VoxelConstants.ChunkSize;
        chunkLength = VoxelConstants.ChunkSize;
        this.chunkPosition = chunkPosition;

        //blocks = new Block[chunkWidth * chunkHeight * chunkLength];
        InitialiseChunkTest(type);
        //InitialiseActiveFaces();
    }

    private void InitialiseChunkTest(int type)
    {
        Debug.Log("ChunkData: Generate Test Chunk Data");
        float chance = 1f;
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkLength; z++)
                {
                    BlockType selected = BlockType.Air;
                    
                    switch (type)
                    {
                        case 0:
                            selected = BlockType.Solid;
                            break;
                        case 1:
                            selected = y < chunkHeight / 2 ? BlockType.Solid : BlockType.Air;
                            break;
                        case 2:
                            selected = BlockType.Air;
                            break;
                    }

                    Block createdBlock = new Block(selected);
                    blocks.Add(new Vector3(x, y, z), createdBlock);
                    if (z == 0)
                    {
                        frontChunkData.Add(new Vector2(x, y), createdBlock);
                    }
                    else if (z == chunkLength - 1)
                    {
                        backChunkData.Add(new Vector2(x, y), createdBlock);
                    }

                    if (y == 0)
                    {
                        bottomChunkData.Add(new Vector2(x, z), createdBlock);
                    }
                    else if (y == chunkHeight - 1)
                    {
                        topChunkData.Add(new Vector2(x, z), createdBlock);
                    }
                    
                    if (x == 0)
                    {
                        leftChunkData.Add(new Vector2(y, z), createdBlock);
                    }
                    else if (x == chunkWidth - 1)
                    {
                        rightChunkData.Add(new Vector2(y, z), createdBlock);
                    }
                    //blocks[x + y*chunkHeight + z*chunkLength*chunkLength] = new Block(selected);
                }
            }
            //chance -= changeReduce;
        }
        
        Debug.Log("ChunkData: Generate Test Chunk Data Complete");
    }
    
    /*
    public void SetAdjacentChunkData(ChunkData chunkDataToSet, BlockFace direction)
    {
        if (chunkDataToSet != null)
        {
            switch (direction)
            {
                case (BlockFace.Front):
                    if (chunkDataInFront == null)
                    {
                        chunkDataInFront = chunkDataToSet;
                        chunkDataToSet.chunkDataInBack = this;
                    }

                    break;
                case (BlockFace.Top):
                    if (chunkDataInTop == null)
                    {
                        chunkDataInTop = chunkDataToSet;
                        chunkDataToSet.chunkDataInBottom = this;
                    }

                    break;
                case (BlockFace.Right):
                    if (chunkDataInRight == null)
                    {
                        chunkDataInRight = chunkDataToSet;
                        chunkDataToSet.chunkDataInLeft = this;
                    }

                    break;
                case (BlockFace.Left):
                    if (chunkDataInLeft == null)
                    {
                        chunkDataInLeft = chunkDataToSet;
                        chunkDataToSet.chunkDataInRight = this;
                    }

                    break;
                case (BlockFace.Back):
                    if (chunkDataInBack == null)
                    {
                        chunkDataInBack = chunkDataToSet;
                        chunkDataToSet.chunkDataInFront = this;
                    }

                    break;
                case (BlockFace.Bottom):
                    if (chunkDataInBottom == null)
                    {
                        chunkDataInBottom = chunkDataToSet;
                        chunkDataToSet.chunkDataInTop = this;
                    }

                    break;
            }
        }
    }
    */
    
    /*
    private void InitialiseChunk()
    {
        float chance = 1f;
        float changeReduce = 1f / (chunkWidth * 1 * 1);
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkLength; z++)
                {
                    BlockType selected = Random.value < chance ? BlockType.Solid : BlockType.Air;
                    blockDict.Add(new Vector3(x, y, z) + chunkPosition, new Block(selected));

                }
            }
            //chance -= changeReduce;
        }
        SetPrimValues();
    }

    public bool AddBlock(BlockType type, Vector3 position)
    {
        foreach (Vector3 blockPosition in blockDict.Keys)
        {
            if (blockPosition == position)
            {
                block.Type = type;
                return true;
            }
        }
        return false;
    }

    public void SetPrimValues()
    {
        int blockNum = 0;
        int nextSet = 8;
        foreach (KeyValuePair<Vector3, Block> block in blockDict)
        {
            if (block.Value.Type == BlockType.Solid)
            {
                for (int i = 0; i < VoxelConstants.CubeVertecies.Length; i++)
                {
                    chunkVertecies.Add(VoxelConstants.CubeVertecies[i] + block.Key);
                }

                for (int i = 0; i < 6; i++)
                {
                    if (CheckAdjacent(block.Key, VoxelConstants.CubeVertexCheck[i]))
                    {
                        for (int o = 0; o < 6; o++)
                        {
                            chunkTriangles.Add(VoxelConstants.Triangles[i, o] + blockNum * nextSet);
                        }
                    }
                }

                for (int i = 0; i < VoxelConstants.UV.Length; i++)
                {
                    chunkUVs.Add(VoxelConstants.UV[i]);
                }
                blockNum++;
            }

            
        }
    }

    private bool CheckAdjacent(Vector3 initialPos, Vector3 checkBlock)
    {
        Vector3 keyCheck = initialPos + checkBlock;
        if (blockDict.ContainsKey(keyCheck))
        {
            if (blockDict[keyCheck].Type != BlockType.Air)
            {
                return false;
            }
        }
        return true;
    }
    
    */
    
}
