using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public List<Chunk> chunks = new List<Chunk>();
    public Material material;
    private int chunkNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                for (int y = 0; y < 3; y++)
                {
                    SpawnChunk(new Vector3(x * 16, y * 16, z * 16));
                }
            }
        }

    }

    private void SpawnChunk(Vector3 pos)
    {
        GameObject newChunk = new GameObject("chunk_" + chunkNum.ToString());
        Chunk chunkTest = newChunk.AddComponent<Chunk>();
        chunkTest.InitialiseChunk(1, pos, material);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
