using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetController : MonoBehaviour
{
    public Slider forceSlider; 
    public Transform scorpion, ball, ballTarget;
    public IK_tentacles ikTentacles;
    private Vector3 _scorpionOriginalPosition, _ballOriginalPosition, _ballTargetOriginalPosition;
    private IK_Scorpion _scorpionScript;
    // Start is called before the first frame update
    void Start()
    {
        _scorpionScript = scorpion.gameObject.GetComponent<IK_Scorpion>();
        _scorpionOriginalPosition = scorpion.position;
        _ballOriginalPosition = ball.position;
        _ballTargetOriginalPosition = ballTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            forceSlider.value = 0;
            _scorpionScript.animTime = 0;
            //scorpion.position = _scorpionOriginalPosition;
            ikTentacles.stopBall = !ikTentacles.stopBall;
            ball.position = _ballOriginalPosition;
            ball.GetComponent<MovingBall>().shootBall = false;
            ball.GetComponent<MovingBall>().movementEulerSpeed = Vector3.zero;
            ballTarget.position = _ballTargetOriginalPosition;
        }
    }
}
