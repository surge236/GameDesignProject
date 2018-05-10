using UnityEngine;
using UnityEngine.UI;

namespace Deprecated {
	public class BindButtons : MonoBehaviour {
		public Button Attack;
		public Button Attack1;
		public Button Attack2;
		public Button Attack3;
		public Button Defend;
		public Button Continue;

		public GameObject Battle;
	
		// Use this for initialization
		void Start () {
			Attack.onClick.AddListener(AttackClick);
		
			Attack1.onClick.AddListener(delegate { EnemySelection(1); });
			Attack2.onClick.AddListener(delegate { EnemySelection(2); });
			Attack3.onClick.AddListener(delegate { EnemySelection(3); });
		
			Defend.onClick.AddListener(DefendClick);
		
			Continue.onClick.AddListener(ContinueClick);
		}

		void AttackClick() {
			var currentEnemies = Battle.GetComponent<BattleController>().GetEnemies();
			Attack.GetComponent<AttackButtonScript>().ActivateButtons(currentEnemies);
		}

		void EnemySelection(int enemyNumber) {
			Battle.GetComponent<BattleController>().PlayerAction("attack_enemy" + enemyNumber);
		}

		void DefendClick() {
			Battle.GetComponent<BattleController>().PlayerAction("defend");
		}

		void ContinueClick() {
			Battle.GetComponent<BattleController>().ProcessContinue(Continue.gameObject.CompareTag("battle_over"));
		}
	}
}
