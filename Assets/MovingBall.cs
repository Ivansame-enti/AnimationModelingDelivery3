using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingBall : MonoBehaviour
{
    [SerializeField]
    IK_tentacles _myOctopus;

    //movement speed in units per second
    [Range(-1.0f, 1.0f)]
    [SerializeField]
    private float _movementSpeed = 10f;

    public Transform ballTarget;
    private bool _shootBall = false;
    public float ballSpeed=10f;
    private Vector3 ballDirection;

    private float _timer;
    private float _stopForce=2;

    public Slider forceSlider;
    // Start is called before the first frame update
    void Start()
    {
        _timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;

        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");

        ballTarget.position = ballTarget.position + new Vector3(-horizontalInput * _movementSpeed * Time.deltaTime, verticalInput * _movementSpeed * Time.deltaTime, 0); //Movemos el target en vez de la pelota

    }

    private void FixedUpdate()
    {
        if (_shootBall)
        {
            GetComponent<Rigidbody>().AddForce(ballDirection.normalized * ballSpeed, ForceMode.Impulse);
            _shootBall = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        _myOctopus.NotifyShoot();
        _shootBall = true;
        ballSpeed = forceSlider.value;
        ballDirection = ballTarget.position - this.transform.position;
    }
}
