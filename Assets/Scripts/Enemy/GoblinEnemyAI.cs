using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoblinEnemyAI : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    private Vector3 startpos;
    private Vector3 roampos;
    SpriteRenderer sprite;

    [SerializeField]
    private GameObject DuplicateEnemyAI;

    [SerializeField]
    float Targetrange;
    [SerializeField]
    float Attackrange;
    [SerializeField]
    float Attackdmg;
    [SerializeField]
    float AttackDelay;
    float LastAttackTime = 0f;
    float mudmovespeed;
    float defaultspeed;

    public AudioClip attackSFX;

    private GameObject gameController;

    private Animator animator;

    //States
    private enum State
    {
        Roaming,
        Idle,
        Chase,
        Attack,
        ReturnToStart,
    }

    private State state;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        startpos = transform.position;
        roampos = GetRoamingpos();
        state = State.Idle;
        sprite = GetComponent<SpriteRenderer>();
        defaultspeed = agent.speed;
        mudmovespeed = agent.speed * 0.5f;

        gameController = GameObject.FindGameObjectWithTag("GameController");

        animator = GetComponent<Animator>();

        if ((gameController.GetComponent<GameController>().levelCount / 3) > 0)
        {
            int multiplier = gameController.GetComponent<GameController>().levelCount / 3;
            Attackdmg += (int)(Attackdmg * 0.3f) * multiplier;
            GetComponent<Health>().maxhealth += (GetComponent<Health>().maxhealth * 0.2f) * multiplier;
            GetComponent<Health>().currhealth = GetComponent<Health>().maxhealth;
        }

    }

    private void Update()
    {
        Vector3 playerdir = (player.position - transform.position).normalized;
        if (playerdir.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        if (GetComponent<EnemyController>().CollidingwithMud)
        {
            agent.speed = mudmovespeed;
        }
        else
        {
            agent.speed = defaultspeed;
        }
        switch (state)
        {
            default:
            case State.Roaming:
                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(roampos);
                    float ReachedposDist = 2f;
                    if (Vector3.Distance(transform.position, roampos) < ReachedposDist)  //if reached roam pos
                    {
                        roampos = GetRoamingpos();
                    }
                }
                FindTarget(playerdir);
                break;
            case State.Idle:
                animator.SetBool("Walking", false);
                FindTarget(playerdir);
                break;
            case State.Chase:
                agent.SetDestination(player.position);
                if (Vector3.Distance(transform.position, player.position) < Attackrange)
                {
                    //if player wihtin attack range
                    state = State.Attack;
                    agent.isStopped = true;
                    animator.SetBool("Walking", false);

                }
                if (Vector3.Distance(transform.position, player.position) > Targetrange)
                {
                    state = State.ReturnToStart;
                }
                break;
            case State.Attack:
                if (Vector3.Distance(transform.position, player.position) > Attackrange)
                {
                    state = State.Chase;
                    agent.isStopped = false;
                    animator.SetBool("Walking", true);

                }
                if (Time.time > LastAttackTime + AttackDelay) // 
                {
                    AttackPlayer();
                }
                break;
            case State.ReturnToStart:
                float ReachedposDist2 = 2f;
                agent.SetDestination(startpos);
                if (Vector3.Distance(transform.position, startpos) < ReachedposDist2)
                {
                    state = State.Idle;
                }
                break;
        }
    }

    public void AttackPlayer()
    {
        LastAttackTime = Time.time;
        player.GetComponent<playerMovement>().TakeDamage(Attackdmg);

        animator.SetTrigger("Attacking");
        AudioUtility.CreateSFX(attackSFX, transform.position, AudioUtility.AudioGroups.Enemy, 10.0f);
    }

    private Vector3 GetRoamingpos()
    {
        Vector3 roampos = startpos + GetRandomDir() * Random.Range(5f, 15f);

        return roampos;
    }

    public static Vector3 GetRandomDir()
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    private void FindTarget(Vector2 dir)
    {
        if (Vector3.Distance(transform.position, player.position) < Targetrange)
        {
            //if player wihtin target range
            state = State.Chase;
            animator.SetBool("Walking", true);
        }
    }
}

