using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
	private float movementSpeed, distance, idleTimer_Script = 3;
	public float editor_WalkSpeed = 1f, editor_RunSpeed = 2f, editor_idleTimer = 3, 
		walkingRange = 50, walkDetection = 5, runDetection = 10, minimalDetection = 5;

	[SerializeField] private NavMeshAgent agent;
	public Animator enemyAnimator;

	[SerializeField] private EnemyState currentState;
	[SerializeField] private Transform player;

	public static int playerDecibel = 0;

	private void Start()
	{
		playerDecibel = 0;
		SetCurrentState(EnemyState.Idle);
		idleTimer_Script = editor_idleTimer;
	}

	private void Update()
	{
		agent.speed = movementSpeed;
		enemyAnimator.SetFloat("velocity", agent.velocity.magnitude);
		distance = Vector3.Distance(player.position, agent.transform.position);
		EnemyBehaviour();
		SoundDetection();
	}

	private void SoundDetection()
    {
        if (distance <= runDetection && distance > walkDetection && playerDecibel == 2) SetCurrentState(EnemyState.Chase);
        else if (distance <= walkDetection && distance > minimalDetection && playerDecibel != 0) SetCurrentState(EnemyState.Chase);
		else if (distance <= minimalDetection) SetCurrentState(EnemyState.Chase);
	}

	private void EnemyBehaviour()
	{
		switch (currentState)
		{
			case EnemyState.Idle:
				IdleBehaviour();
				break;
			case EnemyState.Roaming:
				RoamingBehaviour();
				break;
			case EnemyState.Chase:
				ChaseBehaviour();
				break;
			default:
				break;
		}
	}

	private void IdleBehaviour()
    {
		if(agent.hasPath == false)
        {
			movementSpeed = 0;
			idleTimer_Script -= Time.deltaTime;
			if (idleTimer_Script <= 0)
			{
				idleTimer_Script = editor_idleTimer;
				SetCurrentState(EnemyState.Roaming);
			}
		}
	}

	private void RoamingBehaviour()
	{
		if (agent.hasPath == true)
		{
			if (agent.remainingDistance > 4)
			{
				movementSpeed += Time.deltaTime;
				if (movementSpeed >= editor_WalkSpeed) movementSpeed = editor_WalkSpeed;
			}
			else if (agent.remainingDistance <= 4)
			{
				movementSpeed -= Time.deltaTime * 0.5f;
				if (movementSpeed <= 0.2f) movementSpeed = 0.2f;
			}
		}
		else if (agent.hasPath == false)
		{
			if (movementSpeed == 0) agent.SetDestination(RandomPosition());
			else if (movementSpeed != 0) SetCurrentState(EnemyState.Idle);
		}
		
		if (distance <= runDetection && playerDecibel != 0 || distance <= minimalDetection)
		{
			agent.SetDestination(player.position);
			SetCurrentState(EnemyState.Chase);
		}
	}

	private void ChaseBehaviour()
	{
		if(agent.hasPath == true)
        {
			movementSpeed += Time.deltaTime;
			if (movementSpeed >= editor_RunSpeed) movementSpeed = editor_RunSpeed;
		}
		else if (agent.hasPath == false)
		{
			movementSpeed -= Time.deltaTime;
			if (movementSpeed <= editor_WalkSpeed)
			{
				movementSpeed = editor_WalkSpeed;
				SetCurrentState(EnemyState.Roaming);
			}
		}

		if (distance <= runDetection && distance > minimalDetection && playerDecibel != 0) agent.SetDestination(player.position);
		else if (distance <= minimalDetection) agent.SetDestination(player.position);
	}

	private void SetCurrentState(EnemyState newState)
	{
		currentState = newState;

		switch (currentState)
		{
			case EnemyState.Idle:
				break;
			case EnemyState.Roaming:
				break;
			case EnemyState.Chase:
				break;
			default:
				break;
		}
	}

	private Vector3 RandomPosition()
	{
		Vector3 randomPos = UnityEngine.Random.insideUnitCircle * walkingRange;

		Vector3 newRandomPosition = transform.position + randomPos;

		return newRandomPosition;
	}

	private enum EnemyState
	{
		Idle,
		Roaming,
		Chase
	}
}