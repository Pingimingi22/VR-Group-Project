using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRender : MonoBehaviour
{

    public Transform m_pointer;
    LineRenderer m_lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        m_lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(m_pointer.position, m_pointer.forward);

        //m_lineRenderer.SetWidth(0.1f, 0.1f);

        m_lineRenderer.startWidth = 0.1f;
        m_lineRenderer.endWidth = 0.1f;
        m_lineRenderer.SetPosition(0, ray.origin);
        m_lineRenderer.SetPosition(1, ray.origin + 100 * ray.direction);
    }
}
