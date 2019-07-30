using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ship : MonoBehaviour {
    Rigidbody2D rigidBody;
    public PolygonCollider2D shipBaseCollider;
    public CircleCollider2D shipBodyCollider;
    public EdgeCollider2D floorCollider;
    float speed = 2f;
    float moonGravityPercent = 0.1653f;
    float rotationSpeed = 50f;
    float shipFuel;
    float maxFuel = 100;
    float maxVelocityForLanding = 1.3f;
    float inclinationAngle = 50f;
    float maxSpeed = 2f;
    public Vector2 startPosition;
    public NeuralNet nn;
    public bool isAlive = true;
    public bool landed = false;
    public float distanceFromGround;
    public float bestDistanceFromGround;
    float maxDistanceFromGround;
    float startHorizontal, endHorizontal;
    float collisionSpeed = 0;
    bool outOfBounds = false;
    float landedTime = 0;
    void Awake() {
        nn = new NeuralNet(5, 8, 8, 2);
        rigidBody = GetComponent<Rigidbody2D>();
    }
    void Start() {
        Restart();

        var topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
        var bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        float camTopY = topRight.y;
        float camBottomY = bottomLeft.y;
        startHorizontal = bottomLeft.x;
        endHorizontal = topRight.x;
        maxDistanceFromGround = camTopY - camBottomY;
        bestDistanceFromGround = maxDistanceFromGround;

        floorCollider = GameObject.FindGameObjectWithTag("Ground").GetComponent<EdgeCollider2D>();
        rigidBody.gravityScale = rigidBody.gravityScale * moonGravityPercent; 
    }
    void Update() {
        if (isAlive) {
            distanceFromGround = Vector2.Distance(transform.position, floorCollider.ClosestPoint(transform.position)); 
            if (distanceFromGround < bestDistanceFromGround) {
                bestDistanceFromGround = distanceFromGround;
            }
        }
        if (landed) {
            landedTime += Time.deltaTime;
        }

        if (shipFuel > 0 && !landed) {
            var output = GetNNOutput().Select(e => (float)System.Math.Round(e, 2)).ToArray();
            rigidBody.AddForce(transform.up * speed * output[0]);
            shipFuel -= (.1f * output[0]);
            var angleMult = output[1] - .5f;
            var newRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.eulerAngles.y, inclinationAngle * angleMult);
            transform.rotation = Quaternion.RotateTowards(this.transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
            if (rigidBody.velocity.magnitude > maxSpeed) {
                rigidBody.velocity = rigidBody.velocity.normalized * maxSpeed;
            }
        }
        if (!IsInBounds()) {
            outOfBounds = true;
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
        var distanceFromGroundInput = distanceFromGround / maxDistanceFromGround;
        var horizontalPositionInput = (transform.position.x - startHorizontal) / (endHorizontal - startHorizontal); 
        var inclinationInput = (transform.rotation.eulerAngles.z + inclinationAngle) / 100;
        return nn.Activate(velocityInput, fuelInput, distanceFromGroundInput, horizontalPositionInput, inclinationInput);
    }

    public float GetScore() {
        var distanceScore = (maxDistanceFromGround - distanceFromGround) * 100 / maxDistanceFromGround;
        var fuelScore = (maxFuel - shipFuel) * 100 / maxFuel;
        var score = fuelScore + distanceScore + landedTime;
        if (landed) {
            score += 1000;
        }
        if (outOfBounds) {
            score = (score - 1000) > 1 ? score - 1000 : 1;
        }
        score = (score - collisionSpeed * 10) > 1 ? score - collisionSpeed * 10 : 1;
        return score;
    }

    void Die() {
        isAlive = false;
        landed = false;
        this.gameObject.SetActive(false);
    }

    public void Restart() {
        this.outOfBounds = false;
        this.isAlive = true;
        this.shipFuel = maxFuel;
        this.collisionSpeed = 0;
        this.transform.position = startPosition;
        this.rigidBody.velocity = Vector2.zero;
        this.transform.rotation = Quaternion.identity;
        this.distanceFromGround = maxDistanceFromGround;
        this.gameObject.SetActive(true);
        this.landed = false;
        this.landedTime = 0;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.relativeVelocity.magnitude > maxVelocityForLanding || shipBodyCollider.IsTouching(collision.collider)) {
            collisionSpeed = collision.relativeVelocity.magnitude;
            Die();
        }
        if (shipBaseCollider.IsTouching(collision.collider)) {
            landed = true;
        }
    }

}
