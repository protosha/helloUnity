using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {

	[SerializeField] private float flySpeed = 2.0f;
	[SerializeField] private float lifespan = 2.0f;

	private Rigidbody2D projRB;

	void Awake() {
		projRB = this.GetComponent<Rigidbody2D>();
	}

	// Use this for initialization
	void Start() {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnProjectileSpawned(Direction direction) {
		Debug.Log("Inside spawn method");
		if (direction == Direction.Right) {
			this.projRB.AddForce(Vector2.right * flySpeed);
		} else if (direction == Direction.Left) {
			this.transform.rotation = Quaternion.Euler(0, 0, 180f);
			this.projRB.AddForce(Vector2.left * flySpeed);
		}

		Destroy(this.gameObject, lifespan);
	}
}
