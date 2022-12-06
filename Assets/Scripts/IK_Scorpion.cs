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
    private Quaternion[] _originalTaillRotations;

    [Header("Legs")]
    public Transform[] legs;
    public Transform[] legTargets;
    public Transform[] futureLegBases;

    public Slider forceSlider;
    private bool _sliderGoUp=true;
    public float _slideSpeed = 7f;

    //Iks
    //TAIL
    Transform _tailTarget;
    MyTentacleController _tail;
    float[] _tailAngles = null;
    Vector3[] _tailAxis = null;
    Vector3[] _tailOffset = null;
    private float _deltaGradient = 0.1f; // Used to simulate gradient (degrees)
    private float _learningRate = 3.0f; // How much we move depending on the gradient
    private float _distanceThreshold = 5.0f;

    //LEGS
    Transform[] _legTargets = null;
    Transform[] _legRoots = null;
    Transform[] _legFutureBases = null;
    MyTentacleController[] _legs = new MyTentacleController[6];
    bool _startWalk;
    private List<Vector3[]> _copy;
    private List<float[]> _distances;
    private float _legThreshold = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        /*_originalTaillRotations = new Quaternion[5];
        _originalTaillRotations[0] = tail.rotation;
        _originalTaillRotations[1] = tail.GetChild(1).rotation;
        _originalTaillRotations[2] = tail.GetChild(1).GetChild(1).rotation;
        _originalTaillRotations[3] = tail.GetChild(1).GetChild(1).GetChild(1).rotation;
        _originalTaillRotations[2] = tail.GetChild(1).GetChild(1).GetChild(1).rotation;*/
        _myController.InitLegs(legs,futureLegBases,legTargets);
        _myController.InitTail(tail);
    }

    // Update is called once per frame
    void Update()
    {
        if(animPlaying)
            animTime += Time.deltaTime;

        NotifyTailTarget();
        
        if (Input.GetKey(KeyCode.Space))
        {
            if (forceSlider.value >= forceSlider.maxValue) _sliderGoUp = false;
            if (forceSlider.value <= forceSlider.minValue) _sliderGoUp = true;

            if(_sliderGoUp) forceSlider.value += Time.deltaTime * _slideSpeed;
            else forceSlider.value -= Time.deltaTime * _slideSpeed;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            StartWalk();
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
