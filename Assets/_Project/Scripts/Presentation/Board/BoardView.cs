using Game.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Presentation
{
    public class BoardView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] SpriteRenderer boardRenderer;
        [SerializeField] PieceView piecePrefab;
        [Header("Visual")]
        [SerializeField] PieceSkin whiteSkin;
        [SerializeField] PieceSkin blackSkin;
        [SerializeField] BoardTheme[] themes;
        [Header("Test")]
        [Tooltip("Creates a board in the initial position when pressing Play. Turn it off when the real game provides the Board.")]
        [SerializeField] bool createTestBoardOnStart = true;
        readonly Dictionary<Piece, PieceView> views = new Dictionary<Piece, PieceView>();
        Board board;

        void Start()
        {
            if (themes != null && themes.Length > 0)
            {
                ApplyTheme(themes[0]);
            }

            if (createTestBoardOnStart && board == null)
            {
                var testBoard = new Board();
                testBoard.SetupInitialPosition();
                Initialize(testBoard);
            }
        }

        public void Initialize(Board newBoard)
        {
            board = newBoard;
            Rebuild();
        }

        public Vector3 GridToWorld(Square square)
        {
            return transform.position + new Vector3(square.X - 3.5f, square.Y - 3.5f, 0f);
        }

        public void Rebuild()
        {
            foreach (var view in views.Values)
            {
                if (view != null)
                {
                    Destroy(view.gameObject);
                }
            }
            views.Clear();

            if (board == null) return;

            foreach (var (square, piece) in board.AllPieces())
            {
                PieceView view = Instantiate(piecePrefab, transform);
                view.name = $"{piece.Color} {piece.Type} {square}";
                view.Bind(piece, GetSkin(piece.Color));
                view.MoveTo(GridToWorld(square));
                views[piece] = view;
            }
        }

        public void SetSkins(PieceSkin white, PieceSkin black)
        {
            whiteSkin = white;
            blackSkin = black;

            foreach (var view in views.Values)
            {
                view.SetSkin(GetSkin(view.Piece.Color));
            }
        }

        public bool ApplyThemeById(string themeId)
        {
            if (themes == null) return false;

            foreach (var theme in themes)
            {
                if(theme != null && theme.Id == themeId)
                {
                    ApplyTheme(theme);
                    return true;
                }
            }

            return false;
        }

        public void ApplyTheme(BoardTheme theme)
        {
            if (theme == null || boardRenderer == null) return;

            boardRenderer.sprite = theme.BoardSprite;
        }

        PieceSkin GetSkin(PieceColor color)
        {
            return color == PieceColor.White ? whiteSkin : blackSkin;
        }
    }
}
