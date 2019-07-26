using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ship : MonoBehaviour {
    Rigidbody2D rigidBody;
    public PolygonCollider2D shipBaseCollider;
    public CircleCollider2D shipBodyCollider;
    float speed = 2f;
    public bool isAiControlled = false;
    float moonGravityPercent = 0.1653f;
    float rotationSpeed = 50f;
    public float shipFuel = 20;
    float maxFuel = 20;
    float maxVelocityForLanding = 1f;
    float inclinationAngle = 25f;
    float maxSpeed = 2f;
    public Vector2 startPosition;
    public NeuralNet nn;
    public bool isAlive = true;
    public bool landed = false;
    void Awake() {
        nn = new NeuralNet(2, 8, 8, 3);
    }
    void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.gravityScale = rigidBody.gravityScale * moonGravityPercent; 
    }
    void Update() {
        // if (!isAiControlled) {
        //     CheckInputs();
        // }

        if (shipFuel > 0) {
            var output = GetNNOutput();
            if (output[0] > 0.5) {
                rigidBody.AddForce(transform.up * speed);
                shipFuel -= .1f;
            }
            if (output[1] > 0.5) {
                var newRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.eulerAngles.y, inclinationAngle);
                transform.rotation = Quaternion.RotateTowards(this.transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
            }
            if (output[2] > 0.5) {
                var newRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.eulerAngles.y, -inclinationAngle);
                transform.rotation = Quaternion.RotateTowards(this.transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
            }
            if (rigidBody.velocity.magnitude > maxSpeed) {
                rigidBody.velocity = rigidBody.velocity.normalized * maxSpeed;
            }
        }
        if (!IsInBounds()) {
            Die();
        }
    }

    bool IsInBounds() {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (GeometryUtility.TestPlanesAABB(planes , shipBodyCollider.bounds))
            return true;
        else
            return false;
    }

    float[] GetNNOutput() {
        var velocityInput = rigidBody.velocity.magnitude / maxSpeed;
        var fuelInput = shipFuel / maxFuel; 
        return nn.Activate(velocityInput, fuelInput);
    }

    void Die() {
        isAlive = false;
        landed = false;
        this.gameObject.SetActive(false);
    }

    public void Restart() {
        this.shipFuel = maxFuel;
        this.transform.position = startPosition;
        this.rigidBody.velocity = Vector2.zero;
        this.transform.rotation = Quaternion.identity;
        this.gameObject.SetActive(true);
        this.landed = false;
    }

    // void CheckInputs() {
    //     if (Input.GetKey(KeyCode.UpArrow) && shipFuel > 0) {
    //         rigidBody.AddForce(transform.up * speed);
    //         shipFuel -= .1f;
    //     }
    //     if (Input.GetKey(KeyCode.LeftArrow)) {
    //         var newRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.eulerAngles.y, inclinationAngle);
    //         transform.rotation = Quaternion.RotateTowards(this.transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
    //     }
    //     if (Input.GetKey(KeyCode.RightArrow)) {
    //         var newRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.eulerAngles.y, -inclinationAngle);
    //         transform.rotation = Quaternion.RotateTowards(this.transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
    //     }
    // }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.relativeVelocity.magnitude > maxVelocityForLanding || shipBodyCollider.IsTouching(collision.collider)) {
            Die();
        }
        if (shipBaseCollider.IsTouching(collision.collider)) {
            landed = true;
        }
    }

}
