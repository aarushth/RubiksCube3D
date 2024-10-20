using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TouchControls : MonoBehaviour{

    public float distance;

    public float sensitivity;
    public float yaw = 0f;
    public float pitch = 0f;
    public float flip = 0f;
    float defYaw = 135;
    float defPitch = -35;

    float yawRot = 135;
    float pitchRot = -35;
    float yawRate;
    float pitchRate;
    float flipRate = 0;
    float flipRot = 0;
    public Camera cam;
    float tChange = 0.01f;
    Transform startOb;
    Vector3 start = Vector3.zero;

    Transform endOb;
    Vector3 end = Vector3.zero;
     
    Vector2 startScreen = Vector2.zero;
    Vector2 endScreen = Vector2.zero;
    // Update is called once per frame
    float t;
    bool resetting = false;
    private static Dictionary<Vector3, string> moveVectors = new Dictionary<Vector3, string>{
        {new Vector3(0, 0, -1), "f"},
        {new Vector3(1, 0, 0), "r"},
        {new Vector3(-1, 0, 0), "l"},
        {new Vector3(0, 0, 1), "b"},
        {new Vector3(0, 1, 0), "u"},
        {new Vector3(0, -1, 0), "d"}
    };
    void FixedUpdate(){

        if (resetting)
        {
            if (t <= 1 || yaw != yawRot || pitch != pitchRot || flip != flipRot)
            {
                yaw = Mathf.Lerp(yawRate, yawRot, t);
                pitch = Mathf.Lerp(pitchRate, pitchRot, t);
                flip = Mathf.Lerp(flipRate, flipRot, t);
                t += tChange;
                Rotate(Quaternion.Euler(pitch, yaw, 0));
            }
            else
            {
                resetting = false;
                t = 0;
            }
        }
        else {
            flip = fli ? 180 : 0;
        }
        
        if (Input.touchCount > 0) {
            if (Input.touches[0].phase == TouchPhase.Began && start == Vector3.zero) {
                startScreen = Input.touches[0].position;
                Ray ray = cam.ScreenPointToRay(Input.touches[0].position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    if (hit.collider != null) {
                        startOb = hit.collider.GetComponent<Transform>();
                        start = startOb.position;
                    }
                }
            }else if (start != Vector3.zero && endScreen == Vector2.zero) {
                Ray ray = cam.ScreenPointToRay(Input.touches[0].position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<Transform>().position == start)
                        {
                            endScreen = Vector2.zero;
                        }
                        else {
                            endScreen = Input.touches[0].position;
                            endOb = hit.collider.GetComponent<Transform>();
                            end = endOb.position;
                        }
                    }
                }
            }

            if (startScreen != Vector2.zero && endScreen != Vector2.zero && start != Vector3.zero && end != Vector3.zero) {
                //normalVector
                start = start / 1.5f;
                start = floor(start);

                Vector3 parenta = startOb.transform.parent.gameObject.transform.position;
                Vector3 parentb = endOb.transform.parent.gameObject.transform.position;
                Vector3 pos = new Vector3(parenta.x == parentb.x ? parenta.x : 0f, parenta.y == parentb.y ? parenta.y : 0f, parenta.z == parentb.z ? parenta.z : 0f);
                Vector3 fin = pos - start;
                Vector3 rot1 = parentb - parenta;
                
                if (fin == pos) {
                    end = end / 1.5f;
                    end = floor(end);
                    fin = pos - end;
                    start = end;
                }

                Vector3 dir = Vector3.Cross(rot1, start);
                dir.Normalize();
                fin.Normalize();
                bool isPr = dir == fin;
                
                string value;
                if (moveVectors.TryGetValue(fin, out value))
                {
                    cam.GetComponent<PieceControl>().receiveMove(value, isPr);
                }
            }
            else if(Input.touches[0].phase != TouchPhase.Began && startScreen != Vector2.zero && start == Vector3.zero)
            {
                Vector2 delta = Input.touches[0].deltaPosition;
                yaw += delta.x * sensitivity * Time.fixedDeltaTime * (fli?-1:1);
                pitch += delta.y * sensitivity * Time.fixedDeltaTime * (fli ? -1 : 1);

                Rotate(Quaternion.Euler(pitch, yaw, 0f));
            }
            if (endScreen != Vector2.zero) {
                res();
            }
        }
    }
    private bool fli = false;
    public void Flip() {
        fli = !fli;
        flipRate = flip;
        flipRot = fli ? 180 : 0;
        reset(yaw, pitch * -1);

        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, flip);
        //Debug.Log(flip);
    }
    private void Rotate(Quaternion rotation) {
        Vector3 positionOffset = rotation * new Vector3(0, 0, distance);
        transform.position = Vector3.zero + positionOffset;
        transform.LookAt(Vector3.zero);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, flip);

    }
    private Vector3 floor(Vector3 v)
    {
        float x = (int)v.x;
        float y = (int)v.y;
        float z = (int)v.z;
        return new Vector3(x, y, z);
    }
    public void reset(float y, float p) {
        yawRot = y;
        pitchRot = p;
        yawRate = yaw;
        pitchRate = pitch;
        t = 0;
        res();
        
        tChange = 1.0f/25;
        resetting = true;
        
    }
    public void reset()
    {
        yawRot = defYaw;
        pitchRot = defPitch;
        yawRate = yaw;
        pitchRate = pitch;
        t = 0;
        res();
        
        tChange = 1.0f / 30;
        resetting = true;

    }
    private void res() {
        start = Vector3.zero;
        end = Vector3.zero;
        startScreen = Vector2.zero;
        endScreen = Vector2.zero;
    }
}
