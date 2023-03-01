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

    public ChunkData chunkDataInFront;
    public ChunkData chunkDataInTop;
    public ChunkData chunkDataInRight;
    public ChunkData chunkDataInLeft;
    public ChunkData chunkDataInBack;
    public ChunkData chunkDataInBottom;
    
    public Dictionary<Vector2, int> frontFace = new Dictionary<Vector2, int>();
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
                    blocks.Add(new Vector3(x, y, z), new Block(selected));
                    //blocks[x + y*chunkHeight + z*chunkLength*chunkLength] = new Block(selected);
                }
            }
            //chance -= changeReduce;
        }
        Debug.Log("ChunkData: Generate Test Chunk Data Complete");
    }

    public void SetAdjacentChunkData(ChunkData chunkDataToSet, BlockFace direction)
    {
        switch (direction)
        {
            case(BlockFace.Front) :
                if (chunkDataInFront == null)
                {
                    
                }
                break;
            case(BlockFace.Top) : 
                break;
            case(BlockFace.Right) :
                break;
            case(BlockFace.Left) : 
                break;
            case(BlockFace.Back) : 
                break;
            case(BlockFace.Bottom) : 
                break;
        }
    }
    
    
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
