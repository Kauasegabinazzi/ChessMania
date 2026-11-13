using NUnit.Framework;

namespace Game.Core.Tests
{
    public class EnPassantTests
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

        static bool HasEnPassant(Board board, string pawn)
        {
            return MoveGenerator.GetLegalMoves(board, Sq(pawn)).Exists(m => m.IsEnPassant);
        }

        static Board BlackJustDoublePushed()
        {
            var board = new Board();
            Place(board, "a1", PieceType.King, PieceColor.White);
            Place(board, "e5", PieceType.Pawn, PieceColor.White);
            Place(board, "h8", PieceType.King, PieceColor.Black);
            Place(board, "d7", PieceType.Pawn, PieceColor.Black);
            Play(board, "a1", "b1");
            Play(board, "d7", "d5");
            return board;
        }

        [Test]
        public void Available_RightAfterDoublePush()
        {
            var board = BlackJustDoublePushed();
            Piece blackPawn = board.GetPiece(Sq("d5"));

            Move move = MoveGenerator.GetLegalMoves(board, Sq("e5")).Find(m => m.IsEnPassant);

            Assert.AreEqual(Sq("d6"), move.To);
            Assert.AreSame(blackPawn, move.Captured);
        }

        [Test]
        public void MakeAndUndo_RemovesAndRestoresPawn()
        {
            var board = BlackJustDoublePushed();
            Piece whitePawn = board.GetPiece(Sq("e5"));
            Piece blackPawn = board.GetPiece(Sq("d5"));

            Play(board, "e5", "d6");
            Assert.AreSame(whitePawn, board.GetPiece(Sq("d6")));
            Assert.IsNull(board.GetPiece(Sq("d5")));
            Assert.IsNull(board.GetPiece(Sq("e5")));

            board.UndoMove();
            Assert.AreSame(whitePawn, board.GetPiece(Sq("e5")));
            Assert.AreSame(blackPawn, board.GetPiece(Sq("d5")));
            Assert.IsNull(board.GetPiece(Sq("d6")));
        }

        [Test]
        public void NotAvailable_OneMoveLater()
        {
            var board = BlackJustDoublePushed();
            Play(board, "b1", "a1");
            Play(board, "h8", "g8");

            Assert.IsFalse(HasEnPassant(board, "e5"));
        }

        [Test]
        public void NotAvailable_AfterTwoSingleSteps()
        {
            var board = new Board();
            Place(board, "a1", PieceType.King, PieceColor.White);
            Place(board, "e5", PieceType.Pawn, PieceColor.White);
            Place(board, "h8", PieceType.King, PieceColor.Black);
            Place(board, "d7", PieceType.Pawn, PieceColor.Black);
            Play(board, "a1", "b1");
            Play(board, "d7", "d6");
            Play(board, "b1", "a1");
            Play(board, "d6", "d5");

            Assert.IsFalse(HasEnPassant(board, "e5"));
        }

        [Test]
        public void BlackCanTakeEnPassant()
        {
            var board = new Board();
            Place(board, "a1", PieceType.King, PieceColor.White);
            Place(board, "e2", PieceType.Pawn, PieceColor.White);
            Place(board, "h8", PieceType.King, PieceColor.Black);
            Place(board, "d4", PieceType.Pawn, PieceColor.Black);
            Play(board, "e2", "e4");

            Move move = MoveGenerator.GetLegalMoves(board, Sq("d4")).Find(m => m.IsEnPassant);

            Assert.IsTrue(move.IsEnPassant);
            Assert.AreEqual(Sq("e3"), move.To);
        }

        [Test]
        public void NotAllowed_WhenItExposesKing()
        {
            var board = new Board();
            Place(board, "a5", PieceType.King, PieceColor.White);
            Place(board, "e5", PieceType.Pawn, PieceColor.White);
            Place(board, "a1", PieceType.Rook, PieceColor.White);
            Place(board, "h8", PieceType.King, PieceColor.Black);
            Place(board, "h5", PieceType.Rook, PieceColor.Black);
            Place(board, "d7", PieceType.Pawn, PieceColor.Black);
            Play(board, "a1", "a2");
            Play(board, "d7", "d5");

            Assert.IsFalse(HasEnPassant(board, "e5"));
        }
    }
}
