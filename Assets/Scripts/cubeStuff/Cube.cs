using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cube 
{
    public Orientation PositiveX = new Orientation(0, new Vector3(1, 0, 0), Colour.RED);
    public Orientation PositiveY = new Orientation(1, new Vector3(0, 1, 0), Colour.WHITE);
    public Orientation PositiveZ = new Orientation(2, new Vector3(0, 0, 1), Colour.BLUE);
    public Orientation NegativeX = new Orientation(0, new Vector3(-1, 0, 0), Colour.ORANGE);
    public Orientation NegativeY = new Orientation(1, new Vector3(0, -1, 0), Colour.YELLOW);
    public Orientation NegativeZ = new Orientation(2, new Vector3(0, 0, -1), Colour.GREEN);


    private Piece[,,] cube = new Piece[3,3,3];
    public Piece[,,] getCube()
    {
        return cube;
    }
    private Piece[,,] getCubeCopy()
    {
        Piece[,,] cubeCopy = new Piece[3,3,3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    cubeCopy[i,j,k] = cube[i,j,k];
                }
            }
        }
        return cubeCopy;
    }
    private Face whichXFace(int i)
    {
        if (i < 0)
        {
            return new Face(Colour.ORANGE,  NegativeX);
        }
        else if (i > 0)
        {
            return new Face(Colour.RED,  PositiveX);
        }
        else
        {
            return null;
        }
    }
    private Face whichYFace(int i)
    {
        if (i < 0)
        {
            return new Face(Colour.YELLOW,  NegativeY);
        }
        else if (i > 0)
        {
            return new Face(Colour.WHITE,  PositiveY);
        }
        else
        {
            return null;
        }
    }
    private Face whichZFace(int i)
    {
        if (i < 0)
        {
            return new Face(Colour.GREEN,  NegativeZ);
        }
        else if (i > 0)
        {
            return new Face(Colour.BLUE,  PositiveZ);
        }
        else
        {
            return null;
        }
    }
    public Cube()
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                for (int k = -1; k < 2; k++)
                {
                    switch (Math.Abs(i) + Math.Abs(j) + Math.Abs(k))
                    {
                        case 3:
                            cube[i + 1,j + 1,k + 1] = new Corner(whichXFace(i), whichYFace(j), whichZFace(k), new Vector3(i + 1, j + 1, k + 1));
                            break;
                        case 2:
                            cube[i + 1,j + 1,k + 1] = new Edge(whichXFace(i), whichYFace(j), whichZFace(k), new Vector3(i + 1, j + 1, k + 1));
                            break;
                        case 1:
                            cube[i + 1,j + 1,k + 1] = new Center(whichXFace(i), whichYFace(j), whichZFace(k), new Vector3(i + 1, j + 1, k + 1));
                            break;
                        case 0:
                            cube[i + 1,j + 1,k + 1] = null;
                            break;
                    }
                }
            }
        }
    }

    private void upPriv()
    {
        Piece[,,] temp = getCubeCopy();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cube[i,2,j].rotate(PositiveY);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cube[i,2,j] = temp[2 - j,2,i];
                cube[i,2,j].setPos(new Vector3(i, 2, j));
            }
        }
    }
    public Move up()
    {
        upPriv();
        return new Move("u", false);
    }
    public Move upPrime()
    {
        upPriv();
        upPriv();
        upPriv();
        return new Move("u", true);
    }

    private void downPriv()
    {
        Piece[,,] temp = getCubeCopy();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cube[i,0,j].rotate(NegativeY);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cube[i,0,j] = temp[j,0,2 - i];
                cube[i,0,j].setPos(new Vector3(i, 0, j));
            }
        }
    }
    public Move down( )
    {
        downPriv();
        return new Move("d", false);
    }
    public Move downPrime( )
    {
        downPriv();
        downPriv();
        downPriv();
        return new Move("d", true);
    }

    private void rightPriv()
    {
        Piece[,,] temp = getCubeCopy();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cube[2,j,i].rotate(PositiveX);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cube[2,j,i] = temp[2,i,2 - j];
                cube[2,j,i].setPos(new Vector3(2, j, i));
            }
        }
    }
    public Move right( )
    {
        rightPriv();
        return new Move("r", false);
    }
    public Move rightPrime( )
    {
        rightPriv();
        rightPriv();
        rightPriv();
        return new Move("r", true);
    }

    private void leftPriv()
    {

        Piece[,,] temp = getCubeCopy();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cube[0,j,i].rotate(NegativeX);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cube[0,j,i] = temp[0,2 - i,j];
                cube[0,j,i].setPos(new Vector3(0, j, i));
            }
        }
    }
    public Move left( )
    {
        leftPriv();
        return new Move("l", false);
    }
    public Move leftPrime( )
    {
        leftPriv();
        leftPriv();
        leftPriv();
        return new Move("l", true);
    }

    private void backPriv()
    {
        Piece[,,] temp = getCubeCopy();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cube[i,j,2].rotate(PositiveZ);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cube[i,j,2] = temp[j,2 - i,2];
                cube[i,j,2].setPos(new Vector3(i, j, 2));
            }
        }
    }
    public Move back( )
    {
        backPriv();
        return new Move("b", false);
    }
    public Move backPrime( )
    {
        backPriv();
        backPriv();
        backPriv();
        return new Move("b", true);
    }

    private void frontPriv()
    {
        Piece[,,] temp = getCubeCopy();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cube[i,j,0].rotate(NegativeZ);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cube[i,j,0] = temp[2 - j,i,0];
                cube[i,j,0].setPos(new Vector3(i, j, 0));
            }
        }
    }
    public Move front( )
    {
        frontPriv();
        return new Move("f", false);
    }
    public Move frontPrime( )
    {
        frontPriv();
        frontPriv();
        frontPriv();
        return new Move("f", true);
    }

    public void turn(Move m) { 
        string side = m.getMove();
        bool dir = m.getDir();
        Move not;
        switch (side) {
            case "f":
                not = (dir) ? frontPrime() : front();
                break;
            case "b":
                not = (dir)? backPrime() : back();
                break;
            case "l":
                not = (dir)? leftPrime():left();
                break;
            case "r":
                not = (dir)? rightPrime():right();
                break;
            case "u":
                not = (dir)? upPrime():up();
                break;
            case "d":
                not = (dir)? downPrime():down();
                break;
        }
    }
    public Move turnAroundOrientation(Orientation o)
    {
        if (o.Equals(PositiveX)) {
            return right();
        } else if (o.Equals(NegativeX)) {
            return left();
        } else if (o.Equals(PositiveZ)) {
            return back();
        } else if (o.Equals(NegativeZ)) {
            return front();
        } else if (o.Equals(PositiveY)) {
            return up();
        } else if (o.Equals(NegativeY)) {
            return down();
        }
        return null;
    }
    public bool  crossSolved()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (cube[i,0,j].isEdge())
                {
                    if (!cube[i,0,j].isSolved())
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    public Edge getCrossPiece()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    if (cube[i,j,k] != null)
                    {
                        if (cube[i,j,k].isEdge() && cube[i,j,k].hasFace(Colour.YELLOW) && !(cube[i,j,k].isSolved()))
                        {
                            return (Edge)cube[i,j,k];
                        }
                    }
                }
            }
        }
        return null;
    }
    public bool  bottomCornersSolved()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (cube[i,0,j].isCorner())
                {
                    if (!cube[i,0,j].isSolved())
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    public Corner getBottomCornerPiece()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    if (cube[i,j,k] != null)
                    {
                        if (cube[i,j,k].isCorner() && cube[i,j,k].hasFace(Colour.YELLOW) && !(cube[i,j,k].isSolved()))
                        {
                            return (Corner)cube[i,j,k];
                        }
                    }
                }
            }
        }
        return null;
    }
    public bool  secondLayerSolved()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (cube[i,1,j] != null && !cube[i,1,j].isSolved())
                {
                    return false;
                }
            }
        }
        return true;
    }
    public Edge getSecondLayerPiece()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 1; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    if (cube[i,j,k] != null && cube[i,j,k].isEdge() && !cube[i,j,k].isSolved() && !cube[i,j,k].hasFace(Colour.WHITE))
                    {
                        return (Edge)cube[i,j,k];
                    }
                }
            }
        }
        return null;
    }
    public bool  topCrossSolved()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (cube[i,2,j].isEdge() && !cube[i,2,j].isColourFacingDirection(Colour.WHITE,  PositiveY))
                {
                    return false;
                }
            }
        }
        return true;
    }
    public bool  noTopCross()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (cube[i,2,j].isEdge() && cube[i,2,j].isColourFacingDirection(Colour.WHITE,  PositiveY))
                {
                    return false;
                }
            }
        }
        return true;
    }
    public bool  barTopCross()
    {
        if ((cube[1,2,0].isColourFacingDirection(Colour.WHITE,  PositiveY) && cube[1,2,2].isColourFacingDirection(Colour.WHITE,  PositiveY)) || (cube[0,2,1].isColourFacingDirection(Colour.WHITE,  PositiveY) && cube[2,2,1].isColourFacingDirection(Colour.WHITE,  PositiveY)))
        {
            return true;
        }
        return false;
    }
    public bool  barTopCrossAligned()
    {
        if (cube[0,2,1].isColourFacingDirection(Colour.WHITE,  PositiveY) && cube[2,2,1].isColourFacingDirection(Colour.WHITE,  PositiveY))
        {
            return true;
        }
        return false;
    }
    public bool  OLLcomplete()
    {
        for (int i = 0; i < 3; i += 2)
        {
            for (int j = 0; j < 3; j += 2)
            {
                if (!cube[i,2,j].isColourFacingDirection(Colour.WHITE, PositiveY))
                {
                    return false;
                }
            }
        }
        return true;
    }
    public int OLLCorners()
    {
        int ans = 0;
        for (int i = 0; i < 3; i += 2)
        {
            for (int j = 0; j < 3; j += 2)
            {
                if (cube[i,2,j].isColourFacingDirection(Colour.WHITE, PositiveY))
                {
                    ans++;
                }
            }
        }
        return ans;
    }
    public Piece getPiece(int x, int y, int z)
    {
        return cube[x,y,z];
    }
    public bool  topCornersSolved()
    {
        for (int i = 0; i < 3; i += 2)
        {
            for (int j = 0; j < 3; j += 2)
            {
                if (!cube[i,2,j].isSolved())
                {
                    return false;
                }
            }
        }
        return true;
    }
    public bool  topEdgesSolved()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (cube[i,2,j].isEdge() && !cube[i,2,j].isSolved())
                {
                    return false;
                }
            }
        }
        return true;
    }
    public Edge getTopSolvedEdge()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (cube[i,2,j].isEdge() && cube[i,2,j].isSolved())
                {
                    return (Edge)cube[i,2,j];
                }
            }
        }
        return null;
    }
}
