using UnityEngine;
using UnityEngine.UI;

namespace Deprecated {
	public class AttackButtonScript : MonoBehaviour {
		public GameObject Attack1;
		public GameObject Attack2;
		public GameObject Attack3;

		private bool _isActive;

		private void Start() {
			_isActive = false;
			
			
		}
	
		public void ActivateButtons(GameObject[] enemies) {
			if (_isActive) {
				DeactivateButtons();
			}
			else {
				_isActive = true;
				if (enemies[0].gameObject.activeSelf) {
					Attack1.gameObject.SetActive(true);
					//Attack1.GetComponentInChildren<Text>().text = "Attack " + enemies[0].GetComponent<global::EnemyScript>().Name;
				}
				if (enemies[1].gameObject.activeSelf) {
					Attack2.gameObject.SetActive(true);
					//Attack2.GetComponentInChildren<Text>().text = "Attack " + enemies[1].GetComponent<global::EnemyScript>().Name;
				}
				if (enemies[2].gameObject.activeSelf) {
					Attack3.gameObject.SetActive(true);
					//Attack3.GetComponentInChildren<Text>().text = "Attack " + enemies[2].GetComponent<global::EnemyScript>().Name;
				}
			}
		}

		public void DeactivateButtons() {
			_isActive = false;
			Attack1.gameObject.SetActive(false);
			Attack2.gameObject.SetActive(false);
			Attack3.gameObject.SetActive(false);
		}
	}
}
