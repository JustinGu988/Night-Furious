using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakingScript : MonoBehaviour
{
    [SerializeField] private float upThresholdDistance;
    [SerializeField] private float downThresholdDistance;
    [SerializeField] private float speed;

    private int _dir = 1; // Either 1 or -1 depending on the movement direction

    void Start()
    {
        
    }

    private void Update()
    {
        transform.localPosition +=
            Vector3.up * (Time.deltaTime * this.speed * this._dir);
        
        if (transform.localPosition.y > this.downThresholdDistance)
            this._dir = -1;
        else if (transform.localPosition.y < this.upThresholdDistance) 
            this._dir = 1;

    }
    
}
