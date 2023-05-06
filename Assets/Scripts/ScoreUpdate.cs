using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUpdate : MonoBehaviour
{

    [SerializeField] private GameObject car;
    [SerializeField] private Text text;

    private float startDist;
    private float startTime;
    private int currentScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        text.text = "Score: 0";
        startTime = Time.time;
        startDist = car.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        calculateScore();
        text.text = "Score: " + currentScore;
    }

    private void calculateScore()
    {
        if (car.GetComponent<Rigidbody>().drag != 500)
        {
            var aliveTime = Time.time - startTime;
            var dist = car.transform.position.z - startDist;

            currentScore = Mathf.Max(0, (int)(dist - (2 * aliveTime)));
        }
    }
}
