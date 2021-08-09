using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private Transform player;

    float LastAttackTime = 0f;

    public GameObject ArrowPrefab;
    public AudioClip attackSFX;

    [SerializeField]
    float Attackdmg;
    [SerializeField]
    float AttackDelay;
    [SerializeField]
    float Attackrange;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time > LastAttackTime + AttackDelay && Vector3.Distance(transform.position, player.position) < Attackrange) // 
        {
            AttackPlayer();
        }
    }

    public void AttackPlayer()
    {
        LastAttackTime = Time.time;
        Vector3 playerdir = (player.position - transform.position).normalized;

        GameObject arrow = Instantiate(ArrowPrefab, transform.position, Quaternion.identity);
        arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(playerdir.x, playerdir.y) * 14;
        arrow.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(playerdir.y, playerdir.x) * Mathf.Rad2Deg);
        arrow.GetComponent<EnemyArrow>().damage = Attackdmg;
        //animator.SetTrigger("Attacking");
        AudioUtility.CreateSFX(attackSFX, transform.position, AudioUtility.AudioGroups.Enemy, 10.0f);

    }
}
