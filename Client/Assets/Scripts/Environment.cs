using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment
{
    public IEnumerable<Item> Items { get; private set; }
    public Environment(IEnumerable<Item> items)
    {
        this.Items = items;
    }
}