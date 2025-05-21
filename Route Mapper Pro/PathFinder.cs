using System;
using System.Collections.Generic;
using System.IO;

namespace Route_Mapper_Pro
{
    public class PriorityQueue<T>
    {
        private List<(T item, double priority)> heap = new List<(T, double)>();
        public int Count => heap.Count;

        public void Enqueue(T item, double priority)
        {
            heap.Add((item, priority));
            HeapifyUp(heap.Count - 1);
        }

        public T Dequeue()
        {
            if (heap.Count == 0)
                throw new InvalidOperationException("The queue is empty.");
            T result = heap[0].item;
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            if (heap.Count > 0)
                HeapifyDown(0);
            return result;
        }

        public T Peek()
        {
            if (heap.Count == 0)
                throw new InvalidOperationException("The queue is empty.");
            return heap[0].item;
        }

        public bool Contains(T item)
        {
            return heap.Exists(pair => EqualityComparer<T>.Default.Equals(pair.item, item));
        }

        public void Remove(T item)
        {
            int index = heap.FindIndex(pair => EqualityComparer<T>.Default.Equals(pair.item, item));
            if (index < 0)
                return;

            heap[index] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            if (index < heap.Count)
            {
                HeapifyDown(index);
                HeapifyUp(index);
            }
        }

        public void UpdatePriority(T item, double newPriority)
        {
            int index = heap.FindIndex(pair => EqualityComparer<T>.Default.Equals(pair.item, item));
            if (index < 0)
                return;

            double oldPriority = heap[index].priority;
            heap[index] = (item, newPriority);

            if (newPriority < oldPriority)
                HeapifyUp(index);
            else
                HeapifyDown(index);
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (heap[index].priority >= heap[parent].priority)
                    break;
                (heap[index], heap[parent]) = (heap[parent], heap[index]);
                index = parent;
            }
        }

        private void HeapifyDown(int index)
        {
            int lastIndex = heap.Count - 1;
            while (index < heap.Count)
            {
                int left = index * 2 + 1;
                int right = index * 2 + 2;
                int smallest = index;
                if (left <= lastIndex && heap[left].priority < heap[smallest].priority)
                    smallest = left;
                if (right <= lastIndex && heap[right].priority < heap[smallest].priority)
                    smallest = right;
                if (smallest == index)
                    break;
                (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
                index = smallest;
            }
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
            const double eps = 1e-12;
            // Taking Input
            int n = mapData.Intersections.Count;
            Point[] intersections = new Point[n];
            for (int i = 0; i < n; i++) // O(N)
            {
                var intersection = mapData.Intersections[i];
                intersections[i] = new Point(intersection.x, intersection.y);
            }

            List<(int Node, double Cost)>[] roads = new List<(int, double)>[n];
            for (int i = 0; i < n; i++) // O(N)
            {
                roads[i] = new List<(int, double)>();
            }

            // Dictionary to Save the Length of Each Road with Key pair of (Starting Intersection, Ending Intersection) and value is the length of the road
            Dictionary<(int, int), double> roadLengths = new Dictionary<(int, int), double>();

            foreach (var road in mapData.Roads) // O(N)
            {
                int u = road.fromId;
                int v = road.toId;
                double len = road.length;
                double speed = road.speed;
                double cost = len / speed;

                roadLengths[(u, v)] = len;
                roadLengths[(v, u)] = len;

                roads[u].Add((v, cost));
                roads[v].Add((u, cost));
            }


            foreach (var query in queries) // O(Q * |E| log |V|)
            {

                // Starting of The Solution for each Query
                Point start = new Point(query.SourceX, query.SourceY);
                Point end = new Point(query.DestX, query.DestY);
                double r = query.R / 1000.0; // Convert R from Meters to Kilo Meters
                double rSquared = r * r; // Make R Squared to use it in compare to avoid The Log in reduantant Square root
                double[] dist = new double[n]; // Distance array to store the minmum distance to reach each intersection
                int[] parent = new int[n]; // array parent to backtrack with it to compute the path
                for (int i = 0; i < n; i++)
                {
                    dist[i] = double.PositiveInfinity;
                    parent[i] = -1;
                }

                var pq = new PriorityQueue<int>();


                double[] distToStart = new double[n]; // Dist from Starting Point to Each intersection Pre-Computed
                double[] distToEndSq = new double[n]; // Dist from Each intersecion to Ending Point but I Saved it Squared to Compare it with R ^ 2 instead or R to Avoid The Additional Log
                bool[] withinRangeFromStart = new bool[n]; // Cache The Comparsion to use it again instead of re-calc it with high constant factor
                bool[] withinRangeFromEnd = new bool[n]; // Cache The Comparsion to use it again instead of re-calc it with high constant factor

                for (int i = 0; i < n; i++)
                {
                    double distSqStart = SquaredDistance(intersections[i], start);
                    double distSqEnd = SquaredDistance(intersections[i], end);
                    // Caching Code
                    withinRangeFromStart[i] = distSqStart <= rSquared + eps;
                    withinRangeFromEnd[i] = distSqEnd <= rSquared + eps;

                    if (withinRangeFromStart[i])
                    {
                        distToStart[i] = Math.Sqrt(distSqStart);
                    }

                    distToEndSq[i] = distSqEnd;
                }

                for (int i = 0; i < n; i++)
                {
                    if (withinRangeFromStart[i])
                    {
                        dist[i] = distToStart[i] / walkSpeed;
                        pq.Enqueue(i, dist[i]);
                    }
                }

                double directDistance = EuclideanDistance(start, end);
                double bestTime = directDistance <= r + eps ? directDistance / walkSpeed : double.PositiveInfinity;
                int bestNode = -1;

                // Dijkstra's algorithm
                while (pq.Count > 0)
                {
                    int cur = pq.Dequeue();
                    double cost = dist[cur];

                    if (cost > dist[cur] + eps)
                        continue;

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

                    if (cost > bestTime)
                        continue;

                    foreach (var (neighbor, edgeCost) in roads[cur])
                    {
                        double newCost = cost + edgeCost;
                        if (newCost < dist[neighbor] - eps)
                        {
                            dist[neighbor] = newCost;
                            parent[neighbor] = cur;

                            pq.Enqueue(neighbor, newCost);
                        }
                    }
                }

                List<int> path = new List<int>();
                if (bestNode != -1)
                {
                    int pathLength = 0;
                    int current = bestNode;
                    while (current != -1)
                    {
                        pathLength++;
                        current = parent[current];
                    }
                    path = new List<int>(pathLength);
                    current = bestNode;
                    while (current != -1)
                    {
                        path.Add(current);
                        current = parent[current];
                    }
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

                    for (int i = 1; i < path.Count; i++)
                    {
                        int u = path[i - 1];
                        int v = path[i];
                        drivingDistance += roadLengths[(u, v)];
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