using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int startingHealth = 100;

    public Image healthBar;

    // Using an event to maximise component re-use. Any other component can 
    // listen to this event to do arbitrary actions when this game object dies.

    private int _currentHealth;

    private int CurrentHealth
    {
        get => this._currentHealth;
        set
        {
            // Using a C# property to ensure the onHealthChanged event is
            // consistently fired when the health changes, and also to check if
            // the object has died (<= 0 health). It's not really different to
            // the concept of a "setter" as per OOP good practice, however, we
            // can still treat it like an integer variable (add, subtract, etc).
            this._currentHealth = value;
            var frac = this._currentHealth / (float)this.startingHealth;
            healthBar.fillAmount = frac;
            if (CurrentHealth <= 0) // Did we die?
            {
                // Destroy ourselves.
                //Destroy(gameObject);
                var renderers = GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers) {
                    renderer.enabled = false;
                }
                GetComponent<Rigidbody>().drag = 500;
            }
        }
    }

    private void Start()
    {
        ResetHealthToStarting();
    }

    public void ResetHealthToStarting()
    {
        CurrentHealth = this.startingHealth;
    }

    public void ApplyDamage(int damage)
    {
        CurrentHealth -= damage;
    }

    public int GetCurrentHealth()
    {
        return _currentHealth;
    }
}
