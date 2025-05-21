using System;
using System.Collections.Generic;
using System.IO;

namespace Route_Mapper_Pro
{
    // QueueItem for use with SortedSet
    public class QueueItem : IComparable<QueueItem>
    {
        public int Node { get; }
        public double Priority { get; }

        public QueueItem(int node, double priority)
        {
            Node = node;
            Priority = priority;
        }

        public int CompareTo(QueueItem other)
        {
            // First compare by priority
            int priorityComparison = Priority.CompareTo(other.Priority);
            if (priorityComparison != 0)
                return priorityComparison;

            // If priorities are equal, compare by node ID to ensure consistent ordering
            return Node.CompareTo(other.Node);
        }
    }

    internal class PathFinder
    {
        public struct Point
        {
            public double X;
            public double Y;

            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        // Faster distance calculation - using squared distance where possible to avoid sqrt
        private static double SquaredDistance(in Point a, in Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }

        private static double EuclideanDistance(in Point a, in Point b)
        {
            return Math.Sqrt(SquaredDistance(a, b));
        }

        public static MapData Solve(MapData mapData, List<Query> queries)
        {
            const double walkSpeed = 5.0;
            const double eps = 1e-9; // Smaller epsilon for more precision

            // Convert intersections to Points - preallocate the array size
            int n = mapData.Intersections.Count;
            Point[] intersections = new Point[n];
            for (int i = 0; i < n; i++)
            {
                var intersection = mapData.Intersections[i];
                intersections[i] = new Point(intersection.x, intersection.y);
            }

            // Build adjacency lists for the road network - more efficient than dictionaries
            List<(int Node, double Cost, double Length)>[] roads = new List<(int, double, double)>[n];
            for (int i = 0; i < n; i++)
            {
                roads[i] = new List<(int, double, double)>(4); // Preallocation with typical degree
            }

            foreach (var road in mapData.Roads)
            {
                int u = road.fromId;
                int v = road.toId;
                double len = road.length;
                double speed = road.speed;
                double cost = len / speed;

                roads[u].Add((v, cost, len));
                roads[v].Add((u, cost, len));
            }

            // Process each query
            foreach (var query in queries)
            {
                Point start = new Point(query.SourceX, query.SourceY);
                Point end = new Point(query.DestX, query.DestY);
                double r = query.R / 1000.0;
                double rSquared = r * r; // Square the radius for squared distance comparisons

                // Use arrays for better cache locality
                double[] dist = new double[n];
                int[] parent = new int[n];
                bool[] inQueue = new bool[n]; // To track nodes in the queue
                for (int i = 0; i < n; i++)
                {
                    dist[i] = double.PositiveInfinity;
                    parent[i] = -1;
                    inQueue[i] = false;
                }

                // Use SortedSet as priority queue
                var pq = new SortedSet<QueueItem>();

                // Precompute distances to start and end
                double[] distToStart = new double[n];
                double[] distToEndSq = new double[n]; // Store squared distances to avoid sqrt
                bool[] withinRangeFromStart = new bool[n];
                bool[] withinRangeFromEnd = new bool[n];

                for (int i = 0; i < n; i++)
                {
                    double distSqStart = SquaredDistance(intersections[i], start);
                    double distSqEnd = SquaredDistance(intersections[i], end);

                    withinRangeFromStart[i] = distSqStart <= rSquared + eps;
                    withinRangeFromEnd[i] = distSqEnd <= rSquared + eps;

                    if (withinRangeFromStart[i])
                    {
                        distToStart[i] = Math.Sqrt(distSqStart); // Only compute sqrt when needed
                    }

                    distToEndSq[i] = distSqEnd;
                }

                // Initialize queue with reachable nodes from start
                for (int i = 0; i < n; i++)
                {
                    if (withinRangeFromStart[i])
                    {
                        dist[i] = distToStart[i] / walkSpeed;
                        pq.Add(new QueueItem(i, dist[i]));
                        inQueue[i] = true;
                    }
                }

                // Check direct walking path
                double directDistance = EuclideanDistance(start, end);
                double bestTime = directDistance <= r + eps ? directDistance / walkSpeed : double.PositiveInfinity;
                int bestNode = -1;

                // Dijkstra's algorithm
                while (pq.Count > 0)
                {
                    // Get and remove the minimum element
                    var current = pq.Min;
                    pq.Remove(current);

                    int cur = current.Node;
                    double cost = current.Priority;
                    inQueue[cur] = false;

                    // Skip if we've found a better path to this node already
                    if (cost > dist[cur] + eps)
                        continue;

                    // Early termination check - if current node is within walking range of end
                    if (withinRangeFromEnd[cur])
                    {
                        double endDistance = Math.Sqrt(distToEndSq[cur]);
                        double totalCost = cost + (endDistance / walkSpeed);
                        if (totalCost < bestTime)
                        {
                            bestTime = totalCost;
                            bestNode = cur;
                        }
                    }

                    // Skip exploring further if we have better paths
                    if (cost > bestTime)
                        continue;

                    // Explore neighbors
                    foreach (var (neighbor, edgeCost, length) in roads[cur])
                    {
                        double newCost = cost + edgeCost;
                        if (newCost < dist[neighbor] - eps)
                        {
                            dist[neighbor] = newCost;
                            parent[neighbor] = cur;

                            // For SortedSet, we need to add a new instance
                            if (inQueue[neighbor])
                            {
                                // We can't directly update priority, so remove and add again
                                pq.RemoveWhere(item => item.Node == neighbor);
                            }

                            pq.Add(new QueueItem(neighbor, newCost));
                            inQueue[neighbor] = true;
                        }
                    }
                }

                // Reconstruct path
                List<int> path = new List<int>();
                if (bestNode != -1)
                {
                    // First count the path length to properly allocate the list
                    int pathLength = 0;
                    int current = bestNode;
                    while (current != -1)
                    {
                        pathLength++;
                        current = parent[current];
                    }

                    // Allocate the list with exact capacity
                    path = new List<int>(pathLength);

                    // Fill the list in reverse order
                    current = bestNode;
                    while (current != -1)
                    {
                        path.Add(current);
                        current = parent[current];
                    }

                    // Reverse the path once to get correct order
                    path.Reverse();
                }

                // Calculate distances
                double drivingDistance = 0;
                double walkingDistanceStart = 0;
                double walkingDistanceEnd = 0;

                if (path.Count > 0)
                {
                    walkingDistanceStart = distToStart[path[0]];
                    walkingDistanceEnd = Math.Sqrt(distToEndSq[path[path.Count - 1]]);

                    // Calculate driving distance using the path
                    for (int i = 1; i < path.Count; i++)
                    {
                        int u = path[i - 1];
                        int v = path[i];

                        // Find edge length between u and v
                        foreach (var (neighbor, _, length) in roads[u])
                        {
                            if (neighbor == v)
                            {
                                drivingDistance += length;
                                break;
                            }
                        }
                    }
                }
                else if (directDistance <= r + eps)
                {
                    // Direct walking path case
                    walkingDistanceStart = directDistance;
                }

                double totalWalk = walkingDistanceStart + walkingDistanceEnd;
                double totalDriving = drivingDistance;

                // Add query result
                mapData.QueryResults.Add(new QueryResult
                {
                    PathIds = path,
                    ShortestTime = bestTime * 60, // Convert to minutes
                    TotalDistance = totalWalk + totalDriving,
                    WalkingDistance = totalWalk,
                    VehicleDistance = totalDriving
                });
            }

            return mapData;
        }

        public static MapData SolveWithTiming(MapData mapData, string FileName)
        {
            // Read Query File
            var stopwatchRead = System.Diagnostics.Stopwatch.StartNew();
            List<Query> queries = mapData.ReadQueryFile(FileName);
            mapData.QueryResults.Clear();
            stopwatchRead.Stop();

            // Get Solution and time Without IO
            var stopwatchSolve = System.Diagnostics.Stopwatch.StartNew();
            mapData = Solve(mapData, queries);
            stopwatchSolve.Stop();

            // Save time
            mapData.QueryExecutionTime = stopwatchSolve.ElapsedMilliseconds;
            mapData.QueryExecutionTime_with_IO = mapData.QueryExecutionTime_Load_map +
                                                stopwatchRead.ElapsedMilliseconds +
                                                stopwatchSolve.ElapsedMilliseconds;

            // Print Output File
            string name = Path.GetFileNameWithoutExtension(FileName);
            string outputFilePath = Path.Combine(
                    Path.GetDirectoryName(FileName),
                    "output" + name[name.Length - 1] + ".txt"
                );
            MapData.WriteOutputFile(outputFilePath, mapData);

            return mapData;
        }
    }
}