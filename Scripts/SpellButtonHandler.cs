using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButtonHandler : MonoBehaviour {

	public Button[] SpellSubButtons;
	public Button[] TargetButtons;
	public BattleController BattleHandler;

	private bool _isActive;
	private int _spellClicked;

	void Start() {
		_isActive = false;
		_spellClicked = -1;

		for (var i = 0; i < SpellSubButtons.Length; i++) {
			var index = i;
			SpellSubButtons[i].onClick.AddListener(delegate {
				ActivateSpellTargets(index);
			});
		}
	}

	public void ActivateButtons(GameObject currentPlayer) {
		if (!_isActive) {
			_isActive = true;
			
			var spells = currentPlayer.GetComponent<PlayerScript>().Spells;
			for (int i = 0; i < spells.Length; i++) {
				SpellSubButtons[i].gameObject.SetActive(true);
				SpellSubButtons[i].GetComponentInChildren<Text>().text = spells[i].Name;
			}
		}
		else {
			DeativateButtons();
		}
	}

	public void DeativateButtons() {
		_isActive = false;
		
		foreach (var spellButton in SpellSubButtons) {
			spellButton.gameObject.SetActive(false);
		}
		DeactivateSpellTargets();
	}

	private void ActivateSpellTargets(int spellKey) {
		if (_spellClicked != spellKey) {
			if (_spellClicked != -1) {
				DeactivateSpellTargets();
			}
			
			_spellClicked = spellKey;
			
			var currentPlayer = BattleHandler.GetCurrentActor();
			var currentSpell = currentPlayer.GetComponent<PlayerScript>().Spells[spellKey];

			var toCheck = currentSpell.IsHeal || currentSpell.IsDefend ? BattleHandler.GetPlayers() : BattleHandler.GetEnemies();
			for (var i = 0; i < TargetButtons.Length; i++) {
				var target = TargetButtons[i];
				
				target.onClick.RemoveAllListeners();

				if (i < toCheck.Length && Util.IsAlive(toCheck[i])) {
					target.gameObject.SetActive(true);
					target.GetComponentInChildren<Text>().text = currentSpell.GetPrefix() + Util.GetName(toCheck[i]);
					
					var index = i;
					target.onClick.AddListener(delegate {
						BattleHandler.playerAction("spell_" + (spellKey + 1) + "_" + (index + 1));
					});
				}
			}
		}
		else {
			DeactivateSpellTargets();
		}
	}

	private void DeactivateSpellTargets() {
		_spellClicked = -1;
		
		foreach (var tb in TargetButtons) {
			tb.gameObject.SetActive(false);
		}
	}
}
