using NUnit.Framework;

namespace Game.Core.Tests
{
    public class CheckTests
    {
        static Square Sq(string name) => new Square(name[0] - 'a', name[1] - '1');

        static void Place(Board board, string square, PieceType type, PieceColor color)
        {
            board.SetPiece(Sq(square), new Piece(type, color));
        }

        [Test]
        public void IsInCheck_RookOnSameFile_IsCheck()
        {
            var board = new Board();
            Place(board, "e1", PieceType.King, PieceColor.White);
            Place(board, "e8", PieceType.Rook, PieceColor.Black);
            Assert.IsTrue(MoveGenerator.IsInCheck(board, PieceColor.White));
        }

        [Test]
        public void IsInCheck_BlockedByPiece_IsNotCheck()
        {
            var board = new Board();
            Place(board, "e1", PieceType.King, PieceColor.White);
            Place(board, "e2", PieceType.Pawn, PieceColor.White);
            Place(board, "e8", PieceType.Rook, PieceColor.Black);
            Assert.IsFalse(MoveGenerator.IsInCheck(board, PieceColor.White));
        }

        [Test]
        public void PinnedPiece_CannotMove()
        {
            var board = new Board();
            Place(board, "e1", PieceType.King, PieceColor.White);
            Place(board, "e2", PieceType.Bishop, PieceColor.White);
            Place(board, "e8", PieceType.Rook, PieceColor.Black);

            var moves = MoveGenerator.GetLegalMoves(board, Sq("e2"));

            Assert.AreEqual(0, moves.Count);
        }

        [Test]
        public void King_CannotMoveIntoAttackedSquare()
        {
            var board = new Board();
            Place(board, "e1", PieceType.King, PieceColor.White);
            Place(board, "d8", PieceType.Rook, PieceColor.Black);
            var moves = MoveGenerator.GetLegalMoves(board, Sq("e1"));

            Assert.AreEqual(3, moves.Count);
            Assert.IsFalse(moves.Exists(m => m.To.X == Sq("d1").X));
        }

        [Test]
        public void InCheck_OnlyBlockingMoveIsLegal()
        {
            var board = new Board();
            Place(board, "e1", PieceType.King, PieceColor.White);
            Place(board, "a2", PieceType.Rook, PieceColor.White);
            Place(board, "e8", PieceType.Rook, PieceColor.Black);
            var moves = MoveGenerator.GetLegalMoves(board, Sq("a2"));
            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(Sq("e2"), moves[0].To);
        }

        [Test]
        public void King_CanCaptureUndefendedAttacker()
        {
            var board = new Board();
            Place(board, "e1", PieceType.King, PieceColor.White);
            Place(board, "e2", PieceType.Queen, PieceColor.Black);
            var moves = MoveGenerator.GetLegalMoves(board, Sq("e1"));
            Assert.AreEqual(1, moves.Count);
            Assert.IsTrue(moves[0].IsCapture);
        }

        [Test]
        public void GetLegalMoves_LeavesBoardUnchanged()
        {
            var board = new Board();
            board.SetupInitialPosition();
            Piece knight = board.GetPiece(Sq("g1"));

            MoveGenerator.GetLegalMoves(board, Sq("g1"));

            Assert.AreEqual(0, board.MoveCount);
            Assert.AreSame(knight, board.GetPiece(Sq("g1")));
            Assert.IsFalse(knight.HasMoved);
            Assert.IsNull(board.GetPiece(Sq("f3")));
        }
    }
}
