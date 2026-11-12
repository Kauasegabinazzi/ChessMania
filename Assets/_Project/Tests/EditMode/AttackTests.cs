using NUnit.Framework;

namespace Game.Core.Tests
{
    public class AttackTests
    {
        static Square Sq(string name) => new Square(name[0] - 'a', name[1] - '1');

        static Board With(string square, PieceType type, PieceColor color)
        {
            var board = new Board();
            board.SetPiece(Sq(square), new Piece(type, color));
            return board;
        }

        [Test]
        public void Pawns_AttackDiagonallyForward_NotBackward()
        {
            var white = With("e4", PieceType.Pawn, PieceColor.White);
            Assert.IsTrue(MoveGenerator.IsSquareAttacked(white, Sq("d5"), PieceColor.White));
            Assert.IsTrue(MoveGenerator.IsSquareAttacked(white, Sq("f5"), PieceColor.White));
            Assert.IsFalse(MoveGenerator.IsSquareAttacked(white, Sq("d3"), PieceColor.White));
            Assert.IsFalse(MoveGenerator.IsSquareAttacked(white, Sq("e5"), PieceColor.White));

            var black = With("e5", PieceType.Pawn, PieceColor.Black);
            Assert.IsTrue(MoveGenerator.IsSquareAttacked(black, Sq("d4"), PieceColor.Black));
            Assert.IsFalse(MoveGenerator.IsSquareAttacked(black, Sq("d6"), PieceColor.Black));
        }

        [Test]
        public void Knight_AttacksItsJumps()
        {
            var board = With("g8", PieceType.Knight, PieceColor.Black);

            Assert.IsTrue(MoveGenerator.IsSquareAttacked(board, Sq("f6"), PieceColor.Black));
            Assert.IsTrue(MoveGenerator.IsSquareAttacked(board, Sq("e7"), PieceColor.Black));
            Assert.IsFalse(MoveGenerator.IsSquareAttacked(board, Sq("g6"), PieceColor.Black));
        }

        [Test]
        public void SlidingAttack_StopsAtFirstPiece()
        {
            var board = With("a1", PieceType.Rook, PieceColor.White);
            board.SetPiece(Sq("a4"), new Piece(PieceType.Pawn, PieceColor.Black));

            Assert.IsTrue(MoveGenerator.IsSquareAttacked(board, Sq("a3"), PieceColor.White));
            Assert.IsTrue(MoveGenerator.IsSquareAttacked(board, Sq("a4"), PieceColor.White));
            Assert.IsFalse(MoveGenerator.IsSquareAttacked(board, Sq("a5"), PieceColor.White));

        }

        [Test]
        public void Queen_AttacksDiagonally()
        {
            var board = With("a1", PieceType.Queen, PieceColor.White);

            Assert.IsTrue(MoveGenerator.IsSquareAttacked(board, Sq("h8"), PieceColor.White));
            Assert.IsFalse(MoveGenerator.IsSquareAttacked(board, Sq("b3"), PieceColor.White));
        }
    }
}
