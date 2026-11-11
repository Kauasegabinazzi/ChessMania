using System;
using System.Collections.Generic;
using Game.Core;

namespace Game.AI
{
    public class MinimaxAI : IChessAI
    {
        const int Infinity = 1000000;

        readonly int depth;
        readonly Random random;

        public MinimaxAI(int depth, int? seed = null)
        {
            this.depth = Math.Max(1, depth);
            random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public Move? ChooseMove(Board board)
        {
            var moves = MoveGenerator.GetAllLegalMoves(board, board.SideToMove);
            if (moves.Count == 0) return null;

            var bestMoves = new List<Move>();
            int bestScore = -Infinity;

            foreach (var move in moves)
            {
                board.MakeMove(move);
                int score = -Search(board, depth - 1, 1);
                board.UndoMove();

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoves.Clear();
                    bestMoves.Add(move);
                }
                else if (score == bestScore)
                {
                    bestMoves.Add(move);
                }
            }

            return bestMoves[random.Next(bestMoves.Count)];
        }

        int Search(Board board, int depth, int ply)
        {
            PieceColor side = board.SideToMove;
            var moves = MoveGenerator.GetAllLegalMoves(board, side);

            if (moves.Count == 0)
                return MoveGenerator.IsInCheck(board, side) ? -(Evaluation.MateScore - ply) : 0;
            if (depth == 0)
                return Evaluation.Evaluate(board, side);

            int best = -Infinity;

            foreach(var move in moves)
            {
                board.MakeMove(move);
                int score = -Search(board, depth - 1, ply + 1);
                board.UndoMove();

                if (score > best)
                    best = score;
            }

            return best;
        }
    }
}
