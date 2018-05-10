using UnityEngine;

//Contains various utility functions that can be used throughout the project
public static class Util {
    //Checks if the given actor is a player
    public static bool IsPlayer(GameObject actor) {
        return actor.GetComponent<PlayerScript>() != null;
    }

    //Checks is the given actor is an enemy
    public static bool IsEnemy(GameObject actor) {
        return actor.GetComponent<EnemyScript>() != null;
    }

    //Gets the name of the given actor
    public static string GetName(GameObject actor) {
        if (IsPlayer(actor)) {
            return actor.GetComponent<PlayerScript>().Name;
        }
        else if (IsEnemy(actor)) {
            return actor.GetComponent<EnemyScript>().Name;
        }
        return "";
    }
	
    public static int GetCurrentHealth(GameObject actor) {
        if (IsPlayer(actor)) {
            return actor.GetComponent<PlayerScript>().CurrentHealth;
        }
        else if (IsEnemy(actor)) {
            return actor.GetComponent<EnemyScript>().CurrentHealth;
        }
        return 0;
    }

    public static int GetMaxHealth(GameObject actor) {
        if (IsPlayer(actor)) {
            return actor.GetComponent<PlayerScript>().getMaxHealth();
        }
        else if (IsEnemy(actor)) {
            return actor.GetComponent<EnemyScript>().MaxHealth;
        }
        return 0;
    }

    public static void applyDamage(GameObject actor, int damage) {
        if (IsPlayer(actor)) {
            actor.GetComponent<PlayerScript>().takeDamage(damage);
        }
        else if (IsEnemy(actor)) {
            actor.GetComponent<EnemyScript>().takeDamage(damage);
        }
    }

    public static void setHealth(GameObject actor, int health) {
        if (IsPlayer(actor)) {
            actor.GetComponent<PlayerScript>().CurrentHealth = health;
        }
        else if (IsEnemy(actor)) {
            actor.GetComponent<EnemyScript>().CurrentHealth = health;
        }
    }
    
    public static int getStrength(GameObject actor) {
        if (IsPlayer(actor)) {
            return actor.GetComponent<PlayerScript>().getStrength();
        }
        else if (IsEnemy(actor)) {
            return actor.GetComponent<EnemyScript>().Strength;
        }
        return 0;
    }
    
    public static int getDefense(GameObject actor) {
        if (IsPlayer(actor)) {
            return actor.GetComponent<PlayerScript>().getDefense();
        }
        else if (IsEnemy(actor)) {
            return actor.GetComponent<EnemyScript>().Defense;
        }
        return 0;
    }
	
    //Gets the speed of the given actor
    public static int GetSpeed(GameObject actor) {
        if (IsPlayer(actor)) {
            return actor.GetComponent<PlayerScript>().getSpeed();
        }
        else if (IsEnemy(actor)) {
            return actor.GetComponent<EnemyScript>().Speed;
        }
        return 0;
    }

    //Gets the DOA status of the given actor
    public static bool IsAlive(GameObject actor) {
        if (IsPlayer(actor)) {
            return actor.GetComponent<PlayerScript>().IsAlive();
        }
        else if (IsEnemy(actor)) {
            return actor.GetComponent<EnemyScript>().IsAlive();
        }
        return false;
    }

    public static void MoveForward(GameObject actor) {
        if (IsPlayer(actor)) {
            actor.GetComponent<PlayerScript>().MoveForward();
        }
        else if (IsEnemy(actor)) {
            actor.GetComponent<EnemyScript>().MoveForward();
        }
    }
    
    public static void MoveBack(GameObject actor) {
        if (IsPlayer(actor)) {
            actor.GetComponent<PlayerScript>().MoveBack();
        }
        else if (IsEnemy(actor)) {
            actor.GetComponent<EnemyScript>().MoveBack();
        }
    }
}