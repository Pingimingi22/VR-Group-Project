using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    public float m_bulletLifeTime = 0;

    private bool m_isActive = false;
    private float m_counter = 0.0f;

    public ParticleSystem m_explosionParticles;

    // Start is called before the first frame update
    void Start()
    {
        m_isActive = true; // Setting to true so the timer can start.
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isActive)
        {
            m_counter += Time.deltaTime;

            if (m_counter >= m_bulletLifeTime)
            {
                // The bullet has reached it's max life time.
                if (this.gameObject.tag == "Shell")
                {
                    Debug.Log("destroy torpedo");
                    m_explosionParticles.transform.parent = null;

                    m_explosionParticles.Play();

                    Destroy(m_explosionParticles.gameObject, m_explosionParticles.main.duration);
                }

                Destroy(gameObject);
            }
        }
    }
}
