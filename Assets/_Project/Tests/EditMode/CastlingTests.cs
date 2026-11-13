using NUnit.Framework;

namespace Game.Core.Tests
{
    public class CastlingTests
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

        static Board WhiteReadyToCastle()
        {
            var board = new Board();
            Place(board, "e1", PieceType.King, PieceColor.White);
            Place(board, "a1", PieceType.Rook, PieceColor.White);
            Place(board, "h1", PieceType.Rook, PieceColor.White);
            Place(board, "e8", PieceType.King, PieceColor.Black);
            return board;
        }

        static bool CanCastleTo(Board board, string king, string target)
        {
            return MoveGenerator.GetLegalMoves(board, Sq(king)).Exists(m => m.IsCastling && m.To == Sq(target));
        }

        [Test]
        public void BothSides_Available()
        {
            var board = WhiteReadyToCastle();

            Assert.IsTrue(CanCastleTo(board, "e1", "g1"));
            Assert.IsTrue(CanCastleTo(board, "e1", "c1"));
        }

        [Test]
        public void BlackCanCastle()
        {
            var board = new Board();
            Place(board, "e1", PieceType.King, PieceColor.White);
            Place(board, "e8", PieceType.King, PieceColor.Black);
            Place(board, "h8", PieceType.Rook, PieceColor.Black);

            Assert.IsTrue(CanCastleTo(board, "e8", "g8"));
        }

        [Test]
        public void NotAllowed_AfterKingMoved()
        {
            var board = WhiteReadyToCastle();
            Play(board, "e1", "f1");
            Play(board, "e8", "d8");
            Play(board, "f1", "e1");
            Play(board, "d8", "e8");

            Assert.IsFalse(CanCastleTo(board, "e1", "g1"));
            Assert.IsFalse(CanCastleTo(board, "e1", "c1"));
        }

        [Test]
        public void NotAllowed_AfterRookMoved()
        {
            var board = WhiteReadyToCastle();
            Play(board, "h1", "h2");
            Play(board, "e8", "d8");
            Play(board, "h2", "h1");
            Play(board, "d8", "e8");

            Assert.IsFalse(CanCastleTo(board, "e1", "g1"));
            Assert.IsTrue(CanCastleTo(board, "e1", "c1"));
        }

        [Test]
        public void NotAllowed_WithPieceBetween()
        {
            var board = WhiteReadyToCastle();
            Place(board, "b1", PieceType.Knight, PieceColor.White);

            Assert.IsTrue(CanCastleTo(board, "e1", "g1"));
            Assert.IsFalse(CanCastleTo(board, "e1", "c1"));
        }

        [Test]
        public void NotAllowed_WhenInCheck()
        {
            var board = WhiteReadyToCastle();
            Place(board, "e5", PieceType.Rook, PieceColor.Black);

            Assert.IsFalse(CanCastleTo(board, "e1", "g1"));
            Assert.IsFalse(CanCastleTo(board, "e1", "c1"));
        }

        [Test]
        public void NotAllowed_ThroughAttackedSquare()
        {
            var board = WhiteReadyToCastle();
            Place(board, "f8", PieceType.Rook, PieceColor.Black);

            Assert.IsFalse(CanCastleTo(board, "e1", "g1"));
            Assert.IsTrue(CanCastleTo(board, "e1", "c1"));
        }

        [Test]
        public void MakeAndUndo_MovesRookToo()
        {
            var board = WhiteReadyToCastle();

            Piece king = board.GetPiece(Sq("e1"));
            Piece rook = board.GetPiece(Sq("h1"));

            Play(board, "e1", "g1");
            Assert.AreSame(king, board.GetPiece(Sq("g1")));
            Assert.AreSame(rook, board.GetPiece(Sq("f1")));
            Assert.IsNull(board.GetPiece(Sq("h1")));
            Assert.IsNull(board.GetPiece(Sq("e1")));

            board.UndoMove();
            Assert.AreSame(king, board.GetPiece(Sq("e1")));
            Assert.AreSame(rook, board.GetPiece(Sq("h1")));
            Assert.IsNull(board.GetPiece(Sq("f1")));
            Assert.IsNull(board.GetPiece(Sq("g1")));
            Assert.IsFalse(king.HasMoved);
            Assert.IsFalse(rook.HasMoved);
        }

    }
}
