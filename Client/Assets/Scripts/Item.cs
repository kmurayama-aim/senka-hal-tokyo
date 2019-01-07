using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int Id { get; private set; }
    public Vector3 Position { get; private set; }

    public Item(int id, Vector3 position)
    {
        this.Id = id;
        this.Position = position;
    }
}