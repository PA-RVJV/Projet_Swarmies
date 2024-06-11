using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PS.Units
{
    public class UnitStatDisplay : MonoBehaviour
    {
        public float maxHealth, armor, currentHealth;

        [SerializeField] private Image healthBarAmount;

        private bool isPlayerUnit = false;

        private Camera _camera;
        
        private void Start()
        {
            _camera = Camera.main;
            
            try
            {
                maxHealth = gameObject.GetComponentInParent<Player.PlayerUnit>().baseStats.health;
                armor = gameObject.GetComponentInParent<Player.PlayerUnit>().baseStats.armor;
                isPlayerUnit = true;
            }
            catch (Exception)
            {
                Debug.Log("No player Unit. Trying Enemy Unit...");
                try
                {
                    maxHealth = gameObject.GetComponentInParent<Enemy.EnemyUnit>().baseStats.health;
                    armor = gameObject.GetComponentInParent<Enemy.EnemyUnit>().baseStats.armor;
                    isPlayerUnit = false;
                }
                catch (Exception)
                {
                    try
                    {
                        maxHealth = gameObject.GetComponentInParent<Building>().baseStats.health;
                        armor = gameObject.GetComponentInParent<Building>().baseStats.armor;
                        isPlayerUnit = false;
                    }
                    catch (Exception)
                    {
                        Debug.Log("No Unit Scripts found!");
                    }
                }
            }

            currentHealth = maxHealth;
        }
        
        private void Update()
        {
            HandleHealth();
        }
        
        public void TakeDamage(float damage)
        {
            float totalDamage = damage - armor;
            currentHealth -= totalDamage;
        }
        
        private void HandleHealth()
        {
            gameObject.transform.LookAt(gameObject.transform.position + 
                                             _camera.transform.rotation * Vector3.forward, 
                _camera.transform.rotation * Vector3.up);
            
            healthBarAmount.fillAmount = currentHealth / maxHealth;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (isPlayerUnit)
            {   
                
                //InputHandlers.InputManager.instance.selectedUnits.Remove(gameObject.transform);
                Destroy(gameObject.transform.parent.gameObject);
                
            }
            else
            {
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
    }

}
