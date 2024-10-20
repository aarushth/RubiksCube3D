using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PieceControl : MonoBehaviour {


    // Start is called before the first frame update
    void Start() {
        
        nextButton.SetActive(false);
        pieces =  new GameObject[26];
        GameObject c = GameObject.FindGameObjectsWithTag("pieceCube")[0];
        for (int i = 0; i < c.transform.childCount; i++)
        {
            pieces[i] = c.transform.GetChild(i).gameObject;
        }
        cubeSim = new Cube();
        solver = new Solver();
        Application.targetFrameRate = 30;
        learnText.SetText("");
        
    }
    private Solver solver;

    public GameObject[] pieces;

    public TMP_Text moveDisplay;
    public TMP_Text learnText;
    public TMP_Text tryAgainText;
    public GameObject solveButton;
    public GameObject scrambleButton;
    public GameObject nextButton;
    public GameObject learnButton;

    private bool fastSolve = false;
    private Cube cubeSim;
    private List<Move> movesSolver = new List<Move>();
    private Stack<string> movesStackName = new Stack<string>();
    private Queue<Move> moveToDo = new Queue<Move>();
    private Stack<bool> movesStackDir = new Stack<bool>();
    private bool solving = false;
    private static Dictionary<string, Vector3> moveVectors = new Dictionary<string, Vector3>{
        {"f",  new Vector3(0, 0, -1)},
        {"r", new Vector3(1, 0, 0)},
        {"l", new Vector3(-1, 0, 0)},
        {"b", new Vector3(0, 0, 1)},
        {"u", new Vector3(0, 1, 0)},
        {"d", new Vector3(0, -1, 0)}
    };
    private static string[] moves = new string[6] {
        "u","d","l","r","f","b"
    };
    bool rotating = false;
    Vector3 rotateAround = Vector3.zero;

    bool scrambling = false;
    
    int counter = 0;
    int numOfScrambles = 20;
    int move = 0;
    int i = 0;
    
    void Update() {
        /*
        if (needToDoMoves && learnState < 119 && !rotating) {
            fastSolve = true;
            receiveMove(moveToDo.Peek().getMove(), moveToDo.Peek().getDir());
        }
        */
        if (time)
        {
            timer++;
        }
        if (timer > 40)
        {
            tryAgainText.SetText("");
            time = false;
            timer = 0;
        }
        if (scrambling && !rotating)
        {
            int newMove = 0;
            while (newMove % 6 == move % 6)
            {
                newMove = UnityEngine.Random.Range(0, 12);
            }
            move = newMove;
            turn(moves[move % 6], move % 2 == 0);
            counter++;
            if (counter == numOfScrambles)
            {
                counter = 0;
                moveDisplay.SetText("");
                if (learning)
                {
                    next();
                }
                scrambling = false;
            }
        }
        else if (solving && !rotating) {
            if (i < movesSolver.Count)
            {
                turn(movesSolver[i].getMove(), movesSolver[i].getDir());
                i++;
            }
            else {
                movesSolver.Clear();
                i = 0;
                moveDisplay.SetText("");
                movesStackDir.Clear();
                movesStackName.Clear();
                cubeSim = new Cube();
                solving = false;
                fastSolve = false;
                if (learning && !needToDoMoves)
                {
                    next();
                }  
            }
        }
    }

    float full = 1.0f;
    float half = 0.1f;
    public void scramble(int num){
        scrambling = true;
        numOfScrambles = num;
    }
    public void resetMove() {
        if (!scrambling && !solving)
        {
            moveDisplay.SetText("");
        }   
        rotating = false;
    }
    public void solve()
    {
        movesSolver = solver.solveCube(cubeSim);
        solving = true;
        fastSolve=true;
    }
    public int getLength() { 
        return movesSolver.Count;
    }
    public void receiveMove(string s, bool isPrime) {
        if (!learning)
        {
            turn(s, isPrime);
        }
        else {
            moveToDo.TryPeek(out Move m);
            if (m == null) {
                return;
            } else if (s == m.getMove() && isPrime == m.getDir())
            {
                turn(s, isPrime);
                moveToDo.Dequeue();
            }
            else {
                tryAgainText.SetText("thats not right. try again");
                time = true;
                movesSolver.Add(new Move(s, isPrime));
                movesSolver.Add(new Move(s, !isPrime));
                solving = true;
            }
            if (moveToDo.Count == 0) {
                needToDoMoves = false;
                next();
            }
        }
    }
    private bool time = false;
    private int timer = 0;
    public void turn(string s, bool isPrime) {
        if (!rotating) {
            if (!learning)
            {
                moveDisplay.SetText(s + (isPrime ? "'" : ""));
            }
            if (!solving)
            {
                
                movesStackDir.Push(isPrime);
                movesStackName.Push(s);
            }
            cubeSim.turn(new Move(s, isPrime));
            int i = 0;
            rotateAround = moveVectors[s];
            if (rotateAround.x != 0) {
                foreach (GameObject piece in pieces) {
                    piece.GetComponent<PieceReciever>().setSpeed(fastSolve);
                    if (piece.transform.position.x == rotateAround.x && i < 9){
                        if (isPrime){
                            piece.GetComponent<PieceReciever>().rotate(-rotateAround);
                        }else {
                            piece.GetComponent<PieceReciever>().rotate(rotateAround);
                        }
                        i++;
                    }
                }
            } else if (rotateAround.y != 0) {
                foreach (GameObject piece in pieces) {
                    piece.GetComponent<PieceReciever>().setSpeed(fastSolve);
                    if (piece.transform.position.y == rotateAround.y && i < 9){
                        if (isPrime){
                            piece.GetComponent<PieceReciever>().rotate(-rotateAround);
                        }else {
                            piece.GetComponent<PieceReciever>().rotate(rotateAround);
                        }
                        i++;
                    }
                }
            } else if (rotateAround.z != 0) {
                foreach (GameObject piece in pieces){
                    piece.GetComponent<PieceReciever>().setSpeed(fastSolve);
                    if (piece.transform.position.z == rotateAround.z && i < 9){
                        if (isPrime){
                            piece.GetComponent<PieceReciever>().rotate(-rotateAround);
                        }else {
                            piece.GetComponent<PieceReciever>().rotate(rotateAround);
                        }
                        i++;
                    }
                }
            }
            rotating = true;
        }
    }
    private bool learning = false;
    public void learn() {
        foreach (GameObject piece in pieces)
        { 
            piece.GetComponent<PieceReciever>().reset();
        }
        solveButton.SetActive(false);
        scrambleButton.SetActive(false);
        nextButton.SetActive(true);
        learnButton.SetActive(false);

        //learnState = 21;
        learnState = 0;
        learning = true;
        learnStages();
    }
    private string getMovesString(List<Move> m) {
        string output = "";
        for (int i = 0; i < m.Count; i++) {
            output += (m[i].getMove()).ToUpper() + (m[i].getDir() ? "' " : " ");
        }
        return output;
    }
    public void makeTransByName(bool trans, string name) {
        GameObject.FindGameObjectsWithTag(name)[0].GetComponent<PieceReciever>().changeTransparency(trans? half : full);
    }
    public void makeTransByPos(bool trans, Vector3 pos)
    {
        foreach (GameObject piece in pieces)
        {
            Vector3 p = piece.transform.position;
            if (p.x == pos.x && p.y == pos.y && p.z == pos.z)
            {
                piece.GetComponent<PieceReciever>().changeTransparency(trans ? half : full);
            }
        }
    }
    public void makeAllTrans(bool trans)
    {
        foreach (GameObject piece in pieces)
        {
            piece.GetComponent<PieceReciever>().changeTransparency(trans ? half : full);
        
        }
    }
    public void makeTransBySide(bool trans, Vector3 pos)
    {
        foreach (GameObject piece in pieces)
        {
            Vector3 p = piece.transform.position;
            if ((Math.Abs(pos.x) == 1 && p.x == pos.x)||(Math.Abs(pos.y) == 1 && p.y == pos.y)||(Math.Abs(pos.z) == 1 && p.z == pos.z))
            {
                piece.GetComponent<PieceReciever>().changeTransparency(trans ? half : full);
            }
        }
    }
    public void makeTransByNumOfSides(bool trans, int sides) {
        foreach (GameObject piece in pieces)
        {
            Vector3 p = piece.transform.position;
            if (Math.Abs(p.x) + Math.Abs(p.y) + Math.Abs(p.z) == sides)
            {
                piece.GetComponent<PieceReciever>().changeTransparency(trans ? half: full);
            }
        }
    }
    int learnState = 0;
    private bool needToDoMoves = false;
    public void next()
    {
        if (!solving)
        {
            learnState++;
            learnStages();
        }
    }
    private string mov = "";
    private int cast = 0;
    private List<Move> temp = new List<Move>();
    public void learnStages() {
        switch (learnState) {
            // R D2 L' D2 R F2 U2 F2 L U2 F' D' B'
            case 0:
                learnText.SetText("First, lets learn about the different kinds of pieces");
                break;
            case 1:
                learnText.SetText("There are three types of pieces, corners, edges and centers");
                break;
            case 2:
                learnText.SetText("corners have three sides and are always on the corners of the cube");
                makeTransByNumOfSides(true, 1);
                makeTransByNumOfSides(true, 2);
                makeTransByNumOfSides(false, 3);
                break;
            case 3:
                learnText.SetText("edges have two sides and are always in between two corners");
                makeTransByNumOfSides(false, 2);
                makeTransByNumOfSides(true, 3);
                break;
            case 4:
                learnText.SetText("centers have one sides and are always in the center of a face no matter what moves you make");
                makeTransByNumOfSides(true, 2);
                makeTransByNumOfSides(false, 1);
                scramble(10);
                nextButton.SetActive(false);
                break;
            case 5:
                for (int i = 0; i < 10; i++)
                {
                    movesSolver.Add(new Move(movesStackName.Pop(), !movesStackDir.Pop()));
                }
                solving = true;
                nextButton.SetActive(true);
                break;
            case 6:
                learnText.SetText("Next lets learn about move notations");
                makeAllTrans(false);
                gameObject.GetComponent<TouchControls>().reset(180, -35);
                break;
            case 7:
                learnText.SetText("Each letter represents a clockwise turn on that face. For example U means a clockwise turn on the 'up' face. In this case we are assuming green is in front and white is up");
                
                makeAllTrans(true);
                makeTransBySide(false, new Vector3(0,1,0));
                turn("u", false);
                break;
            case 8:
                learnText.SetText("a U' means a counterclockwise turn on the 'up' face");
                turn("u", true);
                break;
            case 9:
                learnText.SetText("Similarly, an F means a clockwise turn on the 'front' face");
                makeAllTrans(true);
                makeTransBySide(false, new Vector3(0, 0, -1));
                turn("f", false);
                break;
            case 10:
                learnText.SetText("and an F' means a counterclockwise turn on the 'front' face");
                turn("f", true);
                break;
            case 11:
                learnText.SetText("R is 'right' clockwise");
                makeAllTrans(true);
                makeTransBySide(false, new Vector3(1, 0, 0));
                turn("r", false);
                break;
            case 12:
                learnText.SetText("and R' is 'right' counterclockwise");
                turn("r", true);
                break;
            case 13:
                learnText.SetText("D is 'down' clockwise");
                makeAllTrans(true);
                makeTransBySide(false, new Vector3(0, -1, 0));
                turn("d", false);
                break;
            case 14:
                learnText.SetText("and D' is 'down' counterclockwise");
                turn("d", true);
                break;
            case 15:
                learnText.SetText("L is 'left' clockwise");
                makeAllTrans(true);
                makeTransBySide(false, new Vector3(-1, 0, 0));
                turn("l", false);
                break;
            case 16:
                learnText.SetText("and L' is 'left' counterclockwise");
                turn("l", true);
                break;
            case 17:
                learnText.SetText("and finally B is 'back' clockwise");
                makeAllTrans(true);
                makeTransBySide(false, new Vector3(0, 0, 1));
                turn("b", false);
                break;
            case 18:
                learnText.SetText("and B' is 'back' counterclockwise");
                turn("b", true);
                break;
            case 19:
                learnText.SetText("Lets Practice. Try out this series of moves: R U R' U'");
                moveToDo.Enqueue(new Move("r", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("r", true));
                moveToDo.Enqueue(new Move("u", true));
                
                nextButton.SetActive(false);
                needToDoMoves = true;
                makeAllTrans(false);
                break;
            case 20:
                nextButton.SetActive(true);
                learnText.SetText("Good job. keep that pattern in mind, we will use it again");
                break;
            case 21:
                learnText.SetText("Lets scramble the cube");
                nextButton.SetActive(false);
                foreach (GameObject piece in pieces)
                {
                    piece.GetComponent<PieceReciever>().reset();
                }
                cubeSim = new Cube();
                // R D2 L' D2 R F2 U2 F2 L U2 F' D' B'
                movesSolver.Add(new Move("r", false));
                movesSolver.Add(new Move("d", false));
                movesSolver.Add(new Move("d", false));
                movesSolver.Add(new Move("l", true));
                movesSolver.Add(new Move("d", false));
                movesSolver.Add(new Move("d", false));
                movesSolver.Add(new Move("r", false));
                movesSolver.Add(new Move("f", false)); 
                movesSolver.Add(new Move("f", false));

                movesSolver.Add(new Move("u", false));
                movesSolver.Add(new Move("u", false));

                movesSolver.Add(new Move("f", false));
                movesSolver.Add(new Move("f", false));
                movesSolver.Add(new Move("l", false));
                movesSolver.Add(new Move("u", false));
                movesSolver.Add(new Move("u", false));
                movesSolver.Add(new Move("f", true));
                movesSolver.Add(new Move("d", true));
                movesSolver.Add(new Move("b", true));
                fastSolve = true;
                solving = true;
                break;
            case 22:
                nextButton.SetActive(true);
                learnText.SetText("The first step to solving a cube is to solve all the edges of the yellow cross on the bottom. We will solve this one piece at a time");
                makeAllTrans(true);
                makeTransByPos(false, new Vector3(0, -1, 0));
                makeTransByPos(false, new Vector3(1, -1, 0));
                makeTransByPos(false, new Vector3(-1, -1, 0));
                makeTransByPos(false, new Vector3(0, -1, 1));
                makeTransByPos(false, new Vector3(0, -1, -1));
                break;
            case 23:
                learnText.SetText("First lets solve the green-yellow edge piece");
                makeAllTrans(true);
                makeTransByPos(false, new Vector3(-1, 0, -1));
                break;
            case 24:
                learnText.SetText("Since the green side is next to the green center, we just have to rotate the face until its in the right place: F'");
                makeTransByPos(false, new Vector3(0, 0, -1));
                makeTransByPos(false, new Vector3(0, -1, 0));
                moveToDo.Enqueue(new Move("f", true));
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 25:
                learnText.SetText("Nice! now lets do the blue-yellow edge");
                makeTransByPos(false, new Vector3(0, -1, 0));
                makeTransByPos(true, new Vector3(0, 0, -1));
                nextButton.SetActive(true);
                makeTransByPos(false, new Vector3(1, -1, 0));
                break;
            case 26:
                learnText.SetText("Since this piece is in a weird spot, we first have to take it out to the top layer: R R");
                moveToDo.Enqueue(new Move("r", false));
                moveToDo.Enqueue(new Move("r", false));
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 27:
                learnText.SetText("Great! Lets rotate the cube so that the blue side is in front. This will make it easier to see where the piece is supposed to go");
                nextButton.SetActive(true);
                makeTransByPos(false, new Vector3(0, 0, 1));
                gameObject.GetComponent<TouchControls>().reset(0, -35);
                break;
            case 28:
                learnText.SetText("Now do U moves to align the blue-yellow edge with the blue center: U'");
                
                nextButton.SetActive(false);
                moveToDo.Enqueue(new Move("u", true));
                needToDoMoves = true;
                break;
            case 29:
                learnText.SetText("Now just do F moves to get the blue-yellow edge into the correct spot: F F");
                moveToDo.Enqueue(new Move("b", false));
                moveToDo.Enqueue(new Move("b", false));

                needToDoMoves = true;
                break;
            case 30:
                learnText.SetText("Nice! Now we'll solve the orange yellow edge. Lets rotate the cube to put the orange center in front");
                makeTransByPos(true, new Vector3(0, 0, 1));
                makeTransByPos(false, new Vector3(-1, 1, 0));
                makeTransByPos(false, new Vector3(-1, 0, 0));
                nextButton.SetActive(true);
                gameObject.GetComponent<TouchControls>().reset(270, -35);
                break;
            case 31:
                learnText.SetText("Since the orange side of the edge isn't next to the orange center, we'll have to use an algorithm to solve this: F D R' D'");
                moveToDo.Enqueue(new Move("l", false));
                moveToDo.Enqueue(new Move("d", false));
                moveToDo.Enqueue(new Move("f", true));
                moveToDo.Enqueue(new Move("d", true));
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 32:
                makeTransByPos(true, new Vector3(-1, 0, 0));
                makeTransByPos(false, new Vector3(-1, 0, 1));
                learnText.SetText("Now lets solve the red-yellow edge");
                nextButton.SetActive(true);
                break;
            case 33:
                nextButton.SetActive(false);
                learnText.SetText("First, we have to get it to the top layer without disturbing other solved pieces: F U F'");
                moveToDo.Enqueue(new Move("l", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("l", true));
                needToDoMoves = true;
                break;
            case 34:
                learnText.SetText("Now lets rotate the cube to have the red center facing the front");
                makeTransByPos(false, new Vector3(1, 0, 0));
                gameObject.GetComponent<TouchControls>().reset(90, -35);
                nextButton.SetActive(true);
                break;
            case 35:
                nextButton.SetActive(false);
                learnText.SetText("Now we do U moves to align it with the red center: U");
                moveToDo.Enqueue(new Move("u", false));
                needToDoMoves = true;
                break;
            case 36:
                learnText.SetText("And finally do the algorithm again because the red side is not next to the red center: F D R' D'");
                moveToDo.Enqueue(new Move("r", false));
                moveToDo.Enqueue(new Move("d", false));
                moveToDo.Enqueue(new Move("b", true));
                moveToDo.Enqueue(new Move("d", true));
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 37: 
                learnText.SetText("And the yellow cross is solved!");
                makeTransByPos(true, new Vector3(1, 0, 0));
                nextButton.SetActive(true);
                break;
            case 38:
                makeTransByPos(true, new Vector3(0, -1, 0));
                makeTransByPos(true, new Vector3(1, -1, 0));
                makeTransByPos(true, new Vector3(-1, -1, 0));
                makeTransByPos(true, new Vector3(0, -1, 1));
                makeTransByPos(true, new Vector3(0, -1, -1));
                learnText.SetText("Now we have to solve the yellow corners, but without breaking the yellow cross we just made");
                makeTransByPos(false, new Vector3(1, -1, 1));
                makeTransByPos(false, new Vector3(-1, -1, 1));
                makeTransByPos(false, new Vector3(1, -1, -1));
                makeTransByPos(false, new Vector3(-1, -1, -1));
                break;
            case 39:
                makeTransByPos(false, new Vector3(1, 1, 1));
                learnText.SetText("Lets start with the yellow-green-red corner");
                makeTransByPos(true, new Vector3(1, -1, 1));
                makeTransByPos(true, new Vector3(-1, -1, 1));
                makeTransByPos(true, new Vector3(1, -1, -1));
                makeTransByPos(true, new Vector3(-1, -1, -1));
                break;
            case 40:
                learnText.SetText("Again, we first do U moves to put the piece on top of where it is supposed to go: U");
                makeTransByPos(false, new Vector3(1, -1, -1));
                moveToDo.Enqueue(new Move("u", false));
                makeTransByPos(false, new Vector3(0, 0, -1));
                makeTransByPos(false, new Vector3(1, 0, 0));
                makeTransByPos(false, new Vector3(0, -1, 0));
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 41:
                learnText.SetText("Now we rotate the cube to make sure the piece is on our right hand side");
                gameObject.GetComponent<TouchControls>().reset(180, -35);
                makeTransByPos(true, new Vector3(1, -1, -1));
                nextButton.SetActive(true);
                break;
            case 42:
                learnText.SetText("Then we repeat this simple algorithm (the one we learned at the start) until the piece is solved: (R U R' U')x1");
                moveToDo.Enqueue(new Move("r", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("r", true));
                moveToDo.Enqueue(new Move("u", true));
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 43:
                learnText.SetText("And we have solved the first corner!");
                nextButton.SetActive(true);
                break;
            case 44:
                learnText.SetText("Now lets solve the yellow-blue-red corner");
                nextButton.SetActive(true);
                makeTransByPos(true, new Vector3(0, 0, -1));
                makeTransByPos(true, new Vector3(1, 0, 0));
                makeTransByPos(true, new Vector3(0, -1, 0));
                makeTransByPos(true, new Vector3(1, -1, -1));
                makeTransByPos(false, new Vector3(-1, 1, 1));
                break;
            case 45:
                learnText.SetText("Again, we first do U moves to put the piece on top of where it is supposed to go: U");
                makeTransByPos(false, new Vector3(1, -1, 1));
                moveToDo.Enqueue(new Move("u", false));
                makeTransByPos(false, new Vector3(0, 0, 1));
                makeTransByPos(false, new Vector3(1, 0, 0));
                makeTransByPos(false, new Vector3(0, -1, 0));
                needToDoMoves = true;
                nextButton.SetActive(false);
                break;
            case 46:
                learnText.SetText("Rotate the cube to make sure the piece is on our right hand side");
                gameObject.GetComponent<TouchControls>().reset(90, -35);
                makeTransByPos(true, new Vector3(1, -1, 1));
                nextButton.SetActive(true);
                break;
            case 47:
                learnText.SetText("And repeat the algorithm until the piece is solved: (R U R' U')x5");
                for (int i = 0; i < 5; i++) {
                    moveToDo.Enqueue(new Move("b", false));
                    moveToDo.Enqueue(new Move("u", false));
                    moveToDo.Enqueue(new Move("b", true));
                    moveToDo.Enqueue(new Move("u", true));
                }
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 48:
                learnText.SetText("Sometimes corners are in the bottom layer but in the wrong spot like this yellow-orange-blue corner");
                makeTransByPos(true, new Vector3(0, 0, 1));
                makeTransByPos(true, new Vector3(1, 0, 0));
                makeTransByPos(true, new Vector3(0, -1, 0));
                makeTransByPos(true, new Vector3(1, -1, 1));

                makeTransByPos(false, new Vector3(-1, -1, -1));
                nextButton.SetActive(true);
                break;
            case 49:
                learnText.SetText("First, we rotate the cube to have the piece on our right hand side");
                gameObject.GetComponent<TouchControls>().reset(270, -35);
                break;
            case 50:
                learnText.SetText("Then do the algorithm once to get the piece into the top layer: R U R' U'");
                    moveToDo.Enqueue(new Move("f", false));
                    moveToDo.Enqueue(new Move("u", false));
                    moveToDo.Enqueue(new Move("f", true));
                    moveToDo.Enqueue(new Move("u", true));
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 51:
                learnText.SetText("Now do U moves to put the piece on top of where it is supposed to go: U");
                makeTransByPos(false, new Vector3(-1, -1, 1));
                moveToDo.Enqueue(new Move("u", false));
                makeTransByPos(false, new Vector3(0, 0, 1));
                makeTransByPos(false, new Vector3(-1, 0, 0));
                makeTransByPos(false, new Vector3(0, -1, 0));
                needToDoMoves = true;
                break;
            case 52:
                learnText.SetText("Then, we rotate the cube to have the piece on our right hand side");
                gameObject.GetComponent<TouchControls>().reset(0, -35);
                makeTransByPos(true, new Vector3(-1, -1, 1));
                nextButton.SetActive(true);
                break;
            case 53:
                learnText.SetText("And repeat the algorithm until the piece is solved: (R U R' U')x1");
                moveToDo.Enqueue(new Move("l", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("l", true));
                moveToDo.Enqueue(new Move("u", true));
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 54:
                learnText.SetText("Awesome! lets solve the last corner, the yellow-orange-green corner");
                makeTransByPos(true, new Vector3(0, 0, 1));
                makeTransByPos(true, new Vector3(-1, 0, 0));
                makeTransByPos(true, new Vector3(0, -1, 0));
                makeTransByPos(true, new Vector3(-1, -1, 1));
                nextButton.SetActive(true);
                makeTransByPos(false, new Vector3(1, 1, 1));
                break;
            case 55:
                learnText.SetText("First do U moves to put the piece on top of where its supposed to go: U'");
                makeTransByPos(false, new Vector3(0, 0, -1));
                makeTransByPos(false, new Vector3(-1, 0, 0));
                makeTransByPos(false, new Vector3(0, -1, 0));
                makeTransByPos(false, new Vector3(-1, -1, -1));

                moveToDo.Enqueue(new Move("u", true));

                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 56:
                makeTransByPos(true, new Vector3(-1, -1, -1));
                learnText.SetText("Then, we rotate the cube to have the piece on our right hand side");
                gameObject.GetComponent<TouchControls>().reset(270, -35);
                makeTransByPos(true, new Vector3(-1, -1, 1));
                nextButton.SetActive(true);
                break;
            case 57:
                learnText.SetText("And finally, repeat the algorithm until the piece is solved: (R U R' U')x5");
                for (int i = 0; i < 5; i++)
                {
                    moveToDo.Enqueue(new Move("f", false));
                    moveToDo.Enqueue(new Move("u", false));
                    moveToDo.Enqueue(new Move("f", true));
                    moveToDo.Enqueue(new Move("u", true));
                }
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 58:
                learnText.SetText("And we have solved the first layer!");
                makeTransBySide(false, new Vector3(0, -1, 0));
                makeTransByNumOfSides(false, 1);
                makeTransByPos(true, new Vector3(0, 1, 0));
                nextButton.SetActive(true);
                break;
            case 59:
                learnText.SetText("Next we have to solve the edges of the second layer");
                makeAllTrans(true);
                makeTransByPos(false, new Vector3(1, 0, 1));
                makeTransByPos(false, new Vector3(-1, 0, 1));
                makeTransByPos(false, new Vector3(1, 0, -1));
                makeTransByPos(false, new Vector3(-1, 0, -1));
                break;
            case 60:
                learnText.SetText("Lets start with the blue-orange edge. First we make sure that the side in front (blue in this case) is aligned with its center");
                makeAllTrans(true);
                makeTransByPos(false, new Vector3(0, 1, 1));
                makeTransByPos(false, new Vector3(0, 0, 1));
                gameObject.GetComponent<TouchControls>().reset(0, -35);
                break;
            case 61:
                learnText.SetText("We need to then notice that it is supposed to go on our right hand side");
                makeTransByPos(false, new Vector3(-1, 0, 1));
                makeTransByPos(false, new Vector3(-1, 0, 0));
                break;
            case 62:
                learnText.SetText("To put it in without breaking what we have already solved we have to do some extra steps. First do a U move moving the piece away from where it is supposed to go: U");
                makeTransByPos(true, new Vector3(-1, 0, 1));
                moveToDo.Enqueue(new Move("u", false));
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 63:
                learnText.SetText("Then, since the piece has to go on our right, do the same algorithm once on the right hand side: R U R' U'");
                moveToDo.Enqueue(new Move("l", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("l", true));
                moveToDo.Enqueue(new Move("u", true));
                needToDoMoves = true;
                break;
            case 64:
                learnText.SetText("Then we rotate the cube to the orange side");
                gameObject.GetComponent<TouchControls>().reset(270, -35);
                nextButton.SetActive(true);
                break;
            case 65:
                learnText.SetText("And now we do the same algorithm, but the opposite way, on our left hand side: L' U' L U");
                nextButton.SetActive(false);
                moveToDo.Enqueue(new Move("b", true));
                moveToDo.Enqueue(new Move("u", true));
                moveToDo.Enqueue(new Move("b", false));
                moveToDo.Enqueue(new Move("u", false));
                needToDoMoves = true;
                break;
            case 66:
                learnText.SetText("The blue-orange edge is solved!");
                nextButton.SetActive(true);
                break;
            case 67:
                learnText.SetText("Next lets solve the blue-red edge. this is a special case because it is already in the second layer, but in the wrong spot");
                makeAllTrans(true);
                makeTransByPos(false, new Vector3(-1, 0, -1));
                break;
            case 68:
                learnText.SetText("First we have to take the piece out to the top layer. We do this by imagining that this white-orange edge on top has to be solved to the blue-red piece's position");
                makeTransByPos(false, new Vector3(-1, 1, 0));
                break;
            case 69:
                learnText.SetText("First do a U move moving the piece away from where it is supposed to go: U");
                moveToDo.Enqueue(new Move("u", false));
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 70:
                learnText.SetText("Then do the algorithm once on the right hand side since the piece has to go to the right: R U R' U'");
                moveToDo.Enqueue(new Move("f", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("f", true));
                moveToDo.Enqueue(new Move("u", true));
                needToDoMoves = true;
                break;
            case 71:
                learnText.SetText("Then we rotate the cube to the green side");
                gameObject.GetComponent<TouchControls>().reset(180, -35);
                nextButton.SetActive(true);
                break;
            case 72:
                learnText.SetText("And now do the algorithm the opposite way, on our left hand side: L' U' L U");
                nextButton.SetActive(false);
                moveToDo.Enqueue(new Move("l", true));
                moveToDo.Enqueue(new Move("u", true));
                moveToDo.Enqueue(new Move("l", false));
                moveToDo.Enqueue(new Move("u", false));
                needToDoMoves = true;
                break;
            case 73:
                learnText.SetText("The blue-red edge is now on the top layer so that we can solve it");
                nextButton.SetActive(true);
                makeTransByPos(true, new Vector3(-1, 0, -1));
                break;
            case 74:
                learnText.SetText("First we do U moves to align the red side with its center: U'");
                moveToDo.Enqueue(new Move("u", true));
                makeTransByPos(false, new Vector3(1, 0, 0));
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 75:
                learnText.SetText("We need to then notice that it is supposed to go on our right hand side");
                gameObject.GetComponent<TouchControls>().reset(90, -35);
                makeTransByPos(false, new Vector3(0, 0, 1));
                makeTransByPos(false, new Vector3(1, 0, 1));
                nextButton.SetActive(true);
                break;
            case 76:
                learnText.SetText("So we do a U move moving the piece away from where it is supposed to go: U");
                makeTransByPos(true, new Vector3(1, 0, 1));
                moveToDo.Enqueue(new Move("u", false));
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 77:
                learnText.SetText("Then do the same algorithm once on the right hand side since the piece has to go to the right: R U R' U'");
                moveToDo.Enqueue(new Move("b", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("b", true));
                moveToDo.Enqueue(new Move("u", true));
                needToDoMoves = true;
                break;
            case 78:
                learnText.SetText("Then we rotate the cube to the blue side");
                gameObject.GetComponent<TouchControls>().reset(0, -35);
                nextButton.SetActive(true);
                break;
            case 79:
                learnText.SetText("And now we have to do the algorithm the opposite way, on our left hand side: L' U' L U");
                nextButton.SetActive(false);
                moveToDo.Enqueue(new Move("r", true));
                moveToDo.Enqueue(new Move("u", true));
                moveToDo.Enqueue(new Move("r", false));
                moveToDo.Enqueue(new Move("u", false));
                needToDoMoves = true;
                break;
            case 80:
                learnText.SetText("The blue-red edge is solved!");
                nextButton.SetActive(true);
                break;
            case 81:
                learnText.SetText("Next lets solve the green-orange edge. Again, we make sure that the side in front (orange) is aligned with its center.");
                makeAllTrans(true);
                makeTransByPos(false, new Vector3(-1, 1, 0));
                makeTransByPos(false, new Vector3(-1, 0, 0));
                gameObject.GetComponent<TouchControls>().reset(270, -35);
                break;
            case 82:
                learnText.SetText("The piece goes on our right side so we do a U move to move it to the left: U");
                makeTransByPos(false, new Vector3(0, 0, -1));
                moveToDo.Enqueue(new Move("u", false));
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 83:
                learnText.SetText("Then do the same algorithm once on the right hand side since the piece has to go to the right: R U R' U'");
                moveToDo.Enqueue(new Move("f", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("f", true));
                moveToDo.Enqueue(new Move("u", true));
                needToDoMoves = true;
                break;
            case 84:
                learnText.SetText("Then we rotate the cube to the green side");
                gameObject.GetComponent<TouchControls>().reset(180, -35);
                nextButton.SetActive(true);
                break;
            case 85:
                learnText.SetText("And now we have to do the algorithm the opposite way, on our left hand side: L' U' L U");
                nextButton.SetActive(false);
                moveToDo.Enqueue(new Move("l", true));
                moveToDo.Enqueue(new Move("u", true));
                moveToDo.Enqueue(new Move("l", false));
                moveToDo.Enqueue(new Move("u", false));
                needToDoMoves = true;
                break;
            case 86:
                learnText.SetText("The green-orange edge is solved!");
                nextButton.SetActive(true);
                break;
            case 87:
                learnText.SetText("Lastly, lets solve the red-green edge");
                makeAllTrans(true);
                makeTransByPos(false, new Vector3(-1, 1, 0));
                break;
            case 88:
                learnText.SetText("First do U moves to align its red face with the red center: U U");
                nextButton.SetActive(false);
                makeTransByPos(false, new Vector3(1, 0, 0));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("u", false));
                needToDoMoves = true;
                break;
            case 89:
                learnText.SetText("Now notice that it is supposed to go on our left hand side. This means we will have to do all the steps in the opposite direction");
                makeTransByPos(false, new Vector3(1, 0, -1));
                makeTransByPos(false, new Vector3(0, 0, -1));
                gameObject.GetComponent<TouchControls>().reset(90, -35);
                nextButton.SetActive(true);
                break;
            case 90:
                learnText.SetText("First do a U' move moving the piece away from where it is supposed to go: U'");
                makeTransByPos(true, new Vector3(1, 0, -1));
                moveToDo.Enqueue(new Move("u", true));
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 91:
                learnText.SetText("Then do the algorithm once on the LEFT hand side since the piece has to go to the left: L' U' L U");
                moveToDo.Enqueue(new Move("f", true));
                moveToDo.Enqueue(new Move("u", true));
                moveToDo.Enqueue(new Move("f", false));
                moveToDo.Enqueue(new Move("u", false));
                needToDoMoves = true;
                break;
            case 92:
                learnText.SetText("Then we rotate the cube to the green side");
                gameObject.GetComponent<TouchControls>().reset(180, -35);
                nextButton.SetActive(true);
                break;
            case 93:
                learnText.SetText("And now we have to do the algorithm the opposite way, on our right hand side: R U R' U'");
                nextButton.SetActive(false);
                moveToDo.Enqueue(new Move("r", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("r", true));
                moveToDo.Enqueue(new Move("u", true));
                needToDoMoves = true;
                break;
            case 94:
                learnText.SetText("And the second layer is solved!");
                makeAllTrans(false);
                makeTransBySide(true, new Vector3(0, 1, 0));
                nextButton.SetActive(true);
                break;
            case 95:
                makeAllTrans(true);
                nextButton.SetActive(false);
                movesSolver.Add(new Move("u", false));
                
                movesSolver.Add(new Move("f", false));
                movesSolver.Add(new Move("r", false));
                movesSolver.Add(new Move("u", false));
                movesSolver.Add(new Move("r", true));
                movesSolver.Add(new Move("u", true));
                movesSolver.Add(new Move("f", true));
                fastSolve = true;
                solving = true;
                break;
            case 96:
                learnText.SetText("The next step is to solve the white cross, but this time don't need to put the pieces in the correct spot, we just need the white sides to face upwards");
                makeAllTrans(true);
                makeTransByPos(false, new Vector3(0, 1, 0));
                makeTransByPos(false, new Vector3(1, 1, 0));
                makeTransByPos(false, new Vector3(-1, 1, 0));
                makeTransByPos(false, new Vector3(0, 1, 1));
                makeTransByPos(false, new Vector3(0, 1, -1));
                nextButton.SetActive(true);
                break;
            case 97:
                learnText.SetText("There are many possible cases, but for all of them we will use the same algorithm");
                break;
            case 98:
                learnText.SetText("If none of the white faces are on top do the algorithm from any side: F (R U R' U') F'");
                nextButton.SetActive(false); 
                moveToDo.Enqueue(new Move("f", false));
                moveToDo.Enqueue(new Move("r", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("r", true));
                moveToDo.Enqueue(new Move("u", true));
                moveToDo.Enqueue(new Move("f", true));
                needToDoMoves = true;
                break;
            case 99:
                learnText.SetText("If the white faces make an L shape, rotate the cube so that the L is in the back left");
                nextButton.SetActive(true);
                gameObject.GetComponent<TouchControls>().reset(0, -35);
                break;
            case 100:
                learnText.SetText("Then do the algorithm: F (R U R' U') F'");
                nextButton.SetActive(false);
                moveToDo.Enqueue(new Move("b", false));
                moveToDo.Enqueue(new Move("l", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("l", true));
                moveToDo.Enqueue(new Move("u", true));
                moveToDo.Enqueue(new Move("b", true));
                needToDoMoves = true;
                break;
            case 101:
                learnText.SetText("If the white faces make a straight line, rotate the cube so the line is parallel to you");
                nextButton.SetActive(true);
                break;
            case 102:
                learnText.SetText("Then do the algorithm: F (R U R' U') F'");
                nextButton.SetActive(false);
                moveToDo.Enqueue(new Move("b", false));
                moveToDo.Enqueue(new Move("l", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("l", true));
                moveToDo.Enqueue(new Move("u", true));
                moveToDo.Enqueue(new Move("b", true));
                needToDoMoves = true;
                break;
            case 103:
                learnText.SetText("And the white cross is solved!");
                nextButton.SetActive(true);
                break;
            case 104:
                learnText.SetText("The next step is to actually put the white-edges in their correct spots");
                makeTransByNumOfSides(false, 1);
                makeTransByPos(true, new Vector3(0, -1, 0));
                break;
            case 105:
                learnText.SetText("We need to do U moves until atleast two edges are in the correct spot: U");
                nextButton.SetActive(false);
                moveToDo.Enqueue(new Move("u", false));
                needToDoMoves = true;
                break;
            case 106:
                learnText.SetText("If the two correct pieces are next to each other rotate the cube so that they are in the back right");
                nextButton.SetActive(true);
                gameObject.GetComponent<TouchControls>().reset(0, -35);
                break;
            case 107:
                learnText.SetText("The two correct pieces may also be opposite to each other in which case you can do the algorithm in the next step from any side");
                break;
            case 108:
                learnText.SetText("Now do this algorithm: R U R' U R U U R'");
                nextButton.SetActive(false);
                moveToDo.Enqueue(new Move("l", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("l", true));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("l", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("l", true));
                needToDoMoves = true;
                break;
            case 109:
                learnText.SetText("And then do U moves to put them into the correct spot again: U");
                nextButton.SetActive(false);
                moveToDo.Enqueue(new Move("u", false));
                needToDoMoves = true;
                break;
            case 110:
                learnText.SetText("And the white edges are solved. Note: you may have to repeat this step multiple times to solve all the edges");
                nextButton.SetActive(true);
                break;
            case 111:
                learnText.SetText("Now we need to put the white corners in their spots");
                makeAllTrans(true);
                makeTransByPos(false, new Vector3(1, 1, 1));
                makeTransByPos(false, new Vector3(1, 1, -1));
                makeTransByPos(false, new Vector3(-1, 1, 1));
                makeTransByPos(false, new Vector3(-1, 1, -1));
                break;
            case 112:
                learnText.SetText("This is the trickiest step because we put the corners into their spot but not necessarily rotated the correct way");
                break;
            case 113:
                learnText.SetText("For Example, the red-green-white corner is in the correct spot but it isn't rotated the right way. For this step it is considered solved");
                makeAllTrans(true);
                makeTransByPos(false, new Vector3(1, 1, -1));
                makeTransByPos(false, new Vector3(0, 0, -1));
                makeTransByPos(false, new Vector3(1, 0, 0));
                makeTransByPos(false, new Vector3(0, 1, 0));
                break;
            case 114:
                learnText.SetText("If all of the corners are in the right spot we could skip this step, but in this case only the red-green-white corner is correct");
                makeAllTrans(true);
                makeTransByPos(false, new Vector3(1, 1, 1));
                makeTransByPos(false, new Vector3(1, 1, -1));
                makeTransByPos(false, new Vector3(-1, 1, 1));
                makeTransByPos(false, new Vector3(-1, 1, -1));
                break;
            case 115:
                learnText.SetText("Rotate the cube so that the solved corner (red-green-white) is on our right hand side. If no corners are solved do the algorithm from any side");
                nextButton.SetActive(true);
                gameObject.GetComponent<TouchControls>().reset(180, -35);
                break;
            case 116:
                learnText.SetText("Now do this algorithm: U R U' L' U R' U' L");
                nextButton.SetActive(false);
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("r", false));
                moveToDo.Enqueue(new Move("u", true));
                moveToDo.Enqueue(new Move("l", true));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("r", true));
                moveToDo.Enqueue(new Move("u", true));
                moveToDo.Enqueue(new Move("l", false));
                needToDoMoves = true;
                break;
            case 117:
                learnText.SetText("Then check to see if all of the corners are in the correct spots. In this case they are not so we have to do the algorithm again");
                nextButton.SetActive(true);
                makeTransByNumOfSides(false, 1);
                makeTransByPos(true, new Vector3(0, -1, 0));
                break;
            case 118:
                learnText.SetText("Do the algorithm again: U R U' L' U R' U' L");
                nextButton.SetActive(false);
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("r", false));
                moveToDo.Enqueue(new Move("u", true));
                moveToDo.Enqueue(new Move("l", true));
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("r", true));
                moveToDo.Enqueue(new Move("u", true));
                moveToDo.Enqueue(new Move("l", false));
                needToDoMoves = true;
                break;
            case 119:
                learnText.SetText("Now we can see that all the corners are in the correct spot");
                nextButton.SetActive(true);
                break;
            case 120:
                learnText.SetText("And for the last step, we have to rotate the corners to their correct orientations. To make this easier lets first flip the cube");
                gameObject.GetComponent<TouchControls>().Flip();
                break;
            case 121:
                learnText.SetText("First we focus on the corner on our bottom right, the white-green-orange corner");
                makeAllTrans(true);
                makeTransByPos(false, new Vector3(-1, 1, -1));
                break;
            case 122:
                learnText.SetText("Do the same algorithm we learned at the start until the corner is solved. Don't worry about the rest of the cube: (R U R' U')x4");
                for (int i = 0; i < 4; i++)
                {
                    moveToDo.Enqueue(new Move("l", false));
                    moveToDo.Enqueue(new Move("d", false));
                    moveToDo.Enqueue(new Move("l", true));
                    moveToDo.Enqueue(new Move("d", true));
                }
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 123:
                learnText.SetText("Now we find the next corner we want to solve, the white-blue-orange corner and do a D' move to bring it to our right hand side: D'");
                makeAllTrans(true);
                moveToDo.Enqueue(new Move("u", true));
                needToDoMoves = true;
                makeTransByName(false, "WBO");
                break;
            case 124:
                learnText.SetText("Then do the algorithm again until the corner is solved: (R U R' U')x4");
                for (int i = 0; i < 4; i++)
                {
                    moveToDo.Enqueue(new Move("l", false));
                    moveToDo.Enqueue(new Move("d", false));
                    moveToDo.Enqueue(new Move("l", true));
                    moveToDo.Enqueue(new Move("d", true));
                }
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 125:
                learnText.SetText("Then find the next (and last) corner to solve, the white-green-red corner and do D moves to bring it to our right hand side: D D");
                makeAllTrans(true);
                moveToDo.Enqueue(new Move("u", false));
                moveToDo.Enqueue(new Move("u", false));
                needToDoMoves = true;
                makeTransByName(false, "WGR");
                break;
            case 126:
                learnText.SetText("Then do the algorithm again until the corner is solved: (R U R' U')x4");
                for (int i = 0; i < 4; i++)
                {
                    moveToDo.Enqueue(new Move("l", false));
                    moveToDo.Enqueue(new Move("d", false));
                    moveToDo.Enqueue(new Move("l", true));
                    moveToDo.Enqueue(new Move("d", true));
                }
                nextButton.SetActive(false);
                needToDoMoves = true;
                break;
            case 127:
                learnText.SetText("Do one last D move to put the bottom layer back together: D");
                makeAllTrans(false);
                moveToDo.Enqueue(new Move("u", true));
                needToDoMoves = true;
                break;
            case 128:
                learnText.SetText("Congratulations!!! you have solved a rubiks cube!");
                nextButton.SetActive(true);
                gameObject.GetComponent<TouchControls>().Flip();
                gameObject.GetComponent<TouchControls>().reset();
                break;
            case 129:
                learnText.SetText("");
                nextButton.SetActive(false);
                foreach (GameObject piece in pieces)
                {
                    piece.GetComponent<PieceReciever>().reset();
                }
                solveButton.SetActive(true);
                scrambleButton.SetActive(true);
                learnButton.SetActive(true);
                cubeSim = new Cube();
                learnState = 0;
                learning = false;
                break;
        }
    }
}