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
    public ChunkData chunkData= null;
    
    public Chunk chunkInFront = null;
    public Chunk chunkInTop = null;
    public Chunk chunkInRight = null;
    public Chunk chunkInLeft = null;
    public Chunk chunkInBack = null;
    public Chunk chunkInBottom = null;
    
    public int testChunkType;
    public Material material;
    public World world;
    public int chunkID;
    void Awake()
    {
        
        
        //InitialiseChunk(0, new Vector3(0, 0, 0), material);
    }

    void OnDrawGizmosSelected()
    {
        if (chunkData != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawCube(chunkData.chunkPosition + new Vector3(1, 1, 1) * VoxelConstants.ChunkSize / 2, new Vector3(1, 1, 1) * VoxelConstants.ChunkSize);
            
        }
    }

    public void InitialiseChunk(int type, Vector3 chunkPosition, Material chunkMaterial, World worldReference, int chunkIdSet)
    {
        chunkID = chunkIdSet;
        world = worldReference;
        chunkRenderer = GetComponent<ChunkRenderer>();
        Debug.Log("Chunk - InitialiseChunk: Create chunk data.");
        ChunkData chunkDataInit = new ChunkData(chunkPosition, type);
        Debug.Log("Chunk - InitialiseChunk: Initialise chunk data to chunk.");
        chunkRenderer.InitialiseChunkData(chunkDataInit, chunkMaterial);
        Debug.Log("Chunk - InitialiseChunk: Set chunk data chunk renderer.");
        chunkData = chunkRenderer.chunkData;
        //Debug.Log("Chunk - InitialiseChunk: Initialise active triangle faces based on voxels in chunk.");
        //UpdateActiveFaces();
        Debug.Log("Chunk - InitialiseChunk: Set triangles.");
        SetTriangles();
        Debug.Log("Chunk - InitialiseChunk: Update chunk renderer.");
        chunkRenderer.UpdateChunkRender(chunkData);
    }

    public void UpdateChunk()
    {
        Debug.Log("Chunk - UpdateChunk: Update active triangle faces based on voxels in chunk.");
        UpdateActiveFaces();
        Debug.Log("Chunk - UpdateChunk: Update active triangle faces based on voxels in adjacent chunks.");
        UpdateActiveFacesOnBorder();
        Debug.Log("Chunk - UpdateChunk: Set triangles.");
        SetTriangles();
        chunkRenderer.UpdateChunkRender(chunkData);
    }
    private void Update()
    {
        /*
        if (chunkID == 0)
        {
            foreach (KeyValuePair<Vector3, Block> vectorBlock in chunkData.blocks)
            {
                vectorBlock.Value.Type = Random.value < 0.5f ? BlockType.Solid : BlockType.Air;
            }

            UpdateActiveFaces();
            SetTriangles();
            chunkRenderer.UpdateChunkRender(chunkData);
        }
        */
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
        Debug.Log("Chunk - UpdateActiveFacesOnBorder: Test Front.");
        if (chunkInFront != null)
        {
            foreach (KeyValuePair<Vector2, Block> vectorBlock in chunkData.frontChunkData)
            {
                Vector2 position = vectorBlock.Key;
                bool checkFace = CheckBlockAdjacencyInAdjacentChunk(position, chunkInFront.chunkData.backChunkData);
                vectorBlock.Value.Adjacent[VoxelConstants.FACE_FRONT] = checkFace;
            }
        }
        Debug.Log("Chunk - UpdateActiveFacesOnBorder: Test Back.");
        if (chunkInBack != null)
        {
            foreach (KeyValuePair<Vector2, Block> vectorBlock in chunkData.backChunkData)
            {
                Vector2 position = vectorBlock.Key;
                bool checkFace = CheckBlockAdjacencyInAdjacentChunk(position, chunkInBack.chunkData.frontChunkData);
                vectorBlock.Value.Adjacent[VoxelConstants.FACE_BACK] = checkFace;
            }
        }
        
        Debug.Log("Chunk - UpdateActiveFacesOnBorder: Test Top.");
        if (chunkInTop != null)
        {
            foreach (KeyValuePair<Vector2, Block> vectorBlock in chunkData.topChunkData)
            {
                Vector2 position = vectorBlock.Key;
                bool checkFace = CheckBlockAdjacencyInAdjacentChunk(position, chunkInTop.chunkData.bottomChunkData);
                vectorBlock.Value.Adjacent[VoxelConstants.FACE_UP] = checkFace;
            }
        }
        Debug.Log("Chunk - UpdateActiveFacesOnBorder: Test Bottom.");
        if (chunkInBottom != null)
        {
            foreach (KeyValuePair<Vector2, Block> vectorBlock in chunkData.bottomChunkData)
            {
                Vector2 position = vectorBlock.Key;
                bool checkFace = CheckBlockAdjacencyInAdjacentChunk(position, chunkInBottom.chunkData.topChunkData);
                vectorBlock.Value.Adjacent[VoxelConstants.FACE_BOTTOM] = checkFace;
            }
        }
        
        Debug.Log("Chunk - UpdateActiveFacesOnBorder: Test Right.");
        if (chunkInRight != null)
        {
            foreach (KeyValuePair<Vector2, Block> vectorBlock in chunkData.rightChunkData)
            {
                Vector2 position = vectorBlock.Key;
                bool checkFace = CheckBlockAdjacencyInAdjacentChunk(position, chunkInRight.chunkData.leftChunkData);
                vectorBlock.Value.Adjacent[VoxelConstants.FACE_RIGHT] = checkFace;
            }
        }
        Debug.Log("Chunk - UpdateActiveFacesOnBorder: Test Left.");
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
}
