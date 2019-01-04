using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int Id;
    public int Score;
    public Vector3 Position;

    public Player(int id, int score, Vector3 pos)
    {
        this.Id = id;
        this.Score = score;
        this.Position = pos;
    }
}
