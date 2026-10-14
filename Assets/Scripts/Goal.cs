using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent ( typeof(Renderer) )]
public class Goal : MonoBehaviour
{   
    // MissionDemolition polls this to know the castle is cleared.
    static public bool goalMet = false;

    void OnTriggerEnter( Collider other ) {
        Projectile proj = other.GetComponent<Projectile>();
        if ( proj != null ) {
            Goal.goalMet = true;
            // Raise opacity so the goal reads as completed.
            Material mat = GetComponent<Renderer>().material;
            Color c = mat.color;
            c.a = 0.75f;
            mat.color = c;
        }
    }
    
}
