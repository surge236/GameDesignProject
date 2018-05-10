using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// NOTE: how to pull the gameobject stats: enemy_3.GetComponent<EnemyScript>().getSpeed();
// NOTE: for each action, be sure to update the eventInfo and activeInfo
// NOTE: Remember to update activeInfo whenever the activeActor variable is incremented.

public class BattleController : MonoBehaviour {

     public GameObject eventInfo; // Displays the battle text
     public GameObject activeInfo; // Dispays the active actor
     public Text healthDisplay; // Display the health of the actors

     public float stepDistance; // How far forward the player moves to indicate their turn

     public GameObject mapManager; // The map manager
     public PartyHandler PartyController;

     // The player's party
     public GameObject player_1;
     public GameObject player_2;
     public GameObject player_3;
     public GameObject player_4;

     // The enemy's party
     public GameObject enemy_1;
     public GameObject enemy_2;
     public GameObject enemy_3;

     // Array of battling characters
     private GameObject[] turnOrder;

     // Buttons
     public Button attackButton;
     public Button attack1Button;
     public Button attack2Button;
     public Button attack3Button;
     public Button defendButton;
     public Button continueButton;
     public Button SpellButton;
     public Button resetButton; // TEMPORARY ////////////////////////////////////////////////

     private int activeActor; // currently active in the turn order 
     private int numActors; // number of characters participating in the match

     private bool _bossFight;
     private bool _finalFight; // Initially set to 0, is set to 1 by the mapManager if the current fight is the last.

     private bool _attackActive;

     // Used by checkActiveText(), it is set up so that whenever the active actor is changed, the old active actor
     // moves back and the current one moves forward. This check prevents the old active actor from moving back on
     // the first run of the method since the old active actor never moved forward.
     private bool _isFirstBattle = true;

     // Currently just used so the game knows that the very first battle is occurring in order to set the event text
     // to the story text setting the stage for the game. This could potentially be used later to present text based
     // on the value of the flag. (specifically used in setScene())
     private int flag = 0;

     //private bool _debugSpellsAssigned;

	// Use this for initialization
	void Start () {
	     _bossFight = false;
          _finalFight = false;
	     _attackActive = false;
          initButtons(); // Sets up the buttons
	     _isFirstBattle = true;

	     //_debugSpellsAssigned = false;
	}

     public GameObject GetCurrentActor() {
          return turnOrder[activeActor];
     }

     public GameObject[] GetPlayers() {
          GameObject[] players = {player_1, player_2, player_3, player_4};
          return players;
     }

     public GameObject[] GetEnemies() {
          GameObject[] enemies = {enemy_1, enemy_2, enemy_3};
          return enemies;
     }

     public void setBossFight(bool setTo) {
          _bossFight = setTo;
     }
     
     // Used by mapManager to change the finalFight global and signal to the battleController that the
     // current fight is the last
     public void setFinalFight(bool setTo) {
          _finalFight = setTo;
     }

     // Get the number of actors participating in battle by checking if
     // each of the player and enemy gameobjects are active, increment
     // numActors by 1 for each that is.
     void setNumActors() {
          numActors = 0;
          if (player_1.activeSelf) numActors++;
          if (player_2.activeSelf) numActors++;
          if (player_3.activeSelf) numActors++;
          if (player_4.activeSelf) numActors++;
          if (enemy_1.activeSelf) numActors++;
          if (enemy_2.activeSelf) numActors++;
          if (enemy_3.activeSelf) numActors++;
     }

     // Whenever a unit is defeated, set them to inactive then call this function
     // to remove them from the turn order.
     void removeFromOrder(GameObject removed) {
          numActors--; // Decrement the number of actors in the fight
          GameObject[] tempArray = turnOrder; // so that we don't have to resort
          turnOrder = new GameObject[numActors]; // reinitialize the turnorder so we can restore

          // iterate through the old turn order till we find the object to remove
          for (int i = 0; i < numActors; i++) {
               if (tempArray[i] != removed) {
                    turnOrder[i] = tempArray[i];
               }
               else {
                    // Since the activeActor will have already incremented, decrement it
                    // if it is in the indexes affected by the removed actor.
                    if (activeActor > i) {
                         activeActor = activeActor - 1 % numActors;
                    }
                    // Move all the turnorder after the removed actor forward one.
                    for (int u = i; u < numActors; u++) {
                         turnOrder[u] = tempArray[u + 1];
                    }
                    i = numActors; // at this point we are done so just force the loop to exit.
               }
          }
     }

     // Create the turn order by initializing the turnOrder array to
     // the size of numActors - 1. Then, setting the active gameobjects
     // within the array. Next, sort the array based on the speed of the
     // gameobjects. Remember to set the activeActor to 0.
     void createTurnOrder() {
          turnOrder = new GameObject[numActors]; // initialize array

          // Create the turnorder array unordered
          int y = 0;
          if (player_1.gameObject.activeSelf) {
               turnOrder[y] = player_1; y++;
          }
          if (player_2.gameObject.activeSelf) {
               turnOrder[y] = player_2; y++;
          }
          if (player_3.gameObject.activeSelf) {
               turnOrder[y] = player_3; y++;
          }
          if (player_4.gameObject.activeSelf) {
               turnOrder[y] = player_4; y++;
          }
          if (enemy_1.gameObject.activeSelf) {
               turnOrder[y] = enemy_1; y++;
          }
          if (enemy_2.gameObject.activeSelf) {
               turnOrder[y] = enemy_2; y++;
          }
          if (enemy_3.gameObject.activeSelf) {
               turnOrder[y] = enemy_3; y++;
          }

          for (var i = 0; i < numActors; i++) {
               for (var j = i + 1; j < numActors; j++) {
                    if (Util.GetSpeed(turnOrder[i]) < Util.GetSpeed(turnOrder[j])) {
                         var temp = turnOrder[i];
                         turnOrder[i] = turnOrder[j];
                         turnOrder[j] = temp;
                    }
               }
          }
          
          activeActor = -1; // make sure to start at the beginning of the order.
     }

     // Check if the player's party is wiped by checking their health,
     // if the entire party is wiped, return true, else false.
     bool battleLose() {
          return !Util.IsAlive(player_1) && !Util.IsAlive(player_2) && !Util.IsAlive(player_3) && !Util.IsAlive(player_4);
     }

     // Check if the enemy's party is wiped by checking their health,
     // if the entire party is wiped, return true, else false.
     // If the battle is won and the finalFight global is set to 1, call gameWin()
     // If the battle is won and the finalFight global is set to 0, activate the roll button
     bool battleWin() {
          return !Util.IsAlive(enemy_1) && !Util.IsAlive(enemy_2) && !Util.IsAlive(enemy_3);
     }

     private void battleEndCheck() {
          disableAllButtons();
          
          if (battleWin()) {
               MoveAllBack();
               if (_finalFight) {
                    gameWin();
               }
               else {
                    if (_bossFight) {
                         _bossFight = false; // The boss fight is over
                         bossDefeat();
                    }
                    else {
                         changeEventText("Congratulations, you defeated the enemies!");
                    }
                    
                    mapManager.GetComponent<MapManager>().setRollButton(true);
               }

//               if (!_debugSpellsAssigned) {
//                    //TEMPORARY FOR DEBUGGING. SHOULD BE REMOVED BEFORE BUILDING
//                    for (var i = 0; i < 4; i++) {
//                         PartyController.AssignSpells(i);
//                    }
//
//                    _debugSpellsAssigned = true;
//               }
               
          }
          else if (battleLose()) {
               changeEventText("GAME OVER");
               resetButton.gameObject.SetActive(true);
          }
          else {
               continueButton.gameObject.SetActive(true);
          }
     }

     private void bossDefeat() {
          changeEventText("Congratulations! You defeated the boss!"
               + "\nThe party has all learned new spells!");
          
          PartyController.AssignSpells(mapManager.GetComponent<MapManager>().Zone);
     }

     // Called when the final battle is complete, should set the event text to the end of game
     // story text.
     private void gameWin() {
          // Disable all buttons
          disableAllButtons();
          mapManager.GetComponent<MapManager>().setRollButton(false);
          continueButton.gameObject.SetActive(false);

          // PLACEHOLDER: Display the end of game story text
          changeEventText("As the final blow lands, the dark culmination of all pollution in the land"
               + " bursts into five seperate spheres each representing one of the five unclean elements."
               + " After a moment, the spheres glow with a bright light and revert to their cleaner forms."
               + " The land has been cleansed of its impurities and can begin to recover.");
     }

     // Calls the enemyAction function if the activeActor is an enemy, otherwise, if the active
     // actor is a player, turn on the action buttons (attack, defend, etc) to allow for player actions,
     // and wait for a button to call the playerAction function. (continue, attack target_1,
     // attack target_2, attack target_3, and defend buttons all call this function)
     void battle() {          
          // Move along with the turn order
          do {
               activeActor = ++activeActor % numActors;
          } while (!Util.IsAlive(turnOrder[activeActor]));
          changeActiveText();
          
          // if it is a player's turn
          if (Util.IsPlayer(GetCurrentActor())) {
               enableBattleButtons();
          }
          // if it is an enemy's turn
          else if (Util.IsEnemy(GetCurrentActor())) {
               enemyAction();
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
     public void playerAction(string action) {
          var activePlayer = turnOrder[activeActor].GetComponent<PlayerScript>();

          // disable the buttons
          disableAllButtons();
          
          // Stop the player from defending or debuffing
          activePlayer.turnReset();
          
          //moveBack(activePlayer.gameObject);

          // Perform the player action and update the event text
          // attack enemy_1
          if (action == "attack_1") {
               var targetedEnemy = enemy_1;
               PerformAttack(activePlayer.gameObject, targetedEnemy);
          }
          // attack enemy_2
          else if (action == "attack_2") {
               var targetedEnemy = enemy_2;
               PerformAttack(activePlayer.gameObject, targetedEnemy);
          }
          // attack enemy_3
          else if (action == "attack_3") {
               var targetedEnemy = enemy_3;
               PerformAttack(activePlayer.gameObject, targetedEnemy);
          }
          // defend
          else if (action == "defend") {
               var activeName = Util.GetName(activePlayer.gameObject);
               changeEventText(activeName + " is defending!");
               activePlayer.defend();
          }
          //spell management
          else if (action.Substring(0, 5) == "spell") {
               var spellNum = int.Parse(action.Substring(action.Length - 3, 1));
               var targetNum = int.Parse(action.Substring(action.Length - 1, 1));
               
               var spell = activePlayer.Spells[spellNum - 1];
               if (spell.IsHeal) {
                    var target = GetPlayers()[targetNum - 1];
                    HealAlly(activePlayer.gameObject, target, spell);
               }
               else if (spell.IsDefend) {
                    var target = GetPlayers()[targetNum - 1];
                    DefendAlly(activePlayer.gameObject, target);
               }
               else if (spell.IsDebuff) {
                    var defender = GetEnemies()[targetNum - 1];
                    DebuffEnemy(activePlayer.gameObject, defender, spell);
               }
               else {
                    var defender = GetEnemies()[targetNum - 1];
                    PerformSpell(activePlayer.gameObject, defender, spell);
               }
          }
          
          battleEndCheck();
     }

     // INDEX: returns 1 if the battle was lost, 2 if it was won, and 0 if the battle continues.
     // Gets the attacker's attack and the defender's defense, then subtracts the attack from
     // the defense and deals that much damage to the defending actor. If the remaining health
     // of the defender is less than or equal to 0, call the battleWin and battleLose functions.
     private void PerformAttack(GameObject attacker, GameObject defender) {
          int attackerAttack = Util.getStrength(attacker);
          int defenderDefend = Util.getDefense(defender);
          string attackerName = Util.GetName(attacker);
          string defenderName = Util.GetName(defender);

          // Performs damage calculations.
          int damageDealt = attackerAttack - defenderDefend;
          if (damageDealt <= 0) 
               damageDealt = 1;
          
          // Changes the defender's health
          Util.applyDamage(defender, damageDealt);
          var remHealth = Util.GetCurrentHealth(defender);

          // Changes the event text to battle results
          changeEventText(attackerName + " attacks " + defenderName + ", dealing " + damageDealt + " damage."
               + "\nHealth remaining: " + remHealth);

          // destroy the defender if it died
          if (remHealth <= 0) {
               defender.SetActive(false);
               removeFromOrder(defender);
          }
     }

     /**
      * Handles Attack Spells attacking, defending, and text display
      **/
     private void PerformSpell(GameObject attacker, GameObject defender, Spell spellUsed) {
          var baseDamage = Util.getStrength(attacker);
          var fullDamage = (int) Math.Round(baseDamage * spellUsed.Multiplier, MidpointRounding.AwayFromZero);
          var appliedDamage = fullDamage - Util.getDefense(defender);
          if (appliedDamage <= 0) {
               appliedDamage = 1;
          }
          
          Util.applyDamage(defender, appliedDamage);
          var remainingHealth = Util.GetCurrentHealth(defender);
          changeEventText(Util.GetName(attacker) + " casts " + spellUsed.Name + " on "
                          + Util.GetName(defender) + " for " + appliedDamage + " damage."
                          + "\nHealth remaining: " + remainingHealth);

          if (remainingHealth <= 0) {
               defender.SetActive(false);
               removeFromOrder(defender);
          }
     }

     /**
      * Handles deubffing an enemy & displaying the info
      **/
     private void DebuffEnemy(GameObject attacker, GameObject enemy, Spell spellUsed) {
          if (Util.IsEnemy(enemy)) {
               enemy.GetComponent<EnemyScript>().debuff(spellUsed);
               
               changeEventText(Util.GetName(attacker) + " cut " + Util.GetName(enemy) + "'s Defense by " + spellUsed.GetDebuffText() + "!");
          }
     }

     /**
      * Handles healing an ally & displaying the info
      **/
     private void HealAlly(GameObject healer, GameObject target, Spell spellUsed) {
          var baseHeal = Util.getStrength(healer);
          var fullHeal = (int) Math.Round(baseHeal * spellUsed.Multiplier, MidpointRounding.AwayFromZero);
          
          //The multiplier for healing is negative, so this will actually restore health.
          Util.applyDamage(target, fullHeal);
          
          changeEventText(Util.GetName(healer) + " casts " + spellUsed.Name + " on "
                          + Util.GetName(target) + " for " + -fullHeal + " health."
                          + "\nHealth remaining: " + Util.GetCurrentHealth(target));
     }

     /**
      * Handles defending an ally & displaying the info
      **/
     private void DefendAlly(GameObject defender, GameObject target) {
          if (Util.IsPlayer(defender) && Util.IsPlayer(target)) {
               defender.GetComponent<PlayerScript>().DefendAlly(target);
               
               changeEventText(Util.GetName(defender) + " is defending " + Util.GetName(target) + "!");
          } 
     }

     // Chooses a random player character, then attacks them, then increments the active actor
     // and waits for the continue button to be pressed thus calling the battle function.
     void enemyAction() {
          var attacker = turnOrder[activeActor]; // who is attacking
          GameObject defender; // who is defending

          var targets = GetPlayers();
          do {
               defender = targets[Random.Range(0, targets.Length)];
          } while (!Util.IsAlive(defender));

          // Initiates the attack, then moves the battle system along to the next person
          PerformAttack(attacker, defender);

          // Checks if the battle is over
          battleEndCheck();
          continueButton.gameObject.SetActive(true);
          //moveBack(attacker);
     }

     // Call whenever the activeActor is incremented, the function should change the active actor text.
     void changeActiveText() {
          // Have the player whose turn it is move forward, and the last player move back

          // See the comment for the check global if these seems confusing
          if (!_isFirstBattle) {
               MoveAllBack();
          }
          else {
               _isFirstBattle = false;
          }
          Util.MoveForward(turnOrder[activeActor]);

          // Get the name of the active actor, then display that it is their turn.
          string actorName = Util.GetName(GetCurrentActor());
          activeInfo.GetComponent<Text>().text = actorName + "'s turn";
          
          setHealthDisplay();
     }

     // Call to display new event text when necessary
     void changeEventText(string eventText) {
          eventInfo.GetComponent<Text>().text = eventText;
     }

     // **TEMP FUNCTION DO NOT IMPLEMENT YET**
     // Disables unnecessary scene objects, enables necessary ones
     void setScene() {
          // Disable all buttons (enable those that are necessary when necessary)
          disableAllButtons();
          // Set the on screen text to relvant values.

          // The text to set the story for the game
          if (flag == 0) {
               changeEventText("The forces of coal, gas, oil, and nuclear power have claimed the land for their own and"
                               + " slowly robbed it of its beauty. To counter this threat, a representative from each of the forces of"
                               + " water, wind, earch, and sun step forward to fight. Lead these representatives to victory as they move "
                               + "through the lands each of these dark forces call home in order to usurp the unclean king from his throne.");
          }
          // No important flag event is occurring
          else {
               changeEventText("");
          }
          flag++;
     }

     // enable player action buttons or combat
     void enableBattleButtons() {
          attackButton.gameObject.SetActive(true);
          defendButton.gameObject.SetActive(true);
          continueButton.gameObject.SetActive(false);

          if (GetCurrentActor().GetComponent<PlayerScript>().Spells.Length > 0) {
               SpellButton.gameObject.SetActive(true);
               SpellButton.gameObject.GetComponent<SpellButtonHandler>().DeativateButtons();
          }
     }

     // Disable all on screen buttons
     void disableAllButtons() {
          attackButton.gameObject.SetActive(false);
          attack1Button.gameObject.SetActive(false);
          attack2Button.gameObject.SetActive(false);
          attack3Button.gameObject.SetActive(false);
          defendButton.gameObject.SetActive(false);
          continueButton.gameObject.SetActive(false);
          SpellButton.gameObject.SetActive(false);
          SpellButton.gameObject.GetComponent<SpellButtonHandler>().DeativateButtons();
          
          mapManager.GetComponent<MapManager>().setRollButton(false);
     }

     // Sets the battle scene based on the enemies passed as parameters.
     // Zone index passed is so the enemy's know how strong they need to be based on which world is active
     // (the index passed is the actual index + 1 so on index 0 the index passed is 1 to indicate world 1).
     public void setBattleScene(GameObject enemy1, GameObject enemy2, GameObject enemy3, int zoneIndex) {
          enemy_1 = ReplaceEnemy(enemy1, enemy_1, zoneIndex);
          enemy_2 = ReplaceEnemy(enemy2, enemy_2, zoneIndex);
          enemy_3 = ReplaceEnemy(enemy3, enemy_3, zoneIndex);

          var players = GetPlayers();
          foreach (var player in players) {
               var ps = player.GetComponent<PlayerScript>();
               ps.turnReset();
          }

          // Reset the system
          _isFirstBattle = true;
          setNumActors(); // Sets the numActors global
          setHealthDisplay(); // Sets the health display to accurate values.
          createTurnOrder(); // Sets the array to the necessary values.
          setScene(); // Disables/Enables relevant scene objects
          battle();
     }

     private GameObject ReplaceEnemy(GameObject enemyIn, GameObject enemyToReplace, int zoneIndex) {
          var enemyShouldExist = enemyIn != null;
          if (enemyShouldExist) {
               enemyIn = Instantiate(enemyIn, enemyToReplace.transform.position, enemyToReplace.transform.rotation, null);
               Destroy(enemyToReplace);
               enemyToReplace = enemyIn;

               // Change the enemy stats based on the world index
               enemyIn.GetComponent<EnemyScript>().setEnemyStats(zoneIndex);
          }
          enemyToReplace.SetActive(enemyShouldExist);
          
          return enemyToReplace;
     }

     // **TEMP FUNCTION DO NOT IMPLEMENT YET**
     // Initializes the battle scene (does the same thing as
     // start() just restated so that we can recreate battles.
     void initBattle() {
          // Reset the system
          setNumActors(); // Sets the numActors global
          setHealthDisplay(); // Sets the health display to accurate values.
          createTurnOrder(); // Sets the array to the necessary values.

          foreach (var actor in turnOrder) {
               actor.SetActive(true);

               if (Util.IsPlayer(actor)) {
                    actor.GetComponent<PlayerScript>().turnReset();
               }
          }

          setScene(); // Disables/Enables relevant scene objects
          disableAllButtons();
          battle();
     }

     public void MoveAllBack() {
          foreach (var actor in turnOrder) {
               Util.MoveBack(actor);
          }
     }

     public void setHealthDisplay() {
          healthDisplay.text = "";

          var players = GetPlayers();
          foreach (var player in players) {
               if (healthDisplay.text != "") {
                    healthDisplay.text += "\n";
               }
               
               healthDisplay.text += Util.GetName(player) + ": "
                                     + Util.GetCurrentHealth(player) + "/"
                                     + Util.GetMaxHealth(player);
          }
     }

     // Called by the map manager to alert that a change in zone has occurred, as a result, the
     // battle manager should change the player's stats based on the world number that they are
     // moving to.
     public void zoneChangeAlert(int zoneIndex) {
          player_1.GetComponent<PlayerScript>().setPlayerStats(zoneIndex);
          player_2.GetComponent<PlayerScript>().setPlayerStats(zoneIndex);
          player_3.GetComponent<PlayerScript>().setPlayerStats(zoneIndex);
          player_4.GetComponent<PlayerScript>().setPlayerStats(zoneIndex);
     }

     //Button Functions/////////////////////////////////////////////////////////

     public void initButtons() {
          // Set the attack button listener
          attackButton.onClick.AddListener(attackOnClick);

          // Set the specific target button listeners
          attack1Button.onClick.AddListener(attack1OnClick);
          attack2Button.onClick.AddListener(attack2OnClick);
          attack3Button.onClick.AddListener(attack3OnClick);

          // Set the defend button listener
          defendButton.onClick.AddListener(defendOnClick);

          // Set the continue button listener
          continueButton.onClick.AddListener(continueOnClick);
          
          SpellButton.onClick.AddListener(delegate {
               SpellButton.GetComponent<SpellButtonHandler>().ActivateButtons(GetCurrentActor());
          });

          // TEMPORARY //////////////////////////////////////////////////////////////////
          // Set the reset button listener
          resetButton.onClick.AddListener(resetOnClick);
          /////////////////////////////////////////////////////////////////////////////
     }

     // Enable and disable the attack 1-3 buttons
     public void attackOnClick() {
          if (!_attackActive) {
               _attackActive = true;
               
               var enemies = GetEnemies();
               Button[] attackButtons = {attack1Button, attack2Button, attack3Button};

               for (var i = 0; i < enemies.Length; i++) {
                    if (Util.IsAlive(enemies[i])) {
                         attackButtons[i].gameObject.SetActive(true);
                         attackButtons[i].gameObject.GetComponentInChildren<Text>().text =
                              "Attack " + Util.GetName(enemies[i]);
                    }
               }
          }
          else {
               _attackActive = false;
               
               attack1Button.gameObject.SetActive(false);
               attack2Button.gameObject.SetActive(false);
               attack3Button.gameObject.SetActive(false);
          }
     }

     // Attack the enemy in space 1, then enable the continue button
     // and disable the attack buttons
     private void attack1OnClick() {
          _attackActive = false;
          if (Util.IsAlive(enemy_1)) {
               playerAction("attack_1");
          }
     }

     // Attack the enemy in space 2, then enable the continue button
     // and disable the attack buttons
     private void attack2OnClick() {
          _attackActive = false;
          if (Util.IsAlive(enemy_2)) { 
               playerAction("attack_2");
          }
     }

     // Attack the enemy in space 3, then enable the continue button
     // and disable the attack buttons
     private void attack3OnClick() {
          _attackActive = false;
          if (Util.IsAlive(enemy_3)) {
               playerAction("attack_3");
          }
     }

     // Have the player defend, then enable the continue button
     public void defendOnClick() {
          playerAction("defend");
          continueButton.gameObject.SetActive(true);
     }

     // Continue to the next stage of combat by running battle
     public void continueOnClick() {
          battle();
     }

     // Temporary ///////////////////////////////////////////////////////////
     // Continue to the next stage of combat by running battle
     public void resetOnClick() {
          initBattle();
          resetButton.gameObject.SetActive(false);
     }
     ///////////////////////////////////////////////////////////////////////

}
