using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenerateRoad : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject roadUnitTemplate;
    [SerializeField] private int initialNumber;
    [SerializeField] private float leftRoadBound;
    [SerializeField] private float rightRoadBound;
    [SerializeField] private float roadOffset;
    [SerializeField] private float y = 0f;
    [SerializeField] private float xNotRotate = -20.54f;
    [SerializeField] private float xRotate = 16.9f;
    [SerializeField] private float zNotRotate = 115f;
    [SerializeField] private float zRotate = 75f;
    [SerializeField] private GameObject[] obstacleTemplates;
    [SerializeField] private float maxObstacleSpawnRate = 3;
    [SerializeField] private float minObstacleSpawnRate = 0.05f;
    [SerializeField] private float incObstacleSpawnRate = 0.05f;
    // Division is in "player width"
    [SerializeField] private int carDivision = 3;
    [SerializeField] private int roadWidthDivision = 8;
    [SerializeField] private int roadLengthDivision = 38;
    [SerializeField] private GameObject[] powerupTemplates;
    [SerializeField] private float powerupSpawnRate = 1;
    private bool rotateRoad = true;
    private float zNextRoad = 0f;
    private float zNextObs = 0f;
    private float obsSpawnRate;
    //private bool firstRoad = true;
    private float roadLength;
    private float roadWidth;
    private float divisionLength;
    private float divisionWidth;
    private int prevPath; // values of -1, 0, or 1 to denote where next
    private int prevDivision = 0; // 0--1--2--3-- for width division 4
    private Bounds prevBounds;
    void Start()
    {
        roadLength = (zNotRotate + zRotate) / 2f;
        roadWidth = Mathf.Abs(leftRoadBound - rightRoadBound);
        divisionLength = roadLength / (float)roadLengthDivision;
        roadWidthDivision *= carDivision;
        divisionWidth = roadWidth / (float)roadWidthDivision;
        prevDivision = roadWidthDivision / 2;
        prevPath = 0;
        obsSpawnRate = minObstacleSpawnRate;

        for (int i = 0; i < initialNumber; i++)
        {
            GenerateUnit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        var roadCount = GameObject.FindGameObjectsWithTag("RoadUnit");
        if (roadCount.Length < initialNumber)
        {
            GenerateUnit();
        }
    }

    void GenerateUnit()
    {
        // Road generation part
        var road = Instantiate(this.roadUnitTemplate);
        var roadTransform = road.transform;
        var roadStart = zNextRoad + roadOffset;
        //Debug.Log(road.GetComponent<Renderer>().bounds);
        if (rotateRoad)
        {
            zNextRoad += zRotate;
            roadTransform.localPosition = new Vector3(xRotate, y, roadOffset + zNextRoad);
            roadTransform.localRotation = Quaternion.AngleAxis(180, Vector3.up);
            rotateRoad = false;
        }
        else
        {
            zNextRoad += zNotRotate;
            roadTransform.localPosition = new Vector3(xNotRotate, y, roadOffset + zNextRoad);
            rotateRoad = true;
        }

        // Obstacle generation part

        var obStart = zNextObs + roadOffset;
        zNextObs += roadLength;

        obsSpawnRate = obsSpawnRate > maxObstacleSpawnRate ? maxObstacleSpawnRate : obsSpawnRate + incObstacleSpawnRate;

        // Create road grid: -1 means empty, 0 means car path, 1 and above means some unique obstacle
        int[,] roadGrid = new int[roadLengthDivision, roadWidthDivision];
        for (int i = 0; i < roadLengthDivision; i++)
        {
            for (int j = 0; j < roadWidthDivision; j++)
            {
                roadGrid[i, j] = -1;
            }
        }

        // Leave at least one path that the car can viably pass through
        var z = obStart;
        int halfCarDiv = carDivision / 2;
        for (int i = 0; i < roadLengthDivision; i++)
        {
            prevPath = PickNextPath(prevPath);
            var newDiv = prevDivision + prevPath;
            prevDivision = newDiv >= roadWidthDivision - halfCarDiv ? roadWidthDivision - halfCarDiv - 1 : newDiv <= 0 ? 1 : newDiv;
            //var xCenter = prevDivision * divisionWidth + divisionWidth / 2f;
            z += divisionLength;
            roadGrid[i, prevDivision] = 0;
            for (int j = 1; j <= halfCarDiv; j++)
            {
                roadGrid[i, prevDivision + j] = 0;
                roadGrid[i, prevDivision - j] = 0;
            }
            if (carDivision % 2 == 0)
            {
                roadGrid[i, prevDivision - halfCarDiv] = -1;
            }
            // For now, spawn power up once at every road unit
            if (i == 0)
            {
                var powerup = Instantiate(this.powerupTemplates[UnityEngine.Random.Range(0, powerupTemplates.Length)]);
                var x = leftRoadBound + prevDivision * divisionWidth + divisionWidth / 2f;
                powerup.transform.position = new Vector3(x, y + 1f, z);
            }
        }

        // Obstacle spawning part
        for (int i = 1; i <= obsSpawnRate; i++)
        {
            // Gets a random object and its relevant boundings
            GameObject obstacle = Instantiate(this.obstacleTemplates[UnityEngine.Random.Range(0, obstacleTemplates.Length)]);
            obstacle.GetComponent<ObstacleController>().MakeBounds();
            var obsBounds = obstacle.GetComponent<Renderer>().bounds;
            //Debug.Log(obsBounds + " vs " + divisionWidth * carDivision);

            // Gets random position on grid
            int obsLengthDiv = Mathf.CeilToInt(obsBounds.size.z / (float)divisionLength);
            int obsWidthDiv = Mathf.CeilToInt(obsBounds.size.x / (float)divisionWidth);
            int halfObsLengthDiv = obsLengthDiv / 2;
            int halfObsWidthDiv = obsWidthDiv / 2;
            //Debug.Log(obsLengthDiv + " vs " + obsWidthDiv + " vs " + halfObsLengthDiv + " vs " + halfObsWidthDiv);
            int lengthDivLoc = UnityEngine.Random.Range(halfObsLengthDiv, roadLengthDivision - halfObsLengthDiv);
            int widthDivLoc = UnityEngine.Random.Range(halfObsWidthDiv, roadWidthDivision - halfObsWidthDiv);

            // Checks that random position is actually not occupied
            var taken = false;
            for (int j = lengthDivLoc - halfObsLengthDiv; j < lengthDivLoc + halfObsLengthDiv; j++)
            {
                for (int k = widthDivLoc - halfObsWidthDiv; k < widthDivLoc + halfObsWidthDiv; k++)
                {
                    // If it is, ciaoy bye bye my guy
                    // Because I do not want another while loop in here again
                    // It was really bad the last time
                    // Don't do it, unless you can guarantee that the object WILL fit somehow
                    if (roadGrid[j, k] != -1)
                    {
                        taken = true;
                        Destroy(obstacle);
                        break;
                    }
                }
            }

            // If the space is available, marks it on grid and moves the obstacle
            if (!taken)
            {
                for (int j = lengthDivLoc - halfObsLengthDiv; j < lengthDivLoc + halfObsLengthDiv; j++)
                {
                    for (int k = widthDivLoc - halfObsWidthDiv; k < widthDivLoc + halfObsWidthDiv; k++)
                    {
                        roadGrid[j, k] = i;
                        //Debug.Log(roadGrid[j, k]);
                    }
                }
                var xNew = leftRoadBound + (widthDivLoc + obsWidthDiv / 2f) * divisionWidth;
                var zNew = z + (lengthDivLoc + obsLengthDiv / 2f) * divisionLength;
                obstacle.transform.position = new Vector3(xNew, y + 1f, zNew);
                //Debug.Log(obstacle.transform.position);
            }
        }

        /* for (int i = 0; i < roadLengthDivision; i++)
        {
            int[] row = new int[roadWidthDivision];
            for (int j = 0; j < roadWidthDivision; j++)
            {
                row[j] = roadGrid[i, j];
            }
            Debug.Log(String.Join(", ", row));
        } */
    }

    // Picks next direction car path should be
    private int PickNextPath(int prev)
    {
        int[] possiblePath = { -1, 0, 1 };
        int startIndex = prev == 1 ? 1 : 0;
        int endIndex = possiblePath.Length - (prev == -1 ? 1 : 0);
        return possiblePath[UnityEngine.Random.Range(startIndex, endIndex)];
    }
}
