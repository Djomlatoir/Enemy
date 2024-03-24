using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy : NetworkBehaviour
{
    public NavMeshAgent agent;

    // public ClientNetworkTransform player;
    public Transform player;

    public Transform testplay;

    public GameObject[] AllObjects;
    public GameObject NearestOBJ;

    float distance;
    float nearestDistance = 10000;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
   // public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Start()
    {
        
    }


    private void Awake()
    {
        testplay = GameObject.FindGameObjectWithTag("TestPlayer").transform;

        // player = GameObject.Find("Player(Clone)").transform;



        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        AllObjects = GameObject.FindGameObjectsWithTag("Player");

       
        //  if (GameObject.Find("Player(Clone)") != null)
        //  player = GameObject.Find("Player(Clone)").transform;
        // Debug.Log("PlayerTest = " + testplay.transform.position);
        //player = player.transform;
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        //  Vector3 playerPosi = player.transform.position;
        //   Debug.Log("Player transform =" + player.transform.position);
        // player = GameObject.FindWithTag("Player");
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
        { 

            Patroling(); 
        
        }
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
        for (int i = 0; i < AllObjects.Length; i++)
        {
            distance = Vector3.Distance(this.transform.position, AllObjects[i].transform.position);
           // Debug.Log("Najblizi " + AllObjects[i]);
            if (distance < nearestDistance)
            {
                NearestOBJ = AllObjects[i];
                nearestDistance = distance;
              //  Debug.Log("Najblizi novi " + AllObjects[i]);

            }
            player = NearestOBJ.transform;
        }
        
    }

    private void Patroling()
    {
       
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
         agent.SetDestination(player.position);
      // Debug.Log("Player transform ="+ player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            ///Attack code here
          //  Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
          //  rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
          //  rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

  
}
