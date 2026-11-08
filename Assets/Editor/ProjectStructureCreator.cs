using System.IO;
using UnityEditor;
using UnityEngine;

public static class ProjectStructureCreator
{
    // O "_" faz a pasta aparecer no topo da janela Project.
    const string Root = "_Project";

    static readonly string[] Folders =
    {
        // Arte
        "Art/Sprites/Pieces",
        "Art/Sprites/Board",
        "Art/Sprites/Cards",
        "Art/Sprites/UI",
        "Art/Sprites/Backgrounds",
        "Art/Sprites/VFX",
        "Art/Fonts",
        "Art/Animations",
        "Art/Materials",
        "Art/Shaders",
 
        // Áudio
        "Audio/Music",
        "Audio/SFX",
 
        // Prefabs
        "Prefabs/Pieces",
        "Prefabs/Cards",
        "Prefabs/Board",
        "Prefabs/UI",
        "Prefabs/VFX",
 
        // Dados (ScriptableObjects)
        "ScriptableObjects/Jokers",
        "ScriptableObjects/Enemies",
        "ScriptableObjects/Pieces",
        "ScriptableObjects/Config",
 
        // Cenas e configurações
        "Scenes",
        "Settings",
 
        // Código
        "Scripts/Core",           // regras do xadrez, C# puro (sem Unity)
        "Scripts/AI",             // minimax, avaliação, C# puro
        "Scripts/Modifiers",      // jokers, gatilhos e pontuação
        "Scripts/Run",            // loop roguelike: rodadas, loja, progressão
        "Scripts/Presentation/Board",
        "Scripts/Presentation/Cards",
        "Scripts/Presentation/UI",
        "Scripts/Presentation/Audio",
        "Scripts/Presentation/VFX",
        "Scripts/Presentation/Input",
 
        // Testes
        "Tests/EditMode",
        "Tests/PlayMode",
    };

    [MenuItem("Tools/Project/Create Folder Structure")]
    public static void Create()
    {
        string dataPath = Application.dataPath;

        foreach (var folder in Folders)
            MakeFolder(Path.Combine(dataPath, Root, folder));

        // Assets de terceiros ficam separados dos seus.
        MakeFolder(Path.Combine(dataPath, "ThirdParty"));

        // Assembly definitions: separam o código por camada.
        // Core e AI não enxergam a Unity (noEngineReferences), então
        // podem ser testados sem abrir cena nenhuma.
        string scripts = Path.Combine(dataPath, Root, "Scripts");

        WriteAsmdef(Path.Combine(scripts, "Core", "Game.Core.asmdef"),
            "Game.Core", new string[0], noEngine: true);

        WriteAsmdef(Path.Combine(scripts, "AI", "Game.AI.asmdef"),
            "Game.AI", new[] { "Game.Core" }, noEngine: true);

        WriteAsmdef(Path.Combine(scripts, "Modifiers", "Game.Modifiers.asmdef"),
            "Game.Modifiers", new[] { "Game.Core" }, noEngine: false);

        WriteAsmdef(Path.Combine(scripts, "Run", "Game.Run.asmdef"),
            "Game.Run", new[] { "Game.Core", "Game.Modifiers" }, noEngine: false);

        WriteAsmdef(Path.Combine(scripts, "Presentation", "Game.Presentation.asmdef"),
            "Game.Presentation",
            new[] { "Game.Core", "Game.AI", "Game.Modifiers", "Game.Run",
                    "Unity.TextMeshPro" },
            noEngine: false);

        AssetDatabase.Refresh();
        Debug.Log("Estrutura de pastas e assembly definitions criadas.");
    }

    static void MakeFolder(string path)
    {
        Directory.CreateDirectory(path);

        // Pastas vazias não vão para o Git. O arquivo .keep resolve isso.
        // A Unity ignora arquivos que começam com ponto.
        string keep = Path.Combine(path, ".keep");
        if (!File.Exists(keep))
            File.WriteAllText(keep, "");
    }

    static void WriteAsmdef(string path, string name, string[] references, bool noEngine)
    {
        if (File.Exists(path)) return;

        string refs = references.Length == 0
            ? ""
            : "\"" + string.Join("\", \"", references) + "\"";

        string json =
            "{\n" +
            $"  \"name\": \"{name}\",\n" +
            $"  \"rootNamespace\": \"{name}\",\n" +
            $"  \"references\": [{refs}],\n" +
            "  \"includePlatforms\": [],\n" +
            "  \"excludePlatforms\": [],\n" +
            "  \"allowUnsafeCode\": false,\n" +
            "  \"overrideReferences\": false,\n" +
            "  \"precompiledReferences\": [],\n" +
            "  \"autoReferenced\": true,\n" +
            "  \"defineConstraints\": [],\n" +
            "  \"versionDefines\": [],\n" +
            $"  \"noEngineReferences\": {(noEngine ? "true" : "false")}\n" +
            "}\n";

        File.WriteAllText(path, json);
    }
}