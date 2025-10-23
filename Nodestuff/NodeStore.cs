using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CSC.Nodestuff
{
    public sealed class NodeStore(string fileName)
    {
        public readonly NodePositionSorting Positions = new();
        private readonly Dictionary<Node, List<Node>> childs = [];
        private readonly Dictionary<Node, List<Node>> parents = [];
        private readonly string fileName = fileName;

        public void Add(Node node)
        {
            if (childs.TryAdd(node, []))
            {
                _nodes.Add(node);
            }
            parents.TryAdd(node, []);
        }

        public int Count => childs.Count;

        private readonly List<Node> _nodes = [];
        public ReadOnlyCollection<Node> Nodes => _nodes.AsReadOnly();

        public void Remove(Node node)
        {
            _nodes.Remove(node);
            childs.Remove(node);
            parents.Remove(node);
            Positions.ClearNode(node);

            foreach (var childs in childs.Values)
            {
                childs.Remove(node);
            }

            foreach (var parents in parents.Values)
            {
                parents.Remove(node);
            }

            Main.ClearNodePos(node, fileName);
        }

        public void Clear()
        {
            _nodes.Clear();
            childs.Clear();
            parents.Clear();
            Positions.Clear();
        }

        public bool Contains(Node node)
        {
            bool contains = ContainsKey(node);
            if (contains)
            {
                return true;
            }
            else
            {
                contains = ContainsValue(node);
            }
            return contains;
        }

        public bool ContainsKey(Node node)
        {
            bool contains = _nodes.Contains(node);
            return contains;
        }

        public bool ContainsValue(Node node)
        {
            foreach (var child in childs.Values)
            {
                if (child.Contains(node))
                {
                    return true;
                }
            }
            foreach (var parent in parents.Values)
            {
                if (parent.Contains(node))
                {
                    return true;
                }
            }
            return false;
        }

        public void AddParent(Node node, Node parent)
        {
            Add(node);
            Add(parent);
            if (!parents[node].Contains(parent))
            {
                parents[node].Add(parent);
            }

            if (!childs[parent].Contains(node))
            {
                childs[parent].Add(node);
            }
        }

        public void RemoveParent(Node node, Node parent)
        {
            if (parents.TryGetValue(node, out var list))
            {
                list.Remove(parent);
            }

            if (childs.TryGetValue(parent, out list))
            {
                list.Remove(node);
            }
        }

        public void ClearParents(Node node)
        {
            if (parents.TryGetValue(node, out var list))
            {
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        RemoveChild(list[i], node);
                    }
                    list.Clear();
                }
            }
        }

        public void AddChild(Node node, Node child)
        {
            Add(node);
            Add(child);
            if (!childs[node].Contains(child))
            {
                childs[node].Add(child);
            }

            if (!parents[child].Contains(node))
            {
                parents[child].Add(node);
            }
        }

        public void RemoveChild(Node node, Node child)
        {
            if (childs.TryGetValue(node, out var list))
            {
                list.Remove(child);
            }

            if (parents.TryGetValue(child, out list))
            {
                list.Remove(node);
            }
        }

        public void ClearChilds(Node node)
        {
            if (childs.TryGetValue(node, out var list))
            {
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        RemoveParent(list[i], node);
                    }
                    list.Clear();
                }
            }
        }

        public void AddParents(Node node, IEnumerable<Node> parents_)
        {
            foreach (var item in parents_)
            {
                AddParent(node, item);
            }
        }

        public void AddChilds(Node node, IEnumerable<Node> childs_)
        {
            foreach (var item in childs_)
            {
                AddChild(node, item);
            }
        }

        public List<Node> Childs(Node node)
        {
            if (childs.TryGetValue(node, out var list))
            {
                return list;
            }
            else
            {
                return [];
            }
        }

        public List<Node> Parents(Node node)
        {
            if (parents.TryGetValue(node, out var list))
            {
                return list;
            }
            else
            {
                return [];
            }
        }

        public Family this[Node node]
        {
            get
            {
                return new Family(Childs(node), Parents(node));
            }
        }

        public void Replace(Node node, Node replacement)
        {
            List<Node> childs = [.. Childs(node)];
            List<Node> parents = [.. Parents(node)];

            ClearChilds(node);
            ClearParents(node);
            Remove(node);

            Add(replacement);
            AddChilds(replacement, childs);
            AddParents(replacement, parents);

            var pos = node.Position;
            Main.ClearNodePos(node, fileName);
            Main.ClearNodePos(node, replacement.FileName);
            Main.SetNodePos(replacement, fileName);
            replacement.Position = pos;
        }

        internal bool AreConnected(Node node1, Node node2)
        {
            if (Childs(node1).Contains(node2))
            {
                return true;
            }
            if (Parents(node1).Contains(node2))
            {
                return true;
            }
            if (Childs(node2).Contains(node1))
            {
                return true;
            }
            if (Parents(node2).Contains(node1))
            {
                return true;
            }
            return false;
        }
    }

    public record struct Family(List<Node> Childs, List<Node> Parents)
    {
        public static implicit operator (List<Node> childs, List<Node> parents)(Family value) => (value.Childs, value.Parents);
        public static implicit operator Family((List<Node> childs, List<Node> parents) value) => new(value.childs, value.parents);
    }
}
