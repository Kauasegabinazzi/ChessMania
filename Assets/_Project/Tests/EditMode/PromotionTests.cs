using NUnit.Framework;

namespace Game.Core.Tests
{
    public class PromotionTests
    {
        static Square Sq(string name) => new Square(name[0] - 'a', name[1] - '1');

        static void Place(Board board, string square, PieceType type, PieceColor color)
        {
            board.SetPiece(Sq(square), new Piece(type, color));
        }

        static Board WhitePawnOnE7()
        {
            var board = new Board();
            Place(board, "a1", PieceType.King, PieceColor.White);
            Place(board, "h1", PieceType.King, PieceColor.Black);
            Place(board, "e7", PieceType.Pawn, PieceColor.White);
            return board;
        }

        [Test]
        public void PawnOnSeventhRank_HasFourPromotions()
        {
            var moves = MoveGenerator.GetLegalMoves(WhitePawnOnE7(), Sq("e7"));

            Assert.AreEqual(4, moves.Count);
            Assert.IsTrue(moves.TrueForAll(m => m.To == Sq("e8") && m.IsPromotion));
            Assert.IsTrue(moves.Exists(m => m.Promotion == PieceType.Queen));
            Assert.IsTrue(moves.Exists(m => m.Promotion == PieceType.Knight));
        }

        [Test]
        public void PromotionWithCapture_AddsFourMore()
        {
            var board = WhitePawnOnE7();
            Place(board, "d8", PieceType.Rook, PieceColor.Black);

            var moves = MoveGenerator.GetLegalMoves(board, Sq("e7"));

            Assert.AreEqual(8, moves.Count);
            Assert.AreEqual(4, moves.FindAll(m => m.IsCapture).Count);
        }

        [Test]
        public void BlackPawn_PromotesOnFirstRank()
        {
            var board = new Board();
            Place(board, "a8", PieceType.King, PieceColor.White);
            Place(board, "h8", PieceType.King, PieceColor.Black);
            Place(board, "d2", PieceType.Pawn, PieceColor.Black);

            var moves = MoveGenerator.GetLegalMoves(board, Sq("d2"));

            Assert.AreEqual(4, moves.Count);
            Assert.IsTrue(moves.TrueForAll(m => m.To == Sq("d1") && m.IsPromotion));
        }

        [Test]
        public void MakeAndUndo_Promotion()
        {
            var board = WhitePawnOnE7();
            var pawn = board.GetPiece(Sq("e7"));
            var moves = MoveGenerator.GetLegalMoves(board, Sq("e7"));
            Move toQueen = moves.Find(m => m.Promotion == PieceType.Queen);

            board.MakeMove(toQueen);
            Assert.AreSame(pawn, board.GetPiece(Sq("e8")));
            Assert.AreEqual(PieceType.Queen, pawn.Type);

            board.UndoMove();
            Assert.AreSame(pawn, board.GetPiece(Sq("e7")));
            Assert.AreEqual(PieceType.Pawn, pawn.Type);
            Assert.IsNull(board.GetPiece(Sq("e8")));
        }
    }
}
