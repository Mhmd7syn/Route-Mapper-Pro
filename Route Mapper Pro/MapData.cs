using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Route_Mapper_Pro
{
    public class QueryResult
    {
        public List<int> PathIds { get; set; } = new List<int>();
        public double ShortestTime { get; set; } = 0.0;       // in minutes
        public double TotalDistance { get; set; } = 0.0;      // in km
        public double WalkingDistance { get; set; } = 0.0;    // in km
        public double VehicleDistance { get; set; } = 0.0;    // in km
    }
    public class Query
    {
        public double SourceX { get; set; }
        public double SourceY { get; set; }
        public double DestX { get; set; }
        public double DestY { get; set; }
        public double R { get; set; } // Max walking distance in meters
    }
    public class MapData
    {
        public List<(int id, float x, float y)> Intersections { get; set; }
        public List<(int fromId, int toId, float length, float speed)> Roads { get; set; }
        public List<QueryResult> QueryResults { get; set; } = new List<QueryResult>();
        public long QueryExecutionTime_Load_map { get; set; } = 0;   // in ms
        public long QueryExecutionTime { get; set; } = 0;   // in ms
        public long QueryExecutionTime_with_IO { get; set; } = 0;   // in ms
        public MapData()
        {
            Intersections = new List<(int id, float x, float y)>();
            Roads = new List<(int fromId, int toId, float length, float speed)>();
            QueryResults = new List<QueryResult>();
            QueryResults.Add(new QueryResult()); // Initialize with an empty QueryResult
        }
        public void LoadMapFile(string filePath)
        {
            var stopwatchRead = System.Diagnostics.Stopwatch.StartNew();
            string[] lines = File.ReadAllLines(filePath);
            int lineIndex = 0;

            Intersections.Clear();
            Roads.Clear();

            // Read intersections
            int intersectionCount = int.Parse(lines[lineIndex++]);
            for (int i = 0; i < intersectionCount; i++)
            {
                string[] parts = lines[lineIndex++].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Intersections.Add((
                    int.Parse(parts[0]),       // ID (integer)
                    float.Parse(parts[1]),     // X coordinate (float)
                    float.Parse(parts[2])      // Y coordinate (float)
                ));
            }

            // Read roads
            int roadCount = int.Parse(lines[lineIndex++]);
            for (int i = 0; i < roadCount; i++)
            {
                string[] parts = lines[lineIndex++].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Roads.Add((
                    int.Parse(parts[0]),      // From ID
                    int.Parse(parts[1]),      // To ID
                    float.Parse(parts[2]),     // Length (float)
                    float.Parse(parts[3])      // Speed (float)
                ));
            }
            stopwatchRead.Stop();
            QueryExecutionTime_Load_map = stopwatchRead.ElapsedMilliseconds;
        }
        public List<Query> ReadQueryFile(string filename)
        {
            var queries = new List<Query>();
            var lines = File.ReadAllLines(filename);

            if (lines.Length == 0)
                return queries;

            int Q = int.Parse(lines[0].Trim());

            for (int i = 1; i <= Q && i < lines.Length; i++)
            {
                var parts = lines[i].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 5)
                    continue;

                var query = new Query
                {
                    SourceX = double.Parse(parts[0], CultureInfo.InvariantCulture),
                    SourceY = double.Parse(parts[1], CultureInfo.InvariantCulture),
                    DestX = double.Parse(parts[2], CultureInfo.InvariantCulture),
                    DestY = double.Parse(parts[3], CultureInfo.InvariantCulture),
                    R = double.Parse(parts[4], CultureInfo.InvariantCulture)
                };
                queries.Add(query);
            }

            return queries;
        }

        public static void WriteOutputFile(string filename, MapData map)
        {
            var stopwatchWrite = System.Diagnostics.Stopwatch.StartNew();
            using (var writer = new StreamWriter(filename))
            {
                foreach (var result in map.QueryResults)
                {
                    // Write intersection IDs
                    writer.WriteLine(string.Join(" ", result.PathIds));

                    // Write shortest time
                    writer.WriteLine($"{result.ShortestTime.ToString("0.00", CultureInfo.InvariantCulture)} mins");

                    // Write total distance
                    writer.WriteLine($"{result.TotalDistance.ToString("0.00", CultureInfo.InvariantCulture)} km");

                    // Write walking and vehicle distances
                    writer.WriteLine($"{result.WalkingDistance.ToString("0.00", CultureInfo.InvariantCulture)} km");
                    writer.WriteLine($"{result.VehicleDistance.ToString("0.00", CultureInfo.InvariantCulture)} km");

                    // Empty line after each query
                    writer.WriteLine();
                }

                // Write execution times
                writer.WriteLine($"{map.QueryExecutionTime} ms");
                writer.WriteLine();
                stopwatchWrite.Stop();
                map.QueryExecutionTime_with_IO += stopwatchWrite.ElapsedMilliseconds;
                writer.WriteLine($"{map.QueryExecutionTime_with_IO} ms");
            }
        }
        public void LoadOutputFile(string filePath)
        {
            QueryResults.Clear();
            string[] lines = File.ReadAllLines(filePath);
            int i = 0;

            try
            {
                while (i < lines.Length)
                {
                    // Skip empty lines
                    while (i < lines.Length && string.IsNullOrWhiteSpace(lines[i]))
                    {
                        i++;
                    }

                    if (i >= lines.Length) break;

                    // Check if we've reached the execution times section
                    if (lines[i].Contains("ms"))
                    {
                        break;
                    }

                    QueryResult result = new QueryResult();

                    // 1. Read path IDs
                    if (i < lines.Length)
                    {
                        result.PathIds = new List<int>(Array.ConvertAll(
                            lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                            int.Parse));
                        i++;
                    }

                    // 2. Read shortest time (remove " mins" and parse)
                    if (i < lines.Length && lines[i].Contains("mins"))
                    {
                        result.ShortestTime = double.Parse(
                            lines[i].Replace(" mins", "").Trim(),
                            System.Globalization.CultureInfo.InvariantCulture);
                        i++;
                    }

                    // 3. Read total distance (remove " km" and parse)
                    if (i < lines.Length && lines[i].Contains("km"))
                    {
                        result.TotalDistance = double.Parse(
                            lines[i].Replace(" km", "").Trim(),
                            System.Globalization.CultureInfo.InvariantCulture);
                        i++;
                    }

                    // 4. Read walking distance (remove " km" and parse)
                    if (i < lines.Length && lines[i].Contains("km"))
                    {
                        result.WalkingDistance = double.Parse(
                            lines[i].Replace(" km", "").Trim(),
                            System.Globalization.CultureInfo.InvariantCulture);
                        i++;
                    }

                    // 5. Read vehicle distance (remove " km" and parse)
                    if (i < lines.Length && lines[i].Contains("km"))
                    {
                        result.VehicleDistance = double.Parse(
                            lines[i].Replace(" km", "").Trim(),
                            System.Globalization.CultureInfo.InvariantCulture);
                        i++;
                    }

                    QueryResults.Add(result);

                    // Skip empty line after each query
                    if (i < lines.Length && string.IsNullOrWhiteSpace(lines[i]))
                    {
                        i++;
                    }
                }

                // Read execution times
                List<long> executionTimes = new List<long>();
                while (i < lines.Length)
                {
                    if (!string.IsNullOrWhiteSpace(lines[i]) && lines[i].Contains("ms"))
                    {
                        string timeStr = lines[i].Replace("ms", "").Trim();
                        if (long.TryParse(timeStr, out long time))
                        {
                            executionTimes.Add(time);
                        }
                        i++;
                    }
                    else
                    {
                        i++;
                    }
                }

                QueryExecutionTime = executionTimes[0];
                QueryExecutionTime_with_IO = executionTimes[1];
            }
            catch (Exception ex)
            {
                throw new FormatException("Error parsing output file: " + ex.Message);
            }
        }

    }
}