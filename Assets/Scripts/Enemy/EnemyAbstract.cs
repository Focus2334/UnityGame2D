﻿using Player;
using UnityEngine;
using UnityEngine.AI;
using Weapon;
using ScObjects;
using UI;

namespace Enemy
{
    public abstract class EnemyAbstract : MonoBehaviour, ICanTakeDamage
    {
        [SerializeField] private EnemyMovement movement;
        [SerializeField] private WeaponScript weapon;
        [SerializeField] private WeaponScObject weaponScObject;
        [SerializeField] private EnemyScObject enemySc;
        [SerializeField] private Rigidbody2D enemyRigidbody2D;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private PlayerScript target;
        [SerializeField] private GameObject points;
        [SerializeField] private bool onPreFire;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private GameObject elimEffect;
        
        private float currentHp;
        
        public bool OnPreFire => onPreFire;
        
        public Rigidbody2D EnemyRigidbody2D => enemyRigidbody2D;
        
        public NavMeshAgent Agent => agent;
        
        public PlayerScript Target { get => target; set => target = value; }
        
        public void SetScObject(EnemyScObject newEnemySc) => enemySc = newEnemySc;

        public EnemyScObject EnemySc => enemySc;

        protected void Start()
        {
            CurrentValues.IncrementEnemies();
            currentHp = enemySc.Machine.HitPoints;
            weapon.SetScObject(enemySc.Weapon);
            gameObject.GetComponent<SpriteRenderer>().sprite = enemySc.Machine.Sprite;
        }

        protected internal void Update()
        {
            if (Target is not null)
            {
                movement.CheckIsOnFire();
                movement.MoveUpdate();
                if (movement.IsOnFire)
                    weapon.Fire();
            }
        }

        public bool TakeDamage(float value)
        {
            Hitmarker.ChangeCursor();
            currentHp -= value;
            Instantiate(hitEffect, transform.position, new Quaternion());
            if (currentHp <= 0) 
                Death();
            return true;
        }
        
        private bool Death()
        {
            CurrentValues.DecrementEnemies();
            Instantiate(points, transform.position, transform.rotation);
            Instantiate(elimEffect, transform.position, new Quaternion());
            Destroy(gameObject);
            return true;
        }
    }
}   