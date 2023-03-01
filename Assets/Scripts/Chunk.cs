using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof (ChunkRenderer))]
public class Chunk : MonoBehaviour
{
    public ChunkRenderer chunkRenderer;
    public ChunkData chunkData;
    public int testChunkType;
    public Material material;
    void Awake()
    {
        
        
        InitialiseChunk(0, new Vector3(0, 0, 0), material);
    }

    public void InitialiseChunk(int type, Vector3 chunkPosition, Material chunkMaterial)
    {
        chunkRenderer = GetComponent<ChunkRenderer>();
        Debug.Log("1");
        ChunkData chunkDataInit = new ChunkData(chunkPosition, type);
        Debug.Log("2");
        chunkRenderer.InitialiseChunkData(chunkDataInit, chunkMaterial);
        Debug.Log("3");
        chunkData = chunkRenderer.chunkData;
        Debug.Log("4");
        InitialiseActiveFaces();
        Debug.Log("5");
        SetTriangles();
        Debug.Log("6");
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

}
