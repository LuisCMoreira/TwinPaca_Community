using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class rotationText : MonoBehaviour
{

    public TMP_Text myTMPDirection;

    public cubeRotation rotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        myTMPDirection.text = "Cube Rotation :" + rotation.dataFromAPI ;
    }
}
