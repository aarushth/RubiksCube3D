using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corner : Piece
{


    public Corner(Face face0, Face face1, Face face2, Vector3 p) : base(face0, face1, face2, p){}

    public override bool isCorner()
    {
        return true;
    }

}