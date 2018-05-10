using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

     public string Name;
     public int baseStrength;
     private int Strength;
     public double strengthGrowth;
     public int baseDefense;
     private int Defense;
     public double defenseGrowth;
     public int baseSpeed;
     private int Speed;
     public double speedGrowth;
     public int baseMaxHealth;
     public int MaxHealth;
     public double healthGrowth;
     public int _currentHealth;
     
     private bool _isDefending;
     private bool _isForward;
     
     private PlayerScript _allyDefending;
     private EnemyScript _enemyDebuffing;
     
     public int ClassKey;
     private Spell[] _spells;

	// Use this for initialization
	void Start () {
          Strength = baseStrength;
          Defense = baseDefense;
          Speed = baseSpeed;
          MaxHealth = baseMaxHealth;
          _currentHealth = MaxHealth;
	     
          _isDefending = false;
	     _isForward = false;
	     
	     _allyDefending = null;
	     _enemyDebuffing = null;
	     
	     _spells = new Spell[0];
	}

     // Pass in the index of the active zone (so we know which world we are on sequentially speaking)
     // The counting for the index passed in should be 1 if world 1, not 0 for world 1 (just to clarify)
     // Changes the player's stats based on the number of worlds that have been beaten.
     public void setPlayerStats(int zoneIndex) {
          // change the strength
          double growthAmount = zoneIndex * strengthGrowth;
          growthAmount *= baseStrength;
          Debug.Log("Growth Amount: " + growthAmount);
          double newStrength = baseStrength + growthAmount;
          Strength = Mathf.CeilToInt((float)newStrength);
          Debug.Log("Zone Index: " + zoneIndex + " newStrength: " + Strength + " Player: " + Name);

          // change the defense
          growthAmount = zoneIndex * defenseGrowth;
          growthAmount *= baseDefense;
          double newDefense = baseDefense + growthAmount;
          Defense = Mathf.CeilToInt((float)newDefense);
          Debug.Log("Zone Index: " + zoneIndex + " newDefense: " + Defense + " Player: " + Name);

          // change the speed
          growthAmount = zoneIndex * speedGrowth;
          growthAmount *= baseSpeed;
          double newSpeed = baseSpeed + growthAmount;
          Speed = Mathf.CeilToInt((float)newSpeed);
          Debug.Log("Zone Index: " + zoneIndex + " newSpeed: " + Speed + " Player: " + Name);

          // change the max health (keep the current health the same, that way they can potentially
          // heal up higher using healing spells but their actual current health doesn't change).
          growthAmount = zoneIndex * healthGrowth;
          growthAmount *= baseMaxHealth;
          double newMaxHealth = baseMaxHealth + growthAmount;
          MaxHealth = Mathf.CeilToInt((float)newMaxHealth);
          Debug.Log("Zone Index: " + zoneIndex + " newHealth: " + MaxHealth + " Player: " + Name);

     }
     
     public int CurrentHealth {
          get { return _currentHealth; }
          set { _currentHealth = value; }
     }

     // Code reworks required the rebranding of some variable as public to private,
     // rather then mess around with which ones were changed, I'm just gonna add a
     // getter and setter for all of them.
     public int getMaxHealth() { return MaxHealth; }
     public void setMaxHealth(int value) { MaxHealth = value; }
     public int getStrength() { return Strength; }
     public void setStrength(int value) { Strength = value; }
     public int getDefense() { return Defense; }
     public void setDefense(int value) { Defense = value; }
     public int getSpeed() { return Speed; }
     public void setSpeed(int value) { Speed = value; }

     public void takeDamage(int damage) {
          _currentHealth -= damage;
          if (_currentHealth < 0) {
               _currentHealth = 0;
          }
          else if (_currentHealth > MaxHealth) {
               _currentHealth = MaxHealth;
          }
     }

     public void turnReset() {
          if (_isDefending) {
               Defense /= 2;
               _isDefending = false;
          }
          
          if (_allyDefending != null) {
               _allyDefending.Defense /= 2;
               _allyDefending = null;
          }
          
          if (_enemyDebuffing != null) {
               _enemyDebuffing.restoreDebuff();
               _enemyDebuffing = null;
          }
     }
     
     // Toggles the player's state of defending, if they are defending
     // stops them from defending, otherwise, has them defend.
     public void defend() {
          Defense *= 2;
          _isDefending = true;
     }
     
     public bool IsDefending {
          get { return _isDefending; }
     }

     public void DefendAlly(GameObject ally) {
          if (Util.IsPlayer(ally)) {
               _allyDefending = ally.GetComponent<PlayerScript>();
               _allyDefending.Defense *= 2;
          }
     }

     public bool IsAlive() {
          return _currentHealth > 0;
     }

     public Spell[] Spells {
          get { return _spells; }
     }
     
     public void LearnSpell(Spell toLearn) {
          var currentLength = _spells.Length;
          
          var temp = new Spell[currentLength + 1];
          for (var i = 0; i < currentLength; i++) {
               temp[i] = _spells[i];
          }
          temp[temp.Length - 1] = toLearn;

          _spells = temp;
     }

     public void MoveForward() {
          if (!_isForward)  {
               var moveRB = gameObject.GetComponent<Rigidbody2D>();
               var newPos = new Vector2(moveRB.position.x + 1, moveRB.position.y);
               moveRB.MovePosition(newPos);

               _isForward = true;
          }
     }

     public void MoveBack() {
          if (_isForward)  {
               var moveRB = gameObject.GetComponent<Rigidbody2D>();
               var newPos = new Vector2(moveRB.position.x - 1, moveRB.position.y);
               moveRB.MovePosition(newPos);

               _isForward = false;
          }
     }
}
