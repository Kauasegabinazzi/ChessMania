namespace Game.AI
{
    public readonly struct AIPreset
    {
        public readonly string Name;
        public readonly int Depth;
        public readonly int Margin;
        public readonly double MistakeChance;

        public AIPreset(string name, int depth, int margin, double mistakeChance)
        {
            Name = name;
            Depth = depth;
            Margin = margin;
            MistakeChance = mistakeChance;
        }

        public IChessAI Create()
        {
            return new MinimaxAI(Depth) { ScoreMargin = Margin, MistakeChance = MistakeChance };
        }
    }

    public static class AIPresets
    {
        public static readonly AIPreset[] All =
        {
            new AIPreset("Easy", 2, 100, 0.25),
            new AIPreset("Medium", 2, 40, 0.1),
            new AIPreset("Hard", 3, 0, 0),
            new AIPreset("Boss", 4, 0, 0),
        };
    }
}
