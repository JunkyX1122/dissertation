using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderTest2 : MonoBehaviour
{
    public struct Cube
    {
        public int id;
        public int type;
        public Vector3Int position;
        public Vector3 velocity;
        public int[] blockMemory;
    }
    private Cube[] data = new Cube[4096];
    private int[] worldData = new int[4096];
    private void Start()
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    Cube cubeData = new Cube();
                    cubeData.id = CalculateArrayIndex(x, y, z);
                    cubeData.position = new Vector3Int(x, y, z);
                    cubeData.velocity = new Vector3(0, -1, 0);
                    
                    if (x == 8 && y == 8 && z == 8)
                    {
                        cubeData.type = 1;
                    }
                    else
                    {
                        cubeData.type = 0;
                    }

                    data[CalculateArrayIndex(x, y, z)] = cubeData;
                    worldData[CalculateArrayIndex(x, y, z)] = cubeData.type;
                }
            }
        }
    }
    private int CalculateArrayIndex(int x, int y, int z)
    {
        return x + y * 16 + z * 16 * 16;
    }
    private void Update()
    {
        //ComputeBuffer buffer = new ComputeBuffer(data.L)
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < 4096; i++)
            {
                Gizmos.color = new Color(1,1,1,1 * data[i].type);
                Gizmos.DrawCube(data[i].position, Vector3.one);
            }
        }
    }
}
