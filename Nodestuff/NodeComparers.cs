namespace CSC.Nodestuff
{
    public class NodeComparers(NodeStore store) : IComparer<Node>
    {
        private readonly NodeStore Store = store;

        public int Compare(Node? x, Node? y)
        {
            if (x is null)
            {
                return -1;
            }
            else if (y is null)
            {
                return 1;
            }
            return Store.Childs(y)?.Count ?? 0 - Store.Childs(x)?.Count ?? 0;
        }
    }
    public class NodeChildComparer(NodeStore store) : IComparer<Node>
    {
        private readonly NodeStore Store = store;

        public int Compare(Node? x, Node? y)
        {
            if (x is null)
            {
                return -1;
            }
            else if (y is null)
            {
                return 1;
            }
            return Store.Parents(x)?.Count ?? 0 - Store.Parents(y)?.Count ?? 0;
        }
    }
}
