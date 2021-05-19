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
    //[SerializeField]
    //float Attackrange = 3f;

    //States
    private enum State
    {
        Roaming,
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
            case State.Chase:
                agent.SetDestination(player.position);
                /*      if (Vector3.Distance(transform.position, player.position) < Attackrange)
                      {
                          //if player wihtin attack range
                          state = State.Attack;
                      }*/
                if (Vector3.Distance(transform.position, player.position) > Targetrange)  //if reached roam pos
                {
                    state = State.ReturnToStart;
                    sprite.color = new Color(1, 1, 1, 1);
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

    private Vector3 GetRoamingpos()
    {
        return startpos + GetRandomDir() * Random.Range(5f, 15f);
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
            sprite.color = new Color(1, 0, 0, 1);
        }
    }
}
