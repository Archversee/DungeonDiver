using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyAI : MonoBehaviour
{

    private Transform player;
    private NavMeshAgent agent;
    private Vector3 startpos;
    private Vector3 roampos;
    SpriteRenderer sprite;

    [SerializeField]
    float Targetrange = 5f;
    [SerializeField]
    float Attackrange = 2f;
    [SerializeField]
    int Attackdmg = 5;
    [SerializeField]
    float AttackDelay = 3f;
    float LastAttackTime = 0f;

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
    }

    private void Update()
    {
        switch (state)
        {
            default:
            case State.Roaming:
                agent.SetDestination(roampos);
                float ReachedposDist = 2f;
                if (Vector3.Distance(transform.position, roampos) < ReachedposDist)  //if reached roam pos
                {
                    roampos = GetRoamingpos();
                }
                FindTarget();
                break;
            case State.Idle:
                FindTarget();
                break;
            case State.Chase:
                agent.SetDestination(player.position);
                if (Vector3.Distance(transform.position, player.position) < Attackrange)
                {
                    //if player wihtin attack range
                    state = State.Attack;
                    agent.isStopped = true;
                    sprite.color = Color.red;
                }
                if (Vector3.Distance(transform.position, player.position) > Targetrange) 
                {
                    state = State.ReturnToStart;
                    sprite.color = new Color(1, 1, 1, 1);
                }
                break;
            case State.Attack:
                if (Vector3.Distance(transform.position, player.position) > Attackrange)
                {
                    state = State.Chase;
                    sprite.color = Color.yellow;
                    agent.isStopped = false;
                }
                if (Time.time > LastAttackTime + 0.1 && sprite.color == Color.green) // 
                {
                    sprite.color = Color.red;
                }
                if (Time.time > LastAttackTime + AttackDelay) // 
                {
                    AttackPlayer();
                }
                break;
            case State.ReturnToStart:
                float ReachedposDist2 = 2f;
                agent.SetDestination(startpos);
                if(Vector3.Distance(transform.position, startpos) < ReachedposDist2)
                {
                    state = State.Roaming;
                }
                //FindTarget();
                break;
        }
    }

    public void AttackPlayer()
    {
        LastAttackTime = Time.time;
        player.GetComponent<playerMovement>().TakeDamage(Attackdmg);
        sprite.color = Color.green;
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

    private void FindTarget()
    {
        if(Vector3.Distance(transform.position, player.position) < Targetrange)
        {
            //if player wihtin target range
            state = State.Chase;
            sprite.color = Color.yellow;
        }
    }
}
