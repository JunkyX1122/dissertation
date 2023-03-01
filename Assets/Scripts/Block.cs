using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public BlockType Type { get; set; }
    public bool[] Adjacent = new bool[6];
    //public Vector3 Position { get; set; }

    public Block(BlockType blockType)
    {
        this.Type = blockType;
        for (int i = 0; i < 6; i++)
        {
            Adjacent[i] = false;
        }
        //this.Position = blockPosition;
    }
}
