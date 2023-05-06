using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShieldManager : MonoBehaviour
{
    [SerializeField] private GameObject shield;
    [SerializeField] private AudioSource shieldActivated;

    [SerializeField] private int startingStatus = 0;
    [SerializeField] [Range(0, 10)] private int chargeRate = 5;
    
    [SerializeField] private AudioSource shieldReady;
    private bool soundPlayed = false;

    [SerializeField] private Image chargeBar;

    private int FULL_CHARGE = 100;

    private float currentStatus;

    // Start is called before the first frame update
    void Start()
    {
        // reset the current status to starting values
        Reset();
    }

    // Update is called once per frame
    void Update()
    {

        if (IsFull()) // if shield is fully charged wait until player presses the spacebar to activate shield
        {
            if (!soundPlayed)
            {
                shieldReady.Play();
                soundPlayed = true;
            }
            
            if (Input.GetKeyDown(KeyCode.Space) && transform.parent.tag == "Player")
            {
                ActivateShield();
                soundPlayed = false;
            }
        }

        if (!IsFull()) // if shield is not fully charged the power up
        {
            ChargeShield();
            chargeBar.fillAmount = currentStatus / FULL_CHARGE;
        }
    }

    public bool IsFull()
    {
        if (currentStatus >= FULL_CHARGE)
        {
            currentStatus = FULL_CHARGE;
            return true;
        }

        return false;
    }

    private void ActivateShield()
    {
        Instantiate(shield, transform.parent);
        transform.parent.tag = "ShieldActive";
        shieldActivated.Play(); // play power up sound
        Reset();
    }

    private void ChargeShield() // automatically charges shield using rate and time
    {
        currentStatus += chargeRate * Time.deltaTime;
    }

    public void ChargeShield(int amount) // adds a certain amount of
    {
        currentStatus += amount;
    }

    public void InstantFill()
    {
        currentStatus = FULL_CHARGE;
        chargeBar.fillAmount = 1;
    }

    public void Reset()
    {
        currentStatus = this.startingStatus;
        chargeBar.fillAmount = startingStatus;
    }
}
