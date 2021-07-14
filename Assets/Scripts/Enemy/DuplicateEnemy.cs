using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DuplicateEnemy : MonoBehaviour
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
    [SerializeField]
    public float stage;

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
        state = State.Roaming;
        sprite = GetComponent<SpriteRenderer>();
        defaultspeed = agent.speed;
        mudmovespeed = agent.speed * 0.5f;

        gameController = GameObject.FindGameObjectWithTag("GameController");

        animator = GetComponent<Animator>();

        if(stage == 2)
        {
            Vector3 local = transform.localScale;
            transform.localScale = new Vector3(local.x * 0.75f, local.y * 0.75f, local.z * 0.75f);
            Attackrange *= 0.75f;
            Attackdmg *= 0.75f;
            GetComponent<Health>().maxhealth -= (GetComponent<Health>().maxhealth * 0.75f);
            GetComponent<Health>().currhealth = GetComponent<Health>().maxhealth;
        }
        else if (stage == 3)
        {
            Vector3 local = transform.localScale;
            transform.localScale = new Vector3(local.x * 0.50f, local.y * 0.50f, local.z * 0.50f);
            Attackrange *= 0.5f;
            Attackdmg *= 0.5f;
            GetComponent<Health>().maxhealth -= (GetComponent<Health>().maxhealth * 0.50f);
            GetComponent<Health>().currhealth = GetComponent<Health>().maxhealth;
        }

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
                FindTarget(playerdir);
                animator.SetBool("Walking", false);
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
                    state = State.Roaming;
                }
                break;
        }
    }

    public void AttackPlayer()
    {
        LastAttackTime = Time.time;
        player.GetComponent<playerMovement>().TakeDamage(Attackdmg);

        animator.SetTrigger("Attacking");
    }

    public void DuplicateSelf()
    {
        if (stage < 3)
        {
            float offset = 0.1f;
            float temp = stage + 1;
            for (float i = 0; i <= stage; i++)
            {
                GameObject enemy = Instantiate(DuplicateEnemyAI, new Vector3(transform.position.x + offset, transform.position.y + offset, 0), Quaternion.identity);
                enemy.GetComponent<DuplicateEnemy>().stage = temp;
                offset += 0.1f;
            }
        }
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

