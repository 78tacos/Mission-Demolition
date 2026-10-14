using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameMode {
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S; // Singleton

    [Header("Inscribed")]
    public Text             uitLevel;   // UIText_Level
    public Text             uitShots;   // UIText_Shots
    public Vector3          castlePos;
    public GameObject[]     castles;

    [Header("Dynamic")]
    public int              level;
    public int              levelMax;
    public int              shotsTaken;
    public GameObject       castle;
    public GameMode         mode = GameMode.idle;
    public string           showing = "Show Slingshot"; // FollowCam mode

    void Start()
    {
        S = this;

        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel() {
        if (castle != null) {
            Destroy( castle );
        }

        Projectile.DESTROY_PROJECTILES();

        castle = Instantiate<GameObject>( castles[level] );
        castle.transform.position = castlePos;

        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;

        // Opening view should include both the slingshot and the castle.
        FollowCam.SWITCH_VIEW( FollowCam.eView.both );
    }

    void UpdateGUI() {
        uitLevel.text = "Level: "+(level+1)+" of "+levelMax;
        uitShots.text = "Shots Taken: "+shotsTaken;
    }

    void Update()
    {
        UpdateGUI();

        if ( (mode == GameMode.playing) && Goal.goalMet ) {
            // Leave playing mode so the level-end path only runs once.
            mode = GameMode.levelEnd;

            FollowCam.SWITCH_VIEW( FollowCam.eView.both );

            // Hold on the wide shot before advancing.
            Invoke("NextLevel", 2f);
        }
    }

    void NextLevel() {
        level++;
        if (level == levelMax)
        {
            SceneManager.LoadScene("_GameOver");
        }
        else
        {
            StartLevel();
        }
    }

    // Slingshot calls this when a shot is released.
    static public void SHOT_FIRED() {
        S.shotsTaken++;
    }

    // Current castle, so the camera can frame it.
    static public GameObject GET_CASTLE() {
        return S.castle;
    }
}
