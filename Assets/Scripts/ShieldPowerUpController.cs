using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerUpController : MonoBehaviour
{
    [SerializeField] private int chargeAmount = 10;

    // called when player collides with a shield power up
    private void OnCollisionEnter(Collision collision)
    {
        // check that the collision is made between the player and the power up
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "ShieldActive") 
        {
            var shield = collision.transform.GetComponentInChildren<ShieldManager>();

            shield.ChargeShield(chargeAmount);

            Destroy(gameObject);
        }
    }
}
