using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetController : MonoBehaviour
{
    public Slider forceSlider; 
    public Transform scorpion, ball, ballTarget;

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
            ball.position = _ballOriginalPosition;
            ball.gameObject.GetComponent<Rigidbody>().AddForce(0, 0, 0);
            ball.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
            ballTarget.position = _ballTargetOriginalPosition;
        }
    }
}
