using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public int LifeTime;
    public Vector3 Velocity;
    public bool NeedsUpdating = true;
    public BlockType Type { get; set; }
    public bool[] Adjacent = new bool[6];

    //public float ElevationError = 0f;
    //public Vector3 Position { get; set; }

    public Block(int lifeTime, Vector3 velocity, BlockType blockType)
    {
        this.LifeTime = lifeTime;
        this.Velocity = velocity;
        this.Type = blockType;
        for (int i = 0; i < 6; i++)
        {
            Adjacent[i] = false;
        }
        //this.Position = blockPosition;
    }

    
}
