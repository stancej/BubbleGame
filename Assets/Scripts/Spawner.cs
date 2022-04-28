using System.Collections.Generic;
using UnityEngine;


public class Spawner : MonoBehaviour
{
    //game difficulty
    public gameDifficulty[] gameDifficulties;
    private enum gameDifficultEnums { Easy = 0, Normal = 1, Hard = 2 }
    private int currentDifInd;
    gameDifficulty currentDiffuclty;

    private List<int> chanceList = new List<int>();
    private int[] enemiesIndArr;

    //enemy spawn chance
    private float currentChanceToSpawnEnemie;
    private float maxChance = 0;
    private float prevChance = 0;
    private float currentEnemyChance;
    private int chanceListSize = 0;

    //spawn time
    private float secBtwSpawn;
    private float nextSpawnTime;
    private float nextIncreaseDif;


    private int enemiesLength;
    private float randValue;

    //
    public static int currentPoints { get; set; }

    //как посредники для сортировки массива вероятности спавна
    private int indVal;
    private float _maxChance;
    private bool isSameVal = true;

    private Transform[] newArrEnemies;
    private Transform currentEnemie;

    private bool isFirstCalc = true;

    private bool isLastDifficult;


    private void Awake()
    {
        currentDiffuclty = gameDifficulties[(int)gameDifficultEnums.Easy];
        currentDifInd = (int)gameDifficultEnums.Easy;
        if(currentDifInd + 1 == gameDifficulties.Length)
        {
            isLastDifficult = true;
        }
    }


    private void Start()
    {
        enemiesIndArr = new int[currentDiffuclty.enemies.Length];


        //set enemies start stats
        for (int i = 0; i < currentDiffuclty.enemies.Length; i++)
        {
            EnemiesBehavior currentEnemieBeh = currentDiffuclty.enemies[i].GetComponent<EnemiesBehavior>();

            //set std starts to _variables
            currentEnemieBeh._timeToNextSpawn = currentEnemieBeh.timeToNextSpawn;
            currentEnemieBeh._chanceToSpawn = currentEnemieBeh.chanceToSpawn;

            currentEnemieBeh._timeToNextSpawn = currentEnemieBeh.timeToNextSpawn / currentDiffuclty.startCoef;
            currentEnemieBeh._chanceToSpawn = currentEnemieBeh.chanceToSpawn * currentDiffuclty.startCoef;
            currentEnemieBeh._damage = currentEnemieBeh.damage * currentDiffuclty.startCoef;
            currentEnemieBeh._health = currentEnemieBeh.startingHealth * currentDiffuclty.startCoef;
            currentEnemieBeh.isSummoned = true;
        }



        enemiesLength = currentDiffuclty.enemies.Length;
        calcMaxChance();
        _maxChance = maxChance;
        //sort up enemies by their spawnChance;
        for (int i = 0; i < enemiesLength; i++)
        {

            maxChance = _maxChance;
            for (int j = 0; j < enemiesLength; j++)
            {
                currentEnemyChance = currentDiffuclty.enemies[j].GetComponent<EnemiesBehavior>()._chanceToSpawn;
                isSameVal = true;
                for (int n = 0; n < i; n++)
                {
                    if (enemiesIndArr[n] == j)
                    {
                        isSameVal = false;
                    }
                }
                if (currentEnemyChance <= maxChance && isSameVal == true)
                {
                    maxChance = currentEnemyChance;
                    indVal = j;
                }
            }
            enemiesIndArr[i] = indVal;
        }

        Transform[] __Enemies = new Transform[currentDiffuclty.enemies.Length];

        //fill the massive...
        for (int i = 0; i < enemiesLength; i++)
        {
            __Enemies[i] = currentDiffuclty.enemies[enemiesIndArr[i]];
        }
        currentDiffuclty.enemies = __Enemies;
        //will need to be replaced with difficult values

        nextIncreaseDif = Time.time + currentDiffuclty.timeToFirstIncreaseEnemiesStats + 2; //set incDif time after start game
        nextSpawnTime = Time.time + 2f;//set spawn time after start game
    }

    private void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            //Enemies spawn time and chose rand enemy from array enemies with him chance
            if (Time.time > nextSpawnTime)
            {
                //select rand enemie
                randValue = (Random.Range(1, 10001)) / 100f;

                //calcule summ enemies spawn chance
                maxChance = 0;
                for (int i = 0; i < enemiesLength/*нужно заменить потом*/; i++)
                {
                    if (maxChance < currentDiffuclty.enemies[i].GetComponent<EnemiesBehavior>()._chanceToSpawn)
                    {
                        maxChance = currentDiffuclty.enemies[i].GetComponent<EnemiesBehavior>()._chanceToSpawn;
                    }
                }
                //calculete chance to spawn enemy
                currentChanceToSpawnEnemie = (randValue * maxChance) / 100;
                for (int i = 0; i < enemiesLength/*нужно заменить потом*/; i++)
                {
                    currentEnemyChance = currentDiffuclty.enemies[i].GetComponent<EnemiesBehavior>()._chanceToSpawn;

                    //calculte which one need to spawn
                    if ((currentEnemyChance >= currentChanceToSpawnEnemie) && ((prevChance - currentEnemyChance >= 0) || isFirstCalc))
                    {
                        if (isFirstCalc == true)
                        {
                            chanceList.Clear();
                            chanceList.Add(i);
                            chanceListSize = 1;
                            isFirstCalc = false;
                        }
                        else if (currentDiffuclty.enemies[chanceList[0]].GetComponent<EnemiesBehavior>()._chanceToSpawn == prevChance)
                        {
                            chanceList.Add(i);
                            chanceListSize++;
                        }
                        else
                        {
                            chanceList.Clear();
                            chanceList.Add(i);
                            chanceListSize = 1;
                        }
                        prevChance = currentEnemyChance;
                    }
                }

                isFirstCalc = true;
                chanceList.TrimExcess();

                //chose one random enemy with same chance to spawn
                if (chanceListSize > 1)
                {
                    int randNewValue = Random.Range(0, chanceListSize);
                    int valueFromList = chanceList[randNewValue];
                    chanceList.Clear();
                    chanceList.Add(valueFromList);
                }

                //create enemy
                currentEnemie = currentDiffuclty.enemies[chanceList[0]];
                Instantiate(currentEnemie);
                

                //nextSpawn
                secBtwSpawn = currentEnemie.GetComponent<EnemiesBehavior>()._timeToNextSpawn;
                nextSpawnTime = Time.time + secBtwSpawn;

                //reset maxChance and prevChance
                maxChance = 0;
                prevChance = 0;

            }

            //difficulty increase every <secBtwIncreaseDif> sec
            if (Time.time > nextIncreaseDif)
            {
                increaseDifficulty(currentDiffuclty.coefSpawn);
            }

            if(currentDiffuclty.pointToSetNextDifficulty <= currentPoints && isLastDifficult != true)
            {
                currentPoints = 0;
                currentDifInd += 1;
                currentDiffuclty = gameDifficulties[currentDifInd];
                Start();
                if(currentDifInd + 1 == gameDifficulties.Length)
                {
                    isLastDifficult = true;
                }
            }

        }
    }

    private void increaseDifficulty(float changeStatValue)
    {
        for (int i = 0; i < currentDiffuclty.enemies.Length; i++)
        {
            EnemiesBehavior currentEnemieBeh = currentDiffuclty.enemies[i].GetComponent<EnemiesBehavior>();
            calcMaxChance();
            if (maxChance != currentEnemieBeh._chanceToSpawn)
            {
                currentEnemieBeh._chanceToSpawn += changeStatValue;
            }
            currentEnemieBeh._timeToNextSpawn -= changeStatValue;
            //установка минимального порога следуйщего спавна
            if (currentEnemieBeh._timeToNextSpawn < currentEnemieBeh.timeToNextSpawn * 0.1f)
            {
                currentEnemieBeh._timeToNextSpawn = currentEnemieBeh.timeToNextSpawn * 0.1f;
            }

            currentEnemieBeh._damage += changeStatValue;
            currentEnemieBeh._health += changeStatValue;
        }
        nextIncreaseDif = Time.time + currentDiffuclty.secBtwIncreaseDif + currentEnemie.GetComponent<EnemiesBehavior>()._timeToNextSpawn;
    }

    private void calcMaxChance()
    {
        for (int i = 0; i < enemiesLength/*нужно заменить потом*/; i++)
        {
            if (maxChance < currentDiffuclty.enemies[i].GetComponent<EnemiesBehavior>()._chanceToSpawn)
            {
                maxChance = currentDiffuclty.enemies[i].GetComponent<EnemiesBehavior>()._chanceToSpawn;
            }
        }
    }


    [System.Serializable]
    public class gameDifficulty
    {
        public Transform[] enemies;


        public int timeToFirstIncreaseEnemiesStats;
        public float secBtwIncreaseDif = 3f;

        //enemies changeble stats coef
        public int pointToSetNextDifficulty;
        
        public float startCoef = 0.75f;
        public float coefSpawn;
        
    }

}

