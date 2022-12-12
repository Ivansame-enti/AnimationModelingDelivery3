using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Vector3 ballSpeed = new Vector3(5,0,0);
    private Vector3 ballDirection;
    private float radius = 0.5f;
    private float airResistance = 0.8f;
    private Vector3 angularVelocity =new Vector3(5,5,0);
    private Vector3 ballDirectionMagnus;
    private float Magnitude1,Magnitude2,Magnitude3;
    private Vector3 Magnitude;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;

        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");

        //update the position
        transform.position = transform.position + new Vector3(-horizontalInput * _movementSpeed * Time.deltaTime, verticalInput * _movementSpeed * Time.deltaTime, 0);

    }

    private void FixedUpdate()
    {
        if (_shootBall) {
            GetComponent<Rigidbody>().AddForce(ballDirection.normalized * Magnitude.x);
            GetComponent<Rigidbody>().AddForce(ballDirection.normalized * ballSpeed.x);
          
        };
    }

    private void OnCollisionEnter(Collision collision)
    {
        _myOctopus.NotifyShoot();
        _shootBall = true;
        ballDirection = ballTarget.position - this.transform.position;
        ballDirectionMagnus = Vector3.Cross(angularVelocity,ballSpeed);
        Magnitude1 = ballDirectionMagnus.x * airResistance;
        Magnitude2 = ballDirectionMagnus.y * airResistance;
        Magnitude3 = ballDirectionMagnus.z * airResistance;
        Magnitude = new Vector3(Magnitude1, Magnitude2, Magnitude3);
    }
}