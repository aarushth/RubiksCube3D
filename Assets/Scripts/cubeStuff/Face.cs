using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Face
{

    private Colour colour;
    private Orientation orientation;


    public Face(Colour c, Orientation o)
    {
        colour = c;
        orientation = o;
    }

    public Orientation getOrientation()
    {
        return orientation;
    }
    public Colour getColour()
    {
        return colour;
    }

    public void setOrientation(Orientation o)
    {
        orientation = o;
    }

}