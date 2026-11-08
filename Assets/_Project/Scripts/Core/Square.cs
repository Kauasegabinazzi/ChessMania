using System;

namespace Game.Core
{
    public readonly struct Square : IEquatable<Square>
    {
        public readonly int X;
        public readonly int Y;
        public Square(int x, int y)
        {
            X = x;
            Y = y;
        }
        public bool IsValid => X >= 0 && X < 8 && Y >= 0 && Y < 8;
        public bool Equals(Square other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is Square other && Equals(other);
        public override int GetHashCode() => X * 8 + Y;
        public static bool operator ==(Square a, Square b) => a.Equals(b);
        public static bool operator !=(Square a, Square b) => !a.Equals(b);
        public override string ToString() => $"{(char)('a' + X)}{Y + 1}";

    }
}
