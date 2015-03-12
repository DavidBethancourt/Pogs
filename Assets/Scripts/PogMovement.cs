using UnityEngine;
using System.Collections;
using System;

public class PogMovement : MonoBehaviour {

    public Vector2 WalkSpeed = new Vector2(1, 1);
    private Vector2 _intendedMovement;
    private Animator _pogAnimator;
    private float _initialRotateQuickness = 1.0f; // bigger is quicker
    private GameObject _turkeyLeg;
    float _angle = 0.0f;

    // Use this for initialization
    void Start()
    {
        _pogAnimator = GetComponent<Animator>();
        _turkeyLeg = GameObject.FindGameObjectWithTag("TurkeyLeg");
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 originalPosition = this.transform.position;
        Vector3 desiredPosition = Vector3.MoveTowards(this.transform.position, _turkeyLeg.transform.position, 1f);
        Vector3 moveDirection = desiredPosition - originalPosition;

        _intendedMovement = new Vector2(WalkSpeed.x * moveDirection.x, WalkSpeed.y * moveDirection.y);
        if ( Math.Abs(_intendedMovement.x) > 0.2f || Math.Abs(_intendedMovement.y) > 0.2f) 
        {
            _pogAnimator.SetBool("Walking", true);
        }
        else
        {
            _pogAnimator.SetBool("Walking", false);
        }

        if (moveDirection != Vector3.zero)
        {
            _angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg + 90; //+90 is because the pog default is facing down
            Quaternion properRotation = Quaternion.AngleAxis(_angle, Vector3.forward);
            if (transform.rotation != properRotation)
            {
                Quaternion current = transform.localRotation;
                transform.rotation = Quaternion.Slerp(current, properRotation, Time.deltaTime * _initialRotateQuickness);
            }
        }
    }

    void FixedUpdate()
    {
        // Apply movement to the rigidbody
        rigidbody2D.velocity = _intendedMovement;
        rigidbody2D.rotation = _angle;
    }
}
