using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int Id;
    public Vector3 Position;

    public Item(int id, Vector3 position)
    {
        this.Id = id;
        this.Position = position;
    }
}