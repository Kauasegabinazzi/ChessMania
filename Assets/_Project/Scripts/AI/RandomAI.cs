using System;
using Game.Core;

namespace Game.AI
{
    public class RandomAI : IChessAI
    {
        readonly Random random;

        public RandomAI(int? seed = null)
        {
            random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public Move? ChooseMove(Board board)
        {
            var moves = MoveGenerator.GetAllLegalMoves(board, board.SideToMove);
            if (moves.Count == 0) return null;

            return moves[random.Next(moves.Count)];
        }
    }
}
