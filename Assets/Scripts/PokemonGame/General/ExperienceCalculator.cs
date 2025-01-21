using PokemonGame.ScriptableObjects;
using UnityEngine;

namespace PokemonGame.General
{
    public class ExperienceCalculator : MonoBehaviour
    {
        public int GetExperienceFromDefeatingBattler(Battler defeated, bool trainer, int noOfBattlersParticipated)
        {
            // Calculations from: https://bulbapedia.bulbagarden.net/wiki/Experience#Experience_gain_in_battle
            
            int exp = 0;

            double a = trainer ? 1.5 : 1;

            // lucky egg
            double e = 1;

            double s = noOfBattlersParticipated;

            double b = defeated.source.baseExpYield;

            double L = defeated.level;

            exp = (int)(b * L / 7 * 1 / s * e * a);

            return exp;
        }

        public bool Captured(Battler target, PokeBall ball)
        {
            float statusBonus = 1;

            if (target.statusEffect.name == "Asleep" || target.statusEffect.name == "Frozen")
            {
                statusBonus = 2;
            }

            switch (target.statusEffect.name)
            {
                case "Paralysed":
                    statusBonus = 1.5f;
                    break;
                case "Poisoned":
                    statusBonus = 1.5f;
                    break;
                case "Burned":
                    statusBonus = 1.5f;
                    break;
            }
            
            float a = ((3f*target.maxHealth - 2f * target.currentHealth)/(3f*target.maxHealth)) * target.catchRate * ball.bonus * statusBonus;
            
            return a <= Random.Range(0, 255);
        }
    }   
}