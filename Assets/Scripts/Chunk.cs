using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof (ChunkRenderer))]
public class Chunk : MonoBehaviour
{
    public ChunkRenderer chunkRenderer;
    public ChunkData chunkData;
    
    public Chunk chunkInFront;
    public Chunk chunkInTop;
    public Chunk chunkInRight;
    public Chunk chunkInLeft;
    public Chunk chunkInBack;
    public Chunk chunkInBottom;
    
    public int testChunkType;
    public Material material;
    public World world;
    public int chunkID;
    void Awake()
    {
        
        
        //InitialiseChunk(0, new Vector3(0, 0, 0), material);
    }

    public void InitialiseChunk(int type, Vector3 chunkPosition, Material chunkMaterial, World worldReference, int chunkIdSet)
    {
        chunkID = chunkIdSet;
        world = worldReference;
        chunkRenderer = GetComponent<ChunkRenderer>();
        Debug.Log("Chunk: Create chunk data.");
        ChunkData chunkDataInit = new ChunkData(chunkPosition, type);
        Debug.Log("Chunk: Initialise chunk data to chunk.");
        chunkRenderer.InitialiseChunkData(chunkDataInit, chunkMaterial);
        Debug.Log("Chunk: Set chunk data chunk renderer.");
        chunkData = chunkRenderer.chunkData;
        Debug.Log("Chunk: Initialise active triangle faces based on voxels in chunk.");
        InitialiseActiveFaces();
        Debug.Log("Chunk: Set triangles.");
        SetTriangles();
        Debug.Log("Chunk: Update chunk renderer.");
        chunkRenderer.UpdateChunkRender(chunkData);
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
            if (vectorBlock.Value.Type == BlockType.Solid)
            {
                //Debug.Log(block.Type == BlockType.Solid);
                for (int i = 0; i < VoxelConstants.CubeVertecies.Length; i++)
                {
                    chunkRenderer.chunkVertecies.Add(VoxelConstants.CubeVertecies[i] + vectorBlock.Key + chunkData.chunkPosition);
                }
            
                for (int i = 0; i < 6; i++)
                {
                    /*
                    switch (i)
                    {
                        case(0) : Debug.Log("Front");
                            break;
                        case(1) : Debug.Log("Top");
                            break;
                        case(2) : Debug.Log("Right");
                            break;
                        case(3) : Debug.Log("Left");
                            break;
                        case(4) : Debug.Log("Back");
                            break;
                        case(5) : Debug.Log("Bottom");
                            break;
                    }
                    */
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
        Debug.Log(chunkRenderer.chunkVertecies.Count);
        Debug.Log(chunkRenderer.chunkTriangles.Count);
        Debug.Log(chunkRenderer.chunkUVs.Count);
    }
    
    private void InitialiseActiveFaces()
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

    private void InitialiseActiveFacesOnBorder()
    {
        foreach (KeyValuePair<Vector2, Block> vectorBlock in chunkData.frontChunkData)
        {
            bool checkFace = CheckBlockAdjacencyInAdjacentChunk(vectorBlock.Key, BlockFace.Front);
            vectorBlock.Value.Adjacent[VoxelConstants.FACE_FRONT] = checkFace;
        }
        foreach (KeyValuePair<Vector2, Block> vectorBlock in chunkData.backChunkData)
        {
            bool checkFace = CheckBlockAdjacencyInAdjacentChunk(vectorBlock.Key, BlockFace.Back);
            vectorBlock.Value.Adjacent[VoxelConstants.FACE_BACK] = checkFace;
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

    private bool CheckBlockAdjacencyInAdjacentChunk(Vector2 positionToCheck, BlockFace chunkToCheckDirection)
    {
        /*
        switch (chunkToCheckDirection)
        {
            case BlockFace.Front:
                if (chunkData.chunkDataInFront.backChunkData.ContainsKey(positionToCheck))
                {
                    if (chunkData.chunkDataInFront.backChunkData[positionToCheck].Type == BlockType.Air)
                    {
                        return true;
                    }
                }
                return false;
            case BlockFace.Back:
                if (chunkData.chunkDataInFront.frontChunkData.ContainsKey(positionToCheck))
                {
                    if (chunkData.chunkDataInFront.frontChunkData[positionToCheck].Type == BlockType.Air)
                    {
                        return true;
                    }
                }
                return false;
                
        }
        */
        return true;
    }
}
