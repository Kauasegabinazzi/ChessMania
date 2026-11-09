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
        [Header("Highlights")]
        [SerializeField] SpriteRenderer highlightPrefab;
        [SerializeField] Color selectedColor = new Color(1f, 0.85f, 0.2f, 0.5f);
        [SerializeField] Color moveColor = new Color(0.3f, 0.9f, 0.4f, 0.45f);
        [SerializeField] Color captureColor = new Color(0.95f, 0.3f, 0.3f, 0.5f);

        readonly List<SpriteRenderer> highlights = new List<SpriteRenderer>();
        public Board Board => board;


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

        public bool TryWorldToGrid(Vector3 worldPosition, out Square square)
        {
            Vector3 local = worldPosition - transform.position;
            square = new Square(Mathf.FloorToInt(local.x + 4f), Mathf.FloorToInt(local.y + 4f));
            return square.IsValid;
        }

        public void ApplyMove(Move move)
        {
            if(move.Captured != null && views.TryGetValue(move.Captured, out PieceView captureView))
            {
                Destroy(captureView.gameObject);
                views.Remove(move.Captured);
            }
            if(views.TryGetValue(move.Piece, out PieceView view))
            {
                view.MoveTo(GridToWorld(move.To));
                view.name = $"{move.Piece.Color} {move.Piece.Type} {move.To}";
            }
        }

        public void ShowHighlights(Square selected, List<Move> moves)
        {
            ClearHighlights();
            AddHighlight(selected, selectedColor);

            foreach(var move in moves)
            {
                AddHighlight(move.To, move.IsCapture ? captureColor : moveColor);
            }
        }

        public void ClearHighlights()
        {
            foreach(var highlight in highlights)
            {
                if (highlight != null)
                    Destroy(highlight.gameObject);
            }
            highlights.Clear();
        }

        void AddHighlight(Square square, Color color)
        {
            SpriteRenderer highlight = Instantiate(highlightPrefab, transform);
            highlight.transform.position = GridToWorld(square);
            highlight.color = color;
            highlights.Add(highlight);
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
