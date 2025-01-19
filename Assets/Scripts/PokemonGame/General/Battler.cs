using System.Linq;

namespace PokemonGame.General
{
    using System;
    using System.Collections.Generic;
    using ScriptableObjects;
    using UnityEngine;

    /// <summary>
    /// The class that contains all the information for a battler
    /// </summary>
    [CreateAssetMenu(order = 7, fileName = "New Battler Prefab", menuName = "Pokemon Game/New Battler Prefab")]
    public class Battler : ScriptableObject
    {
        private BattlerTemplate _oldSource;
        /// <summary>
        /// The source that the battler uses to determine base stats 
        /// </summary>
        public BattlerTemplate source;

        private int _oldLevel;
        /// <summary>
        /// </summary>
        /// The level is what determines stats and if the battler respects the player
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

        public int catchRate;
        
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

        public bool frozen = false;
        
        public bool asleep = false;

        /// <summary>
        /// The primary type of the battler
        /// </summary>
        public BasicType primaryType;
        /// <summary>
        /// The secondary type of the battler
        /// </summary>
        public BasicType secondaryType;
        
        /// <summary>
        /// The list of moves that the battler has
        /// </summary>
        public List<Move> moves;

        /// <summary>
        /// The pp information about this battler's moves
        /// </summary>
        public List<MovePPData> movePpInfos;

        /// <summary>
        /// Invoked when the battler takes damage
        /// </summary>
        public event EventHandler OnTookDamage;
        /// <summary>
        /// Invoked when the battler faints
        /// </summary>
        public event EventHandler<BattlerTookDamageArgs> OnFainted;
        /// <summary>
        /// Invoked when the battlers health updates (includes taking damage)
        /// </summary>
        public event EventHandler OnHealthUpdated;

        /// <summary>
        /// Inflict damage onto the battler
        /// </summary>
        /// <param name="damage">The amount of damage to inflict</param>
        /// <param name="dSource">The source type of the damage</param>
        public void TakeDamage(int damage, DamageSource dSource)
        {
            Debug.Log($"Damage dealt: {damage}/{currentHealth}");
            
            currentHealth -= damage;
            
            OnTookDamage?.Invoke(this, EventArgs.Empty);
            OnHealthUpdated?.Invoke(this, EventArgs.Empty);
            
            if (currentHealth <= 0)
            {
                Fainted(dSource);
            }
        }

        /// <summary>
        /// Heal the battler by the amount to heal, battlers health will not go above the <see cref="maxHealth"/>
        /// </summary>
        /// <param name="amountToHeal">The amount of health to gain</param>
        public void Heal(int amountToHeal)
        {
            UpdateHealth(currentHealth + amountToHeal);
        }

        /// <summary>
        /// Give the battler exp, will handle leveling up and stat gains
        /// </summary>
        /// <param name="amountToGain">The amount of exp to give</param>
        public void GainExp(int amountToGain)
        {
            // TODO: all the fucking logic for carrying over exp and other shit like leveling up and evolving
            exp += amountToGain;
        }

        /// <summary>
        /// Handle fainting
        /// </summary>
        /// <param name="source">Source of damage that caused the battler to faint</param>
        private void Fainted(DamageSource source)
        {
            currentHealth = 0;
            isFainted = true;
            OnFainted?.Invoke(this, new BattlerTookDamageArgs(source, this));
            Debug.Log("Battler Fainted");
        }

        /// <summary>
        /// Used to change the battlers current health without inflicting damage
        /// </summary>
        /// <param name="newHealth">The health to set it to</param>
        public void UpdateHealth(int newHealth)
        {
            currentHealth = newHealth;
            
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            OnHealthUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void Revive(bool full)
        {
            isFainted = false;

            if (full)
            {
                UpdateHealth(maxHealth);
                foreach (var ppInfo in movePpInfos)
                {
                    ppInfo.CurrentPP = ppInfo.MaxPP;
                }
            }
            else
            {
                UpdateHealth(maxHealth/2);
            }
        }

        /// <summary>
        /// Trys to make the battler learn a move
        /// </summary>
        /// <param name="moveToLearn">The move you want the battler to learn</param>
        /// <returns>Weather the battler was able to learn the move</returns>
        public bool LearnMove(Move moveToLearn)
        {
            if (moves.Count < 4)
            {
                moves.Add(moveToLearn);
                movePpInfos.Add(new MovePPData(moveToLearn.basePP, moveToLearn.basePP));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Test if a battler can learn a move
        /// </summary>
        /// <param name="moveToLearn">Move to test</param>
        /// <returns></returns>
        public bool CanLearn(Move moveToLearn)
        {
            return source.moves.Contains(moveToLearn);
        }

        /// <summary>
        /// Used for updating stats and such outside of runtime
        /// </summary>
        private void OnValidate()
        {
            if (!statusEffect)
            {
                statusEffect = StatusEffect.Healthy;
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
            if (moves.Count > 4)
            {
                moves.RemoveAt(4);
            }
        }
        
        /// <summary>
        /// Updates the stats of the battler
        /// </summary>
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
        /// <summary>
        /// Updates the properties of the battler that derive from the source template
        /// </summary>
        private void UpdateSource()
        {
            primaryType = source.primaryType;
            secondaryType = source.secondaryType;
            texture = source.texture;
            catchRate = source.catchRate;

            if (string.IsNullOrEmpty(name))
            {
                name = source.name;
            }
        }

        public void UpdateLevel(int newLevel)
        {
            level = newLevel;
            UpdateStats();
            currentHealth = maxHealth;
        }

        /// <summary>
        /// Returns a battler that has been created using the parameters given
        /// </summary>
        /// <param name="source">The Battler Template that the new battler will use to calculate stats</param>
        /// <param name="level">The <see cref="level"/> of the new battler</param>
        /// <param name="statusEffect">The status effect that the new battler will have</param>
        /// <param name="name">The nickname of the new battler</param>
        /// <param name="moves">Moves that the battler has</param>
        /// <param name="autoAssignHealth">Auto assign health to the <see cref="maxHealth"/> when creating</param>
        /// <returns>A battler that has been created using the parameters given</returns>
        public static Battler Init(BattlerTemplate source, int level, StatusEffect statusEffect, string name, List<Move> moves, bool autoAssignHealth)
        {
            Battler returnBattler = CreateInstance<Battler>();
            
            returnBattler.source = source;
            returnBattler.UpdateLevel(level);
            returnBattler.name = name;
            returnBattler.isFainted = false;
            returnBattler.exp = 0;
            returnBattler.statusEffect = statusEffect;
            returnBattler.primaryType = source.primaryType;
            returnBattler.secondaryType = source.secondaryType;
            returnBattler.moves = new List<Move>();
            returnBattler.movePpInfos = new List<MovePPData>();
            returnBattler.catchRate = source.catchRate;

            foreach (var move in moves)
            {
                if(move != null)
                {
                    returnBattler.LearnMove(move);
                }
            }
            
            returnBattler.UpdateStats();

            if (autoAssignHealth)
                returnBattler.currentHealth = returnBattler.maxHealth;

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
                battler.moves, true);

            returnBattler.currentHealth = battler.currentHealth;
            returnBattler.isFainted = battler.isFainted;
            returnBattler.frozen = battler.frozen;
            returnBattler.asleep = battler.asleep;
            
            return returnBattler;
        }
    }
}