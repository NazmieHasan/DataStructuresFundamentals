namespace Tree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Tree<T> : IAbstractTree<T>
    {
        private readonly List<Tree<T>> _children;

        public Tree(T key, params Tree<T>[] children)
        {
            this.Key = key;
            this._children = new List<Tree<T>>();

            foreach (var child in children)
            {
                this.AddChild(child);
                child.Parent = this;
            }
        }

        public T Key { get; private set; }

        public Tree<T> Parent { get; private set; }


        public IReadOnlyCollection<Tree<T>> Children
            => this._children.AsReadOnly();

        public void AddChild(Tree<T> child)
        {
            this._children.Add(child);
        }

        public void AddParent(Tree<T> parent)
        {
            this.Parent = parent;
        }

        public string GetAsString()
        {
            StringBuilder result = new StringBuilder();
            this.OrderDfsForString(0, result, this);

            return result.ToString().Trim();
        }

        public Tree<T> GetDeepestLeftomostNode()
        {
            Func<Tree<T>, bool> leafKeysPredicate = 
                (node) => this.IsLeaf(node);
            var leafNodes = this.OrderBfsNodes(leafKeysPredicate);
            int deepestNodeDepth = 0;
            Tree<T> deepestNode = null; 

            foreach (var node in leafNodes)
            {
                int currentDepth = this.GetDepthFromLeafToParent(node);
                if (currentDepth > deepestNodeDepth)
                {
                    deepestNodeDepth = currentDepth;
                    deepestNode = node;
                }
            }

            return deepestNode;
        }

        public List<T> GetLeafKeys()
        {
            Func<Tree<T>, bool> leafKeysPredicate =
                (node) => this.IsLeaf(node);

            return this.OrderBfs(leafKeysPredicate);
        }

        public List<T> GetMiddleKeys()
        {
            Func<Tree<T>, bool> middleKeysPredicate
                = (node) => this.IsMiddle(node);

            return this.OrderBfs(middleKeysPredicate);
        }

        public List<T> GetLongestPath()
        {
            var depestNode = this.GetDeepestLeftomostNode();
            var resultedPath = new Stack<T>();
            var currentNode = depestNode;

            while (currentNode != null)
            {
                resultedPath.Push(currentNode.Key);
                currentNode = currentNode.Parent;
            }

            return new List<T>(resultedPath);
        }

        public List<List<T>> PathsWithGivenSum(int sum)
        {
            var result = new List<List<T>>();
            var currentPath = new List<T>();
            currentPath.Add(this.Key);
            int currentSum = Convert.ToInt32(this.Key);
            this.GetPathWithSumDfs(this, result, currentPath, ref currentSum, sum);
            return result;
        }

        public List<Tree<T>> SubTreesWithGivenSum(int sum)
        {
            var subtreesWithGivenSum = new List<Tree<T>>();
            var allNodes = this.OrderBfsNodes();

            foreach (var node in allNodes)
            {
                int subtreeSum = this.GetSubtreeSumDfs(node);

                if (subtreeSum == sum)
                {
                    subtreesWithGivenSum.Add(node);
                }
            }

            return subtreesWithGivenSum;
        }

        private void OrderDfsForString(int depth,
            StringBuilder result,
            Tree<T> subtree)
        {
            result
                .AppendLine(new string(' ', depth) + subtree.Key);

            foreach (var child in subtree.Children)
            {
                this.OrderDfsForString(depth + 2, result, child);
            }
        }

        private bool IsLeaf(Tree<T> node)
        {
            return node.Children.Count == 0;
        }

        private bool IsRoot(Tree<T> node)
        {
            return node.Parent == null;
        }

        private bool IsMiddle(Tree<T> node)
        {
            return node.Parent != null && node.Children.Count > 0;
        }

        private List<T> OrderBfs(Func<Tree<T>, bool> predicate)
        {
            var result = new List<T>();
            var nodes = new Queue<Tree<T>>();

            nodes.Enqueue(this);

            while (nodes.Count > 0)
            {
                var currentNode = nodes.Dequeue();

                if (predicate.Invoke(currentNode))
                {
                    result.Add(currentNode.Key);
                }

                foreach (var child in currentNode.Children)
                {
                    nodes.Enqueue(child);
                }
            }

            return result;
        }

        private List<Tree<T>> OrderBfsNodes(Func<Tree<T>, bool> predicate = null)
        {
            var result = new List<Tree<T>>();
            var nodes = new Queue<Tree<T>>();

            nodes.Enqueue(this);

            while (nodes.Count > 0)
            {
                var currentNode = nodes.Dequeue();

                if (predicate != null)
                {
                    if (predicate.Invoke(currentNode))
                    {
                        result.Add(currentNode);
                    }
                }
                else
                {
                    result.Add(currentNode);
                }

                foreach (var child in currentNode.Children)
                {
                    nodes.Enqueue(child);
                }
            }

            return result;
        }

        private int GetDepthFromLeafToParent(Tree<T> node)
        {
            int depth = 0;
            Tree<T> current = node;
            while (current.Parent != null)
            {
                depth++;
                current = current.Parent;
            }

            return depth;
        }

        private void GetPathWithSumDfs(
            Tree<T> current, 
            List<List<T>> wantedPaths, 
            List<T> currentPath,
            ref int currentSum,
            int wantedSum)
        {
            foreach (var child in current.Children)
            {
                currentPath.Add(child.Key);
                currentSum += Convert.ToInt32(child.Key);
                this.GetPathWithSumDfs(child, wantedPaths, currentPath, ref currentSum, wantedSum);
            }

            if (currentSum == wantedSum)
            {
                wantedPaths.Add(new List<T>(currentPath));
            }

            currentSum -= Convert.ToInt32(current.Key);
            currentPath.RemoveAt(currentPath.Count - 1);
        }

        private int GetSubtreeSumDfs(Tree<T> currentNode)
        {
            int currentSum = Convert.ToInt32(currentNode.Key);
            int childSum = 0;

            foreach (var childNode in currentNode.Children)
            {
                childSum += this.GetSubtreeSumDfs(childNode);
            }

            return currentSum + childSum;
        }

    }
}
