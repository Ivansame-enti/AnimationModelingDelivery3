using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingBall : MonoBehaviour
{
    private float GRAVITY = -1f;
    private float MASS = 1f;
    [SerializeField]
    IK_tentacles _myOctopus;

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

    private float _timer;
    private float _stopForce=2;

    public Slider forceSlider;
    // Start is called before the first frame update
    void Start()
    {
        _acceleration = new Vector3(0, GRAVITY * MASS, 0);
        _timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.identity;

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
        movementEulerSpeed = movementEulerSpeed + _acceleration * Time.deltaTime;
        transform.position = (transform.position + movementEulerSpeed * Time.deltaTime);
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
