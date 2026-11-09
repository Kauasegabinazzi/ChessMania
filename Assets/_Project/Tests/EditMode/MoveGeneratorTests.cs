using NUnit.Framework;

namespace Game.Core.Tests
{
    public class MoveGeneratorTests
    {
        static readonly Square A1 = new Square(0, 0);
        static readonly Square A2 = new Square(0, 1);
        static readonly Square A3 = new Square(0, 2);
        static readonly Square D4 = new Square(3, 3);

        static Board BoardWith(PieceType type, Square square)
        {
            var board = new Board();
            board.SetPiece(square, new Piece(type, PieceColor.White));
            return board;
        }

        [TestCase(PieceType.Rook, 14)]
        [TestCase(PieceType.Bishop, 13)]
        [TestCase(PieceType.Queen, 27)]
        [TestCase(PieceType.Knight, 8)]
        public void PieceAloneOnD4_HasExpectedMoveCount(PieceType type, int expected)
        {
            Board board = BoardWith(type, D4);

            var moves = MoveGenerator.GetMoves(board, D4);

            Assert.AreEqual(expected, moves.Count);
        }

        [Test]
        public void KnightInCorner_HasTwoMoves()
        {
            Board board = BoardWith(PieceType.Knight, A1);

            var moves = MoveGenerator.GetMoves(board, A1);

            Assert.AreEqual(2, moves.Count);
        }

        [Test]
        public void InitialPosition_OnlyKnightsCanMove()
        {
            var board = new Board();
            board.SetupInitialPosition();

            foreach(var(square, piece) in board.AllPieces())
            {
                if (piece.Type == PieceType.Pawn || piece.Type == PieceType.King) continue;

                int expected = piece.Type == PieceType.Knight ? 2 : 0;
                Assert.AreEqual(expected, MoveGenerator.GetMoves(board, square).Count, $"{piece.Color} {piece.Type} on {square}");
            }
        }

        [Test]
        public void Rook_StopsBeforeFriendlyPiece()
        {
            Board board = BoardWith(PieceType.Rook, A1);
            board.SetPiece(A3, new Piece(PieceType.Pawn, PieceColor.White));

            var moves = MoveGenerator.GetMoves(board, A1);

            Assert.AreEqual(8, moves.Count);
            Assert.IsTrue(moves.Exists(m => m.To == A2));
            Assert.IsFalse(moves.Exists(m => m.To == A3));
        }

        [Test]
        public void Rook_CapturesEnemyButDoesNotPassThrough()
        {

            Board board = BoardWith(PieceType.Rook, A1);
            Piece enemy = new Piece(PieceType.Pawn, PieceColor.Black);
            board.SetPiece(A3, enemy);

            var moves = MoveGenerator.GetMoves(board, A1);

            Assert.AreEqual(9, moves.Count);
            Move capture = moves.Find(m => m.To == A3);
            Assert.IsTrue(capture.IsCapture);
            Assert.AreSame(enemy, capture.Captured);
        }
    }
}
