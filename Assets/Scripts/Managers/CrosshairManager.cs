using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    public GameObject outerBeam;
    public GameObject cylinder;
    Vector3 scaleChange;
    public float lengthX;
    public float initialXPos;

    // Start is called before the first frame update
    void Awake()
    {
        initialXPos = this.gameObject.transform.localPosition.x;
        if (initialXPos > 0)
        {
            lengthX = (outerBeam.transform.position.x - cylinder.transform.position.x - 0.05f) * 10;
        }
        else
        {
            lengthX = (cylinder.transform.position.x - outerBeam.transform.position.x - 0.05f) * 10;
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
            //Debug.Log("added length");
        }
        else if (this.gameObject.transform.localScale.x > lengthX)
        {
            scaleChange.x = lengthX;

            this.gameObject.transform.localScale = scaleChange;
            //Debug.Log("removed length");
        }


        //Matrix4x4 toSubSpace = theSubmarineTransform.whatever.GetInverse(); //TODO Replace this with actual code that works
        Matrix4x4 toSubSpace = transform.root.localToWorldMatrix.inverse;
        Vector4 cylinderPos = toSubSpace * cylinder.transform.position;
        Vector4 outerBeamPos = toSubSpace * outerBeam.transform.position;

        if (initialXPos > 0)
        {
            lengthX = (outerBeamPos.x - cylinderPos.x - 0.05f) * 10;
        }
        else
        {
            lengthX = (cylinderPos.x - outerBeamPos.x - 0.05f) * 10;
        }
    }
}
