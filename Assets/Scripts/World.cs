using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public struct Cell
{
    public Vector3Int Position;
    public int Type;
    public Vector3 Velocity;
    public int BackFace;
    public int TopFace;
    public int RightFace;
    public int LeftFace;
    public int FrontFace;
    public int BottomFace;
    
}
public class World : MonoBehaviour
{
    public Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>();
    public List<Vector3> chunksToUpdate = new List<Vector3>();
    public Material material;
    public bool UpdateChunks = true;
    public bool RenderChunks = true;
    private int chunkNum = 0;

    public Dictionary<Vector3, Block> worldBlocks = new Dictionary<Vector3, Block>();
    public Cell[] indivCellDatasInput;
    public Cell[] indivCellDatas;

    public int worldSize = 1;
    private int chunkSize;
    private int chunkXSize = 0;
    private int chunkYSize = 0;
    private int chunkZSize = 0;
    public int totalBlockCount;
    public ComputeShader computeShaderUpdate;
    public ComputeShader computeShaderFace;
    
    private void Start()
    {
        chunkXSize = worldSize;
        chunkYSize = worldSize;
        chunkZSize = worldSize;
        chunkSize = VoxelConstants.ChunkSize;
        int chunkID = 0;
        int chunkX = 0;
        int chunkY = 0;
        int chunkZ = 0;
        int totalBlocks = (chunkSize * chunkSize * chunkSize) * (chunkXSize * chunkYSize * chunkZSize);
        int blockCounter = 0;
        totalBlockCount = totalBlocks;
        indivCellDatasInput = new Cell[totalBlocks];
        indivCellDatas = new Cell[totalBlocks];
        //Debug.Log("TOTAL BLOCKS: "+ indivCellDatas.Length);
        
        while (chunkX < chunkXSize)
        {
            while (chunkY < chunkYSize)
            {
                while (chunkZ < chunkZSize)
                {
                    Vector3 chunkWorldPosition = new Vector3(chunkX, chunkY, chunkZ) * chunkSize;
                    
                    
                    List<Vector3> newChunkData = new List<Vector3>();
                    for (int x = 0; x < chunkSize; x++)
                    {
                        for (int y = 0; y < chunkSize; y++)
                        {
                            for (int z = 0; z < chunkSize; z++)
                            {
                                BlockType selected = BlockType.Air;
                                int lifeTime = -1;
                                if (chunkY == chunkYSize - 1)
                                {
                                    selected = BlockType.Sand;
                                }

                                Block createdBlock = new Block(lifeTime, new Vector3(0, 0, 0), selected);
                                Vector3 pos = new Vector3(x, y, z) + chunkWorldPosition;
                                newChunkData.Add(pos);
                                worldBlocks.Add(pos, createdBlock);
                                
                                int idT = 0;
                                switch (selected)
                                {
                                    case BlockType.Stone:
                                        idT = 1;
                                        break;
                                    case BlockType.Sand:
                                        idT = 2;
                                        break;
                                }
                                Cell cellData = new Cell
                                {
                                    Position = Vector3Int.FloorToInt(pos),
                                    Type = idT,
                                    Velocity = Vector3.zero,
                                    BackFace = 0,
                                    TopFace = 0,
                                    RightFace = 0,
                                    LeftFace = 0,
                                    FrontFace = 0,
                                    BottomFace = 0
                                };

                                int posInd = CalculateArrayIndex(pos);
                                //Debug.Log(pos + " : " + posInd );
                                indivCellDatas[posInd] =
                                    cellData;
                                indivCellDatasInput = indivCellDatas;
                                blockCounter++;
                            }

                            blockCounter++;
                        }

                        blockCounter++;
                    }
                    
                    
                    
                    GameObject newChunk = new GameObject("chunk_" + chunkID.ToString());
                    newChunk.transform.parent = this.transform; 
                    Chunk chunkComponent = newChunk.AddComponent<Chunk>();
                    chunkComponent.InitialiseChunk(chunkWorldPosition, newChunkData, material, this, chunkID);
                    chunks.Add(chunkWorldPosition, chunkComponent);
                    chunksToUpdate.Add(chunkWorldPosition);
                    UpdateAdjactentChunkStore(chunkWorldPosition, chunkComponent);
                    
                    
                    chunkID++;
                    chunkZ++;
                }

                chunkZ = 0;
                chunkY++;
            }

            chunkY = 0;
            chunkX++;
        }

        //UpdateBlocks(indivCellDatasInput, indivCellDatas);
        
        //UpdateChunks();
        //s
        

    }

    private void UpdateBlocks(Cell[] input, Cell[] output)
    {
        //Debug.Log("PRE-UPDATE");
        int vector3IntSize = sizeof(int) * 3;
        int intSize = sizeof(int);
        int vector3Size = sizeof(float) * 3;
        int faceBoolSize = sizeof(int) * 6;
        int totalSize = vector3IntSize+ intSize + vector3Size + faceBoolSize;
        int countLength = output.Length;
        
        ComputeBuffer computeBufferInput = new ComputeBuffer(countLength, totalSize);
        computeBufferInput.SetData(input);
        
        ComputeBuffer computeBufferOutput = new ComputeBuffer(countLength, totalSize);
        computeBufferOutput.SetData(output);
        
        computeShaderUpdate.SetBuffer(0, "cellsInput", computeBufferInput);
        computeShaderUpdate.SetBuffer(0, "cellsOutput", computeBufferOutput);
        computeShaderUpdate.SetInt("worldWidth", chunkSize * chunkXSize);
        computeShaderUpdate.SetInt("worldHeight", chunkSize * chunkYSize);
        computeShaderUpdate.SetInt("worldLength", chunkSize * chunkZSize);
        computeShaderUpdate.Dispatch(0, (countLength) / 64, 1,1);
        
        computeBufferOutput.GetData(output);
        input = output;
        computeBufferInput.Dispose();
        computeBufferOutput.Dispose();
        //Debug.Log("POST-UPDATE");
    }
    
    private void UpdateBlockFaces(Cell[] input, Cell[] output)
    {
        //Debug.Log("PRE-UPDATE");
        int vector3IntSize = sizeof(int) * 3;
        int intSize = sizeof(int);
        int vector3Size = sizeof(float) * 3;
        int faceBoolSize = sizeof(int) * 6;
        int totalSize = vector3IntSize+ intSize + vector3Size + faceBoolSize;
        int countLength = output.Length;
        
        ComputeBuffer computeBufferInput = new ComputeBuffer(countLength, totalSize);
        computeBufferInput.SetData(input);
        
        ComputeBuffer computeBufferOutput = new ComputeBuffer(countLength, totalSize);
        computeBufferOutput.SetData(output);
        
        computeShaderFace.SetBuffer(0, "cellsInput", computeBufferInput);
        computeShaderFace.SetBuffer(0, "cellsOutput", computeBufferOutput);
        computeShaderFace.SetInt("worldWidth", chunkSize * chunkXSize);
        computeShaderFace.SetInt("worldHeight", chunkSize * chunkYSize);
        computeShaderFace.SetInt("worldLength", chunkSize * chunkZSize);
        computeShaderFace.Dispatch(0, (countLength) / 64, 1,1);
        
        computeBufferOutput.GetData(output);
        input = output;
        computeBufferInput.Dispose();
        computeBufferOutput.Dispose();
        //Debug.Log("POST-UPDATE");
    }
    
    public int CalculateArrayIndex(Vector3 ind)
    {
        Vector3Int indInt = Vector3Int.FloorToInt(ind);
        int x = indInt.x;
        int y = indInt.y;
        int z = indInt.z;
        int indexer = x + y * (chunkSize * chunkYSize) + z * ((chunkSize * chunkZSize) * (chunkSize * chunkZSize));
        
        if(x < 0 || x >= chunkXSize * chunkSize)
        {
            return -1;
        }
        if(y < 0 || y >= chunkYSize * chunkSize)
        {
            return -1;
        }
        if(z < 0 || z >= chunkZSize * chunkSize)
        {
            return -1;
        }
        return indexer;
    }
    
    private int counter = 0;
    void Update()
    {
        if (UpdateChunks)
        {
            UpdateBlocks(indivCellDatasInput, indivCellDatas);
        }

        if (RenderChunks)
        {
            UpdateBlockFaces(indivCellDatasInput, indivCellDatas); 
            foreach (KeyValuePair<Vector3, Chunk> chunkUpdate in chunks)
            {
                if (chunkUpdate.Value.isChunkModified || chunkUpdate.Value.blocksModified)
                {
                    chunkUpdate.Value.UpdateChunkRenderer();
                    //chunkUpdate.Value.isChunkModified = false;
                }
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        foreach (KeyValuePair<Vector3, Chunk> chunkUpdate in chunks)
        {
            if (chunkUpdate.Value.isChunkModified || chunkUpdate.Value.blocksModified)
            {
                Gizmos.color = new Color(0, 1, 0, 0.5f);
                Gizmos.DrawCube(chunkUpdate.Value.chunkData.chunkPosition + new Vector3(1, 1, 1) * VoxelConstants.ChunkSize / 2,
                    new Vector3(1, 1, 1) * VoxelConstants.ChunkSize);
            }
        }
    }
  
 
    
    
    
    
    private void UpdateAdjactentChunkStore(Vector3 pos, Chunk chunkTest)
    {
        //Debug.Log("CURRENT CHUNK ID: " + chunkTest.chunkID);
        
        Vector3 chunkKeyCheck = pos + VoxelConstants.FaceFront * VoxelConstants.ChunkSize;
        //Debug.Log("TEST IF CHUNK EXISTS AT " + chunkKeyCheck);
        if (chunks.ContainsKey(chunkKeyCheck))
        {
            chunkTest.chunkInFront = chunks[chunkKeyCheck];
            chunks[chunkKeyCheck].chunkInBack = chunkTest;
            //Debug.Log("CURRENT FRONT CHUNK ID: " + chunkTest.chunkInFront.chunkID);
        }

        chunkKeyCheck = pos + VoxelConstants.FaceBack * VoxelConstants.ChunkSize;
        //Debug.Log("TEST IF CHUNK EXISTS AT " + chunkKeyCheck);
        if (chunks.ContainsKey(chunkKeyCheck))
        {
            chunkTest.chunkInBack = chunks[chunkKeyCheck];
            chunks[chunkKeyCheck].chunkInFront = chunkTest;
            //Debug.Log("CURRENT BACK CHUNK ID: " + chunkTest.chunkInBack.chunkID);
        }
        
        
        chunkKeyCheck = pos + VoxelConstants.FaceTop * VoxelConstants.ChunkSize;
        //Debug.Log("TEST IF CHUNK EXISTS AT " + chunkKeyCheck);
        if (chunks.ContainsKey(chunkKeyCheck))
        {
            chunkTest.chunkInTop = chunks[chunkKeyCheck];
            chunks[chunkKeyCheck].chunkInBottom = chunkTest;
            //Debug.Log("CURRENT TOP CHUNK ID: " + chunkTest.chunkInTop.chunkID);
        }

        chunkKeyCheck = pos + VoxelConstants.FaceBottom * VoxelConstants.ChunkSize;
        //Debug.Log("TEST IF CHUNK EXISTS AT " + chunkKeyCheck);
        if (chunks.ContainsKey(chunkKeyCheck))
        {
            chunkTest.chunkInBottom = chunks[chunkKeyCheck];
            chunks[chunkKeyCheck].chunkInTop = chunkTest;
            //Debug.Log("CURRENT BACK CHUNK ID: " + chunkTest.chunkInBottom.chunkID);
        }
        
        
        chunkKeyCheck = pos + VoxelConstants.FaceRight * VoxelConstants.ChunkSize;
        //Debug.Log("TEST IF CHUNK EXISTS AT " + chunkKeyCheck);
        if (chunks.ContainsKey(chunkKeyCheck))
        {
            chunkTest.chunkInRight = chunks[chunkKeyCheck];
            chunks[chunkKeyCheck].chunkInLeft = chunkTest;
            //Debug.Log("CURRENT RIGHT CHUNK ID: " + chunkTest.chunkInRight.chunkID);
        }

        chunkKeyCheck = pos + VoxelConstants.FaceLeft * VoxelConstants.ChunkSize;
        //Debug.Log("TEST IF CHUNK EXISTS AT " + chunkKeyCheck);
        if (chunks.ContainsKey(chunkKeyCheck))
        {
            chunkTest.chunkInLeft = chunks[chunkKeyCheck];
            chunks[chunkKeyCheck].chunkInRight = chunkTest;
            //Debug.Log("CURRENT BACK CHUNK ID: " + chunkTest.chunkInLeft.chunkID);
        }
    }
    
}