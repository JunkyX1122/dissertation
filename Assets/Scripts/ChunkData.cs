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
    public List<Vector3> blockKeysInChunk = new List<Vector3>();
   
    public ChunkData(Vector3 chunkPosition, List<Vector3> blocksToAdd)
    {
        chunkSize = VoxelConstants.ChunkSize;
        this.chunkPosition = chunkPosition; // Does Nothing rn
        foreach (Vector3 block in blocksToAdd)
        {
            blockKeysInChunk.Add(block);
        }
        //blocks = new Block[chunkWidth * chunkHeight * chunkLength];
        //InitialiseChunkTest(type);
        //InitialiseActiveFaces();
    }
}
