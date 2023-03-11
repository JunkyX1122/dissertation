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

    public int chunkSize { get; private set;  }
    public Vector3 chunkPosition;
    public Dictionary<Vector3, Block> blocks = new Dictionary<Vector3, Block>();

    public Dictionary<Vector2, Block> backChunkData = new Dictionary<Vector2, Block>();
    public Dictionary<Vector2, Block> frontChunkData = new Dictionary<Vector2, Block>();
    
    public Dictionary<Vector2, Block> topChunkData = new Dictionary<Vector2, Block>();
    public Dictionary<Vector2, Block> bottomChunkData = new Dictionary<Vector2, Block>();
    
    public Dictionary<Vector2, Block> rightChunkData = new Dictionary<Vector2, Block>();
    public Dictionary<Vector2, Block> leftChunkData = new Dictionary<Vector2, Block>();

    public Queue<Vector3> positionsToUpdate = new Queue<Vector3>();

    public ChunkData(Vector3 chunkPosition, Dictionary<Vector3, Block> blocksToAdd)
    {
        chunkSize = VoxelConstants.ChunkSize;
        this.chunkPosition = chunkPosition; // Does Nothing rn
        foreach (KeyValuePair<Vector3, Block> vectorBlock in blocksToAdd)
        {
            AddBlockToChunk(vectorBlock.Key, vectorBlock.Value);
        }
        //blocks = new Block[chunkWidth * chunkHeight * chunkLength];
        //InitialiseChunkTest(type);
        //InitialiseActiveFaces();
    }
    
    
    
    public void AddBlockToChunk(Vector3 position, Block block)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        int z = Mathf.RoundToInt(position.z);
        Vector3 newPos = new Vector3(x, y, z);
        blocks.Add(newPos, block);
        positionsToUpdate.Enqueue(newPos);
        //block.SetChunkDataRef(this);
        if (z == 0)
        {
            backChunkData.Add(new Vector2(x, y), block);
        }
        else if (z == chunkSize - 1)
        {
            frontChunkData.Add(new Vector2(x, y), block);
        }

        if (y == 0)
        {
            bottomChunkData.Add(new Vector2(x, z), block);
        }
        else if (y == chunkSize - 1)
        {
            topChunkData.Add(new Vector2(x, z), block);
        }

        if (x == 0)
        {
            leftChunkData.Add(new Vector2(y, z), block);
        }
        else if (x == chunkSize - 1)
        {
            rightChunkData.Add(new Vector2(y, z), block);
        }
    }
    
}
