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

    //Movement type 1
    private InputAction followAction;

    Rigidbody2D rigidbodyThis;

    void Start()
    {
        followAction = InputSystem.actions.FindAction("MoveForward");

        rigidbodyThis = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector2 targetDirection = (mouseWorldPosition - (Vector2)transform.position).normalized;

        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
        float newAngle = Mathf.LerpAngle(rigidbodyThis.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);

        rigidbodyThis.MoveRotation(newAngle);

        if (followAction.IsPressed())
        {
            rigidbodyThis.linearVelocity = Vector2.zero;
            rigidbodyThis.MovePosition(rigidbodyThis.position + (Vector2)transform.up * movementSpeed * Time.fixedDeltaTime);

        }
    }
    
}