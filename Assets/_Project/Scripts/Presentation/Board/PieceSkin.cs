using Game.Core;
using UnityEngine;
using System;

namespace Game.Presentation
{
    [CreateAssetMenu(fileName = "NewPieceSkin", menuName = "Game/Piece Skin")]
    public class PieceSkin : ScriptableObject
    {
        [Serializable]
        public class PieceFrames
        {
            public PieceType type;
            public Sprite[] frames;
        }

        [SerializeField] PieceFrames[] pieces = new PieceFrames[0];

        public Sprite[] GetFrames(PieceType type)
        {
            foreach(var entry in pieces)
            {
                if (entry.type == type)
                    return entry.frames;
            }
            return null;
        }

#if UNITY_EDITOR
        public void SetPieces(PieceFrames[] newPieces)
        {
            pieces = newPieces;
        }
#endif

    }
}
