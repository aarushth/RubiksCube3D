using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move 
{
    private string move;
    private bool dir;
    public Move(string m, bool d)
    {
        this.move = m;
        this.dir = d;
    }
    public string getMove() {
        return move;
    }
    public bool getDir() {
        return dir;
    }
}
