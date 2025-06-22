using System.Collections.Generic;
using UnityEngine;

public static class AStarSearch
{
// 1. start를 openSet에 넣음.
// 2. openSet에서 가장 예상 비용(fScore) 낮은 노드 꺼냄.
// 3. 이웃 노드들 전부 검사 :
//    >> 이미 갔다면 패스.
//    >> 더 좋은 길이면 openSet에 추가 + 정보 갱신.
// 4. goal 만나면 경로 복원.

    public static List<Node> FindPath(Node start, Node goal)
    {
        List<Node> openSet = new List<Node>();      // 탐색 후보
        HashSet<Node> closedSet = new HashSet<Node>(); // 이미 탐색한 노드(다시 탐색 불가용)

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>(); // 경로 추적용
        Dictionary<Node, float> gScore = new Dictionary<Node, float>(); // 시작 >> 해당 노드까지 거리
        Dictionary<Node, float> fScore = new Dictionary<Node, float>(); // 예상 총 거리 (g + h)

        openSet.Add(start);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);

        while (openSet.Count > 0)
        {
            Node current = GetLowestFScoreNode(openSet, fScore);

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Node neighbor in current.connectedNodes)
            {
                if (closedSet.Contains(neighbor)) continue;

                float tentativeGScore = gScore[current] + Distance(current, neighbor);

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentativeGScore >= gScore.GetValueOrDefault(neighbor, float.PositiveInfinity))
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);
            }
        }

        // 경로 없음
        return null;
    }

    private static float Heuristic(Node a, Node b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position); // 노드 a,b의 실제 거리
    }

    private static float Distance(Node a, Node b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    private static Node GetLowestFScoreNode(List<Node> nodes, Dictionary<Node, float> fScore)
    {
        Node bestNode = nodes[0];
        float bestScore = fScore.GetValueOrDefault(bestNode, float.PositiveInfinity);

        foreach (var node in nodes)
        {
            float score = fScore.GetValueOrDefault(node, float.PositiveInfinity);
            if (score < bestScore)
            {
                bestScore = score;
                bestNode = node;
            }
        }

        return bestNode;
    }

    private static List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Node> totalPath = new List<Node> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }
}
