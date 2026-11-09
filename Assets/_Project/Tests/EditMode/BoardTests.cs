using System;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class BoardTests
    {
        static readonly Square G1 = new Square(6, 0);
        static readonly Square F3 = new Square(5, 2);

        [Test]
        public void MakeMove_MovesPieceAndMarksHasMoved()
        {
            var board = new Board();
            board.SetupInitialPosition();
            Piece knight = board.GetPiece(G1);

            board.MakeMove(new Move(G1, F3, knight, null));

            Assert.IsNull(board.GetPiece(G1));
            Assert.AreSame(knight, board.GetPiece(F3));
            Assert.IsTrue(knight.HasMoved);
            Assert.AreEqual(1, board.MoveCount);

        }

        [Test]
        public void UndoMove_RestoresPreviousPosition()
        {
            var board = new Board();
            board.SetupInitialPosition();
            Piece knight = board.GetPiece(G1);
            board.MakeMove(new Move(G1, F3, knight, null));

            bool undone = board.UndoMove();

            Assert.IsTrue(undone);
            Assert.AreSame(knight, board.GetPiece(G1));
            Assert.IsNull(board.GetPiece(F3));
            Assert.IsFalse(knight.HasMoved);
            Assert.AreEqual(0, board.MoveCount);
        }

        [Test]
        public void UndoMove_AfterCapture_RestoresCapturedPiece()
        {
            var board = new Board();
            Piece rook = new Piece(PieceType.Rook, PieceColor.White);
            Piece enemy = new Piece(PieceType.Knight, PieceColor.Black);
            Square from = new Square(0, 0);
            Square to = new Square(0, 5);
            board.SetPiece(from, rook);
            board.SetPiece(to, enemy);

            board.MakeMove(new Move(from, to, rook, enemy));
            board.UndoMove();

            Assert.AreSame(rook, board.GetPiece(from));
            Assert.AreSame(enemy, board.GetPiece(to));
        }

        [Test]
        public void UndoMove_WithEmptyHistory_ReturnsFalse()
        {
            var board = new Board();

            Assert.IsFalse(board.UndoMove());
        }

        [Test]
        public void MakeMove_WithWrongPiece_Throws()
        {
            var board = new Board();
            board.SetupInitialPosition();
            Piece wrongPiece = new Piece(PieceType.Queen, PieceColor.White);

            Assert.Throws<InvalidOperationException>(() => board.MakeMove(new Move(G1, F3, wrongPiece, null)));
        }

        [Test]
        public void SetupInitialPosition_ClearsHistory()
        {
            var board = new Board();
            board.SetupInitialPosition();
            board.MakeMove(new Move(G1, F3, board.GetPiece(G1), null));

            board.SetupInitialPosition();

            Assert.AreEqual(0, board.MoveCount);
            Assert.IsNull(board.LastMove);
        }
    }
}
