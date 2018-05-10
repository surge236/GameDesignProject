using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyHandler : MonoBehaviour {

	public PlayerScript[] Players;
	
	public Spell[] GasSpells;
	public Spell[] OilSpells;
	public Spell[] NuclearSpells;
	public Spell[] CoalSpells;

	public void AssignSpells(int zoneCode) {
		Spell[] spellsToAssign = {};
		switch (zoneCode) {
			case 0: 
				spellsToAssign = GasSpells;
				break;
				
			case 1:
				spellsToAssign = OilSpells;
				break;
				
			case 2: 
				spellsToAssign = NuclearSpells;
				break;
				
			case 3: 
				spellsToAssign = CoalSpells;
				break;
				
		}
		
		foreach (var player in Players) {
			player.LearnSpell(spellsToAssign[player.ClassKey]);
		}
	}
	
}
