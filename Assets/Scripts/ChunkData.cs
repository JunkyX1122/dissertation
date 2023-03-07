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
    public bool isChunkModified = true;

    
    
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


    public ChunkData(Vector3 chunkPosition, Dictionary<Vector3, Block> blocksToAdd)
    {
        chunkWidth = VoxelConstants.ChunkSize;
        chunkHeight = VoxelConstants.ChunkSize;
        chunkLength = VoxelConstants.ChunkSize;
        this.chunkPosition = chunkPosition; // Does Nothing rn
        foreach (KeyValuePair<Vector3, Block> vectorBlock in blocksToAdd)
        {
            AddBlockToChunk(vectorBlock.Key, vectorBlock.Value);
        }
        //blocks = new Block[chunkWidth * chunkHeight * chunkLength];
        //InitialiseChunkTest(type);
        //InitialiseActiveFaces();
    }
    
    
    public void UpdateBlocks()
    {
        int updatedBlockCount = 0;
        foreach (KeyValuePair<Vector3, Block> block in blocks)
        {
            updatedBlockCount += UpdateBlock(block.Key);
        }
        isChunkModified = updatedBlockCount > 0 ;
    }
    public int UpdateBlock(Vector3 blockKey)
    {
        Block currentBlock = GetBlock(blockKey);
        if (currentBlock == null)
        {
            return 0;
        }

        if (currentBlock.Type == BlockType.Sand)
        {
            Vector3 nextKey = blockKey + Vector3.down;
            Block nextBlock = GetBlock(nextKey);
            if (nextBlock != null & nextBlock.Type == BlockType.Air)
            {
                currentBlock.Velocity = Vector3.down;
                TransferBlockData(blocks[blockKey], blocks[nextKey]);
                BlockReset(blocks[blockKey]);
                return 1;
            }
            else
            {
                currentBlock.Velocity = Vector3.zero;
                return 0;
            }
        }

        return 0;
    }

    private Block GetBlock(Vector3 blockKey)
    {
        if (blocks.ContainsKey(blockKey))
        {
            return blocks[blockKey];
        }

        return null;
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
    public void AddBlockToChunk(Vector3 position, Block block)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        int z = Mathf.RoundToInt(position.z);
        blocks.Add(new Vector3(x, y, z), block);
        if (z == 0)
        {
            frontChunkData.Add(new Vector2(x, y), block);
        }
        else if (z == chunkLength - 1)
        {
            backChunkData.Add(new Vector2(x, y), block);
        }

        if (y == 0)
        {
            bottomChunkData.Add(new Vector2(x, z), block);
        }
        else if (y == chunkHeight - 1)
        {
            topChunkData.Add(new Vector2(x, z), block);
        }

        if (x == 0)
        {
            leftChunkData.Add(new Vector2(y, z), block);
        }
        else if (x == chunkWidth - 1)
        {
            rightChunkData.Add(new Vector2(y, z), block);
        }
    }
    
}
