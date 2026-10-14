using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(LineRenderer) )]
public class RubberBand : MonoBehaviour
{
    public AudioSource source;
    
    static List <RubberBand>    RUBBER_BAND_LINES  = new List<RubberBand>();
    static private RubberBand S; // Singleton
    
    public float           easing = .2f;
    private LineRenderer    _line;
    static private bool     _RubberBandPulled = false;
    private GameObject      _RubberBandPivot;
    private Vector3         _pivot;
    private Vector3         _Midpoint;
    static private GameObject      _Projectile;

    void Start()
    {
        Transform _RubberBandPivotTrans = transform.Find("RubberBandPivot");
        _RubberBandPivot = _RubberBandPivotTrans.gameObject;

        _pivot = _RubberBandPivotTrans.position - this.transform.position; 

        _line = GetComponent<LineRenderer>();
        _line.SetPosition( 1, _pivot ); 

        _Midpoint = Vector3.Lerp( _line.GetPosition(0), _line.GetPosition(2), .5f );
        _RubberBandPivot.transform.position = _Midpoint;  
        S = this;
    }

    void FixedUpdate() {

        if (_RubberBandPulled) {
            _pivot = _Projectile.transform.position - this.transform.position;
            _line.SetPosition( 1, _pivot);    
            return;
        }

        Transform _RubberBandPivotTrans = transform.Find("RubberBandPivot");
        _pivot = Vector3.Lerp(_RubberBandPivotTrans.position, _pivot, easing);
        _line.SetPosition( 1, _pivot );        
    }

    static public void RubberBandPulled(GameObject Projectile) {
        _Projectile = Projectile;
        _RubberBandPulled = true;

    }

    static public void RubberBandReleased() {
        _RubberBandPulled = false;
        S.playRubberBandSound();
    }

    public void playRubberBandSound() {
        source.Play();
    }

}
