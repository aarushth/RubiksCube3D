using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public Camera cam;
    public void scramble() {
        cam.GetComponent<PieceControl>().scramble();
    }
}
