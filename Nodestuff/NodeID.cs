namespace CSC.Nodestuff
{
    public readonly ref struct NodeID(string file, NodeType Type, string ID, string text)
    {
        public NodeType Type { get; } = Type;
        public ReadOnlySpan<char> ID { get; } = ID.AsSpan();
        public ReadOnlySpan<char> Text { get; } = text.AsSpan()[..(Math.Min(50, text.Length))];
        public ReadOnlySpan<char> File { get; } = file.AsSpan();

        public static implicit operator long(NodeID n)
        {
            unchecked // Overflow is fine, just wrap
            {
                long hash = 17;
                hash = hash * 23 + (int)n.Type;
                hash = hash * 23 + CalculateHash(n.ID);
                hash = hash * 17 + CalculateHash(n.File);
                hash = hash * 23 + CalculateHash(n.Text);
                return hash;
            }
        }

        static long CalculateHash(ReadOnlySpan<char> read)
        {
            ulong hashedValue = 3074457345618258791ul;
            for (int i = 0; i < read.Length; i++)
            {
                hashedValue += read[i];
                hashedValue *= 3074457345618258799ul;
            }
            return (long)hashedValue;
        }
    }
}
