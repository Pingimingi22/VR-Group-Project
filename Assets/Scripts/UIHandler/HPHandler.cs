using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HPHandler : MonoBehaviour
{
    public PlayerManager m_playerManager;
    public TextMeshProUGUI m_text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_text.text = $"Health: {m_playerManager.m_playerHealth}";
    }
}
