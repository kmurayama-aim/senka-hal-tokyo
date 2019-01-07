using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int Id { get; private set; }
    public int Score { get; private set; }
    public Vector3 Position { get; private set; }

    public Player(int id, int score, Vector3 pos)
    {
        this.Id = id;
        this.Score = score;
        this.Position = pos;
    }
}
