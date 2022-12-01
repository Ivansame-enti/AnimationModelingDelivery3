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
    public float ballSpeed=10f;
    private Vector3 ballDirection;

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
        if (_shootBall) GetComponent<Rigidbody>().AddForce(ballDirection.normalized * ballSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _myOctopus.NotifyShoot();
        _shootBall = true;
        ballDirection = ballTarget.position - this.transform.position;
    }
}
