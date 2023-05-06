using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadUnitController : MonoBehaviour
{
    [SerializeField] private int damageAmount = 50;
    [SerializeField] private string tagToDamage = "Player";
    // Start is called before the first frame update
    void Start()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == this.tagToDamage)
        {
            // Damage object with relevant tag. Note that this assumes the 
            // HealthManager component is attached to the respective object.
            var healthManager = collision.gameObject.GetComponent<HealthManager>();
            healthManager.ApplyDamage(this.damageAmount);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
