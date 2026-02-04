using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerShip : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;

    private InputAction followAction;

    private Rigidbody2D rigidbodyThis;

    public ShipMobilityStats ship_Mobility_Stats;

    void Start()
    {
        followAction = InputSystem.actions.FindAction("MoveForward");

        rigidbodyThis = GetComponent<Rigidbody2D>();


        ship_Mobility_Stats = new ShipMobilityStats()
        {
            speed = movementSpeed,
            turn_Rate = rotationSpeed,
            evasion = 0,
            mass = 1,
            inertia = 1
        };
    }
    void FixedUpdate()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector2 targetDirection = (mouseWorldPosition - (Vector2)transform.position).normalized;

        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
        float newAngle = Mathf.LerpAngle(rigidbodyThis.rotation, targetAngle, ship_Mobility_Stats.turn_Rate * Time.fixedDeltaTime);

        rigidbodyThis.MoveRotation(newAngle);

        if (followAction.IsPressed())
        {
            rigidbodyThis.linearVelocity = Vector2.zero;
            rigidbodyThis.MovePosition(rigidbodyThis.position + (Vector2)transform.up * ship_Mobility_Stats.speed * Time.fixedDeltaTime);
        }
    }

}
public struct ShipMobilityStats
{
    public float speed;
    public float turn_Rate;
    public float evasion;
    public float mass;
    public float inertia;
}