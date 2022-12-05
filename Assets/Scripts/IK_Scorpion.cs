using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OctopusController;

public class IK_Scorpion : MonoBehaviour
{
    MyScorpionController _myController= new MyScorpionController();

    public IK_tentacles _myOctopus;

    [Header("Body")]
    public float animTime;
    public float animDuration = 5;
    bool animPlaying = false;
    public Transform Body;
    public Transform StartPos;
    public Transform EndPos;

    [Header("Tail")]
    public Transform tailTarget;
    public Transform tail;
    //private Quaternion[] _originalTaillRotations;

    [Header("Legs")]
    public Transform[] legs;
    public Transform[] legTargets;
    public Transform[] futureLegBases;

    public Slider forceSlider;
    private bool _sliderGoUp=true;
    public float _slideSpeed = 7f;

    //private bool _firstTimePressed = true;
    //private Vector3 _scorpionOriginalPosition;
    //private Vector3 _ballOriginalPosition;

    // Start is called before the first frame update
    void Start()
    {
        /*_originalTaillRotations = new Quaternion[3];
        _originalTaillRotations[0] = tail.rotation;
        _originalTaillRotations[1] = tail.rotation;
        _originalTaillRotations[2] = tail.rotation;*/
        _myController.InitLegs(legs,futureLegBases,legTargets);
        _myController.InitTail(tail);
        //_scorpionOriginalPosition = this.transform.position;
        //_ballOriginalPosition = tailTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(animPlaying)
            animTime += Time.deltaTime;

        NotifyTailTarget();
        
        if (Input.GetKey(KeyCode.Space))
        {
            /*if (_firstTimePressed)
            {
                this.transform.position = _scorpionOriginalPosition;
                tailTarget.position = _ballOriginalPosition;
                _firstTimePressed = false;
            }*/

            if (forceSlider.value >= forceSlider.maxValue) _sliderGoUp = false;
            if (forceSlider.value <= forceSlider.minValue) _sliderGoUp = true;

            if(_sliderGoUp) forceSlider.value += Time.deltaTime * _slideSpeed;
            else forceSlider.value -= Time.deltaTime * _slideSpeed;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            StartWalk();
            //_firstTimePressed = true;
        }

        if (animTime < animDuration)
        {
            Body.position = Vector3.Lerp(StartPos.position, EndPos.position, animTime / animDuration);
        }
        else if (animTime >= animDuration && animPlaying)
        {
            Body.position = EndPos.position;
            animPlaying = false;
        }

        _myController.UpdateIK();
    }
    
    //Function to send the tail target transform to the dll
    public void NotifyTailTarget()
    {
        _myController.NotifyTailTarget(tailTarget);
    }

    //Trigger Function to start the walk animation
    public void NotifyStartWalk()
    {

        _myController.NotifyStartWalk();
    }

    public void StartWalk()
    {
        NotifyStartWalk();
        animTime = 0;
        animPlaying = true;
    }
}
