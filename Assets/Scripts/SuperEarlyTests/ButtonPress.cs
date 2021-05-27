using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    public GameObject testObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        if (testObj.activeSelf)
        {
            testObj.SetActive(false);
        }
        else
        {
            testObj.SetActive(true);
        }
    }
}
