using Game.Core;
using UnityEngine;

namespace Game.Presentation
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PieceView : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] float framesPerSecond = 4f;

        PieceSkin skin;
        Sprite[] frames;
        float timeOffset;
        int currentFrame = -1;
        public Piece Piece { get; private set; }

        void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Bind(Piece piece, PieceSkin newSkin)
        {
            Piece = piece;

            timeOffset = Random.Range(0f, 10f);

            SetSkin(newSkin);
        }

        public void SetSkin(PieceSkin newSkin)
        {
            skin = newSkin;
            Refresh();
        }

        public void Refresh()
        {
            frames = (skin != null && Piece != null) ? skin.GetFrames(Piece.Type) : null;
            currentFrame = -1;
            ShowFrame();
        }

        void Update()
        {
            ShowFrame();
        }

        void ShowFrame()
        {
            if (frames == null || frames.Length == 0)
            {
                return;
            }

            int index = (int)((Time.time + timeOffset) * framesPerSecond) % frames.Length;
            if(index == currentFrame)
            {
                return;
            }

            currentFrame = index;
            spriteRenderer.sprite = frames[index];
        }

        public void MoveTo(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }
    }
}
