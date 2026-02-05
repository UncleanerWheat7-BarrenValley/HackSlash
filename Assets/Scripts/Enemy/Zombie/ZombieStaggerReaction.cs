using System;
using UnityEngine;

public class ZombieStaggerReaction : MonoBehaviour, IHitReaction
{
    [SerializeField] private float duration = 1.07f; //length of stagger animation
    [SerializeField] AnimationCurve speedCurve; //how fast the zombie will physically move to play the animation

    private float timer;
    private bool active;

    Animator animator;
    private Rigidbody rb;
    private EnemyHealth health;
    private Vector3 hitDirection;

    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int HitX = Animator.StringToHash("HitX");
    private static readonly int HitY = Animator.StringToHash("HitY");
    public bool IsComplete => active && timer >= duration;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        health = GetComponent<EnemyHealth>();
    }

    public void Begin()
    {
        active = true;
        timer = 0f;

        hitDirection = health.LastHitDirection;
        hitDirection.z = 0;
        hitDirection.Normalize();

        Vector2 localHit = GetLocalHitDirection(hitDirection);

        animator.SetBool(Hit, true);
        animator.SetFloat(HitX, localHit.x);
        animator.SetFloat(HitY, localHit.y);
    }

    public void End()
    {
        active = false;
        animator.SetBool(Hit, false);
        animator.SetFloat(HitX, 0);
        animator.SetFloat(HitY, 0);
    }

    private void Update()
    {
        if (!active) return;
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        float speed = speedCurve.Evaluate(t);
        transform.Translate(hitDirection * speed * Time.deltaTime, Space.World);
    }

    private Vector2 GetLocalHitDirection(Vector3 worldDirection)
    {
        float forwardDot = Vector3.Dot(transform.forward, worldDirection);
        float rightDot = Vector3.Dot(transform.right, worldDirection);

        if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot)) return new Vector2(0, forwardDot > 0 ? 1 : -1);
        if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot)) return new Vector2(rightDot > 0 ? 1 : -1, 0);
        else return new Vector2(0, 1);
    }
}