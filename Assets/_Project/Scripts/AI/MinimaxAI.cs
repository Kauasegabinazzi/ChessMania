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
        public int ScoreMargin { get; set; }
        public double MistakeChance { get; set; }
        readonly List<ulong> path = new List<ulong>();

        public MinimaxAI(int depth, int? seed = null)
        {
            this.depth = Math.Max(1, depth);
            random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        //public Move? ChooseMove(Board board)
        //{
        //    NodesSearched = 0;
        //    var moves = MoveGenerator.GetAllLegalMoves(board, board.SideToMove);
        //    if (moves.Count == 0) return null;

        //    OrderMoves(moves);

        //    var bestMoves = new List<Move>();
        //    int bestScore = -Infinity;

        //    foreach (var move in moves)
        //    {
        //        board.MakeMove(move);
        //        int score = -Search(board, depth - 1, 1, -Infinity, -(bestScore - 1));
        //        board.UndoMove();

        //        if (score > bestScore)
        //        {
        //            bestScore = score;
        //            bestMoves.Clear();
        //            bestMoves.Add(move);
        //        }
        //        else if (score == bestScore)
        //        {
        //            bestMoves.Add(move);
        //        }
        //    }

        //    return bestMoves[random.Next(bestMoves.Count)];
        //}

        public Move? ChooseMove(Board board)
        {
            NodesSearched = 0;
            var moves = MoveGenerator.GetAllLegalMoves(board, board.SideToMove);
            if (moves.Count == 0) return null;

            if (MistakeChance > 0 && random.NextDouble() < MistakeChance)
                return moves[random.Next(moves.Count)];

            BuildPath(board);

            OrderMoves(moves);

            var scored = new List<(Move move, int score)>();
            int bestScore = -Infinity;

            foreach (var move in moves)
            {
                board.MakeMove(move);
                int score = -Search(board, depth - 1, 1, -Infinity, -(bestScore - ScoreMargin - 1));
                board.UndoMove();

                scored.Add((move, score));

                if (score > bestScore)
                    bestScore = score;
            }

            var candidates = scored.FindAll(s => s.score >= bestScore - ScoreMargin);
            return candidates[random.Next(candidates.Count)].move;
        }

        void BuildPath(Board board)
        {
            path.Clear();
            var undone = new Stack<Move>();
            int plies = board.HalfmoveClock;

            for (int i = 0; i < plies; i++)
            {
                undone.Push(board.LastMove.Value);
                board.UndoMove();
            }

            path.Add(Zobrist.Hash(board));

            while (undone.Count > 0)
            {
                board.MakeMove(undone.Pop());
                path.Add(Zobrist.Hash(board));
            }
        }

        int SearchMoves(Board board, int depth, int ply, int alpha, int beta)
        {
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

        int Search(Board board, int depth, int ply, int alpha, int beta)
        {
            NodesSearched++;

            ulong hash = Zobrist.Hash(board);
            if (IsDraw(board, hash)) return 0;

            path.Add(hash);
            int score = SearchMoves(board, depth, ply, alpha, beta);
            path.RemoveAt(path.Count - 1);

            return score;
        }

        bool IsDraw(Board board, ulong hash)
        {
            int clock = board.HalfmoveClock;
            if (clock >= 100) return true;
            if (DrawRules.IsInsufficientMaterial(board)) return true;

            int oldest = Math.Max(0, path.Count - clock);
            for (int i = path.Count - 2; i >= oldest; i -= 2)
            {
                if (path[i] == hash) return true;
            }

            return false;
        }

        static void OrderMoves(List<Move> moves)
        {
            moves.Sort((a, b) => CaptureValue(b).CompareTo(CaptureValue(a)));
        }

        static int CaptureValue(Move move)
        {
            //return move.Captured == null ? 0 : Evaluation.PieceValue(move.Captured.Type);

            int value = move.Captured == null ? 0 : Evaluation.PieceValue(move.Captured.Type);

            if (move.IsPromotion)
                value += Evaluation.PieceValue(move.Promotion.Value);

            return value;
        }
    }
}
