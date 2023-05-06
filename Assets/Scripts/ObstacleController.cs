using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class ObstacleController : MonoBehaviour
{
    [SerializeField] private int damageAmount = 50;
    [SerializeField] private string tagToDamage = "Player";
    [SerializeField] private string tagToShield = "ShieldActive";
    /* private bool hitPlayer; */
    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<Rigidbody>().isKinematic = false;
        // hitPlayer = false;

        // Mesh filter for combined object group but I can't make it work 
        /*  var filter = GetComponent<MeshFilter>();

         if (filter == null)
         {
             //Debug.Log("mesh from " + name);
             MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
             CombineInstance[] combine = new CombineInstance[meshFilters.Length];

             int i = 0;
             while (i < meshFilters.Length)
             {
                 combine[i].mesh = meshFilters[i].sharedMesh;
                 combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                 meshFilters[i].gameObject.SetActive(false);

                 i++;
             }
             gameObject.AddComponent<MeshFilter>();
             transform.GetComponent<MeshFilter>().mesh = new Mesh();
             transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
             transform.gameObject.SetActive(true);
             //Debug.Log("2 size of " + name + ":" + GetComponent<Renderer>().localBounds.size.x);
         } */
    }

    public void MakeBounds()
    {
        var filter = GetComponent<MeshFilter>();
        var combinedBounds = GetComponent<Renderer>().bounds;

        if (filter == null)
        {
            //Debug.Log("mesh from " + name);
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            CombineInstance[] combine = new CombineInstance[meshRenderers.Length];

            foreach (MeshRenderer render in meshRenderers)
            {
                combinedBounds.Encapsulate(render.bounds);
            }

            transform.GetComponent<Renderer>().bounds = combinedBounds;

            //Debug.Log("2 size of " + name + ": " + GetComponent<Renderer>().bounds);
            //Debug.Log("3 size of " + name + ": " + combinedBounds);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == tagToShield || collision.gameObject.tag == tag)
        {
            Destroy(gameObject);
        }
        /* if (!hitPlayer && collision.gameObject.tag == "Player") {
            transform.GetComponent<Rigidbody>().isKinematic = true;
            hitPlayer = true;
        } */
        else if (collision.gameObject.tag == this.tagToDamage)
        {
            // Damage object with relevant tag. Note that this assumes the 
            // HealthManager component is attached to the respective object.
            var healthManager = collision.gameObject.GetComponent<HealthManager>();
            healthManager.ApplyDamage(this.damageAmount);

            // Destroy self.
            Destroy(gameObject);
        }
    }

    /* void OnCollisionExit(Collision collision) {
        if (hitPlayer && collision.gameObject.tag == "Player") {
            transform.GetComponent<Rigidbody>().isKinematic = false;
            hitPlayer = false;
        }
    } */

    // Update is called once per frame
    void Update()
    {

    }
}
