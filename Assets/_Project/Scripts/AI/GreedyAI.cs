using Game.Core;
using System;
using System.Collections.Generic;

namespace Game.AI
{
    public class GreedyAI : IChessAI
    {
        readonly Random random;

        public GreedyAI(int? seed = null)
        {
            random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public Move? ChooseMove(Board board)
        {
            PieceColor me = board.SideToMove;
            var moves = MoveGenerator.GetAllLegalMoves(board, me);
            if (moves.Count == 0) return null;

            var bestMoves = new List<Move>();
            int bestScore = int.MinValue;

            foreach(var move in moves)
            {
                board.MakeMove(move);
                int score = MoveGenerator.GetStatus(board) == GameStatus.Checkmate ? Evaluation.MateScore : Evaluation.Evaluate(board, me);
                board.UndoMove();

                if(score > bestScore)
                {
                    bestScore = score;
                    bestMoves.Clear();
                    bestMoves.Add(move);
                }
                else if(score == bestScore)
                {
                    bestMoves.Add(move);
                }
            }

            return bestMoves[random.Next(bestMoves.Count)];
        }
    }
}
