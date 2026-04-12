using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class PlayerShip : ShipMovement
{
    public delegate void TargetSelect(Transform target);
    public TargetSelect targetSelection;
    public Transform emptyTarget;
    private bool emptyTargetActive;

    private InputAction followAction;
    private InputAction targetAction;
    private InputAction rotateAction;


    List<(ShipGenome genome, ModuleStatsTracker tracker)> genomeTrackers;
    protected override void OnStart()
    {
        base.OnStart();

        followAction = InputSystem.actions.FindAction("MoveForward");
        targetAction = InputSystem.actions.FindAction("TargetSelect");
        rotateAction = InputSystem.actions.FindAction("Rotate");

        rotateAction.Enable();

        genomeTrackers = new List<(ShipGenome, ModuleStatsTracker)>();
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
                rigidbodyThis.MovePosition(rigidbodyThis.position + targetDirection * Speed * Time.fixedDeltaTime); 

                foreach (var trackerPair in genomeTrackers)
                {
                    if (trackerPair.tracker != null && trackerPair.genome != null)
                        trackerPair.tracker.RegisterDistanceMoved(trackerPair.genome.speed * Time.fixedDeltaTime);
                }
            }
        }

        if (targetAction.IsPressed())
        {
            Transform target = EnemyManager.Instance.GetClosestEnemy(mouseWorldPosition);
            if (target != null && Vector2.Distance((Vector2)target.position, mouseWorldPosition) < 2f)
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
            rigidbodyThis.AddTorque(rotationDir * turnRate, ForceMode2D.Force);
        }

        // Auto-stabilization
        rigidbodyThis.AddTorque(-rigidbodyThis.angularVelocity * angularDamping, ForceMode2D.Force);
    }
    public void SetStats(ShipMobilityStats shipMobilityStats, (ShipGenome genome, ModuleStatsTracker tracker)[] genomeTrackers)
    {
        //Stats
        speed = shipMobilityStats.speed / 20;//Adjust genome stats to gameplay stats       Max speed in settings would be good if it was reduced(50?)
        turnRate = shipMobilityStats.turnRate;
        mass = shipMobilityStats.mass;
        inertia = 1 + shipMobilityStats.inertia / 20;//Adjust genome stats to gameplay stats, never lower than 1

        rigidbodyThis.mass = mass;
        rigidbodyThis.inertia = inertia;

        speed = Mathf.Min(speed, 4);

        this.genomeTrackers = genomeTrackers.ToList();
    }
}