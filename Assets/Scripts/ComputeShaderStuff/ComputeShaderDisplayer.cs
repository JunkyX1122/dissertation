using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class ComputeShaderDisplayer : MonoBehaviour
{
    /*
    public ComputeShader CellAutoShader;
    public ComputeShader SetInputShader;
    
    public RenderTexture Result;
    public RenderTexture Input;
    */

    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    public Texture3D newGrid;
    void Start()
    {
        /*
        Result = new RenderTexture(16, 16, 32);
        Result.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        Result.volumeDepth = 16;
        Result.enableRandomWrite = true;
        Result.Create();
        
        Input = new RenderTexture(16, 16, 32);
        Input.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        Input.volumeDepth = 16;
        Input.enableRandomWrite = true;
        Input.Create();
        
        CellAutoShader.SetTexture(0, "input", Input);
        CellAutoShader.SetTexture(0, "result", Result);
        
        SetInputShader.SetTexture(0, "input", Input);
        SetInputShader.SetTexture(0, "result", Result);
        */
        renderTexture = new RenderTexture(16, 16, 0);
        renderTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        renderTexture.volumeDepth = 16;
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        //Graphics.SetRandomWriteTarget(0, renderTexture);
        
        computeShader.SetTexture(0, "test", renderTexture);
        computeShader.Dispatch(0, 8, 8, 8);

        newGrid = new Texture3D(16, 16, 16, TextureFormat.RGBA32, false);
        //newGrid.isReadable = true;
        newGrid.Apply(false);
        Graphics.CopyTexture(renderTexture, newGrid);

        //Graphics.Blit(newGrid, renderTexture);
        
        for (int x = 0; x < 16; x++)
        {
            Debug.Log(newGrid.GetPixelBilinear(x,0,0));
        }
    }
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        Gizmos.color = newGrid.GetPixel(x/16, y/16, z/16);
                        Gizmos.DrawSphere(new Vector3(x, y, z), 1);
                    }
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnUpdateStuff(RenderTexture source, RenderTexture dest)
    {
        /*
        CellAutoShader.Dispatch(0, Result.width / 8, Result.height /8, Result.volumeDepth / 8);
        SetInputShader.Dispatch(0, Input.width / 8, Input.height / 8, Result.volumeDepth / 8);
        Graphics.Blit(Result, dest);
        //Gizmos.color = new Color(1, 0, 0, 0.5f);
        //Gizmos.DrawCube(chunkData.chunkPosition + new Vector3(1, 1, 1) * VoxelConstants.ChunkSize / 2, new Vector3(1, 1, 1) * VoxelConstants.ChunkSize);
        */
    }
}
