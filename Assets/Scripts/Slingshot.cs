using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    static private Slingshot S; // We're trying singletons today

    // fields set in the Unity Inspector pane
    [Header("Inscribed")]
    //public GameObject       projectilePrefab;
    public float            velocityMult = 10f;
    public GameObject[]     projectilePrefabs;
    public GameObject       projLinePrefab;
    
    // fields set dynamically
    [Header("UI")]
    // If you use two UI Texts (label + value), assign the label here and the value in projectileNameText.
    public UnityEngine.UI.Text projectileLabelText;
    public UnityEngine.UI.Text projectileNameText;
    [Header("Dynamic")]
    public int              projectileNumber = 0;
    public int              projectileMax;
    public GameObject       launchPoint;
    public Vector3          launchPos;
    public GameObject       projectile;
    //public GameObject       rubberBand;
    public bool             aimingMode;

    void Awake() {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive( false );
        launchPos = launchPointTrans.position;
        projectileMax = projectilePrefabs.Length;
        S = this;
        // Set initial projectile name in UI if available
        if (projectilePrefabs.Length > 0) {
            string name = projectilePrefabs[projectileNumber].name;
            if (projectileLabelText != null && projectileNameText != null) {
                projectileLabelText.text = "Projectile:";
                projectileNameText.text = name;
            } else if (projectileNameText != null) {
                projectileNameText.text = "Projectile: " + name;
            }
        }
    }
    void OnMouseEnter() {
        //print( "Slingshot:OnMouseEnter()" );
        launchPoint.SetActive( true );
    }

    void OnMouseExit() {
        //print( "Slingshot:OnMouseExit()" );
        launchPoint.SetActive( false );
    }

    void OnMouseDown() {
        //print( "Slingshot:OnMouseDown()" );
        // The player has pressed the mouse button while over Slingshot
        aimingMode = true;
        // Instantiate a Projectile
        projectile = Instantiate( projectilePrefabs[projectileNumber] ) as GameObject;
        // Start it at the launchPoint
        projectile.transform.position = launchPos;
        // Set it to is Kinematic for now
        projectile.GetComponent<Rigidbody>().isKinematic = true;
        RubberBand.RubberBandPulled(projectile);
        //rubberBand = GameObject.GetComponentInChildren<RubberBand>();
        //rubberBand = GameObject.Find("RubberBand");
        //rubberBand.RubberBand.RubberBandPulled(projectile);
    }

    void Update() {
        // If Slingshot is not in aimingMode, don't run this code
        if(!aimingMode) return;

        // Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint( mousePos2D );

        // Find the delta from the launchPos to the mousePos3D
        Vector3 mouseDelta = mousePos3D - launchPos;
        // Limit mouseDelta to the radius of the Slingshot SphereCollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude) {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        // Move the projectile to this new postion
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        if ( Input.GetMouseButtonUp(0) ) {
            // The mouse has been released
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            // Switch to slingshot view immediately before setting POI
            FollowCam.SWITCH_VIEW( FollowCam.eView.slingshot );

            FollowCam.POI = projectile; // Set the _MainCamera POI
            // Add a ProjectileLine to the Projectile
            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();
            RubberBand.RubberBandReleased();
        }
    }

    public void switch_projectile() {
        projectileNumber++;
        if (projectileNumber == projectileMax) {
            projectileNumber = 0;
        }
        // Update the UI text to show the name of the current projectile
        if (projectilePrefabs.Length > 0) {
            string name = projectilePrefabs[projectileNumber].name;
            if (projectileLabelText != null && projectileNameText != null) {
                projectileLabelText.text = "Projectile:";
                projectileNameText.text = name;
            } else if (projectileNameText != null) {
                projectileNameText.text = "Projectile: " + name;
            }
        }
    }

    static public void SWITCH_PROJECTILE() {
        S.switch_projectile();
    }
}
