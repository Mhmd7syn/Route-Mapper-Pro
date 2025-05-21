using System;
using System.Collections.Generic;
using System.IO;


namespace Route_Mapper_Pro
{
    public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
    {
        private readonly List<(TElement Element, TPriority Priority)> _elements = new List<(TElement, TPriority)>();

        public int Count => _elements.Count;

        public void Enqueue(TElement element, TPriority priority)
        {
            _elements.Add((element, priority));
            _elements.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public bool TryDequeue(out TElement element, out TPriority priority)
        {
            if (_elements.Count == 0)
            {
                element = default;
                priority = default;
                return false;
            }

            var item = _elements[0];
            _elements.RemoveAt(0);
            element = item.Element;
            priority = item.Priority;
            return true;
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

        private static double EuclideanDistance(in Point a, in Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static MapData Solve(MapData mapData, List<Query> queries)
        {
            const double walkSpeed = 5.0;
            const double eps = 1e-6;

            // Convert intersections to Points
            int n = mapData.Intersections.Count;
            Point[] intersections = new Point[n];
            for (int i = 0; i < n; i++)
            {
                var intersection = mapData.Intersections[i];
                intersections[i] = new Point(intersection.x, intersection.y);
            }

            // Build road network
            Dictionary<int, double>[] roads = new Dictionary<int, double>[n];
            Dictionary<int, double>[] lengths = new Dictionary<int, double>[n];
            for (int i = 0; i < n; i++)
            {
                roads[i] = new Dictionary<int, double>();
                lengths[i] = new Dictionary<int, double>();
            }

            foreach (var road in mapData.Roads)
            {
                int u = road.fromId;
                int v = road.toId;
                double len = road.length;
                double speed = road.speed;
                double cost = len / speed;

                roads[u][v] = cost;
                roads[v][u] = cost;
                lengths[u][v] = len;
                lengths[v][u] = len;
            }

            // Process queries
            foreach (var query in queries)
            {
                Point start = new Point(query.SourceX, query.SourceY);
                Point end = new Point(query.DestX, query.DestY);
                double r = query.R / 1000.0;

                double[] dist = new double[n];
                int[] parent = new int[n];
                for (int i = 0; i < n; i++)
                {
                    dist[i] = double.PositiveInfinity;
                    parent[i] = -1;
                }

                var pq = new PriorityQueue<int, double>();

                // Precompute distances to start and end
                double[] distToStart = new double[n];
                double[] distToEnd = new double[n];
                for (int i = 0; i < n; i++)
                {
                    distToStart[i] = EuclideanDistance(intersections[i], start);
                    distToEnd[i] = EuclideanDistance(intersections[i], end);
                }

                // Initialize queue with reachable nodes from start
                for (int i = 0; i < n; i++)
                {
                    if (distToStart[i] <= r + eps)
                    {
                        dist[i] = distToStart[i] / walkSpeed;
                        pq.Enqueue(i, dist[i]);
                    }
                }

                // Check direct walking path
                double directDistance = EuclideanDistance(start, end);
                double answer = double.PositiveInfinity;
                if (directDistance <= r + eps)
                {
                    answer = directDistance / walkSpeed;
                }

                // Dijkstra's algorithm
                while (pq.Count > 0)
                {
                    if (!pq.TryDequeue(out int cur, out double cost))
                        break;

                    if (cost > dist[cur] + eps)
                        continue;

                    foreach (var neighbor in roads[cur])
                    {
                        int neighborNode = neighbor.Key;
                        double neighborCost = neighbor.Value;
                        double newCost = cost + neighborCost;
                        if (newCost < dist[neighborNode] - eps)
                        {
                            dist[neighborNode] = newCost;
                            parent[neighborNode] = cur;
                            pq.Enqueue(neighborNode, newCost);
                        }
                    }
                }

                // Find best end node
                int bestNode = -1;
                for (int i = 0; i < n; i++)
                {
                    if (distToEnd[i] <= r + eps)
                    {
                        double totalCost = dist[i] + (distToEnd[i] / walkSpeed);
                        if (totalCost < answer - eps)
                        {
                            answer = totalCost;
                            bestNode = i;
                        }
                    }
                }

                // Reconstruct path
                List<int> path = new List<int>();
                if (bestNode != -1)
                {
                    int current = bestNode;
                    while (current != -1)
                    {
                        path.Add(current);
                        current = parent[current];
                    }
                    path.Reverse();
                }

                // Calculate distances
                double drivingDistance = 0;
                double walkingDistanceStart = bestNode != -1 ? distToStart[path[0]] : 0;
                double walkingDistanceEnd = bestNode != -1 ? distToEnd[path[path.Count - 1]] : 0;

                if (path.Count > 0)
                {
                    for (int i = 1; i < path.Count; i++)
                    {
                        drivingDistance += lengths[path[i - 1]][path[i]];
                    }
                }
                else if (directDistance <= r + eps)
                {
                    walkingDistanceStart = directDistance;
                }

                double totalWalk = walkingDistanceStart + walkingDistanceEnd;
                double totalDriving = drivingDistance;

                // Add query result
                mapData.QueryResults.Add(new QueryResult
                {
                    PathIds = path,
                    ShortestTime = answer * 60,
                    TotalDistance = totalWalk + totalDriving,
                    WalkingDistance = totalWalk,
                    VehicleDistance = totalDriving
                });
            }

            return mapData;
        }

        public static MapData SolveWithTiming(MapData mapData, string FileName)
        {
            //Read Query File
            var stopwatchRead = System.Diagnostics.Stopwatch.StartNew();
            List<Query> queries = mapData.ReadQueryFile(FileName);
            mapData.QueryResults.Clear();
            stopwatchRead.Stop();


            //Get Solution and time Without IO
            var stopwatchSolve = System.Diagnostics.Stopwatch.StartNew();
            mapData = Solve(mapData, queries);
            stopwatchSolve.Stop();

            // save time
            mapData.QueryExecutionTime = stopwatchSolve.ElapsedMilliseconds;
            mapData.QueryExecutionTime_with_IO = mapData.QueryExecutionTime_Load_map +
                                                stopwatchRead.ElapsedMilliseconds +
                                                stopwatchSolve.ElapsedMilliseconds
                                                ;

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