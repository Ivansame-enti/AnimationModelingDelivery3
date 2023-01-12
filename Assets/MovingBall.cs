﻿using System.Collections;
using System.Collections.Generic;
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
    private Vector3 _acceleration;
    private Vector3 ballDirection;
    private Vector3 ballDirectionMagnus;

    private float _timer;
    private float _stopForce=2;

    private Vector3 magnusForce;
    Vector3 angularVelocity;

    public Slider forceSlider;
    public Slider magnusSlider;

    private bool showArrows=false;
    public GameObject greyArrow;
    //public GameObject blueArrow;
    public GameObject greenArrow;
    public GameObject redArrow;

    private float _simulationTime = 0.3f;
    private float _simulationIterations = 50f;

    private Vector3 hitPoint;

    // Start is called before the first frame update
    void Start()
    {
        greyArrow.SetActive(false);
        //blueArrow.SetActive(false);
        redArrow.SetActive(false);
        greenArrow.SetActive(false);

        _acceleration = new Vector3(0, GRAVITY * MASS, 0);
        _timer = 0;
        angularVelocity = new Vector3(1,1,1);
    }

    // Update is called once per frame
    void Update()
    {
        if(_myScorpion._tailTarget!=null) hitPoint = new Vector3(_myScorpion._tailTarget.position.x + Mathf.Lerp(-0.5f, +0.5f, Mathf.InverseLerp(magnusSlider.minValue, magnusSlider.maxValue, magnusSlider.value)), _myScorpion._tailTarget.position.y, _myScorpion._tailTarget.position.z);

        if (Input.GetKeyDown(KeyCode.I))
        {
            showArrows = !showArrows;
        }

        if (showArrows)
        {
            ShowRedArrow();
            ShowGreenArrow();
            ShowGreyArrow();
            ShowBlueArrow();
        } else
        {
            greyArrow.SetActive(false);
            //blueArrow.SetActive(false);
            redArrow.SetActive(false);
            greenArrow.SetActive(false);
        }

        transform.rotation = Quaternion.identity;

        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");

        ballTarget.position = ballTarget.position + new Vector3(-horizontalInput * _movementSpeed * Time.deltaTime, verticalInput * _movementSpeed * Time.deltaTime, 0); //Movemos el target en vez de la pelota

        if (shootBall)
        {
            EulerStep();
        }
    }

    private void EulerStep()
    {
        //ballDirectionMagnus = Vector3.Cross(new Vector3(0, velocidadAngular, 0), ballDirection.normalized * ballSpeed);

        float magnusCoefficient = magnusSlider.value;
        //Vector3 MagnusForce = S * ballDirectionMagnus;

        //Vector3 finalForce = MagnusForce + ballDirection.normalized * ballSpeed;

        magnusForce = magnusCoefficient * Vector3.Cross(angularVelocity, movementEulerSpeed);

        movementEulerSpeed = movementEulerSpeed /*+ magnusForce*/ + _acceleration * Time.deltaTime;
        transform.position = (transform.position + movementEulerSpeed * Time.deltaTime);
    }

    private void ShowRedArrow()
    {
        redArrow.SetActive(true);
        redArrow.transform.rotation = Quaternion.LookRotation(_acceleration, Vector3.up);
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

    private void ShowBlueArrow()
    {
        //blueArrow.SetActive(true);
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