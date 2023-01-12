using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OctopusController;

namespace OctopusController
{
    internal class MyTentacleController

    //MAINTAIN THIS CLASS AS INTERNAL
    {

        TentacleMode tentacleMode;
        Transform[] _bones;
        Transform _endEffectorSphere;

        public Transform[] Bones { get => _bones; }

        //Exercise 1.
        public Transform[] LoadTentacleJoints(Transform root, TentacleMode mode)
        {
            //TODO: add here whatever is needed to find the bones forming the tentacle for all modes
            //you may want to use a list, and then convert it to an array and save it into _bones
            tentacleMode = mode;

            switch (tentacleMode)
            {
                case TentacleMode.LEG:

                    _bones = new Transform[4];

                    _bones[0] = root.GetChild(0);

                    _bones[1] = _bones[0].GetChild(1);

                    _bones[2] = _bones[1].GetChild(1);

                    _bones[3] = _bones[2].GetChild(1);



                    //TODO: in _endEffectorsphere you keep a reference to the base of the leg

                    _endEffectorSphere = _bones[3];
                    break;
                case TentacleMode.TAIL:
                    _bones = new Transform[6];
                    _bones[0] = root;

                    for (int i = 1; _bones[i - 1].childCount > 0; i++)
                    {
                        _bones[i] = _bones[i - 1].GetChild(1);
                    }

                    _endEffectorSphere = _bones[_bones.Length - 1];

                    //TODO: in _endEffectorsphere you keep a reference to the red sphere 
                    break;
                case TentacleMode.TENTACLE:
                    //IMPLEMENTAR EL END EFECTOR
                    _bones = new Transform[53];

                    root = root.GetChild(0).GetChild(0);
                    _bones[0] = root;

                    for (int i = 1; _bones[i - 1].childCount > 0; i++)

                    {

                        _bones[i] = _bones[i - 1].GetChild(0);
                        //Debug.Log(_bones[i]); //FUNCIONA

                    };
                    //QUITAR ULTIMO ELEMENTO DEL BOUNDS[] 
                    _endEffectorSphere = _bones[_bones.Length - 2];

                    //TODO: in _endEffectorphere you  keep a reference to the sphere with a collider attached to the endEffector
                    break;
            }
            return Bones;
        }
    }
}


public class IK_tentacles : MonoBehaviour
{

    [SerializeField]
    Transform[] _tentaclesOriginal = new Transform[4];

    [SerializeField]
    Transform[] _randomTargets;

    MyOctopusController _myController = new MyOctopusController();

    [Header("Exercise 3")]
    [SerializeField, Range(0, 360)]
    float _twistMin;

    [SerializeField, Range(0, 360)]
    float _twistMax;

    [SerializeField, Range(0, 360)]
    float _swingMin;

    [SerializeField, Range(0, 360)]
    float _swingMax;

    [SerializeField]
    bool _updateTwistSwingLimits = false;

    [SerializeField]
    float TwistMin { set { _myController.TwistMin = value; } }


    /*****************Inverse Kinematics********************/
    MyTentacleController[] _tentacles = new MyTentacleController[4];

    Transform _currentRegion;
    Transform _target;

    Transform[] _randomTargets2;

    float _timer = 0f;
    float TIMER_ENDED = 3f;

    bool _region1b = false;
    bool _region2b = false;
    bool _region3b = false;
    bool _region4b = false;

    [SerializeField]
    float[] _theta, _sin, _cos;

    bool _done = false;

    [SerializeField]
    private int _mtries = 10;

    [SerializeField]
    private int _tries = 0;


    readonly float _epsilon = 0.1f;

    #region public methods

    /*******************************************************/

    public bool stopBall = true; //Bool que indica si tiene que parar la pelota o no

    #region public methods


    public void NotifyTarget(Transform target, Transform region)
    {
        NotifyTargetIK(target, region);

    }

    public void NotifyShoot()
    {
        NotifyShootIK();
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {

        _myController.TestLogging(gameObject.name);
        Init(_tentaclesOriginal, _randomTargets);

        _myController.TwistMax = _twistMax;
        _myController.TwistMin = _twistMin;
        _myController.SwingMax = _swingMax;
        _myController.SwingMin = _swingMin;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateTentacles();

        if (_updateTwistSwingLimits)
        {
            _myController.TwistMax = _twistMax;
            _myController.TwistMin = _twistMin;
            _myController.SwingMax = _swingMax;
            _myController.SwingMin = _swingMin;
            _updateTwistSwingLimits = false;
        }

    }

    /***********************************************************Inverse kinematics**************************************************************/
    public void Init(Transform[] tentacleRoots, Transform[] randomTargets)
    {

        _tentacles = new MyTentacleController[tentacleRoots.Length];
        _cos = new float[53];
        _theta = new float[53];
        _sin = new float[53];

        _randomTargets = new Transform[randomTargets.Length];

        for (int i = 0; i < randomTargets.Length; i++)
        {
            _randomTargets[i] = randomTargets[i];
        }


        for (int i = 0; i < tentacleRoots.Length; i++)
        {
            _tentacles[i] = new MyTentacleController();
            _tentacles[i].LoadTentacleJoints(tentacleRoots[i], TentacleMode.TENTACLE);
        }
    }

    public void NotifyTargetIK(Transform target, Transform region)
    {
        _currentRegion = region;
        _target = target;
    }

    public void NotifyShootIK()
    {

        Debug.Log("Shoot");

        if (_currentRegion.name == "region1")
        {
            _region1b = true;
        }
        else if (_currentRegion.name == "region2")
        {

            _region2b = true;
        }
        else if (_currentRegion.name == "region3")
        {
            _region3b = true;
        }
        else if (_currentRegion.name == "region4")
        {
            _region4b = true;
        }
    }


    public void UpdateTentacles()
    {
        if (!_done)
        {

            for (int t = 0; t < _tentacles.Length; t++)
            {

                if (_region1b == true && t == 0 && stopBall)
                {
                    ApplyCCD(t, _target);
                }
                else if (_region2b == true && t == 1 && stopBall)
                {
                    ApplyCCD(t, _target);
                }
                else if (_region3b == true && t == 2 && stopBall)
                {
                    ApplyCCD(t, _target);
                }
                else if (_region4b == true && t == 3 && stopBall)
                {
                    ApplyCCD(t, _target);
                }
                else
                {
                    ApplyCCD(t, _randomTargets[t]);
                }
            }

            if (_region1b || _region2b || _region3b || _region3b || _region4b) TimerReset();
        }

        for (int t = 0; t < _tentacles.Length; t++)
        {
            if (_region1b == true && t == 0 && stopBall)
            {
                ResetTentacle(t, _target);

            }
            else if (_region2b == true && t == 1 && stopBall)
            {
                ResetTentacle(t, _target);
            }
            else if (_region3b == true && t == 2 && stopBall)
            {
                ResetTentacle(t, _target);

            }
            else if (_region4b == true && t == 3 && stopBall)
            {
                ResetTentacle(t, _target);

            }
            else
            {
                ResetTentacle(t, _randomTargets[t]);
            }
        }
    }

    void TimerReset()
    {
        if (_timer >= TIMER_ENDED)
        {
            _region1b = false;
            _region2b = false;
            _region3b = false;
            _region4b = false;
            _timer = 0f;
            //stopBall = !stopBall;
        }
        else
        {
            _timer += Time.deltaTime;
        }



    }

    void ResetTentacle(int t, Transform target)
    {
        Vector3 distance;

        distance = target.transform.position - _tentacles[t].Bones[_tentacles[t].Bones.Length - 2].transform.position;
        if (distance.magnitude <= _epsilon)
        {
            _done = true;
        }
        else
        {
            _done = false;
        }
        if (_tentacles[t].Bones[_tentacles[t].Bones.Length - 2].transform.position != target.transform.position)
        {
            _tries = 0;
        }
    }

    void ApplyCCD(int numeroTentaculo, Transform targetPosT)
    {
        if (_tries <= _mtries)
        {
            for (int i = _tentacles[numeroTentaculo].Bones.Length - 2; i >= 0; i--)
            {
                // transform.LookAt(camera);
                Vector3 r1 = (_tentacles[numeroTentaculo].Bones[_tentacles[numeroTentaculo].Bones.Length - 2].transform.position - _tentacles[numeroTentaculo].Bones[i].transform.position).normalized;
                Vector3 r2 = (targetPosT.transform.position - _tentacles[numeroTentaculo].Bones[i].transform.position).normalized;

                if (r1.magnitude * r2.magnitude > 0.001f)
                {
                    _cos[i] = Vector3.Dot(r1, r2);
                    Vector3 crossR1R2 = Vector3.Cross(r1, r2);
                    _sin[i] = crossR1R2.magnitude;
                }

                Vector3 axis = Vector3.Cross(r1, r2).normalized;
                _theta[i] = Mathf.Acos(_cos[i]);
                if (_sin[i] < 0)
                {
                    _theta[i] = -_theta[i];

                }
                _theta[i] = (180 / Mathf.PI) * _theta[i];


                  if (_theta[i] > 0.1)
                 {

               

                // int twist = 0;



                //  _tentacles[numeroTentaculo].Bones[i].transform.Rotate(axis, _theta[i], Space.World);

                _tentacles[numeroTentaculo].Bones[i].transform.Rotate(axis, _theta[i], Space.World);

                //_tentacles[numeroTentaculo].Bones[i].transform.rotation.eulerAngles

                //  bones[boneIndex].rotation = Quaternion.Lerp(bones[boneIndex].rotation, targetRotation, weight); // aplicar rotación con peso de IK

                float lerpWeight = 0;  //MOVER CON LERP, SPOILER NOVA
                float timeToReachTarget = 5f;
                // for (int h = _tentacles[numeroTentaculo].Bones.Length - 2; h >= 0; h--)
                //  {
                //Vector3 newPosition = Vector3.Lerp(_tentacles[numeroTentaculo].Bones[numeroTentaculo].transform.position, targetPosT.transform.position, lerpWeight);
                //  Vector3 newPosition = Vector3.Lerp(_tentacles[numeroTentaculo].Bones[i].transform.position, targetPosT.transform.position, lerpWeight);
                //   _tentacles[numeroTentaculo].Bones[numeroTentaculo].transform.position = newPosition;
                //  lerpWeight += Time.deltaTime / timeToReachTarget;
                //  }
                 }





            }
            _tries++;
        }
        #endregion
        /*******************************************************************************************************************************************/
    }
}
