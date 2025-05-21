using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Route_Mapper_Pro
{
    public partial class Form1 : Form
    {
        private Point panel_location;
        private List<int> currentRoute = new List<int>();
        private int currentRouteIndex = 0;
        private MapData mapData = new MapData();
        private float currentZoom = 1.0f; // 1.0 = 100% zoom
        private const float ZoomIncrement = 0.25f; // How much to zoom in/out per step
        private const float MinZoom = 0.5f; // Minimum zoom level
        private const float MaxZoom = 3.0f; // Maximum zoom level
        private PointF zoomCenter = PointF.Empty;
        private Point dragStart;
        private bool isDragging = false;
        private float CoordinateScale { get; set; } = 10f; // Scale normalized coords to pixels
        private Dictionary<int, (float x, float y)> _scaledIntersectionCache;
        private DateTime _lastCacheInvalidation = DateTime.MinValue;

        public Form1()
        {
            InitializeComponent();

            // Enable double buffering
            this.DoubleBuffered = true;

            CoordinateScale = 10f;

            // Initialize panel location at center
            panel_location = new Point(
                mapPanel.Width / 2 - (int)(0.5f * CoordinateScale),
                mapPanel.Height / 2 - (int)(0.5f * CoordinateScale)
            );

            currentRoute = new List<int>();

            // Set up event handlers
            mapPanel.MouseWheel += mapPanel_MouseWheel;
            mapPanel.MouseDown += mapPanel_MouseDown;
            mapPanel.MouseUp += mapPanel_MouseUp;
            mapPanel.MouseMove += mapPanel_MouseMove;

            mapPanel.Invalidate();
            UpdateCurrentLocationDisplay();
            if (mapData.QueryResults.Count > 0)
            {
                UpdateRouteInfo(mapData.QueryResults[0]);
            }
        }

        private void mapPanel_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Apply transformations
            g.TranslateTransform(panel_location.X, panel_location.Y);
            g.ScaleTransform(currentZoom, currentZoom);

            // Set high quality rendering
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // Draw all elements
            DrawEdges(g, mapData.Roads, Color.Blue);
            if (CoordinateScale >= 10f)
                DrawIntersections(g);

            if (currentRoute.Count > 1)
            {
                DrawRoute(g, currentRoute, 0);
            }
        }
        private float CalculateDynamicScale()
        {
            if (mapData.Intersections == null || mapData.Intersections.Count == 0)
                return 10f; // Default scale if no points

            // Find min and max coordinates
            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;

            foreach (var point in mapData.Intersections)
            {
                minX = Math.Min(minX, point.x);
                minY = Math.Min(minY, point.y);
                maxX = Math.Max(maxX, point.x);
                maxY = Math.Max(maxY, point.y);
            }

            // Calculate required scale to fit the points in the panel
            float widthRatio = mapPanel.Width / (maxX - minX);
            float heightRatio = mapPanel.Height / (maxY - minY);

            // Use the smaller ratio to ensure everything fits
            float scale = Math.Min(widthRatio, heightRatio) * 0.9f; // 90% to add some margin

            // Apply minimum and maximum limits
            return scale;
        }
        private void mapPanel_MouseMove(object sender, MouseEventArgs e)
        {
            // Update zoom center
            zoomCenter = e.Location;

            // Handle panning
            if (isDragging)
            {
                panel_location.X += e.X - dragStart.X;
                panel_location.Y += e.Y - dragStart.Y;
                dragStart = e.Location;
                mapPanel.Invalidate();
            }
        }

        private void mapPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                ZoomAtPoint(currentZoom + ZoomIncrement, e.Location);
            }
            else
            {
                ZoomAtPoint(currentZoom - ZoomIncrement, e.Location);
            }
        }

        private void ZoomAtPoint(float newZoom, PointF fixedPoint)
        {
            // Convert screen point to world coordinates
            PointF worldBeforeZoom = new PointF(
                (fixedPoint.X - panel_location.X) / currentZoom,
                (fixedPoint.Y - panel_location.Y) / currentZoom
            );

            // Apply new zoom level with constraints
            newZoom = Math.Max(MinZoom, Math.Min(MaxZoom, newZoom));

            // Calculate new panel location to keep the point fixed
            panel_location.X = (int)(fixedPoint.X - worldBeforeZoom.X * newZoom);
            panel_location.Y = (int)(fixedPoint.Y - worldBeforeZoom.Y * newZoom);

            currentZoom = newZoom;

            mapPanel.Invalidate();
            UpdateZoomLabel();
        }
        private void UpdateZoomLabel()
        {
            // Add a label to your form to display zoom level
            zoomLabel.Text = $"{currentZoom * 100:F0}%";
        }
        private void mapPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)
            {
                isDragging = true;
                dragStart = e.Location;
                mapPanel.Cursor = Cursors.Hand;
            }
        }
        private void mapPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            mapPanel.Cursor = Cursors.Default;
        }
        private void CenterViewOnPoints()
        {
            if (mapData.Intersections == null || mapData.Intersections.Count == 0)
                return;

            // Find center of all points
            float centerX = 0, centerY = 0;
            foreach (var point in mapData.Intersections)
            {
                centerX += point.x;
                centerY += point.y;
            }
            centerX /= mapData.Intersections.Count;
            centerY /= mapData.Intersections.Count;

            // Calculate panel location to center this point
            panel_location = new Point(
                mapPanel.Width / 2 - (int)(centerX * CoordinateScale * currentZoom),
                mapPanel.Height / 2 - (int)(centerY * CoordinateScale * currentZoom)
            );

            mapPanel.Invalidate();
        }
        private void UpdateScaledIntersectionCache()
        {
            // Only rebuild cache if data changed or zoom/scale changed
            if ((DateTime.Now - _lastCacheInvalidation).TotalSeconds < 0.5 &&
                _scaledIntersectionCache != null)
                return;

            _scaledIntersectionCache = mapData.Intersections?
                .ToDictionary(
                    i => i.id,
                    i => (i.x * CoordinateScale, i.y * CoordinateScale)
                );

            _lastCacheInvalidation = DateTime.Now;
        }
        private void DrawIntersections(Graphics g)
        {
            if (mapData.Intersections == null || mapData.Intersections.Count == 0)
                return;

            int nodeSize = (int)(12 / currentZoom);
            using (var labelFont = new Font("Arial", Math.Max(6, 8 / currentZoom)))
            using (var brush = new SolidBrush(Color.Red))
            {
                // Pre-calculate scaled positions
                var scaledPoints = mapData.Intersections
                    .Select(i => new
                    {
                        i.id,
                        X = i.x * CoordinateScale,
                        Y = i.y * CoordinateScale
                    })
                    .ToList();

                // Draw in batches
                foreach (var point in scaledPoints)
                {
                    g.FillEllipse(brush, point.X - nodeSize / 2, point.Y - nodeSize / 2, nodeSize, nodeSize);
                    g.DrawString(point.id.ToString(), labelFont, Brushes.Black,
                                point.X + nodeSize / 2 + 2,
                                point.Y + nodeSize / 2 + 2);
                }
            }
        }

        private void DrawEdges(Graphics g, List<(int fromId, int toId, float length, float speed)> edgesToDraw, Color edgeColor)
        {
            if (edgesToDraw == null || edgesToDraw.Count == 0)
                return;

            using (var pen = new Pen(edgeColor, 2))
            using (var labelFont = new Font("Arial", 8 / currentZoom))
            {
                // Pre-cache all needed nodes
                var nodeCache = new Dictionary<int, (float x, float y)>();
                foreach (var edge in edgesToDraw)
                {
                    if (!nodeCache.ContainsKey(edge.fromId))
                    {
                        var node = mapData.Intersections.Find(i => i.id == edge.fromId);
                        if (node != default)
                            nodeCache[edge.fromId] = (node.x * CoordinateScale, node.y * CoordinateScale);
                    }

                    if (!nodeCache.ContainsKey(edge.toId))
                    {
                        var node = mapData.Intersections.Find(i => i.id == edge.toId);
                        if (node != default)
                            nodeCache[edge.toId] = (node.x * CoordinateScale, node.y * CoordinateScale);
                    }
                }

                // Draw all edges
                foreach (var edge in edgesToDraw)
                {
                    if (nodeCache.TryGetValue(edge.fromId, out var fromNode) &&
                        nodeCache.TryGetValue(edge.toId, out var toNode))
                    {
                        g.DrawLine(pen, fromNode.x, fromNode.y, toNode.x, toNode.y);

                        if (CoordinateScale >= 10f)
                        {
                            string roadInfo = $"{edge.length}m\n{edge.speed}km/h";
                            float midX = (fromNode.x + toNode.x) / 2;
                            float midY = (fromNode.y + toNode.y) / 2;
                            g.DrawString(roadInfo, labelFont, Brushes.DarkGreen, midX, midY);
                        }
                    }
                }
            }
        }

        public void DrawBlackEdgesBetween(List<int> intersectionIds)
        {
            if (intersectionIds == null || intersectionIds.Count < 2)
                return;

            currentRoute = intersectionIds;
            UpdateCurrentLocationDisplay();
            mapPanel.Invalidate();
        }

        private void DrawRoute(Graphics g, List<int> route, int currentPosition)
        {
            if (route == null || route.Count < 2)
                return;

            UpdateScaledIntersectionCache(); // Ensure cache is up to date

            using (var pathPen = new Pen(Color.Black, 3 / currentZoom))
            {
                // Draw all path segments
                for (int i = 0; i < route.Count - 1; i++)
                {
                    if (_scaledIntersectionCache.TryGetValue(route[i], out var fromNode) &&
                        _scaledIntersectionCache.TryGetValue(route[i + 1], out var toNode))
                    {
                        g.DrawLine(pathPen, fromNode.x, fromNode.y, toNode.x, toNode.y);
                    }
                }
            }

            // Draw special points (start, end, current)
            if (_scaledIntersectionCache.TryGetValue(route[0], out var startNode))
            {
                DrawSpecialPoint(g, (route[0], startNode.x, startNode.y), "Start",
                               Brushes.Green, false);
            }

            if (_scaledIntersectionCache.TryGetValue(route[route.Count - 1], out var endNode))
            {
                DrawSpecialPoint(g, (route[route.Count - 1], endNode.x, endNode.y),
                               "Destination", Brushes.Purple, true);
            }

            if (currentPosition >= 0 && currentPosition < route.Count &&
                _scaledIntersectionCache.TryGetValue(route[currentPosition], out var currentNode))
            {
                DrawSpecialPoint(g, (route[currentPosition], currentNode.x, currentNode.y),
                               "Current Location", Brushes.Blue, false);
            }
        }

        private void UpdateCurrentLocationDisplay()
        {
            if (currentRoute != null && currentRoute.Count > 0)
            {
                var currentIntersection = mapData.Intersections.Find(i => i.id == currentRoute[0]);
                if (currentIntersection != default)
                {
                    curr_loc_label.Text = $"( {currentIntersection.x}, {currentIntersection.y} )";
                }

                var endIntersection = mapData.Intersections.Find(i => i.id == currentRoute[currentRoute.Count - 1]);
                if (endIntersection != default)
                {
                    dest_label.Text = $"( {endIntersection.x}, {endIntersection.y} )";
                }

                // Show current route number
                route_number_label.Text = $"Route {currentRouteIndex + 1} of {mapData.QueryResults.Count}";
            }
            else
            {
                curr_loc_label.Text = "Not Available";
                dest_label.Text = "Not Available";
                route_number_label.Text = "No Active Route";
            }
        }

        private void DrawSpecialPoint(Graphics g, (int id, float x, float y) intersection,
                    string label, Brush color, bool isDestination)
        {

            float x = intersection.x;
            float y = intersection.y;

            int nodeSize = (int)((isDestination ? 20 : 18) / currentZoom);
            int outlineSize = nodeSize + 4;
            Font labelFont = new Font("Arial", Math.Max(8, 9 / currentZoom), FontStyle.Bold);

            g.FillEllipse(Brushes.White, x - outlineSize / 2, y - outlineSize / 2, outlineSize, outlineSize);
            g.FillEllipse(color, x - nodeSize / 2, y - nodeSize / 2, nodeSize, nodeSize);

            if (isDestination)
            {
                PointF center = new PointF(x, y);
                DrawStar(g, center, nodeSize / 2 - 2, nodeSize / 4, 5, Brushes.White);
            }

            SizeF textSize = g.MeasureString(label, labelFont);
            float labelY = y - nodeSize - textSize.Height - 2;

            g.FillRectangle(Brushes.White,
                           x - textSize.Width / 2 - 2,
                           labelY - 1,
                           textSize.Width + 4,
                           textSize.Height + 2);

            g.DrawString(label, labelFont, Brushes.Black,
                         x - textSize.Width / 2,
                         labelY);

            Font idFont = new Font("Arial", Math.Max(7, 8 / currentZoom), FontStyle.Bold);
            g.DrawString(intersection.id.ToString(), idFont,
                         isDestination ? Brushes.White : Brushes.Black,
                         x - 4,
                         y - 6);
        }

        private void DrawStar(Graphics g, PointF center, float outerRadius, float innerRadius, int points, Brush brush)
        {
            double angle = Math.PI / points;
            PointF[] starPoints = new PointF[points * 2];

            for (int i = 0; i < points * 2; i++)
            {
                float radius = i % 2 == 0 ? outerRadius / currentZoom : innerRadius / currentZoom; // Scale radii
                starPoints[i] = new PointF(
                    center.X + (float)(radius * Math.Sin(i * angle)),
                    center.Y + (float)(radius * Math.Cos(i * angle))
                );
            }

            g.FillPolygon(brush, starPoints);
        }

        private void load_route_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Map Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Load the map file
                mapData.LoadMapFile(openFileDialog.FileName);

                CoordinateScale = CalculateDynamicScale(); // Calculate scale after loading
                CenterViewOnPoints();
                currentRoute.Clear();
                mapPanel.Invalidate();
            }
        }

        private void plot_route_btn_click(object sender, EventArgs e)
        {
            var choiceDialog = new Form()
            {
                Text = "Select Input Type",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                Width = 300,
                Height = 150
            };

            var label = new Label()
            {
                Text = "Would you like to load:",
                Left = 20,
                Top = 20,
                Width = 250,
                AutoSize = true
            };

            var queryButton = new Button()
            {
                Text = "Query File (process new routes)",
                Left = 20,
                Top = 50,
                Width = 250,
                DialogResult = DialogResult.Yes
            };

            var outputButton = new Button()
            {
                Text = "Output File (load existing results)",
                Left = 20,
                Top = 80,
                Width = 250,
                DialogResult = DialogResult.No
            };

            queryButton.Click += (s, args) => { choiceDialog.Close(); };
            outputButton.Click += (s, args) => { choiceDialog.Close(); };

            choiceDialog.Controls.Add(label);
            choiceDialog.Controls.Add(queryButton);
            choiceDialog.Controls.Add(outputButton);
            choiceDialog.AcceptButton = queryButton;
            choiceDialog.CancelButton = outputButton;

            var result = choiceDialog.ShowDialog(this);

            if (result == DialogResult.Yes)
            {
                plot_route_btn_click_queryFile(sender, e);
            }
            else if (result == DialogResult.No)
            {
                plot_route_btn_click_outputFile(sender, e);
            }
        }

        private void plot_route_btn_click_outputFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Output Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.Title = "Select Output File to Load";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    mapData.LoadOutputFile(openFileDialog.FileName);
                    if (mapData.QueryResults.Count > 0)
                    {
                        CoordinateScale = CalculateDynamicScale();
                        currentRouteIndex = 0;
                        DrawBlackEdgesBetween(mapData.QueryResults[currentRouteIndex].PathIds);
                        UpdateRouteInfo(mapData.QueryResults[currentRouteIndex]);
                        MessageBox.Show("Output file loaded successfully!", "Success",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("The selected output file contains no valid results.",
                                      "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading output file: {ex.Message}",
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void plot_route_btn_click_queryFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Query Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.Title = "Select Query File to Process";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    mapData = PathFinder.SolveWithTiming(mapData, openFileDialog.FileName);
                    if (mapData.QueryResults.Count > 0)
                    {
                        CoordinateScale = CalculateDynamicScale();
                        currentRouteIndex = 0;
                        DrawBlackEdgesBetween(mapData.QueryResults[currentRouteIndex].PathIds);
                        UpdateRouteInfo(mapData.QueryResults[currentRouteIndex]);
                    }
                    else
                    {
                        MessageBox.Show("No routes were found for the given queries.",
                                      "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing query file: {ex.Message}",
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateRouteInfo(QueryResult result)
        {

            shortest_time_label.Text = $"{result.ShortestTime:F2} mins";
            path_length_label.Text = $"{result.TotalDistance:F2} km";
            walking_dist_label.Text = $"{result.WalkingDistance:F2} km";
            vehicle_dist_label.Text = $"{result.VehicleDistance:F2} km";
            time_label.Text = $"{mapData.QueryExecutionTime} ms";
            time_with_IO_label.Text = $"{mapData.QueryExecutionTime_with_IO} ms";
        }

        private void next_route_btn_Click(object sender, EventArgs e)
        {
            if (mapData.QueryResults == null || mapData.QueryResults.Count == 0)
            {
                MessageBox.Show("No routes available.");
                return;
            }

            // Increment route index (with wrap-around)
            currentRouteIndex = (currentRouteIndex + 1) % mapData.QueryResults.Count;

            // Get the new route
            var nextRoute = mapData.QueryResults[currentRouteIndex];

            // Update display
            DrawBlackEdgesBetween(nextRoute.PathIds);
            UpdateRouteInfo(nextRoute);

            // Reset current position to start of route
            UpdateCurrentLocationDisplay();
            mapPanel.Invalidate();
        }
    }
}