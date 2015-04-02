using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PogMovement : MonoBehaviour {

    public Vector2 WalkSpeed = new Vector2(1, 1);
    private Vector2 _intendedMovement;
    private Animator _pogAnimator;
    private float _initialRotateQuickness = 1.0f; // bigger is quicker
    private GameObject _turkeyLeg;
    private float _angle = 0.0f;
    private DateTime _lastPathFinderAttempt = DateTime.MinValue;
    private List<SimplePoint> _path = null;
    // Use this for initialization
    void Start()
    {
        _pogAnimator = GetComponent<Animator>();
        _turkeyLeg = GameObject.FindGameObjectWithTag("TurkeyLeg");
    }

    // Update is called once per frame
    void Update()
    {
        if (_path == null && ((DateTime.Now - _lastPathFinderAttempt).TotalSeconds > 5)) // don't find paths on every frame - else chaos will ensue
        {
            _lastPathFinderAttempt = DateTime.Now;
            SimplePoint start = new SimplePoint(Convert.ToInt32(Math.Round(this.transform.position.x)), Convert.ToInt32(Math.Round(this.transform.position.y)));
            SimplePoint end = new SimplePoint(Convert.ToInt32(Math.Round(_turkeyLeg.transform.position.x)), Convert.ToInt32(Math.Round(_turkeyLeg.transform.position.y)));
            PathFinder finder = new PathFinder();
            _path = finder.FindPath(start, end, GameManager.instance.BoardLogic.Positions.WalkableMap);
        }

        if (_path != null && _path.Count > 0)
        {
            Vector3 originalPosition = this.transform.position;
            Vector3 nextNodeInPath = new Vector3(_path[0].X, _path[0].Y, 0f);
            Vector3 moveDirection = (nextNodeInPath - originalPosition).normalized;

            _intendedMovement = new Vector2(WalkSpeed.x * moveDirection.x, WalkSpeed.y * moveDirection.y);
            if (Math.Abs(_intendedMovement.x) > 0.2f || Math.Abs(_intendedMovement.y) > 0.2f)
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

            Vector3 distanceToNextNode = nextNodeInPath - this.transform.position;
            if (distanceToNextNode.magnitude < 0.1f)
            {
                _path.RemoveAt(0);
            }
        }
        else
        {
            _intendedMovement = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        // Apply movement to the rigidbody
        GetComponent<Rigidbody2D>().velocity = _intendedMovement;
        GetComponent<Rigidbody2D>().rotation = _angle;
    }
}
