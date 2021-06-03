using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalCrosshairManager : MonoBehaviour
{
    public GameObject outerBeam;
    public GameObject cylinder;
    Vector3 scaleChange;
    public float lengthX;
    public float initialZPos;

    // Start is called before the first frame update
    void Awake()
    {
        initialZPos = this.gameObject.transform.localPosition.z;
        if (initialZPos > 0)
        {
            lengthX = (cylinder.transform.position.y - outerBeam.transform.position.y - 0.05f) * 10;
        }
        else
        {
            lengthX = (outerBeam.transform.position.y - cylinder.transform.position.y - 0.05f) * 10;
        }

        scaleChange.y = this.gameObject.transform.localScale.y;
        scaleChange.z = this.gameObject.transform.localScale.z;
        //float length = cylinder.transform.position.x - outerBeam.transform.position.x;
        //scaleChange = new Vector3(length, 0, 0);
        //
        //this.gameObject.transform.localScale += scaleChange;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.transform.localScale.x < lengthX)
        {
            //float length = cylinder.transform.position.x - outerBeam.transform.position.x;
            scaleChange.x = lengthX;

            this.gameObject.transform.localScale = scaleChange;
            Debug.Log("added length");
        }
        else if (this.gameObject.transform.localScale.x > lengthX)
        {
            scaleChange.x = lengthX;

            this.gameObject.transform.localScale = scaleChange;
            Debug.Log("removed length");
        }
        if (initialZPos > 0)
        {
            lengthX = (cylinder.transform.position.y - outerBeam.transform.position.y - 0.05f) * 10;
        }
        else
        {
            lengthX = (outerBeam.transform.position.y - cylinder.transform.position.y - 0.05f) * 10;
        }
    }
}
