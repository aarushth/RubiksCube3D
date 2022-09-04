using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdDisplay : MonoBehaviour{

    public string myAdUnitId = "ad1";
    private bool adStarted = true;
    private bool testMode = false;
    // Start is called before the first frame update
    void Start(){
        Advertisement.Initialize("4909657", testMode);
        myAdUnitId = "ad1";
    }

    // Update is called once per frame
    void Update(){
        if (Advertisement.isInitialized && !adStarted){
            Advertisement.Load(myAdUnitId);
            Advertisement.Show(myAdUnitId);
            adStarted = true;
        }

    }

    public void loadAd(){
        adStarted = false;
    }
}
