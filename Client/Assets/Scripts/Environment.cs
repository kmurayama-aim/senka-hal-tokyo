﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment
{
    public List<Item> Items;
    public Environment(List<Item> items)
    {
        this.Items = items;
    }
}