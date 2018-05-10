using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls enemy initialization and action
 */
namespace Deprecated {
	public class EnemyScript : MonoBehaviour {

		public string Name;
		public int Strength;
		public int Defense;
		public int Speed;
		public int MaxHP;

		private int _currentHP;

		// Use this for initialization
		void Start() {
			_currentHP = MaxHP;
		}

//	// Update is called once per frame
		// Commented out because we shouldn't need this for a turn-based game
//	void Update () {
//          if (health <= 0) this.gameObject.SetActive(false);
//	}

		// Determines what action the enemy should take.
		// Right now, this is always attack.
		public string Action() {
			return "attack";
		}

		// For the time being, this isn't too necessary--it just returns 1.
		// In the future, it'll take into account the attack the player uses + their strength
		// to determine the base damage of the enemy's attack.
		public int CalculateDamage() {
			return 1;
		}

		public int CurrentHP {
			get { return _currentHP; }
			set { _currentHP = value; }
		}

		// Handles checking whether or not the enemy is currently alive
		public bool IsAlive() {
			return _currentHP > 0;
		}

		// Handles the enemy taking damage
		// In the future should instead take defense into acount
		public void TakeDamage(int damage) {
			_currentHP -= damage;

			if (_currentHP <= 0) {
				_currentHP = 0;
				gameObject.SetActive(false);
			}
		}
	}
}
