using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceReciever : MonoBehaviour{

    void Start(){
        Application.targetFrameRate = 20;
    }
    float counter = 90;
    bool isRotating = false;
    float speed = 720;
    Vector3 rotateAround = Vector3.zero;
    public Camera cam;
    void FixedUpdate() {
        if (isRotating) { 
            float rotateAmount = speed * Time.deltaTime;
            if (counter < rotateAmount)
            {
                transform.RotateAround(rotateAround, rotateAround, counter);
                transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
                var vec = transform.eulerAngles;
                int x = (int) Mathf.Round(vec.x / 90) * 90;
                int y = (int) Mathf.Round(vec.y / 90) * 90;
                int z = (int) Mathf.Round(vec.z / 90) * 90;
                transform.eulerAngles = new Vector3(x, y, z);
                counter = 90;
                isRotating = false;
                rotateAround = Vector3.zero;
                cam.GetComponent<PieceControl>().reset();
            }else{
                transform.RotateAround(rotateAround, rotateAround, rotateAmount);
                counter -= rotateAmount;
            }
        }
    }
    public void rotate(Vector3 r) {
        isRotating = true;
        rotateAround = r;
    }
}
