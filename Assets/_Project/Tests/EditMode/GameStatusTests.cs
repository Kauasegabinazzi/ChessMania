using UnityEngine;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class GameStatusTests
    {
        static Square Sq(string name) => new Square(name[0] - 'a', name[1] - '1');

        static void Play(Board board, string from, string to)
        {
            var moves = MoveGenerator.GetLegalMoves(board, Sq(from));
            Move move = moves.Find(m => m.To == Sq(to));

            Assert.IsNotNull(move.Piece, $"Illegal move {from}-{to}");
            board.MakeMove(move);
        }

        [Test]
        public void InitialPosition_HasTwentyLegalMoves()
        {
            var board = new Board();
            board.SetupInitialPosition();

            Assert.AreEqual(20, MoveGenerator.GetAllLegalMoves(board, PieceColor.White).Count);
        }

        [Test]
        public void InitialPosition_IsInProgress()
        {
            var board = new Board();
            board.SetupInitialPosition();

            Assert.AreEqual(GameStatus.InProgress, MoveGenerator.GetStatus(board));
        }

        [Test]
        public void QueenOnH5_IsCheck()
        {
            var board = new Board();
            board.SetupInitialPosition();
            Play(board, "e2", "e4");
            Play(board, "f7", "f6");
            Play(board, "d1", "h5");

            Assert.AreEqual(GameStatus.Check, MoveGenerator.GetStatus(board));
        }

        [Test]
        public void FoolsMate_IsCheckmate()
        {
            var board = new Board();
            board.SetupInitialPosition();
            Play(board, "f2", "f3");
            Play(board, "e7", "e5");
            Play(board, "g2", "g4");
            Play(board, "d8", "h4");

            Assert.AreEqual(GameStatus.Checkmate, MoveGenerator.GetStatus(board));
        }

        [Test]
        public void KingWithNoMovesAndNotInCheck_IsStalemate()
        {
            var board = new Board();
            board.SetPiece(Sq("a1"), new Piece(PieceType.King, PieceColor.White));
            board.SetPiece(Sq("b3"), new Piece(PieceType.Queen, PieceColor.Black));
            board.SetPiece(Sq("h8"), new Piece(PieceType.King, PieceColor.Black));

            Assert.AreEqual(GameStatus.Stalemate, MoveGenerator.GetStatus(board));
        }
    }
}
