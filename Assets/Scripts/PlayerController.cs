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

    [SerializeField] private float maxWalkSpeed = 10f;

	// Jump fields
	[SerializeField] private Transform groundChecker;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private float jumpHeight = 100f;
	private float groundCheckerRadius = 0.2f;
	private bool isOnGround = true;

	// Shooting fields
	[SerializeField] private Transform gunTip;
	[SerializeField] private GameObject projectile;
	[SerializeField] private float shootRate = 0.5f;
	private bool isShooting = false;
	private float lastShot = 0.0f;


    private Rigidbody2D charRB;
    private Animator animator;
    private Direction moveDirection;



    public delegate void MoveDirectionChangedHandler(MoveDirectionEventArgs e);
    public event MoveDirectionChangedHandler MoveDirectionChanged;

    public Direction MoveDirection {
		get {
			return this.moveDirection;
		}
		set {
			// Update direction and call event only with new value
			if (this.moveDirection != value) {
				this.moveDirection = value;

				// Direction changed - call event
				var eventArgs = new MoveDirectionEventArgs(this.moveDirection);
				this.MoveDirectionChanged(eventArgs);
			}
		}
	}


	// Use this for initialization
	void Start () {
        this.charRB = this.GetComponent<Rigidbody2D>();
        this.animator = this.GetComponent<Animator>();
        this.MoveDirectionChanged += OnMoveDirectionChanged;

        // Set direction on start
        this.MoveDirection = Direction.Right;
	}

	// Update is called every frame
	void Update() {
		// Set all animation dependencies
		this.animator.SetBool("isOnGround", this.isOnGround);
		this.animator.SetFloat("xSpeed", Mathf.Abs(this.charRB.velocity.x));
		this.animator.SetFloat("ySpeed", this.charRB.velocity.y);
		this.animator.SetBool("isShooting", this.isShooting);
	}
	
	// Update is called every fixed period of time
	void FixedUpdate () {
        // Get inputs and calculate speed
        float horizAxis = Input.GetAxis("Horizontal");
        float walkSpeed = horizAxis * this.maxWalkSpeed;

        // Movement
        this.charRB.velocity = new Vector2(walkSpeed, this.charRB.velocity.y);

        // Set move direction (all direction logic is performed in the property event)
        if (walkSpeed > 0) {
            this.MoveDirection = Direction.Right;
        } else if (walkSpeed < 0) {
            this.MoveDirection = Direction.Left;
        }

		// Jumps
		bool isJumpPressed = Input.GetAxis("Jump") > 0;
		if (this.isOnGround && isJumpPressed) {
			this.isOnGround = false;
			this.charRB.AddForce(new Vector2(0, this.jumpHeight));
		} else {
			this.isOnGround = Physics2D.OverlapCircle(this.groundChecker.position, this.groundCheckerRadius, this.groundLayer);
		}

		// Depending on the input, provide shooting animation dependencies
		bool isFirePressed = Input.GetAxisRaw("Fire1") > 0;
		bool isFirstShot = false;
		if (isFirePressed && !this.isShooting) { // Apply animation only once
			this.isShooting = true;
			isFirstShot = true;
		} else if (!isFirePressed) {
			this.isShooting = false;
		}

		// Shooting
		if (this.isShooting) {
			this.Shoot(isFirstShot);
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

	private void Shoot(bool isFirstShot) {
		if (Time.time > this.lastShot || isFirstShot) {
			this.lastShot = Time.time + this.shootRate;
			GameObject projectile = Instantiate(this.projectile, this.gunTip.position, Quaternion.identity);
			projectile.SendMessage("OnProjectileSpawned", this.MoveDirection);
		}
	}
}
