using NUnit.Framework;

namespace Game.Core.Tests
{
    public class DrawTests
    {
        static Square Sq(string name) => new Square(name[0] - 'a', name[1] - '1');

        static void Place(Board board, string square, PieceType type, PieceColor color)
        {
            board.SetPiece(Sq(square), new Piece(type, color));
        }

        static void Play(Board board, string from, string to)
        {
            Move move = MoveGenerator.GetLegalMoves(board, Sq(from)).Find(m => m.To == Sq(to));
            board.MakeMove(move);
        }

        static Board KingsOnly()
        {
            var board = new Board();
            Place(board, "e1", PieceType.King, PieceColor.White);
            Place(board, "e8", PieceType.King, PieceColor.Black);
            return board;
        }

        [Test]
        public void KingVsKing_IsDraw()
        {
            Assert.AreEqual(GameStatus.DrawInsufficientMaterial, MoveGenerator.GetStatus(KingsOnly()));
        }

        [Test]
        public void KingAndKnightVsKing_IsDraw()
        {
            var board = KingsOnly();
            Place(board, "b1", PieceType.Knight, PieceColor.White);

            Assert.AreEqual(GameStatus.DrawInsufficientMaterial, MoveGenerator.GetStatus(board));
        }

        [Test]
        public void KingAndRookVsKing_IsNotDraw()
        {
            var board = KingsOnly();
            Place(board, "a1", PieceType.Rook, PieceColor.White);

            Assert.AreEqual(GameStatus.InProgress, MoveGenerator.GetStatus(board));
        }

        [Test]
        public void TwoMinorPieces_IsNotInsufficient()
        {
            var board = KingsOnly();
            Place(board, "c1", PieceType.Bishop, PieceColor.White);
            Place(board, "g8", PieceType.Knight, PieceColor.Black);

            Assert.IsFalse(DrawRules.IsInsufficientMaterial(board));
        }

        [Test]
        public void HalfmoveClock_ResetsOnPawnMoveAndCapture()
        {
            var board = KingsOnly();
            Place(board, "a1", PieceType.Rook, PieceColor.White);
            Place(board, "b2", PieceType.Pawn, PieceColor.White);
            Place(board, "h8", PieceType.Rook, PieceColor.Black);

            Play(board, "a1", "a3");
            Play(board, "h8", "h6");
            Assert.AreEqual(2, board.HalfmoveClock);

            Play(board, "b2", "b4");
            Assert.AreEqual(0, board.HalfmoveClock);

            Play(board, "h6", "h5");
            Play(board, "a3", "a5");
            Assert.AreEqual(2, board.HalfmoveClock);

            Play(board, "h5", "a5");
            Assert.AreEqual(0, board.HalfmoveClock);
        }

        static void KnightsDance(Board board)
        {
            Play(board, "b1", "c3");
            Play(board, "g8", "f6");
            Play(board, "c3", "b1");
            Play(board, "f6", "g8");
        }

        static Board KingsAndKnights()
        {
            var board = KingsOnly();
            Place(board, "b1", PieceType.Knight, PieceColor.White);
            Place(board, "g8", PieceType.Knight, PieceColor.Black);
            return board;
        }

        [Test]
        public void TwoRepetitions_IsNotDrawYet()
        {
            var board = KingsAndKnights();
            KnightsDance(board);

            Assert.AreEqual(GameStatus.InProgress, MoveGenerator.GetStatus(board));
        }

        [Test]
        public void ThreeRepetitions_IsDraw_AndBoardIsUnchanged()
        {
            var board = KingsAndKnights();
            KnightsDance(board);
            KnightsDance(board);

            Assert.AreEqual(GameStatus.DrawRepetition, MoveGenerator.GetStatus(board));
            Assert.AreEqual(8, board.MoveCount);
            Assert.IsNotNull(board.GetPiece(Sq("b1")));
            Assert.IsNotNull(board.GetPiece(Sq("g8")));
        }
    }
}
