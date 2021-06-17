using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeManager : MonoBehaviour
{
    public List<GameObject> prizes;
    private int index = 0;
    private int score = 0;
    //public static int prizeIncrements = 600;
    public int prizeIncrements = 600;
    // Start is called before the first frame update

    //public static List<GameObject> prizesS;
    void Start()
    {

        foreach (GameObject tropthy in prizes)
        {

            tropthy.active = false;
        }

        GameEvents.gameEvents.OnScoreEvent += AddPoints;
        //prizesS = prizes;
    }

    public void AddPoints(int points)
    {
        score += points;
        if (score >= prizeIncrements)
        {
            score -= prizeIncrements;

            if (index < prizes.Count)
            {
                prizes[index].active = true;
                index++;
            }
        }
    }
}
