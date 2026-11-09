using UnityEngine;
using System.Collections.Generic;
using Game.Core;
using UnityEngine.InputSystem;

namespace Game.Presentation
{
    public class BoardInput : MonoBehaviour
    {
        [SerializeField] BoardView boardView;
        [SerializeField] Camera mainCamera;

        Square? selected;
        List<Move> selectedMoves = new List<Move>();

        void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }

        void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return;

            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouse.position.ReadValue());

            if (boardView.TryWorldToGrid(worldPosition, out Square clicked))
                OnSquareClicked(clicked);
            else
                Deselect();
        }

        void Select(Square square)
        {
            selected = square;
            selectedMoves = MoveGenerator.GetMoves(boardView.Board, square);
            boardView.ShowHighlights(square, selectedMoves);
        }

        void Deselect()
        {
            selected = null;
            selectedMoves.Clear();
            boardView.ClearHighlights();
        }

        void OnSquareClicked(Square square)
        {
            Board board = boardView.Board;
            if (board == null) return;

            if (selected.HasValue && TryFindMove(square, out Move move))
            {
                board.MakeMove(move);
                boardView.ApplyMove(move);
                Deselect();
                return;
            }

            Piece piece = board.GetPiece(square);
            bool isOwnPiece = piece != null && piece.Color == board.SideToMove;

            if (isOwnPiece && selected != square)
                Select(square);
            else
                Deselect();
        }

        bool TryFindMove(Square to, out Move move)
        {
            foreach(var candidate in selectedMoves)
            {
                if(candidate.To == to)
                {
                    move = candidate;
                    return true;
                }
            }

            move = default;
            return false;
        }
    }
}
