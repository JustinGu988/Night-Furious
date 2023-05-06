using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfOffScreen : MonoBehaviour
{
    /* void OnBecameInvisible() {
        Destroy(gameObject);
    } */

    void Update() {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 0f, 120f));
        if (screenPosition.y > Screen.height || screenPosition.y < 0)
        Destroy(this.gameObject);
    }
}
