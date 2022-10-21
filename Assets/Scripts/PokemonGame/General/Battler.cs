using System;
using System.Collections.Generic;
using System.Linq;
using PokemonGame.Game;
using PokemonGame.ScriptableObjects;
using UnityEngine;
using Type = PokemonGame.ScriptableObjects.Type;

namespace PokemonGame
{
    /// <summary>
    /// The class that contains all the information for a battler
    /// </summary>
    [Serializable]
    public class Battler : ScriptableObject
    {
        private BattlerTemplate _oldSource;
        /// <summary>
        /// The source that the battler uses to determine base stats 
        /// </summary>
        public BattlerTemplate source;

        private int _oldLevel;
        /// <summary>
        /// The level is what determines stats and if the battler respects the player
        /// </summary>
        public int level;

        /// <summary>
        /// The name of the battler, unlike batter templates this can be changed for nicknames
        /// </summary>
        public new string name;
        /// <summary>
        /// The maximum health of the battler
        /// </summary>
        public int maxHealth;

        /// <summary>
        /// The current health of the battler
        /// </summary>
        public int currentHealth;
        
        /// <summary>
        /// The current amount of experience points the battler has in progressing through its current level
        /// </summary>
        public int exp;
        /// <summary>
        /// The attack statistic for the battler
        /// </summary>
        public int attack;
        /// <summary>
        /// The defense statistic for the battler
        /// </summary>
        public int defense;
        /// <summary>
        /// The special attack statistic for the battler
        /// </summary>
        public int specialAttack;
        /// <summary>
        /// The special defence statistic for the battler
        /// </summary>
        public int specialDefense;
        /// <summary>
        /// The speed statistic for the battler
        /// </summary>
        public int speed;
        /// <summary>
        /// The sprite that the battler uses
        /// </summary>
        
        public Sprite texture;
        /// <summary>
        /// Is the battler fainted
        /// </summary>
        public bool isFainted;
        /// <summary>
        /// The current status effect that the batter has
        /// </summary>
        public StatusEffect statusEffect;

        /// <summary>
        /// The primary type of the battler
        /// </summary>
        public Type primaryType;
        /// <summary>
        /// The secondary type of the battler
        /// </summary>
        public Type secondaryType;

        /// <summary>
        /// The list of moves that the battler has
        /// </summary>
        public List<Move> moves;

        /// <summary>
        /// Inflict damage onto the battler
        /// </summary>
        /// <param name="damage">The amount of damage to inflict</param>
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
                isFainted = true;
        }

        /// <summary>
        /// Used to change the battlers current health without inflicting damage
        /// </summary>
        /// <param name="newHealth">The health to set it to</param>
        public void UpdateHealth(int newHealth)
        {
            currentHealth = newHealth;
            
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }

        //Used for updating stats and such outside of runtime
        private void OnValidate()
        {
            if (!statusEffect)
            {
                if (Registry.GetStatusEffect("Healthy", out StatusEffect gotEffect))
                {
                    statusEffect = gotEffect;
                }
            }
            
            if (_oldLevel != level)
            {
                UpdateStats();
            }

            if (_oldSource != source)
            {
                UpdateStats();
                UpdateSource();
            }

            _oldSource = source;
            _oldLevel = level;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
            
            //Making sure the battler always has 4 moves
            if (moves.Count < 4)
                for (int i = 0; i < 4-moves.Count; i++)
                    moves.Add(null);
        }
        
        //Updates the stats of the battler
        private void UpdateStats()
        {
            if(!source) return;
            
            //maxHealth = Mathf.FloorToInt(0.01f * (2 * source.baseHealth + 15 + Mathf.FloorToInt(0.25f * 15)) * level) + level + 10;
            attack = Mathf.FloorToInt(0.01f * (2 * source.baseAttack + 15 + Mathf.FloorToInt(0.25f * 15)) * level) + 5;
            defense = Mathf.FloorToInt(0.01f * (2 * source.baseDefense + 15 + Mathf.FloorToInt(0.25f * 15)) * level) + 5;
            specialAttack = Mathf.FloorToInt(0.01f * (2 * source.baseSpecialAttack + 15 + Mathf.FloorToInt(0.25f * 15)) * level) + 5;
            specialDefense = Mathf.FloorToInt(0.01f * (2 * source.baseSpecialDefense + 15 + Mathf.FloorToInt(0.25f * 15)) * level) + 5;
            speed = Mathf.FloorToInt(0.01f * (2 * source.baseSpeed + 15 + Mathf.FloorToInt(0.25f * 15)) * level) + 5;
            maxHealth = Mathf.FloorToInt(0.01f * (2 * source.baseHealth + 15 + Mathf.FloorToInt(0.25f * 15)) * level) + 5;
        }

        //Updates the source of the battler
        private void UpdateSource()
        {
            primaryType = source.primaryType;
            secondaryType = source.secondaryType;
            texture = source.texture;
        }

        /// <summary>
        /// Returns a battler that has been created using the parameters given
        /// </summary>
        /// <param name="source">The Battler Template that the new battler will use to calculate stats</param>
        /// <param name="level">The <see cref="level"/> of the new battler</param>
        /// <param name="statusEffect">The status effect that the new battler will have</param>
        /// <param name="name">The nickname of the new battler</param>
        /// <param name="move1">Move that shows up first when battling</param>
        /// <param name="move2">Move that shows up second when battling</param>
        /// <param name="move3">Move that shows up third when battling</param>
        /// <param name="move4">Move that shows up fourth when battling</param>
        /// <param name="autoAssignHealth">Auto assign health to the <see cref="maxHealth"/> when creating</param>
        /// <returns>A battler that has been created using the parameters given</returns>
        public static Battler Init(BattlerTemplate source, int level, StatusEffect statusEffect, string name, Move move1, Move move2, Move move3, Move move4, bool autoAssignHealth)
        {
            Battler returnBattler = CreateInstance<Battler>();
            
            returnBattler.source = source;
            returnBattler.level = level;
            returnBattler.name = name;
            returnBattler.isFainted = false;
            returnBattler.exp = 0;
            returnBattler.statusEffect = statusEffect;
            returnBattler.primaryType = source.primaryType;
            returnBattler.secondaryType = source.secondaryType;
            returnBattler.moves = new Move[4].ToList();
            returnBattler.moves[0] = move1;
            returnBattler.moves[1] = move2;
            returnBattler.moves[2] = move3;
            returnBattler.moves[3] = move4;

            if (autoAssignHealth)
                returnBattler.currentHealth = returnBattler.maxHealth;
            
            returnBattler.UpdateStats();

            returnBattler.texture = source.texture;
            
            return returnBattler;
        }
        
        /// <summary>
        /// Creates an exact copy of the battler it is given
        /// </summary>
        /// <param name="battler">The battler to duplicate</param>
        /// <returns>The copied battler</returns>
        public static Battler CreateCopy(Battler battler)
        {
            Battler returnBattler = Init(battler.source, battler.level, battler.statusEffect, battler.name,
                battler.moves[0], battler.moves[1], battler.moves[2], battler.moves[3], true);

            returnBattler.currentHealth = battler.currentHealth;
            
            return returnBattler;
        }
    }
}