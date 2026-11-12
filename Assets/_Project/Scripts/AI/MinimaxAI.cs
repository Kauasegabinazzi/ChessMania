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
        public int NodesSearched { get; private set; }

        public MinimaxAI(int depth, int? seed = null)
        {
            this.depth = Math.Max(1, depth);
            random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public Move? ChooseMove(Board board)
        {
            NodesSearched = 0;
            var moves = MoveGenerator.GetAllLegalMoves(board, board.SideToMove);
            if (moves.Count == 0) return null;

            OrderMoves(moves);

            var bestMoves = new List<Move>();
            int bestScore = -Infinity;

            foreach (var move in moves)
            {
                board.MakeMove(move);
                int score = -Search(board, depth - 1, 1, -Infinity, -(bestScore - 1));
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

        int Search(Board board, int depth, int ply, int alpha, int beta)
        {
            NodesSearched++;
            PieceColor side = board.SideToMove;
            var moves = MoveGenerator.GetAllLegalMoves(board, side);

            if (moves.Count == 0)
                return MoveGenerator.IsInCheck(board, side) ? -(Evaluation.MateScore - ply) : 0;
            if (depth == 0)
                return Evaluation.Evaluate(board, side);

            OrderMoves(moves);

            foreach (var move in moves)
            {
                board.MakeMove(move);
                int score = -Search(board, depth - 1, ply + 1, -beta, -alpha);
                board.UndoMove();

                if (score >= beta)
                    return beta;

                if (score > alpha)
                    alpha = score;
            }

            return alpha;
        }

        static void OrderMoves(List<Move> moves)
        {
            moves.Sort((a, b) => CaptureValue(b).CompareTo(CaptureValue(a)));
        }

        static int CaptureValue(Move move)
        {
            return move.Captured == null ? 0 : Evaluation.PieceValue(move.Captured.Type);
        }
    }
}
