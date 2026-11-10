using NUnit.Framework;

namespace Game.Core.Tests
{
    public class MoveGeneratorTests
    {
        static readonly Square A1 = new Square(0, 0);
        static readonly Square A2 = new Square(0, 1);
        static readonly Square A3 = new Square(0, 2);
        static readonly Square D4 = new Square(3, 3);
        static readonly Square C5 = new Square(2, 4);
        static readonly Square D2 = new Square(3, 1);
        static readonly Square D3 = new Square(3, 2);
        static readonly Square D5 = new Square(3, 4);
        static readonly Square E5 = new Square(4, 4);
        static readonly Square E7 = new Square(4, 6);


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
        public void InitialPosition_OnlyKnightsAndPawnsCanMove()
        {
            var board = new Board();
            board.SetupInitialPosition();

            foreach(var(square, piece) in board.AllPieces())
            {
                int expected = piece.Type == PieceType.Knight || piece.Type == PieceType.Pawn ? 2 : 0;
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

        [Test]
        public void WhitePawnOnStartRank_CanMoveOneOrTwo()
        {
            Board board = BoardWith(PieceType.Pawn, D2);
            var moves = MoveGenerator.GetMoves(board, D2);
            Assert.AreEqual(2, moves.Count);
        }

        [Test]
        public void WhitePawnAfterStartRank_MovesOnlyOne()
        {
            Board board = BoardWith(PieceType.Pawn, D3);
            var moves = MoveGenerator.GetMoves(board, D3);
            Assert.AreEqual(1, moves.Count);
        }

        [Test]
        public void BlackPawn_MovesDown()
        {
            Board board = new Board();
            board.SetPiece(E7, new Piece(PieceType.Pawn, PieceColor.Black));
            var moves = MoveGenerator.GetMoves(board, E7);
            Assert.AreEqual(2, moves.Count);
            Assert.IsTrue(moves.TrueForAll(m => m.To.Y < E7.Y));
        }

        [Test]
        public void Pawn_BlockedInFront_CannotMove()
        {
            Board board = BoardWith(PieceType.Pawn, D2);
            board.SetPiece(D3, new Piece(PieceType.Knight, PieceColor.Black));
            var moves = MoveGenerator.GetMoves(board, D2);
            Assert.AreEqual(0, moves.Count);
        }

        [Test]
        public void Pawn_CapturesDiagonallyOnlyEnemies()
        {
            Board board = BoardWith(PieceType.Pawn, new Square(3, 3));
            board.SetPiece(C5, new Piece(PieceType.Knight, PieceColor.Black));
            board.SetPiece(E5, new Piece(PieceType.Knight, PieceColor.White));

            var moves = MoveGenerator.GetMoves(board, new Square(3, 3));

            Assert.AreEqual(2, moves.Count);
            Assert.IsTrue(moves.Exists(m => m.To == D5 && !m.IsCapture));
            Assert.IsTrue(moves.Exists(m => m.To == C5 && m.IsCapture));
        }

        [TestCase(3,3,8)]
        [TestCase(0, 0, 3)]
        public void KingAlone_HasExpectedMoveCount(int x, int y, int expected)
        {
            var square = new Square(x, y);
            Board board = BoardWith(PieceType.King, square);

            var moves = MoveGenerator.GetMoves(board, square);

            Assert.AreEqual(expected, moves.Count);
        }
    }
}
