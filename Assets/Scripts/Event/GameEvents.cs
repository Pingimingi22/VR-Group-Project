using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameEvents : MonoBehaviour
{

    public static GameEvents gameEvents;


	// Start is called before the first frame update

	private void Awake()
	{
        gameEvents = this;
    }
	void Start()
    { 
    }

    public event Action<int> OnScoreEvent;
    public void ScoreEvent(int score)
    {
        if (OnScoreEvent != null)
        {
            OnScoreEvent(score);
        }
    }

}
