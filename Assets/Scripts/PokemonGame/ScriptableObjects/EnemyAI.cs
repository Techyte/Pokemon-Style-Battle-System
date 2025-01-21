using PokemonGame.Battle;

namespace PokemonGame.ScriptableObjects
{
    using System;
    using Game.Party;
    using General;
    using UnityEngine;
    using UnityEngine.Events;

    [CreateAssetMenu(order = 5, fileName = "New AI", menuName = "Pokemon Game/New AI")]
    public class EnemyAI : ScriptableObject
    {
        public new string name;

        public UnityEvent<AIMethodEventArgs> aIMethodEvent;
        public UnityEvent<AISwitchEventArgs> aISwitchEvent;

        public void AIMethod(AIMethodEventArgs e)
        {
            aIMethodEvent?.Invoke(e);
        }

        public void AISwitchMethod(AISwitchEventArgs e)
        {
            aISwitchEvent?.Invoke(e);
        }
    }

    public class AIMethodEventArgs : EventArgs
    {
        public AIMethodEventArgs(Battler battlerToUse, Party usableParty, ExternalBattleData battleData)
        {
            this.battlerToUse = battlerToUse;
            this.usableParty = usableParty;
            this.battlerToUse = battlerToUse;
        }
        
        public Battler battlerToUse;
        public Party usableParty;
        public ExternalBattleData battleData;
    }

    public class AISwitchEventArgs : EventArgs
    {
        public AISwitchEventArgs(int currentIndex, Party usableParty, ExternalBattleData battleData)
        {
            this.currentIndex = currentIndex;
            this.usableParty = usableParty;
            this.battleData = battleData;
            newBattlerIndex = currentIndex;
        }

        public int currentIndex;
        public int newBattlerIndex;
        public Party usableParty;
        public ExternalBattleData battleData;
    }
}