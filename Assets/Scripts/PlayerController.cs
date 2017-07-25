using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Left, Right }

public class MoveDirectionEventArgs : EventArgs {
    public Direction MoveDirection { get; private set; }

    public MoveDirectionEventArgs(Direction direction) {
        this.MoveDirection = direction;
    }
}

public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float maxWalkSpeed = 10f;

    private Rigidbody2D charRB;
    private Animator animator;
    private Direction moveDirection = Direction.Right;

    public delegate void MoveDirectionChangedHandler(MoveDirectionEventArgs e);
    public event MoveDirectionChangedHandler MoveDirectionChanged;

    public Direction MoveDirection {
        get {
            return this.moveDirection; 
        }
        set {
            this.moveDirection = value;

            var eventArgs = new MoveDirectionEventArgs(this.moveDirection);
            this.MoveDirectionChanged(eventArgs);
        }
    }


	// Use this for initialization
	void Start () {
        this.charRB = this.GetComponent<Rigidbody2D>();
        this.animator = this.GetComponent<Animator>();
        this.MoveDirectionChanged += OnMoveDirectionChanged;
	}
	
	// Update is called every fixed period of time
	void FixedUpdate () {
        float horizAxis = Input.GetAxis("Horizontal");
        float walkSpeed = horizAxis * this.maxWalkSpeed;

        //charRB.AddForce(new Vector2(horizAxis * this.maxWalkSpeed, 0));
        charRB.velocity = new Vector2(walkSpeed, charRB.velocity.y);

        animator.SetFloat("speed", Mathf.Abs(walkSpeed));

        if (walkSpeed > 0) {
            this.MoveDirection = Direction.Right;
        } else if (walkSpeed < 0) {
            this.MoveDirection = Direction.Left;
        }
	}

    private void OnMoveDirectionChanged(MoveDirectionEventArgs e) {
        Vector3 charScale = this.transform.localScale;
        if (e.MoveDirection == Direction.Right) {
            charScale.x = Mathf.Abs(charScale.x);
        } else if (e.MoveDirection == Direction.Left) {
            charScale.x = -Mathf.Abs(charScale.x);
        }
        this.transform.localScale = charScale;
    }
}
