using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OctopusController;
using System.Linq;

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
    private Transform[] _tailBones;
    private Quaternion[] _originalTaillRotations;

    [Header("Legs")]
    public Transform[] legs;
    public Transform[] legTargets;
    public Transform[] futureLegBases;

    public Slider forceSlider;
    private bool _sliderGoUp=true;
    public float _slideSpeed = 20f;

    /***************************************Iks**************************************/
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
        _originalTaillRotations = new Quaternion[5];
        _tailBones = new Transform[5];
        _tailBones[0] = tail;
        _tailBones[1] = _tailBones[0].GetChild(1);
        _tailBones[2] = _tailBones[1].GetChild(1);
        _tailBones[3] = _tailBones[2].GetChild(1);
        _tailBones[4] = _tailBones[3].GetChild(1);

        for (int i = 0; i < _tailBones.Length; i++) _originalTaillRotations[i] = _tailBones[i].rotation;
        /*_originalTaillRotations[0] = tail.rotation;
        _originalTaillRotations[1] = tail.GetChild(1).rotation;
        _originalTaillRotations[2] = tail.GetChild(1).GetChild(1).rotation;
        _originalTaillRotations[3] = tail.GetChild(1).GetChild(1).GetChild(1).rotation;
        _originalTaillRotations[2] = tail.GetChild(1).GetChild(1).GetChild(1).rotation;*/
        InitLegs(legs,futureLegBases,legTargets);
        InitTail(tail);
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
            _tailTarget = null;
            for (int i = 0; i < _tailBones.Length; i++) _tailBones[i].rotation = _originalTaillRotations[i];
            InitTail(tail);
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

        UpdateIK();
    }
    
    //Function to send the tail target transform to the dll
    public void NotifyTailTarget()
    {
        NotifyTailTargetIK(tailTarget);
    }

    //Trigger Function to start the walk animation
    public void NotifyStartWalk()
    {
        NotifyStartWalkIK();
    }

    public void StartWalk()
    {
        NotifyStartWalk();
        animTime = 0;
        animPlaying = true;
    }

    /*****************************IKs**********************************/
    public void InitLegs(Transform[] LegRoots, Transform[] LegFutureBases, Transform[] LegTargets)
    {
        _legs = new MyTentacleController[LegRoots.Length];
        _legRoots = new Transform[LegRoots.Length];
        _legTargets = new Transform[LegTargets.Length];
        _legFutureBases = new Transform[LegFutureBases.Length];
        _copy = new List<Vector3[]>();
        _distances = new List<float[]>();

        //Legs init
        for (int i = 0; i < LegRoots.Length; i++)
        {
            _legs[i] = new MyTentacleController();
            _legs[i].LoadTentacleJoints(LegRoots[i], TentacleMode.LEG);
            _legRoots[i] = LegRoots[i];
            _legTargets[i] = LegTargets[i];
            _legFutureBases[i] = LegFutureBases[i];
            _copy.Add(new Vector3[_legs[i].Bones.Length]);
            _distances.Add(new float[_legs[i].Bones.Length]);

            for (int x = 0; x < _legs[i].Bones.Length; x++)
            {
                if (x < _legs[i].Bones.Length - 1)
                {
                    _distances[i][x] = (_legs[i].Bones[x + 1].position - _legs[i].Bones[x].position).magnitude;
                }
                else
                {
                    _distances[i][x] = 0f;
                }
            }
        }
    }

    public void InitTail(Transform TailBase)
    {
        _tail = new MyTentacleController();
        _tail.LoadTentacleJoints(TailBase, TentacleMode.TAIL);
        _tailAngles = new float[_tail.Bones.Length];
        _tailAxis = new Vector3[_tail.Bones.Length];
        _tailOffset = new Vector3[_tail.Bones.Length];

        _tailAxis[0] = new Vector3(0, 0, 1);
        _tailAxis[1] = new Vector3(1, 0, 0);
        _tailAxis[2] = new Vector3(1, 0, 0);
        _tailAxis[3] = new Vector3(1, 0, 0);
        _tailAxis[4] = new Vector3(1, 0, 0);
        _tailAxis[5] = new Vector3(1, 0, 0);

        for (int i = 0; i < _tailOffset.Length; i++)
        {
            if (i != 0) _tailOffset[i] = Quaternion.Inverse(_tail.Bones[i - 1].rotation) * (_tail.Bones[i].position - _tail.Bones[i - 1].position);
            else _tailOffset[i] = _tail.Bones[i].position;
        }
    }

    //Check when to start the animation towards target and implement Gradient Descent method to move the joints.
    public void NotifyTailTargetIK(Transform target)
    {
        if (Vector3.Distance(_tail.Bones[_tail.Bones.Length - 1].position, target.position) < _distanceThreshold)
        {
            _tailTarget = target;
        }
    }

    //Notifies the start of the walking animation
    public void NotifyStartWalkIK()
    {
        _startWalk = true;
    }

    //Create the apropiate animations and update the IK from the legs and tail
    public void UpdateIK()
    {
        if(_tailTarget!=null) updateTail();
        if (_startWalk)
        {
            updateLegs();
            updateLegPos();
        }
    }

    private void updateLegPos()
    {
        for (int i = 0; i < _legs.Length; i++)
        {
            if ((_legFutureBases[i].position - _legRoots[i].position).magnitude > _legThreshold)
            {
                _legRoots[i].position = _legFutureBases[i].position;
            }
        }

    }

    //Implement Gradient Descent method to move tail if necessary
    private void updateTail()
    {
        if (_tailTarget != null)
        {
            for (int i = 0; i < _tail.Bones.Length; i++)
            {
                float gradient = CalculateGradient(_tailTarget.position, _tailAngles, i, _deltaGradient);
                _tailAngles[i] -= _learningRate * gradient;
            }

            for (int i = 0; i < _tail.Bones.Length; i++)
            {
                if (_tailAxis[i].x == 1) _tail.Bones[i].localEulerAngles = new Vector3(_tailAngles[i], 0, 0);
                else if (_tailAxis[i].y == 1) _tail.Bones[i].localEulerAngles = new Vector3(0, _tailAngles[i], 0);
                else if (_tailAxis[i].z == 1) _tail.Bones[i].localEulerAngles = new Vector3(0, 0, _tailAngles[i]);
            }
        }
    }

    private float CalculateGradient(Vector3 target, float[] Solution, int i, float delta)
    {
        float gradient = 0;
        float angle = Solution[i];
        float p = DistanceFromTarget(target, Solution);
        Solution[i] += delta;
        float pDelta = DistanceFromTarget(target, Solution);
        gradient = (pDelta - p) / delta;
        Solution[i] = angle;
        return gradient;
    }

    private float DistanceFromTarget(Vector3 target, float[] Solution)
    {
        Vector3 point = ForwardKinematics(Solution);
        return Vector3.Distance(point, target);
    }

    public PositionRotation ForwardKinematics(float[] Solution)
    {
        Vector3 prevPoint = _tail.Bones[0].transform.position;

        Quaternion rotation = _tail.Bones[0].transform.parent.rotation;

        for (int i = 1; i < _tail.Bones.Length; i++)
        {
            rotation *= Quaternion.AngleAxis(Solution[i - 1], _tailAxis[i - 1]);
            Vector3 nextPoint = prevPoint + rotation * _tailOffset[i];
            Debug.DrawLine(prevPoint, nextPoint);

            prevPoint = nextPoint;
        }
        // The end of the effector
        return new PositionRotation(prevPoint, rotation);
    }

    //TODO: implement fabrik method to move legs 
    private void updateLegs()
    {
        for (int i = 0; i < _legs.Length; i++)
        {
            fabrik(_legs[i].Bones, _legTargets[i], _distances[i], _copy[i], _legFutureBases[i]);
        }

    }

    public void fabrik(Transform[] joints, Transform target, float[] distances, Vector3[] copy, Transform futurBase)
    {
        for (int i = 0; i < joints.Length; i++)
        {
            copy[i] = joints[i].position;
        }

        float targetRootDist = Vector3.Distance(copy[0], target.position);

        if (targetRootDist > distances.Sum())
        {
            for (int i = 0; i <= joints.Length - 2; i++)
            {
                joints[i].transform.position = copy[i];
            }
        }
        else
        {
            Vector3[] inversePositions = new Vector3[copy.Length];
            for (int i = (copy.Length - 1); i >= 0; i--)
            {
                if (i == copy.Length - 1)
                {
                    copy[i] = target.transform.position;
                    inversePositions[i] = target.transform.position;
                }
                else
                {
                    Vector3 posPrimaSiguiente = inversePositions[i + 1];
                    Vector3 posBaseActual = copy[i];
                    Vector3 direccion = (posBaseActual - posPrimaSiguiente).normalized;
                    float longitud = distances[i];
                    inversePositions[i] = posPrimaSiguiente + (direccion * longitud);
                }
            }
            for (int i = 0; i < inversePositions.Length; i++)
            {
                if (i == 0)
                {
                    copy[i] = joints[0].position;
                }
                else
                {
                    Vector3 posPrimaActual = inversePositions[i];
                    Vector3 posPrimaSegundaAnterior = copy[i - 1];
                    Vector3 direccion = (posPrimaActual - copy[i - 1]).normalized;
                    float longitud = distances[i - 1];
                    copy[i] = posPrimaSegundaAnterior + (direccion * longitud);
                }
            }
        }

        for (int i = 0; i < joints.Length - 1; i++)
        {
            Vector3 joint01 = (joints[i + 1].position - joints[i].position).normalized;
            Vector3 copy01 = (copy[i + 1] - copy[i]).normalized;

            Vector3 crossBones01 = Vector3.Cross(joint01, copy01).normalized;

            float angle01 = Mathf.Acos(Vector3.Dot(joint01, copy01)) * Mathf.Rad2Deg;

            if (angle01 > 1f)
            {
                joints[i].rotation = Quaternion.AngleAxis(angle01, crossBones01) * joints[i].rotation;
            }
        }

    }

}
