namespace PokemonGame.Battle
{
    public enum TurnItem
    {
        StartDelay,
        PlayerMove,
        OpponentMove,
        PlayerSwapBecauseFainted,
        PlayerSwap,
        OpponentSwap,
        OpponentSwapBecauseFainted,
        PlayerItem,
        OpponentItem,
        EndBattlePlayerWin,
        EndBattleOpponentWin,
        StartOfTurnStatusEffects,
        EndOfTurnStatusEffects,
        PlayerParalysed,
        OpponentParalysed,
        CatchAttempt,
        Run,
    }   
}