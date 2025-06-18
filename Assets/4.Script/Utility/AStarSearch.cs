using System;
using System.Collections.Generic;

public class AStarSearch<TNode, TContext> where TNode : AStarSearch<TNode, TContext>.ISearchNode
{
	public enum Status
	{
		Pending = 0,
		Succeeded = 1,
		NoPath = 2
	}

	public interface ISearchNode
	{
		IEnumerable<Connection> GetConnections(TContext context);

		float EstimateCostToDestination(TNode destination, TContext context);

		bool IsSameNodeThan(TNode otherNode, TContext context);
	}

	public struct Connection
	{
		public TNode ToNode;

		public float Cost;

		public Connection(TNode node, float cost)
		{
			ToNode = node;
			Cost = cost;
		}
	}

	public class Result
	{
		public Status Status;

		public TNode[] Path;

		public float Cost;

		public Result(Status status)
		{
			Status = status;
		}
	}

	private class ExpandedNode : IComparable<ExpandedNode>
	{
		public TNode Node;

		public float CostFromStart;

		public float CostToDestination;

		private ExpandedNode m_Parent;

		public int NbExpansions;

		public float TotalCost
		{
			get
			{
				return CostFromStart + CostToDestination;
			}
		}

		public ExpandedNode Parent
		{
			get
			{
				return m_Parent;
			}
			set
			{
				NbExpansions = ((value != null) ? (value.NbExpansions + 1) : 0);
				m_Parent = value;
			}
		}

		public ExpandedNode(TNode n)
		{
			Parent = null;
			Node = n;
			CostFromStart = 0f;
		}

		public ExpandedNode(TNode n, float costFromStart, float costToDestination, ExpandedNode parent)
		{
			Node = n;
			CostFromStart = costFromStart;
			Parent = parent;
			CostToDestination = costToDestination;
		}

		public int CompareTo(ExpandedNode other)
		{
			if (other == null)
			{
				return 1;
			}
			return (int)(1000f * (other.TotalCost - TotalCost));
		}
	}

	private TNode Destination;

	private TContext Context;

	private List<ExpandedNode> m_OpenList = new List<ExpandedNode>();

	private List<TNode> m_ClosedList = new List<TNode>();

	public AStarSearch(TNode start, TNode destination, TContext context)
	{
		Destination = destination;
		Context = context;
		m_OpenList.Add(new ExpandedNode(start, 0f, start.EstimateCostToDestination(Destination, Context), null));
	}

	public Result Step()
	{
		if (m_OpenList.Count == 0)
		{
			return new Result(Status.NoPath);
		}
		int index = m_OpenList.Count - 1;
		ExpandedNode expandedNode = m_OpenList[index];
		m_OpenList.RemoveAt(index);
		m_ClosedList.Add(expandedNode.Node);
		if (expandedNode.Node.IsSameNodeThan(Destination, Context))
		{
			return BuildPath(expandedNode);
		}
		Connection connection;
		foreach (Connection connection2 in expandedNode.Node.GetConnections(Context))
		{
			connection = connection2;
			if (m_ClosedList.Exists((TNode x) => x.IsSameNodeThan(connection.ToNode, Context)))
			{
				continue;
			}
			float num = expandedNode.CostFromStart + connection.Cost;
			int num2 = m_OpenList.FindIndex((ExpandedNode x) => x.Node.IsSameNodeThan(connection.ToNode, Context));
			if (num2 < 0)
			{
				float costToDestination = connection.ToNode.EstimateCostToDestination(Destination, Context);
				ExpandedNode newNode = new ExpandedNode(connection.ToNode, num, costToDestination, expandedNode);
				InsertOpenNode(newNode);
				continue;
			}
			ExpandedNode expandedNode2 = m_OpenList[num2];
			if (num < expandedNode2.CostFromStart)
			{
				expandedNode2.CostFromStart = num;
				expandedNode2.Parent = expandedNode;
				if (num2 < m_OpenList.Count - 1 && num < m_OpenList[num2 + 1].CostFromStart)
				{
					m_OpenList.RemoveAt(num2);
					InsertOpenNode(expandedNode2);
				}
			}
		}
		return null;
	}

	public Result Resolve()
	{
		Result result = null;
		do
		{
			result = Step();
		}
		while (result == null);
		return result;
	}

	private void InsertOpenNode(ExpandedNode newNode)
	{
		int num = m_OpenList.BinarySearch(newNode);
		if (num < 0)
		{
			num = ~num;
		}
		m_OpenList.Insert(num, newNode);
	}

	private Result BuildPath(ExpandedNode node)
	{
		Result result = new Result(Status.Succeeded);
		result.Cost = node.CostFromStart;
		int num = node.NbExpansions + 1;
		result.Path = new TNode[num];
		while (node != null)
		{
			result.Path[--num] = node.Node;
			node = node.Parent;
		}
		return result;
	}
}
