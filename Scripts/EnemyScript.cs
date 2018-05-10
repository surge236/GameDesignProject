using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls enemy initialization and action
 */
public class EnemyScript : MonoBehaviour {

     public string Name;
     public int Strength;
     public double strengthGrowth;
     public int Defense;
     public double defenseGrowth;
     public int Speed;
     public double speedGrowth;
     public int MaxHealth;
     public double healthGrowth;
     private int _currentHealth;

	private bool _isForward;

	// This is used to store an enemy's defense pre-debuff
	// This is easier than calculation since the debuff is variable based on the spell
	private int _defenseStorage;

	void Start () {
		_currentHealth = MaxHealth;
		_defenseStorage = 0;
		_isForward = false;
	}

     // Pass in the index of the active zone (so we know which world we are on sequentially speaking)
     // The counting for the index passed in should be 1 if world 1, not 0 for world 1 (just to clarify)
     // Changes the enemy's stats based on the number of worlds that have been beaten.
     public void setEnemyStats(int zoneIndex) {
          // change the strength
          double growthAmount = zoneIndex * strengthGrowth;
          growthAmount *= Strength;
          double newStrength = Strength + growthAmount;
          Strength = Mathf.CeilToInt((float) newStrength);
          Debug.Log("Zone Index: " + zoneIndex + " newStrength: " + Strength + " Enemy: " + Name);

          // change the defense
          growthAmount = zoneIndex * defenseGrowth;
          growthAmount *= Defense;
          double newDefense = Defense + growthAmount;
          Defense = Mathf.CeilToInt((float)newDefense);
          Debug.Log("Zone Index: " + zoneIndex + " newDefense: " + Defense + " Enemy: " + Name);

          // change the speed
          growthAmount = zoneIndex * speedGrowth;
          growthAmount *= Speed;
          double newSpeed = Speed + growthAmount;
          Speed = Mathf.CeilToInt((float)newSpeed);
          Debug.Log("Zone Index: " + zoneIndex + " newSpeed: " + Speed + " Enemy: " + Name);

          // change the health
          growthAmount = zoneIndex * healthGrowth;
          growthAmount *= MaxHealth;
          double newMaxHealth = MaxHealth + growthAmount;
          MaxHealth = Mathf.CeilToInt((float)newMaxHealth);
          Debug.Log("Zone Index: " + zoneIndex + " newHealth: " + MaxHealth + " Enemy: " + Name);

          _currentHealth = MaxHealth; // Since the health has changed let's be sure the current health does too.
     }

	public void debuff(Spell spellUsed) {
		_defenseStorage = Defense;
		Defense = (int) Math.Round(Defense * spellUsed.Multiplier, MidpointRounding.AwayFromZero);
	}

	public void restoreDebuff() {
		Defense = _defenseStorage;
	}
     
	public int CurrentHealth {
		get { return _currentHealth; }
		set { _currentHealth = value; }
	}

	public void takeDamage(int damage) {
		_currentHealth -= damage;
		if (_currentHealth < 0) {
			_currentHealth = 0;
		}
	}
	
	public bool IsAlive() {
		return _currentHealth > 0;
	}
	
	public void MoveForward() {
		if (!_isForward)  {
			var moveRB = gameObject.GetComponent<Rigidbody2D>();
			var newPos = new Vector2(moveRB.position.x - 1, moveRB.position.y);
			moveRB.MovePosition(newPos);

			_isForward = true;
		}
	}

	public void MoveBack() {
		if (_isForward)  {
			var moveRB = gameObject.GetComponent<Rigidbody2D>();
			var newPos = new Vector2(moveRB.position.x + 1, moveRB.position.y);
			moveRB.MovePosition(newPos);

			_isForward = false;
		}
	}
}
