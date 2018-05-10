using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deprecated {
	public class PlayerScript : MonoBehaviour {

		public string Name;
		public int Strength;
		public int Defense;
		public int Speed;
		public int MaxHP;

		private int _currentHP;
		private bool _isDefending;

		// Use this for initialization
		void Start() {
			_currentHP = MaxHP;
			IsDefending = false;
		}

//	Commented this out bc with turn-based combat we shouldn't ever need Update
//  Instead, setActive(false) should be called on the function for taking damage
//	// Update is called once per frame
//	void Update () {
//          //if (health <= 0) this.gameObject.SetActive(false);
//    }

		// For the time being, this isn't too necessary--it just returns 1.
		// In the future, it'll take into account the attack the player uses + their strength
		// to determine the base damage of the player's attack.
		public int CalculateDamage() {
			return 1;
		}

		public int CurrentHP {
			get { return _currentHP; }
			set { _currentHP = value; }
		}

		// Getter & Setter for _isDefending
		// To use these, just type in (the object).IsDefending ( + "= true/false" for set)
		public bool IsDefending {
			get { return _isDefending; }
			set { _isDefending = value; }
		}

		// Checks whether or not the player is alive
		public bool IsAlive() {
			return _currentHP > 0;
		}

		// Processor for a player taking damage
		// Should be modified in the future to calculate this damage based on defense
		public void TakeDamage(int damage) {
			_currentHP -= damage;

			if (IsDefending) {
				_currentHP++;
			}

			if (_currentHP <= 0) {
				_currentHP = 0;
				gameObject.SetActive(false);
			}
		}
	}
}
