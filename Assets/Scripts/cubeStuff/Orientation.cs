using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orientation
{

    
    private int value;
    private Vector3 vector;
    private Colour colour;

    public Orientation(int i, Vector3 v, Colour c) {
        value = i;
        vector = v;
        colour = c;
    }

    public int getValue()
    {
        return value;
    }
    public Vector3 getVector()
    {
        return vector;
    }
    public Colour getColour()
    {
        return colour;
    }
    public bool Equals(Orientation o) {
        return (this.value == o.value && this.colour == o.colour);
    }
}
