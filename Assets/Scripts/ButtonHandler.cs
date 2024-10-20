using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    public Camera cam;
    public void Start() {
        cam.useOcclusionCulling = false;
    }
    public void scramble() {
        cam.GetComponent<PieceControl>().scramble(20);
        //cam.GetComponent<TouchControls>().Flip();
    }
    public void reset()
    {
        cam.GetComponent<PieceControl>().solve();
        cam.GetComponent<TouchControls>().reset();
    }
    public void learn() {
        cam.GetComponent<PieceControl>().learn();
        cam.GetComponent<TouchControls>().reset();
    }
    public void next()
    {
        cam.GetComponent<PieceControl>().next();
    }
}
