using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    
    public Orientation PositiveX = new Orientation(0,new Vector3(1, 0, 0), Colour.RED);
    public Orientation PositiveY = new Orientation(1, new Vector3(0, 1, 0), Colour.WHITE);
    public Orientation PositiveZ = new Orientation(2, new Vector3(0, 0, 1), Colour.BLUE);
    public Orientation NegativeX = new Orientation(0, new Vector3(-1, 0, 0), Colour.ORANGE);
	public Orientation NegativeY = new Orientation(1, new Vector3(0, -1, 0), Colour.YELLOW);
    public Orientation NegativeZ = new Orientation(2, new Vector3(0, 0, -1), Colour.GREEN);





    private Dictionary<Orientation, Face> faceByOrientation = new Dictionary<Orientation, Face>();
    private Dictionary<Colour, Face> faceByColour = new Dictionary<Colour, Face>();
    protected List<Face> faces = new List<Face>();
    private Vector3 pos;
    private Vector3 correctPos;
    private static Dictionary<Vector3, Orientation> orientationVectors = new Dictionary<Vector3, Orientation>();
    
    private void putFaces() {
        orientationVectors[new Vector3(1, 0, 0)] = PositiveX;
        orientationVectors[new Vector3(0, 1, 0)] = PositiveY;
        orientationVectors[new Vector3(0, 0, 1)] = PositiveZ;
        orientationVectors[new Vector3(-1, 0, 0)] = NegativeX;
        orientationVectors[new Vector3(0, -1, 0)] = NegativeY;
        orientationVectors[new Vector3(0, 0, -1)] = NegativeZ;
    }
    public virtual bool isEdge()
    {
        return false;
    }
    public void setPos(Vector3 p)
    {
        pos = p;
    }
    public Vector3 getPos()
    {
        return pos;
    }
    public Vector3 getSolvedPos()
    {
        return correctPos;
    }
    public virtual bool isCorner()
    {
        return false;
    }
    public List<Face> getFaces()
    {

        return faces;
    }
    public Piece(Face face0, Face face1, Face face2, Vector3 p)
    {
        putFaces();
        pos = p;
        correctPos = p;
        if (face0 != null)
        {
            faceByOrientation[face0.getOrientation()] = face0;
            faceByColour[face0.getColour()] = face0;
            faces.Add(face0);
        }
        if (face1 != null)
        {
            faceByOrientation[face1.getOrientation()] = face1;
            faceByColour[face1.getColour()] = face1;
            faces.Add(face1);
        }
        if (face2 != null)
        {
            faceByOrientation[face2.getOrientation()] = face2;
            faceByColour[face2.getColour()] = face2;
            faces.Add(face2);
        }

    }

    public Colour getColourOfFace(Orientation o)
    {
        if (faceByOrientation.ContainsKey(o))
        {
            return faceByOrientation[o].getColour();
        }
        return Colour.NULL;
    }
    public Orientation getOrientationOfColour(Colour c)
    {
        for (int i = 0; i < faces.Count; i++)
        {
            if (faces[i].getColour() == c)
            {
                return faces[i].getOrientation();
            }
        }
        return null;
    }
    public bool isSolved()
    {
        for (int i = 0; i < faces.Count; i++)
        {
            if (!faces[i].getColour().Equals(faces[i].getOrientation().getColour()))
            {
                return false;
            }
        }
        return true;
    }
    public bool hasFace(Colour c)
    {
        for (int i = 0; i < faces.Count; i++)
        {
            if (faces[i].getColour() == c)
            {
                return true;
            }
        }
        return false;
    }
    public bool isColourFacingDirection(Colour c, Orientation o)
    {
        if (!hasFace(c))
        {
            return false;
        }
        for (int i = 0; i < faces.Count; i++)
        {
            if (faces[i].getColour() == c && faces[i].getOrientation().Equals(o))
            {
                return true;
            }
        }
        return false;
    }
    public Colour getNotThisColour(Colour c)
    {
        for (int i = 0; i < faces.Count; i++)
        {
            if (faces[i].getColour() != c)
            {
                return faces[i].getColour();
            }
        }
        return Colour.NULL;
    }
    public Colour getNotThisColour(Colour c1, Colour c2)
    {
        for (int i = 0; i < faces.Count; i++)
        {
            if (faces[i].getColour() != c1 && faces[i].getColour() != c2)
            {
                return faces[i].getColour();
            }
        }
        return Colour.NULL;
    }

    public void rotate(Orientation o)
    {
        faceByOrientation.Clear();
        for (int i = 0; i < faces.Count; i++)
        {
            Face f = faces[i];
            if (f.getOrientation().getValue() != o.getValue())
            {
                Vector3 v = Vector3.Cross(o.getVector(), f.getOrientation().getVector());
                f.setOrientation(orientationVectors[v]);
                faceByOrientation[f.getOrientation()] =  f;
                faces[i] = f;
            }
            else
            {
                faceByOrientation[f.getOrientation()] = f;
                faces[i] = f;
            }
        }
    }
}
