using System;
using UnityEngine;

public class Spell : MonoBehaviour {
    public string Name;
    public float Multiplier;

    public bool IsHeal;
    public bool IsDefend;
    public bool IsDebuff;

    public string GetPrefix() {
        var prefix = "Attack";

        if (IsHeal) {
            prefix = "Heal";
        }
        else if (IsDefend) {
            prefix = "Defend";
        }
        else if (IsDebuff) {
            prefix = "Debuff";
        }

        return prefix + " ";
    }

    public string GetDebuffText() {
        var text = "";
        
        if (IsDebuff) {
            if (Multiplier == .75) {
                text = "a quarter";
            }
            else if (Multiplier == .33) {
                text = "a third";
            } 
            else if (Multiplier == .5) {
                text = "half";
            }
            else {
                var rounded = (int) Math.Round(Multiplier * 100, MidpointRounding.AwayFromZero);
                text = 100 - rounded + "%";
            }
        }

        return text;
    }
}