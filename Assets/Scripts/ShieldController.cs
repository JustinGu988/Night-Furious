using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    // shield duration
    [SerializeField] [Range(0, 10)] private float lifetime = 10.0f; // how long the shield lasts for
    [SerializeField] [Range(0, 10)] private float rate = 1.0f; // how quickly the shield life decreases

    private float currentLife;

    // set the current life of shield when instantiated

    private void Awake()
    {
        currentLife = lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        Material mat = GetComponent<Renderer>().material;
        if (currentLife <= 0)
        {
            mat.SetFloat("_Alpha", 0);
            transform.parent.tag = "Player";
            Destroy(gameObject);

        }
        else
        {
            var currentAlpha = mat.GetFloat("_Alpha");
            if (currentAlpha > 0)
            {
                var newAlpha = currentLife/lifetime;
                print(newAlpha);
                mat.SetFloat("_Alpha", newAlpha);
            }

            currentLife -= rate * Time.deltaTime;
        }
    }
}
