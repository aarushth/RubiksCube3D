using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceControl : MonoBehaviour {


    // Start is called before the first frame update
    void Start() {
        pieces = GameObject.FindGameObjectsWithTag("pieceCube");
        int i = 0;
        foreach (GameObject p in pieces){
            if (p.transform.childCount > 1){
                checkPieces[i] = p;
                i++;
                Debug.Log(p);
            }
        }

        Application.targetFrameRate = 20;
    }
    public GameObject[] pieces = new GameObject[26];
    public GameObject[] checkPieces = new GameObject[20];
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
    int move = 0;
    public GameObject ad;
    // Update is called once per frame
    void Update() {
        pCounter = 0;
        if (scrambling && !rotating) {
            int newMove = 0;
            while (newMove%6 == move%6) {
                newMove = Random.Range(0, 12);
            }
            move = newMove;
            turn(moves[move % 6], move % 2 == 0);
            counter++;
            if (counter == 20) {
                counter = 0;
                scrambling = false;
            }
        }
    }

    public void scramble(){
        scrambling = true;
    }
    int pCounter = 0;
    int moveCounter = 0;
    public void reset() {
        if (!scrambling) {
            pCounter++;
            if (pCounter == 9) {
                moveCounter++;
                rotating = false;
                pCounter = 0;
                if (cubeSolved() && moveCounter > 30) {
                    ad.GetComponent<AdDisplay>().loadAd();
                    moveCounter = 0;
                }
            }
        }
        rotating = false;
    }
    public void rotateCube(Vector3 rotate) {
        if (!rotating) { 
            rotating = true;
            rotateAround = rotate;
            foreach (GameObject piece in pieces) {
                piece.GetComponent<PieceReciever>().rotate(rotateAround);
            }
        }
    }
    public void turn(string s, bool isPrime) {
        if (!rotating) { 
            int i = 0;
            rotateAround = moveVectors[s];
            if (rotateAround.x != 0) {
                foreach (GameObject piece in pieces) {
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
    private bool cubeSolved() {
        GameObject checkP = checkPieces[0];
        foreach (GameObject piece in checkPieces) {
            if (piece.transform.eulerAngles != checkP.transform.eulerAngles) {
                Debug.Log(piece.name);
                Debug.Log(piece.transform.eulerAngles);
                return false;
            }
        }
        return true;
    }
}