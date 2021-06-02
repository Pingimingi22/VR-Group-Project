using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HighscoreHandler : MonoBehaviour
{
    public TextMeshProUGUI m_text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_text.text = $"Highscore: {GameManager.m_highScore}";
    }
}
