using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// NOTE: how to pull the gameobject stats: enemy_3.GetComponent<EnemyScript>().getSpeed();
// NOTE: for each action, be sure to update the eventInfo and activeInfo
// NOTE: Remember to update activeInfo whenever the activeActor variable is incremented.

namespace Deprecated {
	public class BattleController : MonoBehaviour {

		public Text EventInfo; // Displays the battle text
		public Text ActiveInfo; // Dispays the active actor
		public GameObject PlayerMenu;
		public GameObject ContinueButton;
	
		// The player's party
		public GameObject Player1;
		public GameObject Player2;
		public GameObject Player3;
		public GameObject Player4;
	
		// The enemy's party
		public GameObject Enemy1;
		public GameObject Enemy2;
		public GameObject Enemy3;
	
		// Array of battling characters
		private GameObject[] _turnOrder;
		private int _currentActorIndex; // currently active in the turn order 
		private int _numActors; // number of characters participating in the match
	
		// Use this for initialization
		private void Start () {
			Random.InitState((int) DateTime.Now.Ticks & 0x0000FFFF);
			DeactivateButtons();
		
			GameObject[] players = {Player1, Player2, Player3, Player4,
				Enemy1, Enemy2, Enemy3};
			SetNumActors(players); // Sets the numActors global
			CreateTurnOrder(players); // Sets the array to the necessary values.
		
			Battle(); // Starts the battle system
		}

		private void DeactivateButtons() {
			PlayerMenu.gameObject.SetActive(false);
			PlayerMenu.GetComponentInChildren<AttackButtonScript>().DeactivateButtons();
			ContinueButton.gameObject.SetActive(false);
		}
	
		// Get the number of actors participating in battle by checking if
		// each of the player and enemy gameobjects are active, increment
		// numActors by 1 for each that is.
		private void SetNumActors(GameObject[] actors) {
			var count = 0;
			foreach (GameObject actor in actors) {
				if (actor != null && actor.gameObject.activeSelf) {
					count++;
				}
			}
			_numActors = count;
		}
	
		// Create the turn order by initializing the turnOrder array to
		// the size of numActors - 1. Then, setting the active gameobjects
		// within the array. Next, sort the array based on the speed of the
		// gameobjects. Remember to set the activeActor to 0.
		private void CreateTurnOrder(GameObject[] actors) {
			_turnOrder = new GameObject[_numActors];
			_currentActorIndex = 0;
		
			var index = 0;
			foreach (var actor in actors) {
				if (actor != null && actor.gameObject.activeSelf) {
					_turnOrder[index] = actor;
					index++;
				}
			}
		
			for (var i = 0; i < _numActors; i++) {
				for (var j = i + 1; j < _numActors; j++) {
					if (GetSpeed(_turnOrder[i]) < GetSpeed(_turnOrder[j])) {
						var temp = _turnOrder[i];
						_turnOrder[i] = _turnOrder[j];
						_turnOrder[j] = temp;
					}
				}
			}
		}

		// Handles going to the next actor's turn
		private void NextActor() {
			_currentActorIndex = ++_currentActorIndex % _numActors;
			DeactivateButtons();
			Battle();
		}

		// Finds the currently active enemies
		// Necessary for Attack button to work properly
		public GameObject[] GetEnemies() {
			var activeEnemies = new GameObject[3];
		
			activeEnemies[0] = Enemy1;
			activeEnemies[1] = Enemy2;
			activeEnemies[2] = Enemy3;
		
			return activeEnemies;
		}
	
		// Check if the player's party is wiped by checking their health,
		// if the entire party is wiped, return true, else false.
		private bool BattleLose() {
			return !IsAlive(Player1) && !IsAlive(Player2) && !IsAlive(Player3) && !IsAlive(Player4);
		}
	
		// Check if the enemy's party is wiped by checking their health,
		// if the entire party is wiped, return true, else false.
		private bool BattleWin() {
			return !IsAlive(Enemy1) && !IsAlive(Enemy2) && !IsAlive(Enemy3);
		}
	
		// Calls the enemyAction function if the activeActor is an enemy, otherwise, if the active
		// actor is a player, turn on the action buttons (attack, defend, etc) to allow for player actions,
		// and wait for a button to call the playerAction function. (continue, attack target_1,
		// attack target_2, attack target_3, and defend buttons all call this function)
		private void Battle() {
			var currentActor = _turnOrder[_currentActorIndex];
			if (currentActor != null && currentActor.gameObject.activeSelf) {
				ChangeActiveText(currentActor);

				if (IsEnemy(currentActor)) {
					var action = currentActor.GetComponent<EnemyScript>().Action();
					EnemyAction(currentActor, action);
				}
				else if (IsPlayer(currentActor)) {
					PlayerMenu.gameObject.SetActive(true);
					currentActor.GetComponent<PlayerScript>().IsDefending = false;
					ChangeEventText("What will " + GetName(currentActor) + " do?");
				}
			}
			else {
				NextActor();
			}
		}
	
		// Action implications: 1 = attack enemy_1, 2 = attack enemy_2, 3 = attack enemy_3, 4 = defend
		// Should only be called by button presses (attack, defend, etc), Deactivate player action buttons,
		// Make sure to set the player
		// isDefending status to false and half their defense if it is currently true, then performs 
		// the player action
		// (if the player action is defend, make sure to set isDefending in player to true and double defense),
		// increments the activeActor, changes the event info, waits for the continue button to be
		// pressed, then recalls the battle function.
		public void PlayerAction(string action) {
			var currentActor = _turnOrder[_currentActorIndex];
		
			switch (action) {
				case "attack_enemy1": {
					PerformAttack(currentActor, Enemy1);
					break;
				}

				case "attack_enemy2": {
					PerformAttack(currentActor, Enemy2);
					break;
				}

				case "attack_enemy3": {
					PerformAttack(currentActor, Enemy3);
					break;
				}

				case "defend": {
					ChangeEventText(GetName(currentActor) + " is defending!");
					currentActor.GetComponent<PlayerScript>().IsDefending = true;
					break;
				}
			
				default: {
					ChangeEventText(GetName(currentActor) + " stood around doing nothing.");
					break;
				}
			}
			DeactivateButtons();
			DisplayContinue(false);
		}
	
		// Chooses a random player character, then attacks them, then increments the active actor
		// and waits for the continue button to be pressed thus calling the battle function.
		private void EnemyAction(GameObject enemy, string action) {
			switch (action) {
				case "attack": {
					GameObject target;
					do {
						target = _turnOrder[(int) (Random.value * _numActors)];
					} while (!IsPlayer(target) || !IsAlive(target));
				
					PerformAttack(enemy, target);
					break;
				}
				default: {
					ChangeEventText("Everyone just kinda sat around and stared at one another in complete awkwardness.");
					break;
				}
			}
			DisplayContinue(false);
		}

		// Displays the Continue Button
		// setBattleEnd is true when the Continue button displayed is specifically after a Battle Win/Loss
		private void DisplayContinue(bool setBattleOver) {
			ContinueButton.gameObject.SetActive(true);

			if (setBattleOver) {
				ContinueButton.gameObject.tag = "battle_over";
			}
		}

		//Handles after clicking the Continue Button
		public void ProcessContinue(bool battleEnd) {
			ContinueButton.gameObject.SetActive(false);
		
			if (battleEnd) {
				ContinueButton.gameObject.tag = "Untagged";
			
				//TODO: Do stuff with restarting battle
			}
			else {
				var win = BattleWin();
				var lose = BattleLose();

				if (win) {
					ChangeEventText("You defeated the enemies!");
					DisplayContinue(true);
				}
				else if (lose) {
					ChangeEventText("GAME OVER");
					DisplayContinue(true);
				}
				else {
					NextActor();
				}
			}
		}
	
		// Gets the attacker's attack and the defender's defense, then subtracts the attack from
		// the defense and deals that much damage to the defending actor. If the remaining health
		// of the defender is less than or equal to 0, call the battleWin and battleLose functions.
		void PerformAttack(GameObject attacker, GameObject defender) {
			var initHP = GetHP(defender);
		
			if (IsPlayer(attacker) && IsEnemy(defender)) {
				defender.GetComponent<EnemyScript>().TakeDamage(attacker.GetComponent<PlayerScript>().CalculateDamage());
			}
			else if (IsEnemy(attacker) && IsPlayer(defender)) {
				defender.GetComponent<PlayerScript>().TakeDamage(attacker.GetComponent<EnemyScript>().CalculateDamage());
			}

			var damageTaken = initHP - GetHP(defender);
			var displayString = GetName(attacker) + " attacked " + GetName(defender) + " for " + damageTaken + " damage!";
		
			if (!IsAlive(defender)) {
				displayString += "\n" + GetName(defender) + " died!";
			}
		
			ChangeEventText(displayString);
		}
	
		// Call whenever the activeActor is incremented, the function should change the active actor text.
		void ChangeActiveText(GameObject actor) {
			ActiveInfo.text = GetName(actor) + "'s Turn";
		}

		// Updates battle text at the bottom to the given string.
		void ChangeEventText(string textIn) {
			EventInfo.text = textIn;
		}
	
//	// **TEMP FUNCTION DO NOT IMPLEMENT YET**
//	// Disables unnecessary scene objects, enables necessary ones
//	void SetScene() {
//	
//	}
//	
//	// **TEMP FUNCTION DO NOT IMPLEMENT YET**
//	// Initializes the battle scene (does the same thing as
//	// start() just restated so that we can recreate battles.
//	void InitBattle() {
//	
//	}

		/************************************ Utility Functions ************************************/
		//These should maybe be moved to their own file at some point
	
		//Checks if the given actor is a player
		private static bool IsPlayer(GameObject actor) {
			return actor.GetComponent<PlayerScript>() != null;
		}

		//Checks is the given actor is an enemy
		private static bool IsEnemy(GameObject actor) {
			return actor.GetComponent<EnemyScript>() != null;
		}

		//Gets the name of the given actor
		private static string GetName(GameObject actor) {
			if (IsPlayer(actor)) {
				return actor.GetComponent<PlayerScript>().Name;
			}
			else if (IsEnemy(actor)) {
				return actor.GetComponent<EnemyScript>().Name;
			}
			return "";
		}
	
		private static int GetHP(GameObject actor) {
			if (IsPlayer(actor)) {
				return actor.GetComponent<PlayerScript>().CurrentHP;
			}
			else if (IsEnemy(actor)) {
				return actor.GetComponent<EnemyScript>().CurrentHP;
			}
			return 0;
		}
	
		//Gets the speed of the given actor
		private static int GetSpeed(GameObject actor) {
			if (IsPlayer(actor)) {
				return actor.GetComponent<PlayerScript>().Speed;
			}
			else if (IsEnemy(actor)) {
				return actor.GetComponent<EnemyScript>().Speed;
			}
			return 0;
		}

		//Gets the DOA status of the given actor
		private static bool IsAlive(GameObject actor) {
			if (IsPlayer(actor)) {
				return actor.GetComponent<PlayerScript>().IsAlive();
			}
			else if (IsEnemy(actor)) {
				return actor.GetComponent<EnemyScript>().IsAlive();
			}
			return false;
		}
	}
}
