using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {

     public int mapSpaces; // The total spaces on the board (make it a multiple of 5 to keep the zones the same length)
     public int numZones; // Indicates the total number of zones (always 5 based on current implementation strategy).

     public Button rollButton; // The button that controls dice rolls.

     public Text zoneRemDisplay; // Text that displays the spaces remaining in the zone
     public Text boardRemDisplay; // Text that displays the spaces remaining on the board.
     public Text lastRollDisplay; // Text that displays the last roll

     public GameObject battleController; // The battle controller.

     public GameObject[] oilEnemies; // Stores the enemies that can spawn in the oil zone.
     public GameObject[] gasEnemies; // Stores the enemies that can spawn in the gas zone.
     public GameObject[] coalEnemies; // Stores the enemies that can spawn in the coal zone.
     public GameObject[] nukeEnemies; // Stores the enemies that can spawn in the nuclear zone.
     public GameObject[] finalEnemies; // Stores the enemies that can spawn in the final zone.
     public GameObject oilBoss; // The boss of the oil zone
     public GameObject gasBoss; // The boss of the gas zone
     public GameObject coalBoss; // The boss of the coal zone
     public GameObject nukeBoss; // The boss of the nuclear zone
     public GameObject finalBoss; // The boss of the final zone

     // Backgrounds for each zone
     public GameObject oilBack;
     public GameObject gasBack;
     public GameObject coalBack;
     public GameObject nukeBack;
     public GameObject finalBack;

     private int currentSpace; // The space the player party is currently on.
     private int zoneSpaces; // The number of spaces for each zone
     private int nextBossSpace; // The space that the next boss will spawn on.
     private int zoneIndex; // Indicates the zone currently active based on the index in the zoneOrder array.

     private bool changeNextRoll; // Used by roll() to check if the zone needs to be changed on the next roll

     // Stores the order the zones appear in
     // Index: 0 = gas zone
     //        1 = oil zone
     //        2 = nuclear zone
     //        3 = coal zone
     //        4 = final zone
     private int[] zoneOrder; 

	// Use this for initialization
	void Start () {
          initButtons();
          determineZoneSpaces();
          determineZoneOrder();
	     
          currentSpace = 1; // Start from space 1 not 0
          nextBossSpace = zoneSpaces; // The first boss space is simply at the space where the zone ends.
          zoneIndex = 0;
          changeNextRoll = false;
          rollButton.gameObject.SetActive(false); // The game starts with a battle, so disable the roll button.
	     
          executeZoneEncounter();
          setZoneRemDisplay();
          setBoardRemDisplay();
	}

     public int Zone {
          get { return zoneIndex; }
     }

     // Sets the zoneRemDisplay global to an appropriate value.
     private void setZoneRemDisplay() {
          int spaceRemaining = nextBossSpace - currentSpace; // since the boss marks the end of the zone.
          zoneRemDisplay.text = "Zone Spaces Remaining: " + spaceRemaining;
     }

     // Sets the boardRemDisplay global to an appropriate value.
     private void setBoardRemDisplay() {
          int spaceRemaining = mapSpaces - currentSpace;
          boardRemDisplay.text = "Total Spaces Remaining: " + spaceRemaining;
     }

     // Sets the order the zones will appear in (currently assumes the number of zones is 5).
     private void determineZoneOrder() {
          zoneOrder = new int[5];
          zoneOrder[4] = 4; // The last zone should always be the final zone.

          int index = 0; // Where in the zone order we are currently assigning values.
          bool[] claimed = {false, false, false, false};
          while (index < 4) { // While there are still numbers to assign
               int num = Random.Range(0, 4);
               if (!claimed[num]) { // If the number hasn't been used.
                    claimed[num] = true;
                    zoneOrder[index] = num; // add the number to the zone order.
                    index++;
               }
          }

          setBackground(true);
     }

     // Determines the spaces per zone (currently assumes the total is a multiple of 5).
     private void determineZoneSpaces() {
          zoneSpaces = mapSpaces / numZones;
     }

     // Determines the next space at which a boss will appear and changes the global variable to reflect this.
     private void getNextBossSpace() {
          nextBossSpace = zoneSpaces * (zoneIndex + 1);
     }

     // Sets the background based on the current active zone
     private void setBackground(bool setTo) {
          if (zoneOrder[zoneIndex] == 0)
               gasBack.SetActive(setTo);
          
          else if (zoneOrder[zoneIndex] == 1)
               oilBack.SetActive(setTo);
          
          else if (zoneOrder[zoneIndex] == 2)
               nukeBack.SetActive(setTo);
          
          else if (zoneOrder[zoneIndex] == 3)
               coalBack.SetActive(setTo);
          
          else if (zoneOrder[zoneIndex] == 4) 
               finalBack.SetActive(setTo);
     }

     // Executes the dice roll.
     private void roll() {
          int diceRoll = Random.Range(1, 6); // Roll is between 1 and 6
          int newSpace = currentSpace + diceRoll; // Where the player should end up.

          lastRollDisplay.text = "Last Roll: " + diceRoll;

          // Checks if the zone needs to be changed this roll (executes on the roll after boss fights)
          if (changeNextRoll) {
               executeZoneChange();
               changeNextRoll = false;
          }

          // If you hit a boss space, do the boss encounter and switch globals to next zone, otherwise, do a
          // standard zone encounter.
          if (newSpace >= nextBossSpace) {
               changeNextRoll = true;
               currentSpace = nextBossSpace;
               setZoneRemDisplay();
               setBoardRemDisplay();
               executeBossEncounter();
          }
          else {
               currentSpace = newSpace;
               setZoneRemDisplay();
               setBoardRemDisplay();
               executeZoneEncounter();
          }
     }

     // Change the globals to reflect a change of zones.
     private void executeZoneChange() {
          setBackground(false); // Disable the old background
          zoneIndex++;
          battleController.GetComponent<BattleController>().zoneChangeAlert(zoneIndex);
          getNextBossSpace();
          setBackground(true); // Enable the new background
     }

     // Execute the boss encounter for the currently active zone.
     private void executeBossEncounter() {
          // Signal to the battle controller that this is the final fight
          if (currentSpace == mapSpaces) {
               battleController.GetComponent<BattleController>().setFinalFight(true);
          }

     // Tell the battle controller to execute a battle with the current zone's boss monster.
          GameObject zoneBoss = null;
          if (zoneOrder[zoneIndex] == 0) zoneBoss = gasBoss;
          else if (zoneOrder[zoneIndex] == 1) zoneBoss = oilBoss;
          else if (zoneOrder[zoneIndex] == 2) zoneBoss = nukeBoss;
          else if (zoneOrder[zoneIndex] == 3) zoneBoss = coalBoss;
          else if (zoneOrder[zoneIndex] == 4) zoneBoss = finalBoss;
          
          battleController.GetComponent<BattleController>().setBossFight(true);

          var boss1 = zoneIndex == 0 ? null : zoneBoss;
          var boss2 = zoneBoss;
          var boss3 = zoneIndex == 0 ? null : zoneBoss;
          
          battleController.GetComponent<BattleController>().setBattleScene(boss1, boss2, boss3, zoneIndex);
     }

     // Execute a standard encounter for the currently active zone.
     private void executeZoneEncounter() {
          // Get three random enemies that should spawn in the current zone.
          GameObject[] enemyArray = {}; // The array of enemies for the current zone
          if (zoneOrder[zoneIndex] == 0) {
               enemyArray = gasEnemies;
          }
          else if (zoneOrder[zoneIndex] == 1) {
               enemyArray = oilEnemies;
          }
          else if (zoneOrder[zoneIndex] == 2) {
               enemyArray = nukeEnemies;
          }
          else if (zoneOrder[zoneIndex] == 3) {
               enemyArray = coalEnemies;
          }
          else if (zoneOrder[zoneIndex] == 4) {
               enemyArray = finalEnemies;
          }

          // Get an index to pull from the enemy array for each enemy (if the number is -1, no enemy spawns,
          // to make sure there is always an enemy the first enemy cannot have this occur
          int index1 = Random.Range(0, enemyArray.Length);
          int index2 = Random.Range(-1, enemyArray.Length);
          int index3 = Random.Range(-1, enemyArray.Length);
          
          GameObject enemy1 = enemyArray[index1];
          GameObject enemy2 = index2 == -1 ? null : enemyArray[index2];
          GameObject enemy3 = index3 == -1 ? null : enemyArray[index3];

          // Pass the enemies to the battle manager.
          battleController.GetComponent<BattleController>().setBattleScene(enemy1, enemy2, enemy3, zoneIndex);
     }

     // Button Functions ///////////////////////////////////////////////////////////////////////////////////////
     public void initButtons() {
          // Set the roll button listener
          Button rollComp = rollButton.GetComponent<Button>();
          rollComp.onClick.AddListener(rollOnClick);
     }

     // On a roll, call the roll function and deactivate the button.
     public void rollOnClick() {
          roll();
          rollButton.gameObject.SetActive(false);
     }

     public void setRollButton(bool setTo) {
          rollButton.gameObject.SetActive(setTo);
     }
}
