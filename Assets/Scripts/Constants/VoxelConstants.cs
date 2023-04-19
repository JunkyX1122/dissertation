using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelConstants
{
    
    public static Vector3[] CubeVertecies = new Vector3[8]{
        new Vector3 (0, 0, 0),//0
        new Vector3 (1, 0, 0),//1
        new Vector3 (1, 1, 0),//2
        new Vector3 (0, 1, 0),//3
        new Vector3 (0, 1, 1),//4
        new Vector3 (1, 1, 1),//5
        new Vector3 (1, 0, 1),//6
        new Vector3 (0, 0, 1),//7
    };
    public static int[,] Triangles = new int[6, 6]
    {
        {0, 2, 1, //face back
            0, 3, 2},
        
        {1, 2, 5, //face top
            1, 5, 6},
        
        {2, 3, 4, //face right
            2, 4, 5},

        {0, 7, 4, //face left
            0, 4, 3},
        
        {5, 4, 7, //face front
            5, 7, 6},
        
        {0, 6, 7, //face bottom
            0, 1, 6}
    };
    
    public static Vector3[] CubeVertexCheck = new Vector3[6]
    {
        new Vector3 (0, 0, -1),
        new Vector3 (0, 1, 0),
        new Vector3 (1, 0, 0),
        new Vector3 (-1, 0, 0),
        new Vector3 (0, 0, 1),
        new Vector3 (0, -1, 0)
    };

    public static Vector3 FaceBack = CubeVertexCheck[0];
    public static Vector3 FaceTop = CubeVertexCheck[1];
    public static Vector3 FaceRight = CubeVertexCheck[2];
    public static Vector3 FaceLeft = CubeVertexCheck[3];
    public static Vector3 FaceFront = CubeVertexCheck[4];
    public static Vector3 FaceBottom = CubeVertexCheck[5];


    public static Vector2[] UV = new Vector2[8]
    {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(1, 1)
    };
    
    public static int ChunkSize = 16;
    public static int WorldSize = 3;
}