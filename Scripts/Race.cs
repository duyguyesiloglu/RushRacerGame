using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Race : MonoBehaviour
{
    public static Race instance;

    public CheckPoints[] allcheckPoints;

    public int totalLaps;

    public CarContoler playerCar;
    public List<CarContoler> allAICars = new List<CarContoler>();

    public int playerPos;

    public float timebetweenPos = .2f;
    private float posCheckCount;

    public bool isStart;
    public float timeBetweenStartCount = 1f;
    private float startCounter;
    public int countdownCurrent = 3;



    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for (int i = 0; i < allcheckPoints.Length; i++)
        {
            allcheckPoints[i].cpNumber = i;
        }
        isStart = true;
        startCounter = timeBetweenStartCount;

        UIManager.instance.countdown.text = countdownCurrent + "!";

    }

    void Update()
    {
        if (isStart)
        {
            startCounter -= Time.deltaTime;
            if(startCounter <= 0)
            {
                countdownCurrent--;
                startCounter = timeBetweenStartCount;
                UIManager.instance.countdown.text = countdownCurrent + "!";

                if (countdownCurrent == 0)
                {
                    isStart = false;
                    UIManager.instance.countdown.gameObject.SetActive(false);
                    UIManager.instance.go.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (posCheckCount <= 0)
            {
                playerPos = 1;

                foreach (CarContoler aicar in allAICars)
                {
                    if (aicar.current > playerCar.current)
                    {
                        playerPos++;
                    }
                    else if (aicar.current == playerCar.current)
                    {
                        if (aicar.nextCheckpoint > playerCar.nextCheckpoint)
                        {
                            playerPos++;
                        }
                        else if (aicar.nextCheckpoint == playerCar.nextCheckpoint)
                        {
                            // Indeks sınırları kontrolü
                            if (aicar.nextCheckpoint >= 0 && aicar.nextCheckpoint < allcheckPoints.Length)
                            {
                                if (Vector3.Distance(aicar.transform.position, allcheckPoints[aicar.nextCheckpoint].transform.position) < Vector3.Distance(playerCar.transform.position, allcheckPoints[aicar.nextCheckpoint].transform.position))
                                {
                                    playerPos++;
                                }
                            }
                        }
                    }
                }

                UIManager.instance.posText.text = playerPos + "/" + (allAICars.Count + 1);
            }
        }
    }
}