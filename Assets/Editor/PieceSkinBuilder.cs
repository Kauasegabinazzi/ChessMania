using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Game.Core;
using Game.Presentation;

public static class PieceSkinBuilder
{
    const int CellSize = 16;
    const int ColumnsPerColor = 5;
    const string OutputFolder = "Assets/_Project/ScriptableObjects/Pieces";

    static readonly PieceType[] RowOrder =
    {
        PieceType.Pawn,
        PieceType.Rook,
        PieceType.Knight,
        PieceType.Bishop,
        PieceType.Queen,
        PieceType.King
    };

    static readonly (string name, int firstRow, int firstCol)[] Skins =
    {
        ("Skin_Blue",  0, 0),
        ("Skin_Green", 0, 5),
        ("Skin_Gray",  6, 0),
        ("Skin_Navy",  6, 5),
    };

    [MenuItem("Tools/Project/Build Piece Skins")]
    public static void Build()
    {
        Texture2D texture = null;
        if (Selection.activeObject is Texture2D selectedTexture)
            texture = selectedTexture;
        else if (Selection.activeObject is Sprite selectedSprite)
            texture = selectedSprite.texture;

        if (texture == null)
        {
            EditorUtility.DisplayDialog("Build Piece Skins",
                "Selecione o PNG das peças (Chess Pieces) na janela Project e rode de novo.", "OK");
            return;
        }

        string path = AssetDatabase.GetAssetPath(texture);

        var cells = AssetDatabase.LoadAllAssetRepresentationsAtPath(path)
            .OfType<Sprite>()
            .Select(sprite => (
                sprite: sprite,
                col: Mathf.RoundToInt(sprite.rect.x / CellSize),
                row: Mathf.RoundToInt((texture.height - sprite.rect.yMax) / CellSize)))
            .ToList();

        if (cells.Count == 0)
        {
            EditorUtility.DisplayDialog("Build Piece Skins",
                "This PNG does not have sliced sprites. Slice it into a 16x16 grid first.", "OK");
            return;
        }

        if (!AssetDatabase.IsValidFolder(OutputFolder))
        {
            Directory.CreateDirectory(OutputFolder);
            AssetDatabase.Refresh();
        }

        foreach (var (skinName, firstRow, firstCol) in Skins)
        {
            var pieces = new PieceSkin.PieceFrames[RowOrder.Length];

            for (int i = 0; i < RowOrder.Length; i++)
            {
                int row = firstRow + i;

                Sprite[] frames = cells
                    .Where(c => c.row == row && c.col >= firstCol && c.col < firstCol + ColumnsPerColor)
                    .OrderBy(c => c.col)
                    .Select(c => c.sprite)
                    .ToArray();

                if (frames.Length == 0)
                    Debug.LogWarning($"{skinName}: no frames found for {RowOrder[i]} (row {row}).");

                pieces[i] = new PieceSkin.PieceFrames { type = RowOrder[i], frames = frames };
            }

            SaveSkin(skinName, pieces);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Skins created for {OutputFolder} ({cells.Count} sprites read).");
    }

    static void SaveSkin(string skinName, PieceSkin.PieceFrames[] pieces)
    {
        string assetPath = $"{OutputFolder}/{skinName}.asset";

        var skin = AssetDatabase.LoadAssetAtPath<PieceSkin>(assetPath);
        if (skin == null)
        {
            skin = ScriptableObject.CreateInstance<PieceSkin>();
            AssetDatabase.CreateAsset(skin, assetPath);
        }

        skin.SetPieces(pieces);
        EditorUtility.SetDirty(skin);
    }
}