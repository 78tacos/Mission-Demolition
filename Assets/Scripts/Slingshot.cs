using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    static private Slingshot S; // Singleton

    [Header("Inscribed")]
    public float            velocityMult = 10f;
    public GameObject[]     projectilePrefabs;
    public GameObject       projLinePrefab;
    
    [Header("UI")]
    // Label is optional. If it is unset, projectileNameText includes the prefix.
    public UnityEngine.UI.Text projectileLabelText;
    public UnityEngine.UI.Text projectileNameText;
    [Header("Dynamic")]
    public int              projectileNumber = 0;
    public int              projectileMax;
    public GameObject       launchPoint;
    public Vector3          launchPos;
    public GameObject       projectile;
    public bool             aimingMode;

    void Awake() {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive( false );
        launchPos = launchPointTrans.position;
        projectileMax = projectilePrefabs.Length;
        S = this;
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
        launchPoint.SetActive( true );
    }

    void OnMouseExit() {
        launchPoint.SetActive( false );
    }

    void OnMouseDown() {
        aimingMode = true;
        projectile = Instantiate( projectilePrefabs[projectileNumber] ) as GameObject;
        projectile.transform.position = launchPos;
        // Stay kinematic while the player is still aiming.
        projectile.GetComponent<Rigidbody>().isKinematic = true;
        RubberBand.RubberBandPulled(projectile);
    }

    void Update() {
        if(!aimingMode) return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint( mousePos2D );

        Vector3 mouseDelta = mousePos3D - launchPos;
        // Pull is limited to the slingshot sphere's radius.
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude) {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        if ( Input.GetMouseButtonUp(0) ) {
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            // Switch view before assigning POI so this frame uses the slingshot framing.
            FollowCam.SWITCH_VIEW( FollowCam.eView.slingshot );

            FollowCam.POI = projectile;
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
