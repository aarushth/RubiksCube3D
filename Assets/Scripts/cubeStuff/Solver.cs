using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solver
{
    public Orientation PositiveX = new Orientation(0, new Vector3(1, 0, 0), Colour.RED);
    public Orientation PositiveY = new Orientation(1, new Vector3(0, 1, 0), Colour.WHITE);
    public Orientation PositiveZ = new Orientation(2, new Vector3(0, 0, 1), Colour.BLUE);
    public Orientation NegativeX = new Orientation(0, new Vector3(-1, 0, 0), Colour.ORANGE);
    public Orientation NegativeY = new Orientation(1, new Vector3(0, -1, 0), Colour.YELLOW);
    public Orientation NegativeZ = new Orientation(2, new Vector3(0, 0, -1), Colour.GREEN);

    private Dictionary<Colour, Orientation> orientationOfColour = new Dictionary<Colour, Orientation>();
    public Solver() {  
		orientationOfColour[Colour.RED] =  PositiveX;
		orientationOfColour[Colour.ORANGE] = NegativeX;
		orientationOfColour[Colour.BLUE] = PositiveZ;
		orientationOfColour[Colour.GREEN] = NegativeZ;
		orientationOfColour[Colour.WHITE] =  PositiveY;
		orientationOfColour[Colour.YELLOW] = NegativeY;
	}


    public List<Move> solveCube(Cube c)
    {
        List<Move> solution = new List<Move>();
        while (!c.crossSolved())
        //for(int i = 0;i<4;i++)
        {
            solution.AddRange(solveCross(c));
        }
        solution.AddRange(solveBottomCorners(c));
        solution.AddRange(solveSecondLayer(c));
        solution.AddRange(solveTopCross(c));
        solution.AddRange(completeOLL(c));
        solution.AddRange(solveTopCorners(c));
        solution.AddRange(solveTopEdges(c));
        //return compress(solution);
        return solution;
    }
    public List<Move> compress(List<Move> moves) {
        Move m = moves[1];
        List<Move> output = new List<Move>();
        int repeat = 1;
        for (int i = 1; i < moves.Count; i++) {
            if (moves[i].getMove() == m.getMove())
            {
                repeat += (moves[i].getDir() ? 3 : 1);
            }
            else 
            {
                switch (repeat%4){
                    case 1:
                        output.Add(new Move(m.getMove(), false));
                        break;
                    case 2:
                        output.Add(new Move(m.getMove(), false));
                        output.Add(new Move(m.getMove(), false));
                        break;
                    case 3:
                        output.Add(new Move(m.getMove(), true));
                        break;
                }
                m = moves[i];
                repeat = (m.getDir() ? 3 : 1);
                
            }
            if (i + 1 == moves.Count) {
                switch (repeat % 4)
                {
                    case 1:
                        output.Add(new Move(m.getMove(), false));
                        break;
                    case 2:
                        output.Add(new Move(m.getMove(), false));
                        output.Add(new Move(m.getMove(), false));
                        break;
                    case 3:
                        output.Add(new Move(m.getMove(), true));
                        break;
                }
            }
        }
        
        //Debug.Log(getMovesString(moves));
        //Debug.Log(getMovesString(output));
            return output;
    }
    private string getMovesString(List<Move> m)
    {
        string output = "";
        for (int i = 0; i < m.Count; i++)
        {
            output += (m[i].getMove()).ToUpper() + (m[i].getDir() ? "' " : " ");
        }
        return output;
    }
    private List<Move> specialMove(Cube c, Corner corner)
    {
        List<Face> tempOrientations = corner.getFaces();
        List<Face> finalOrientations = new List<Face>();
        List<Move> solution = new List<Move>();
        for (int i = 0; i < tempOrientations.Count; i++)
        {
            if (!(tempOrientations[i].getOrientation().Equals(PositiveY) || tempOrientations[i].getOrientation().Equals(NegativeY)))
            {
                finalOrientations.Add(tempOrientations[i]);
            }
        }
        Orientation o1 = finalOrientations[0].getOrientation();
        Orientation o2 = finalOrientations[1].getOrientation();
        if ((o1.Equals(PositiveX) && o2.Equals(NegativeZ)) || (o1.Equals(NegativeZ) && o2.Equals(PositiveX)))
        {
            solution.Add(c.right());
            solution.Add(c.up());
            solution.Add(c.rightPrime());
            solution.Add(c.upPrime());
        }
        else if ((o1.Equals(PositiveX) && o2.Equals(PositiveZ)) || (o1.Equals(PositiveZ) && o2.Equals(PositiveX)))
        {
            solution.Add(c.back());
            solution.Add(c.up());
            solution.Add(c.backPrime());
            solution.Add(c.upPrime());
        }
        else if ((o1.Equals(NegativeX) && o2.Equals(PositiveZ)) || (o1.Equals(PositiveZ) && o2.Equals(NegativeX)))
        {
            solution.Add(c.left());
            solution.Add(c.up());
            solution.Add(c.leftPrime());
            solution.Add(c.upPrime());
        }
        else if ((o1.Equals(NegativeX) && o2.Equals(NegativeZ)) || (o1.Equals(NegativeZ) && o2.Equals(NegativeX)))
        {
            solution.Add(c.front());
            solution.Add(c.up());
            solution.Add(c.frontPrime());
            solution.Add(c.upPrime());
        }
        return solution;
    }

    private List<Move> removePiece(Cube c, Edge e )
    {
        List<Move> solution = new List<Move>();
        List<Face> faces = e.getFaces();
        Orientation o1 = faces[0].getOrientation();
        Orientation o2 = faces[1].getOrientation();
        if ((o1.Equals(PositiveX) && o2.Equals(NegativeZ)) || (o1.Equals(NegativeZ) && o2.Equals(PositiveX)))
        {
            solution.Add(c.rightPrime());
            solution.Add(c.front());
            solution.Add(c.right());
            solution.Add(c.frontPrime());
            solution.AddRange(solveBottomCorners(c));
        }
        else if ((o1.Equals(PositiveX) && o2.Equals(PositiveZ)) || (o1.Equals(PositiveZ) && o2.Equals(PositiveX)))
        {
            solution.Add(c.backPrime());
            solution.Add(c.right());
            solution.Add(c.back());
            solution.Add(c.rightPrime());
            solution.AddRange(solveBottomCorners(c));
        }
        else if ((o1.Equals(NegativeX) && o2.Equals(PositiveZ)) || (o1.Equals(PositiveZ) && o2.Equals(NegativeX)))
        {
            solution.Add(c.leftPrime());
            solution.Add(c.back());
            solution.Add(c.left());
            solution.Add(c.backPrime());
            solution.AddRange(solveBottomCorners(c ));
        }
        else if ((o1.Equals(NegativeX) && o2.Equals(NegativeZ)) || (o1.Equals(NegativeZ) && o2.Equals(NegativeX)))
        {
            solution.Add(c.frontPrime());
            solution.Add(c.left());
            solution.Add(c.front());
            solution.Add(c.leftPrime());
            solution.AddRange(solveBottomCorners(c ));
        }
        return solution;
    }

    private List<Move> crossAlg(Cube c )
    {
        List<Move> solution = new List<Move>();
        solution.Add(c.front());
        solution.Add(c.right());
        solution.Add(c.up());
        solution.Add(c.rightPrime());
        solution.Add(c.upPrime());
        solution.Add(c.frontPrime());
        return solution;
    }

    
    public List<Move> solveCross(Cube c)
    {
        List<Move> solution = new List<Move>();
        
            Edge e = c.getCrossPiece();
            Colour crossColour = Colour.YELLOW;
            Colour notCrossColour = e.getNotThisColour(crossColour);
            while (!e.isSolved())
            {
                if (!(e.isColourFacingDirection(crossColour,  PositiveY) || e.isColourFacingDirection(notCrossColour,  PositiveY)))
                {
                    if (e.isColourFacingDirection(notCrossColour, orientationOfColour[notCrossColour]))
                    {
                        while (!e.isSolved())
                        {
                            solution.Add(c.turnAroundOrientation(orientationOfColour[notCrossColour]));
                        }
                        return solution;
                    }
                    else
                    {
                        int i = 0;
                        Orientation o1 = e.getOrientationOfColour(crossColour);
                        Orientation o2 = e.getOrientationOfColour(notCrossColour);
                        while (!(e.isColourFacingDirection(crossColour,  PositiveY) || e.isColourFacingDirection(notCrossColour,  PositiveY)))
                        { 
                            if (i < 4)
                            {
                                solution.Add(c.turnAroundOrientation(o2));
                            }
                            else
                            {
                                if (i == 4) { 
                                solution.RemoveRange(solution.Count - 4, 4);
                                }
                                solution.Add(c.turnAroundOrientation(o1));
                            }
                            i++;
                        }
                        solution.Add(c.up());
                    if (i > 4)
                    {
                        for (int j = i; j < 8; j++)
                        {
                            solution.Add(c.turnAroundOrientation(o1));
                        }
                    }
                    else {
                        for (int j = i; j < 4; j++)
                        {
                            solution.Add(c.turnAroundOrientation(o2));
                        }
                    }
                    }
                }
                if (e.isColourFacingDirection(crossColour,  PositiveY))
                {
                    while (!e.isColourFacingDirection(notCrossColour, orientationOfColour[notCrossColour]))
                    {
                        solution.Add(c.up());
                    }
                    solution.Add(c.turnAroundOrientation(orientationOfColour[notCrossColour]));
                    solution.Add(c.turnAroundOrientation(orientationOfColour[notCrossColour]));
                }
                if (e.isColourFacingDirection(notCrossColour,  PositiveY))
                {
                    while (!e.isColourFacingDirection(crossColour, orientationOfColour[notCrossColour]))
                    {
                        solution.Add(c.up());
                    }
                    solution.Add(c.turnAroundOrientation(e.getOrientationOfColour(crossColour) ));
                    solution.Add(c.down());
                    solution.Add(c.turnAroundOrientation(e.getOrientationOfColour(notCrossColour) ));
                    solution.Add(c.turnAroundOrientation(e.getOrientationOfColour(notCrossColour) ));
                    solution.Add(c.turnAroundOrientation(e.getOrientationOfColour(notCrossColour) ));
                    solution.Add(c.downPrime());
                }
            }
        return solution;
    }

    private List<Move> solveBottomCorners(Cube c )
    {
        List<Move> solution = new List<Move>();
        while (!c.bottomCornersSolved())
        {
            Corner corner = c.getBottomCornerPiece();
            while (!corner.isSolved())
            {
                if (corner.getPos().y == 2)
                {
                    while (!(corner.getPos().x == corner.getSolvedPos().x && corner.getPos().z == corner.getSolvedPos().z))
                    {
                        solution.Add(c.up());
                    }
                    while (!corner.isSolved())
                    {
                        solution.AddRange(specialMove(c, corner));
                    }
                }
                else if (corner.getPos().y == 0)
                {
                    solution.Add(c.up());
                    solution.AddRange(specialMove(c, corner));
                }
            }
        }
        return solution;
    }

    private List<Move> solveSecondLayer(Cube c )
    {
        List<Move> solution = new List<Move>();
        while (!c.secondLayerSolved())
        {
            Edge e = c.getSecondLayerPiece();
            Colour c1 = e.getNotThisColour(Colour.YELLOW);
            Colour c2 = e.getNotThisColour(c1);
            while (!e.isSolved())
            {
                if (e.getPos().y == 2)
                {
                    if (!e.getOrientationOfColour(c1).Equals(PositiveY))
                    {
                        c1 = c2;
                        c2 = e.getNotThisColour(c1);
                    }
                    while (!e.isColourFacingDirection(c2, orientationOfColour[c2]))
                    {
                        solution.Add(c.up());
                    }
                    int temp = 0;
                    int temp2 = 0;
                    while (e.isColourFacingDirection(c1,  PositiveY) || e.isColourFacingDirection(c1, orientationOfColour[c1]) || e.isColourFacingDirection(c1,  NegativeY))
                    {
                        solution.Add(c.turnAroundOrientation(e.getOrientationOfColour(c2) ));
                        temp++;
                        temp2++;
                    }
                    solution.Add(c.up());
                    solution.Add(c.up());
                    while (temp < 4)
                    {
                        solution.Add(c.turnAroundOrientation(e.getOrientationOfColour(c2) ));
                        temp++;
                    }
                    while (!e.isColourFacingDirection(c2, orientationOfColour[c1]))
                    {
                        solution.Add(c.up());
                    }
                    temp = 0;
                    while (temp < temp2)
                    {
                        solution.Add(c.turnAroundOrientation(orientationOfColour[c2]));
                        temp++;
                    }
                    solution.Add(c.up());
                    solution.Add(c.up());
                    while (temp < 4)
                    {
                        solution.Add(c.turnAroundOrientation(orientationOfColour[c2]));
                        temp++;
                    }
                    while (!e.isColourFacingDirection(c2, orientationOfColour[c1]))
                    {
                        solution.Add(c.up());
                    }
                    temp = 0;
                    while (temp < temp2)
                    {
                        solution.Add(c.turnAroundOrientation(orientationOfColour[c2]));
                        temp++;
                    }
                    while (!e.isColourFacingDirection(c2, orientationOfColour[c2]))
                    {
                        solution.Add(c.up());
                    }
                    while (temp < 4)
                    {
                        solution.Add(c.turnAroundOrientation(orientationOfColour[c2]));
                        temp++;
                    }
                }
                else
                {
                    solution.AddRange(removePiece(c, e ));
                }
            }
        }
        return solution;
    }

    private List<Move> solveTopCross(Cube c )
    {
        List<Move> solution = new List<Move>();
        while (!c.topCrossSolved())
        {
            if (c.noTopCross())
            {
                solution.AddRange(crossAlg(c ));
            }
            else if (c.barTopCross())
            {
                while (!c.barTopCrossAligned())
                {
                    solution.Add(c.up());
                }
                solution.AddRange(crossAlg(c ));
            }
            else if (!c.noTopCross() || !c.barTopCross())
            {
                while (!(c.noTopCross() || c.barTopCross()))
                {
                    solution.AddRange(crossAlg(c ));
                    solution.Add(c.up());
                }
            }
        }
        return solution;
    }

    private List<Move> completeOLL(Cube c )
    {
        List<Move> solution = new List<Move>();
        while (!c.OLLcomplete())
        {
            switch (c.OLLCorners())
            {
                case 0:
                    solution.Add(c.right());
                    solution.Add(c.up());
                    solution.Add(c.up());
                    solution.Add(c.rightPrime());
                    solution.Add(c.upPrime());
                    solution.Add(c.right());
                    solution.Add(c.up());
                    solution.Add(c.rightPrime());
                    solution.Add(c.upPrime());
                    solution.Add(c.right());
                    solution.Add(c.upPrime());
                    solution.Add(c.rightPrime());
                    break;
                case 1:
                    while (!c.getPiece(0, 2, 0).isColourFacingDirection(Colour.WHITE,  PositiveY))
                    {
                        solution.Add(c.up());
                    }
                    solution.Add(c.right());
                    solution.Add(c.up());
                    solution.Add(c.rightPrime());
                    solution.Add(c.up());
                    solution.Add(c.right());
                    solution.Add(c.up());
                    solution.Add(c.up());
                    solution.Add(c.rightPrime());
                    break;
                case 2:
                    while (!((c.getPiece(0, 2, 2).isColourFacingDirection(Colour.WHITE,  PositiveY) && c.getPiece(2, 2, 2).isColourFacingDirection(Colour.WHITE,  PositiveY))
                    || (c.getPiece(0, 2, 2).isColourFacingDirection(Colour.WHITE,  PositiveY) && c.getPiece(2, 2, 0).isColourFacingDirection(Colour.WHITE,  PositiveY))))
                    {
                        solution.Add(c.up());
                    }
                    solution.Add(c.right());
                    solution.Add(c.back());
                    solution.Add(c.rightPrime());
                    solution.Add(c.front());
                    solution.Add(c.right());
                    solution.Add(c.backPrime());
                    solution.Add(c.rightPrime());
                    solution.Add(c.frontPrime());
                    break;
            }
        }
        return solution;
    }
    private List<Move> solveTopCorners(Cube c )
    {
        List<Move> solution = new List<Move>();
        while (!c.getPiece(0, 2, 0).isSolved())
        {
            solution.Add(c.up());
        }
        while (!c.topCornersSolved())
        {
            if (c.getPiece(0, 2, 2).isSolved())
            {
                solution.Add(c.right());
                solution.Add(c.up());
                solution.Add(c.rightPrime());
                solution.Add(c.upPrime());
                solution.Add(c.rightPrime());
                solution.Add(c.front());
                solution.Add(c.right());
                solution.Add(c.right());
                solution.Add(c.upPrime());
                solution.Add(c.rightPrime());
                solution.Add(c.upPrime());
                solution.Add(c.right());
                solution.Add(c.up());
                solution.Add(c.rightPrime());
                solution.Add(c.frontPrime());
                break;
            }
            else if (c.getPiece(2, 2, 0).isSolved())
            {
                solution.Add(c.back());
                solution.Add(c.up());
                solution.Add(c.backPrime());
                solution.Add(c.upPrime());
                solution.Add(c.backPrime());
                solution.Add(c.right());
                solution.Add(c.back());
                solution.Add(c.back());
                solution.Add(c.upPrime());
                solution.Add(c.backPrime());
                solution.Add(c.upPrime());
                solution.Add(c.back());
                solution.Add(c.up());
                solution.Add(c.backPrime());
                solution.Add(c.rightPrime());
            }
            else if (c.getPiece(2, 2, 2).isSolved())
            {
                solution.Add(c.rightPrime());
                solution.Add(c.up());
                solution.Add(c.right());
                solution.Add(c.upPrime());
                solution.Add(c.rightPrime());
                solution.Add(c.frontPrime());
                solution.Add(c.upPrime());
                solution.Add(c.front());
                solution.Add(c.right());
                solution.Add(c.up());
                solution.Add(c.rightPrime());
                solution.Add(c.front());
                solution.Add(c.rightPrime());
                solution.Add(c.frontPrime());
                solution.Add(c.right());
                solution.Add(c.upPrime());
                solution.Add(c.right());
            }
            else
            {
                solution.Add(c.back());
                solution.Add(c.back());
                solution.Add(c.right());
                solution.Add(c.right());
                solution.Add(c.backPrime());
                solution.Add(c.leftPrime());
                solution.Add(c.back());
                solution.Add(c.right());
                solution.Add(c.right());
                solution.Add(c.backPrime());
                solution.Add(c.left());
                solution.Add(c.backPrime());
            }
        }
        return solution;
    }

    private List<Move> solveTopEdges(Cube c )
    {
        List<Move> solution = new List<Move>();
        while (!c.topEdgesSolved())
        {
            Edge e = c.getTopSolvedEdge();
            if (e == null || orientationOfColour[e.getNotThisColour(Colour.WHITE)].Equals(NegativeX))
            {
                solution.Add(c.right());
                solution.Add(c.right());
                solution.Add(c.up());
                solution.Add(c.backPrime());
                solution.Add(c.front());
                solution.Add(c.right());
                solution.Add(c.right());
                solution.Add(c.back());
                solution.Add(c.frontPrime());
                solution.Add(c.up());
                solution.Add(c.right());
                solution.Add(c.right());
            }
            else if (orientationOfColour[e.getNotThisColour(Colour.WHITE)].Equals(PositiveZ))
            {
                solution.Add(c.front());
                solution.Add(c.front());
                solution.Add(c.up());
                solution.Add(c.rightPrime());
                solution.Add(c.left());
                solution.Add(c.front());
                solution.Add(c.front());
                solution.Add(c.right());
                solution.Add(c.leftPrime());
                solution.Add(c.up());
                solution.Add(c.front());
                solution.Add(c.front());
            }
            else if (orientationOfColour[e.getNotThisColour(Colour.WHITE)].Equals(PositiveX))
            {
                solution.Add(c.left());
                solution.Add(c.left());
                solution.Add(c.up());
                solution.Add(c.frontPrime());
                solution.Add(c.back());
                solution.Add(c.left());
                solution.Add(c.left());
                solution.Add(c.front());
                solution.Add(c.backPrime());
                solution.Add(c.up());
                solution.Add(c.left());
                solution.Add(c.left());
            }
            else
            {
                solution.Add(c.back());
                solution.Add(c.back());
                solution.Add(c.up());
                solution.Add(c.leftPrime());
                solution.Add(c.right());
                solution.Add(c.back());
                solution.Add(c.back());
                solution.Add(c.left());
                solution.Add(c.rightPrime());
                solution.Add(c.up());
                solution.Add(c.back());
                solution.Add(c.back());
            }
        }
        return solution;
    }
}
