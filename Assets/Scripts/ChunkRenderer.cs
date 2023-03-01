using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof (MeshFilter))]
[RequireComponent(typeof (MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    public ChunkData chunkData;
    
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    private Material material;
    
    public List<Vector3> chunkVertecies = new List<Vector3>();
    public List<int> chunkTriangles = new List<int>();
    public List<Vector2> chunkUVs = new List<Vector2>();
    public List<Color> chunkBlockColours = new List<Color>();

    public bool isChunkModified
    {
        get
        {
            return chunkData.isChunkModified;
        }
        set
        {
            chunkData.isChunkModified = value;
        }
    }

    public void InitialiseChunkData(ChunkData chunkData, Material chunkMaterial)
    {
        Debug.Log("Initialise Chunk");
        this.chunkData = chunkData;
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = GetComponent<MeshFilter>().mesh;
        material = chunkMaterial;
    }
    
    
    public void UpdateChunkRender(ChunkData chunkData)
    {
        Debug.Log("Updating Chunk");
        mesh.Clear ();
        mesh.vertices = chunkVertecies.ToArray();
        mesh.triangles = chunkTriangles.ToArray();
        mesh.uv = chunkUVs.ToArray();
        List<Material> materialList = new List<Material>();
        materialList.Add(material);
        meshRenderer.materials = materialList.ToArray();
        
        //mesh.colors = chunkBlockColours.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
    }
    
    

}
