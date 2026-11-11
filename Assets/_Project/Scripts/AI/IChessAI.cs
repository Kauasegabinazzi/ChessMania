using Game.Core;

namespace Game.AI
{
    public interface IChessAI
    {
        Move? ChooseMove(Board board);
    }
}
