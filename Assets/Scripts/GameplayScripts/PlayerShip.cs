using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerShip : MonoBehaviour
{
    public delegate void TargetSelect(Transform target);
    public TargetSelect targetSelection;
    public Transform emptyTarget;
    private bool emptyTargetActive;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;

    private InputAction followAction;
    private InputAction targetAction;
    private InputAction rotateAction;

    private Rigidbody2D rigidbodyThis;

    public ShipMobilityStats ship_Mobility_Stats;

    private float angularDamping = 0.05f;

    void Start()
    {
        followAction = InputSystem.actions.FindAction("MoveForward");
        targetAction = InputSystem.actions.FindAction("TargetSelect");
        rotateAction = InputSystem.actions.FindAction("Rotate");

        rotateAction.Enable();

        rigidbodyThis = GetComponent<Rigidbody2D>();


        ship_Mobility_Stats = new ShipMobilityStats()
        {
            speed = movementSpeed / 20,
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

        //float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
        //float newAngle = Mathf.LerpAngle(rigidbodyThis.rotation, targetAngle, ship_Mobility_Stats.turn_Rate * Time.fixedDeltaTime);

        //rigidbodyThis.MoveRotation(newAngle);

        if (followAction.IsPressed())
        {
            if (Vector2.Distance((Vector2)transform.position, mouseWorldPosition) > 0.2f)
            {
                rigidbodyThis.linearVelocity = Vector2.zero;
                rigidbodyThis.MovePosition(rigidbodyThis.position + targetDirection * ship_Mobility_Stats.speed * Time.fixedDeltaTime);
            }
        }

        if (targetAction.IsPressed())
        {
            Transform target = EnemyManager.Instance.GetClosestEnemy(mouseWorldPosition);
            if (target != null && Vector2.Distance((Vector2)target.position, mouseWorldPosition) < 0.2f)
            {
                targetSelection(target);
                emptyTarget.gameObject.SetActive(false);
                emptyTargetActive = false;
            }
            else
            {
                if (emptyTargetActive)
                {
                    emptyTarget.gameObject.SetActive(false);
                    emptyTargetActive = false;
                }
                else
                {
                    emptyTarget.gameObject.SetActive(true);
                    emptyTargetActive = true;
                    emptyTarget.position = mouseWorldPosition;
                    targetSelection(emptyTarget);
                }
            }
        }

        if (rotateAction.IsPressed())
        {
            float rotationDir = rotateAction.ReadValue<float>();
            rigidbodyThis.AddTorque(rotationDir * ship_Mobility_Stats.turn_Rate, ForceMode2D.Force);
        }

        // Auto-stabilization
        rigidbodyThis.AddTorque(-rigidbodyThis.angularVelocity * angularDamping, ForceMode2D.Force);
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