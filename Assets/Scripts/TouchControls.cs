using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControls : MonoBehaviour{
    public Camera cam;
    Vector3 start = Vector3.zero;
    Vector2 startScreen = Vector2.zero;
    Vector2 endScreen = Vector2.zero;
    // Update is called once per frame
    void Update(){
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && start == Vector3.zero){
            startScreen = Input.touches[0].position;
            Ray ray = cam.ScreenPointToRay(Input.touches[0].position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)){
                if (hit.collider != null){
                    start = hit.collider.GetComponent<Transform>().position;
                }
            }
        }else if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended && endScreen == Vector2.zero) {
            endScreen = Input.touches[0].position;
        }

        if (startScreen != Vector2.zero && endScreen != Vector2.zero) {
            start = round(start);
            startScreen = round(startScreen);
            endScreen = round(endScreen);
            float deltaX = endScreen.x - startScreen.x;
            float deltaY = endScreen.y - startScreen.y;
            if (Mathf.Abs(deltaX) > 5 && Mathf.Abs(deltaY) > 5) {
                bool isYGreater = Mathf.Abs(deltaY) >= Mathf.Abs(deltaX);
                bool YgreaterThanZero = deltaY >= 0;
                bool XgreaterThanZero = deltaX >= 0;
                if ((start == new Vector3(1, -1, -1.5f) || start == new Vector3(1, 0, -1.5f) || start == new Vector3(1, 1, -1.5f)) && isYGreater) {
                    cam.GetComponent<PieceControl>().turn("r", !YgreaterThanZero);
                } else if (start == new Vector3(1, 1.5f, 0) && !isYGreater) {
                    cam.GetComponent<PieceControl>().turn("r", !XgreaterThanZero);
                } else if ((start == new Vector3(1.5f, 1, -1) || start == new Vector3(1.5f, 0, -1) || start == new Vector3(1.5f, -1, -1)) && isYGreater) {
                    cam.GetComponent<PieceControl>().turn("f", YgreaterThanZero);
                } else if (start == new Vector3(0, 1.5f, -1) && !isYGreater) {
                    cam.GetComponent<PieceControl>().turn("f", !XgreaterThanZero);
                } else if ((start == new Vector3(1, 1, -1.5f) || start == new Vector3(0, 1, -1.5f) || start == new Vector3(-1, 1, -1.5f) || start == new Vector3(1.5f, 1, -1) || start == new Vector3(1.5f, 1, 0) || start == new Vector3(1.5f, 1, 1)) && !isYGreater) {
                    cam.GetComponent<PieceControl>().turn("u", XgreaterThanZero);
                } else if ((start == new Vector3(1, -1, -1.5f) || start == new Vector3(0, -1, -1.5f) || start == new Vector3(-1, -1, -1.5f) || start == new Vector3(1.5f, -1, -1) || start == new Vector3(1.5f, -1, 0) || start == new Vector3(1.5f, -1, 1)) && !isYGreater) {
                    cam.GetComponent<PieceControl>().turn("d", !XgreaterThanZero);
                } else if ((start == new Vector3(-1, -1, -1.5f) || start == new Vector3(-1, 0, -1.5f) || start == new Vector3(-1, 1, -1.5f)) && isYGreater) {
                    cam.GetComponent<PieceControl>().turn("l", YgreaterThanZero);
                } else if (start == new Vector3(-1, 1.5f, 0) && Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                    cam.GetComponent<PieceControl>().turn("l", XgreaterThanZero);
                } else if ((start == new Vector3(1.5f, -1, 1) || start == new Vector3(1.5f, 0, 1) || start == new Vector3(1.5f, 1, 1)) && isYGreater) {
                    cam.GetComponent<PieceControl>().turn("b", !YgreaterThanZero);
                } else if (start == new Vector3(0, 1.5f, 1) && !isYGreater) {
                    cam.GetComponent<PieceControl>().turn("b", XgreaterThanZero);
                } else if (start == new Vector3(-1, 1.5f, 1)) {
                    if (deltaY / Mathf.Abs(deltaY) == deltaX / Mathf.Abs(deltaX)) {
                        cam.GetComponent<PieceControl>().turn("l", XgreaterThanZero);
                    } else {
                        cam.GetComponent<PieceControl>().turn("b", XgreaterThanZero);
                    }
                } else if (start == new Vector3(-1, 1.5f, -1)) {
                    if (YgreaterThanZero == XgreaterThanZero) {
                        cam.GetComponent<PieceControl>().turn("l", XgreaterThanZero);
                    } else {
                        cam.GetComponent<PieceControl>().turn("f", !XgreaterThanZero);
                    }
                } else if (start == new Vector3(1, 1.5f, 1)) {
                    if (YgreaterThanZero == XgreaterThanZero) {
                        cam.GetComponent<PieceControl>().turn("r", !XgreaterThanZero);
                    } else {
                        cam.GetComponent<PieceControl>().turn("b", XgreaterThanZero);
                    }
                } else if (start == new Vector3(1, 1.5f, -1)) {
                    if (YgreaterThanZero == XgreaterThanZero) {
                        cam.GetComponent<PieceControl>().turn("r", !XgreaterThanZero);
                    } else {
                        cam.GetComponent<PieceControl>().turn("f", !XgreaterThanZero);
                    }
                } else if (start == Vector3.zero) {
                    if (isYGreater) {
                        if (YgreaterThanZero){
                            cam.GetComponent<PieceControl>().rotateCube(Vector3.right);
                        }else{
                            cam.GetComponent<PieceControl>().rotateCube(Vector3.left);
                        }
                    } else {
                        if (XgreaterThanZero){
                            cam.GetComponent<PieceControl>().rotateCube(Vector3.down);
                        }else{
                            cam.GetComponent<PieceControl>().rotateCube(Vector3.up);
                        }
                    }
                }
            }
            reset();
        }
    }

    private Vector3 round(Vector3 v) {
        float x = Mathf.Round(v.x * 10) / 10;
        float y = Mathf.Round(v.y * 10) / 10;
        float z = Mathf.Round(v.z * 10) / 10;
        return new Vector3(x, y, z);
    }
    private Vector2 round(Vector2 v)
    {
        float x = Mathf.Round(v.x);
        float y = Mathf.Round(v.y);
        return new Vector2(x, y);
    }
    private void reset() {
        start = Vector3.zero;
        startScreen = Vector2.zero;
        endScreen = Vector2.zero;
    }
}
