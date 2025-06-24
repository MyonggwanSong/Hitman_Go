using System.Collections.Generic;
using UnityEngine;

public static class AStarSearch
{
    // 1. start를 openSet에 넣음
    // 2. openSet에서 가장 예상 비용(fScore) 낮은 노드 꺼냄
    // 3. 이웃 노드들 전부 검사
    //    >> 이미 갔다면 패스
    //    >> 더 좋은 길이면 openSet에 추가 + 정보 갱신
    // 4. goal 만나면 경로 복원

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

public static Node FindFarthestNode(Node current, Vector3 forward)  // 바라보는 방향 우선. 없으면 반대 방향
{
    if (current == null)
    {
        Debug.LogError("FindFarthestNode: current node is null.");
        return null;
    }

    Node forwardLastNode = FindStraightLastNode(current, forward, 0.7f); // 전방 우선 (45도)
    
    // 전방 없으면 후방 방향 탐색
    if (forwardLastNode != null)
        return forwardLastNode;

    // 후방으로 진행 (180도 뒤로)
    Vector3 backward = -forward;
    Node backwardLastNode = FindStraightLastNode(current, backward, 0.7f);

    return backwardLastNode;
}

// 실제 직선 방향 끝까지 탐색하는 함수
private static Node FindStraightLastNode(Node startNode, Vector3 direction, float dotThreshold) // 직선거리의 가장 멀리있는 노드 찾기
{
    Node currentNode = startNode;
    Node lastNode = startNode;
    Node prevNode = null;

    while (true)
    {
        Node nextNode = null;
        float maxDot = dotThreshold;

        foreach (Node neighbor in currentNode.connectedNodes)
        {
            if (neighbor == prevNode) continue; // 방금 온 노드는 제외

            Vector3 dirToNeighbor = (neighbor.transform.position - currentNode.transform.position).normalized;
            float dot = Vector3.Dot(direction.normalized, dirToNeighbor);

            if (dot > maxDot)
            {
                maxDot = dot;
                nextNode = neighbor;
            }
        }

        if (nextNode != null)
        {
            prevNode = currentNode;
            currentNode = nextNode;
            lastNode = currentNode;
        }
        else
        {
            break; // 더 갈 노드 없음
        }
    }

    return lastNode != startNode ? lastNode : null; // 출발 노드랑 같으면 null 반환
}


}
