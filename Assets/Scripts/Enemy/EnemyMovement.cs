﻿using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Player;
using Unity.Mathematics;

namespace Enemy
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private EnemyScript enemy;
        [SerializeField] private float raycastDistance = 15;

        private NavMeshAgent agent;
        private PlayerScript player;
        private Rigidbody2D enemyRb;
        private float strafeTimer;
        private float onFireTimer = 0.1f;
        private bool strafeRightDirection = true;
        
        public bool IsOnFire { get; private set; }

        private void Awake()
        {
            agent = enemy.Agent;
            enemyRb = enemy.EnemyRigidbody2D;
        }

        private void Start()
        {
            agent.updateUpAxis = false;

            player = enemy.Target;
            strafeTimer = enemy.EnemySc.StrafeTime;
            agent.speed = enemy.EnemySc.Machine.MaxSpeed;
            agent.acceleration = enemy.EnemySc.Machine.Acceleration * 100;
        }

        private void RotateEnemy()
        {
            var enemyPosition = enemyRb.position;
            var playerPosition = player.PlayerRigidbody2D.position;
            var direction = (playerPosition - enemyPosition).normalized;
            RotateEnemyOnDirection(direction);
        }

        private Vector2 GetShootVector(float projSpeed, Vector3 targetVelocity, Vector3 direction)
        {
            var sin = Vector3.Cross(targetVelocity, direction).z / (direction.magnitude * projSpeed);

            if (math.abs(sin) > 1)
                return Vector2.zero;
            
            var cos = math.sqrt(1 - sin * sin);
            var tsign = projSpeed * direction.magnitude * cos - Vector2.Dot(targetVelocity, direction);

            return tsign < 0
                ? Vector2.zero
                : Quaternion.AngleAxis(Mathf.Rad2Deg * -math.asin(sin), Vector3.forward) * direction;
        }
        
        private void RotateEnemyForPreFire()
        {
            var playerPosition = player.PlayerRigidbody2D.position;
            var enemyPosition = enemy.EnemyRigidbody2D.position;
            var playerSpeed = player.PlayerRigidbody2D.linearVelocity;
            var enemyBulletSpeed = enemy.EnemySc.Weapon.Projectile.BulletSpeed;
            
            var direction = GetShootVector(enemyBulletSpeed, playerSpeed, playerPosition - enemyPosition);
            RotateEnemyOnDirection(direction);
        }

        private void RotateEnemyOnDirection(Vector2 direction)
        {
            var targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            enemy.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }
        
        private float DistanceToTarget()
        {
            var enemyPosition = enemy.EnemyRigidbody2D.position;
            var playerPosition = enemy.Target.PlayerRigidbody2D.position;
            var distanceToPlayer = Vector2.Distance(playerPosition, enemyPosition);
            
            return distanceToPlayer;
        }

        private void Strafe()
        {
            var moveDirection = strafeRightDirection ? transform.right : -transform.right;
            var velocity = moveDirection * enemy.EnemySc.StrafeSpeed;
            agent.isStopped = true;
            UpdateStrafe();
            
            enemy.EnemyRigidbody2D.linearVelocity = velocity;
        }

        private void UpdateStrafe()
        {
            strafeTimer -= Time.deltaTime;
            if (strafeTimer <= 0)
            {
                strafeRightDirection = !strafeRightDirection;
                strafeTimer = enemy.EnemySc.StrafeTime;
            }
        }

        public void CheckIsOnFire()
        {
            player = enemy.Target;
            var enemyPosition = enemy.EnemyRigidbody2D.position;
            var playerRig = player.PlayerRigidbody2D;
            var playerPosition = playerRig.position;
            var direction = (playerPosition - enemyPosition).normalized;
            var raycast = Physics2D.RaycastAll(enemyPosition, direction, raycastDistance);

            raycast = raycast.Where(ray => !ray.collider.CompareTag("Enemy")).ToArray();
            if (raycast.Length != 0 && raycast.FirstOrDefault().collider.CompareTag("Player"))
            {
                onFireTimer = 0.1f;
                IsOnFire = true;
            }
            else
            {
                onFireTimer -= Time.deltaTime;
                if (onFireTimer <= 0)
                    IsOnFire = false;
            }
        }

        private void MoveBack()
        {
            var enemyPosition = enemy.EnemyRigidbody2D.position;
            var playerPosition = player.PlayerRigidbody2D.position;
            var directionToPlayer = (playerPosition - enemyPosition).normalized;
            var velocity = -directionToPlayer * enemy.EnemySc.StrafeSpeed;
            agent.isStopped = true;
            enemy.EnemyRigidbody2D.linearVelocity = velocity;
        }

        private void NavMeshMoveEnemy() => agent.SetDestination(player.transform.position);

        protected internal void MoveUpdate()
        {
            if (IsOnFire && enemy.EnemySc.ShootWithAdvance)
                RotateEnemyForPreFire();
            else 
                RotateEnemy();
            
            if (!IsOnFire || DistanceToTarget() > enemy.EnemySc.MoveForwardDistance)
            {
                agent.isStopped = false;
                NavMeshMoveEnemy();
            }
            else if (IsOnFire && DistanceToTarget() < enemy.EnemySc.MoveBackDistance)
            {
                MoveBack();
            }
            else
                Strafe();
        }
    }
}