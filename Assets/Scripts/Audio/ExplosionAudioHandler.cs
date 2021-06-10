using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAudioHandler : MonoBehaviour
{
    private AudioSource m_explosionAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_explosionAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_explosionAudioSource.isPlaying)
        {
           Destroy(gameObject);
        }
    }
}
