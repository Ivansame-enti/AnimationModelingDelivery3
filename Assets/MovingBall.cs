using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovingBall : MonoBehaviour
{
    private float GRAVITY = -9.81f;
    private float MASS = 1f;
    [SerializeField]
    IK_tentacles _myOctopus;
    [SerializeField]
    IK_Scorpion _myScorpion;

    //movement speed in units per second
    [Range(-1.0f, 1.0f)]
    [SerializeField]
    private float _movementSpeed = 10f;

    public Transform ballTarget;
    public bool shootBall = false;
    public float ballSpeed=10f;
    public Vector3 movementEulerSpeed;
    private Vector3 _movementEulerSpeedSimulation, _movementEulerSpeedSimulationMagnus;
    private Vector3 _acceleration;
    private Vector3 ballDirection;
    private Vector3 ballDirectionMagnus;

    private float _timer;
    private float _stopForce=2;

    private Vector3 magnusForce;
    Vector3 angularVelocity;
    Vector3 _magnusForce2;
    Vector3 final_force;

    public Slider forceSlider;
    public Slider magnusSlider;
    private float ballRadius = 1;
    private bool showArrows=false;
    public GameObject greyArrow;
    //public GameObject blueArrow;
    public GameObject greenArrow;
    public GameObject redArrow;
    public GameObject redArrowShoot;
    public GameObject redArrowMagnus;

    private float _simulationTime = 0.3f;
    private float _simulationIterations = 50f;
    public TextMeshProUGUI speedText;
    private Vector3 hitPoint, hitPoint2;
    public GameObject blueParticles, greyParticles;
    private GameObject greyParticleGO, blueParticleGO;
    private float lastForceValue, lastMagnusValue;

    // Start is called before the first frame update
    void Start()
    {
        lastForceValue = forceSlider.value;
        lastMagnusValue = magnusSlider.value;
        greyArrow.SetActive(false);
        //blueArrow.SetActive(false);
        redArrow.SetActive(false);
        redArrowShoot.SetActive(false);
        redArrowMagnus.SetActive(false);
        greenArrow.SetActive(false);

        _acceleration = new Vector3(0, GRAVITY * MASS, 0);
        _timer = 0;
        angularVelocity = new Vector3(1,1,1);
    }

    // Update is called once per frame
    void Update()
    {
        if(_myScorpion._tailTarget!=null) hitPoint = new Vector3(_myScorpion._tailTarget.position.x + Mathf.Lerp(-0.5f, +0.5f, Mathf.InverseLerp(magnusSlider.minValue, magnusSlider.maxValue, magnusSlider.value)), _myScorpion._tailTarget.position.y, _myScorpion._tailTarget.position.z);
        if (blueParticleGO != null) hitPoint2 = new Vector3(blueParticleGO.transform.position.x + Mathf.Lerp(-0.5f, +0.5f, Mathf.InverseLerp(magnusSlider.minValue, magnusSlider.maxValue, magnusSlider.value)), blueParticleGO.transform.position.y, blueParticleGO.transform.position.z);

        if (Input.GetKeyDown(KeyCode.I))
        {
            showArrows = !showArrows;
            _movementEulerSpeedSimulation = (ballTarget.position - this.transform.position).normalized * lastForceValue;
            _movementEulerSpeedSimulationMagnus = (ballTarget.position - this.transform.position).normalized * lastForceValue;
        }

        if (showArrows)
        {
            ShowRedArrow();
            ShowGreenArrow();
            ShowGreyArrow();

            //Instanciamos las particulas
            if (blueParticleGO == null) blueParticleGO = Instantiate(blueParticles, this.transform.position, Quaternion.identity) as GameObject;
            if (greyParticleGO == null) greyParticleGO = Instantiate(greyParticles, this.transform.position, Quaternion.identity) as GameObject;

            //Si se cambian los sliders se resetean las trayectorias de puntos
            if(lastForceValue != forceSlider.value)
            {
                if (greyParticleGO != null) Destroy(greyParticleGO);
                if (blueParticleGO != null) Destroy(blueParticleGO);
                _movementEulerSpeedSimulation = (ballTarget.position - this.transform.position).normalized * lastForceValue;
                _movementEulerSpeedSimulationMagnus = (ballTarget.position - this.transform.position).normalized * lastForceValue;
                lastForceValue = forceSlider.value;
            }

            if (lastMagnusValue != magnusSlider.value)
            {
                if (blueParticleGO != null) Destroy(blueParticleGO);
                _movementEulerSpeedSimulationMagnus = (ballTarget.position - this.transform.position).normalized * lastForceValue;
                lastMagnusValue = magnusSlider.value;
            }

            //Simulamos las particulas
            EulerStepGreyPoints();
            EulerStepBluePoints();

        } else
        {
            greyArrow.SetActive(false);
            redArrow.SetActive(false);
            redArrowShoot.SetActive(false);
            redArrowMagnus.SetActive(false);
            greenArrow.SetActive(false);
            if (blueParticleGO != null) Destroy(blueParticleGO);
            if (greyParticleGO != null) Destroy(greyParticleGO);
        }

        transform.rotation = Quaternion.identity;

        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");
        
        //Si mueves el target se resetea la trayectoria de puntos
        if((horizontalInput!= 0 || verticalInput != 0) && showArrows)
        {
            _movementEulerSpeedSimulation = (ballTarget.position - this.transform.position).normalized * lastForceValue;
            _movementEulerSpeedSimulationMagnus = (ballTarget.position - this.transform.position).normalized * lastForceValue;
            if (blueParticleGO != null) Destroy(blueParticleGO);
            if (greyParticleGO != null) Destroy(greyParticleGO);
        }

        ballTarget.position = ballTarget.position + new Vector3(-horizontalInput * _movementSpeed * Time.deltaTime, verticalInput * _movementSpeed * Time.deltaTime, 0); //Movemos el target en vez de la pelota

        if (shootBall)
        {
            EulerStep();
        }
    }

    private void EulerStep()
    {
        float magnusCoefficient = magnusSlider.value;
        Vector3 radiusVector = hitPoint - transform.position;  //CALCULAMOS EL VECTOR DEL CENTRO DE LA BOLA A DONDE HITEEMOS
        Vector3 rotationVelocity = Vector3.Cross(movementEulerSpeed, radiusVector) / (ballRadius * ballRadius*5);// CALCULAMOS LA VELOCIDAD DE ROTACION
        ballDirectionMagnus = Vector3.Cross(rotationVelocity, ballDirection.normalized);

        speedText.text = rotationVelocity.ToString();

        if (magnusCoefficient >= 0)
        {
            ballDirectionMagnus = ballDirectionMagnus * -1;
        }
        magnusForce = magnusCoefficient * ballDirectionMagnus;

        movementEulerSpeed = movementEulerSpeed + magnusForce + _acceleration * Time.deltaTime;
        transform.position = (transform.position + movementEulerSpeed * Time.deltaTime);
    }

    private void EulerStepGreyPoints()
    {
        _movementEulerSpeedSimulation = _movementEulerSpeedSimulation + _acceleration * Time.deltaTime;
        greyParticleGO.transform.position = (greyParticleGO.transform.position + _movementEulerSpeedSimulation * Time.deltaTime);
    }

    private void EulerStepBluePoints()
    {
        float magnusCoefficient = magnusSlider.value;
        
        Vector3 radiusVector = hitPoint2 - blueParticleGO.transform.position;  //CALCULAMOS EL VECTOR DEL CENTRO DE LA BOLA A DONDE HITEEMOS
        Vector3 rotationVelocity = Vector3.Cross(_movementEulerSpeedSimulationMagnus, radiusVector) / (ballRadius * ballRadius * 5);// CALCULAMOS LA VELOCIDAD DE ROTACION
        
        Vector3 ballDirectionMagnus2 = Vector3.Cross(rotationVelocity, (ballTarget.position - this.transform.position).normalized);

        if (magnusCoefficient >= 0)
        {
            ballDirectionMagnus2 = ballDirectionMagnus2 * -1;
        }
        
        _magnusForce2 = magnusCoefficient * ballDirectionMagnus2;

        _movementEulerSpeedSimulationMagnus = _movementEulerSpeedSimulationMagnus + _magnusForce2 + _acceleration * Time.deltaTime;

        blueParticleGO.transform.position = (blueParticleGO.transform.position + _movementEulerSpeedSimulationMagnus * Time.deltaTime);
    }

    private void ShowRedArrow()
    {
        redArrow.SetActive(true);
        redArrow.transform.rotation = Quaternion.LookRotation(_acceleration, Vector3.up);
        redArrowShoot.SetActive(true);
        redArrowShoot.transform.rotation = Quaternion.LookRotation((ballTarget.position - this.transform.position).normalized * forceSlider.value, Vector3.up);
        redArrowMagnus.SetActive(true);
        redArrowShoot.transform.rotation = Quaternion.LookRotation(_magnusForce2, Vector3.up);
    }

    private void ShowGreenArrow()
    {
        greenArrow.SetActive(true);
        greenArrow.transform.rotation = Quaternion.LookRotation(((ballTarget.position - this.transform.position) * forceSlider.value) * MASS, Vector3.up);
    }

    private void ShowGreyArrow()
    {
        greyArrow.SetActive(true);
        greyArrow.transform.rotation = Quaternion.LookRotation(((ballTarget.position - this.transform.position) * forceSlider.value) + _acceleration * (_simulationTime * _simulationIterations), Vector3.up);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _myOctopus.NotifyShoot();
        shootBall = true;
        ballSpeed = forceSlider.value;
        ballDirection = ballTarget.position - this.transform.position;
        movementEulerSpeed = ballDirection.normalized * ballSpeed;
    }
}