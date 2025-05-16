using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Route_Mapper_Pro
{
    public partial class Form1 : Form
    {
        private Point panel_location;
        private List<int> currentRoute = new List<int>();
        private int currentStepIndex = 0;

        public Form1()
        {
            InitializeComponent();
            panel_location = mapPanel.Location;
            currentRoute = new List<int>();
            currentStepIndex = 0;
            mapPanel.Invalidate(); // Use Invalidate instead of direct painting
        }

        // Sample data: List of intersections (id, x, y)
        private List<(int id, int x, int y)> intersections = new List<(int, int, int)>
        {
            (1, 100, 100),  // Intersection 1
            (2, 300, 150),  // Intersection 2
            (3, 200, 300),  // Intersection 3
            (4, 300, 350)   // Intersection 4
        };

        // Sample data: List of all possible edges (fromId, toId, length, speed)
        private List<(int fromId, int toId, int length, int speed)> allEdges = new List<(int, int, int, int)>
        {
            (1, 2, 150, 50),  // Connection between 1-2
            (1, 3, 200, 40),  // Connection between 1-3
            (2, 4, 120, 60),  // Connection between 2-4
            (3, 4, 180, 45)   // Connection between 3-4
        };

        private void mapPanel_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Draw all edges in default color (blue)
            DrawEdges(g, allEdges, Color.Blue);

            // Draw all intersections
            DrawIntersections(g);

            // Draw the current route if one exists
            if (currentRoute.Count > 1)
            {
                DrawRoute(g, currentRoute, currentStepIndex);
            }
        }

        private void DrawIntersections(Graphics g)
        {
            int nodeSize = 12;
            Font labelFont = new Font("Arial", 8);

            foreach (var intersection in intersections)
            {
                // Draw intersection point
                g.FillEllipse(Brushes.Red,
                             panel_location.X + intersection.x - nodeSize / 2,
                             panel_location.Y + intersection.y - nodeSize / 2,
                             nodeSize, nodeSize);

                // Draw intersection ID
                g.DrawString(intersection.id.ToString(), labelFont, Brushes.Black,
                             panel_location.X + intersection.x + nodeSize / 2,
                             panel_location.Y + intersection.y + nodeSize / 2);
            }
        }

        private void DrawEdges(Graphics g, List<(int fromId, int toId, int length, int speed)> edgesToDraw, Color edgeColor)
        {
            using (Pen roadPen = new Pen(edgeColor, 2))
            {
                Font labelFont = new Font("Arial", 8);

                foreach (var edge in edgesToDraw)
                {
                    var fromNode = intersections.Find(i => i.id == edge.fromId);
                    var toNode = intersections.Find(i => i.id == edge.toId);

                    if (fromNode != default && toNode != default)
                    {
                        g.DrawLine(roadPen,
                                 panel_location.X + fromNode.x,
                                 panel_location.Y + fromNode.y,
                                 panel_location.X + toNode.x,
                                 panel_location.Y + toNode.y);

                        int midX = (fromNode.x + toNode.x) / 2;
                        int midY = (fromNode.y + toNode.y) / 2;

                        string roadInfo = $"{edge.length}m\n{edge.speed}km/h";
                        g.DrawString(roadInfo, labelFont, Brushes.DarkGreen,
                                    panel_location.X + midX,
                                    panel_location.Y + midY);
                    }
                }
            }
        }

        public void DrawBlackEdgesBetween(List<int> intersectionIds)
        {
            if (intersectionIds == null || intersectionIds.Count < 2)
                return;

            // Store the current route and reset position
            currentRoute = intersectionIds;
            currentStepIndex = 0;

            UpdateCurrentLocationDisplay();
            mapPanel.Invalidate(); // Trigger full repaint
        }

        private void DrawRoute(Graphics g, List<int> route, int currentPosition)
        {
            if (route == null || route.Count < 2)
                return;

            // Draw the path edges (thicker black lines)
            using (Pen pathPen = new Pen(Color.Black, 3))
            {
                for (int i = 0; i < route.Count - 1; i++)
                {
                    var fromNode = intersections.Find(x => x.id == route[i]);
                    var toNode = intersections.Find(x => x.id == route[i + 1]);

                    if (fromNode != default && toNode != default)
                    {
                        g.DrawLine(pathPen,
                                 panel_location.X + fromNode.x,
                                 panel_location.Y + fromNode.y,
                                 panel_location.X + toNode.x,
                                 panel_location.Y + toNode.y);
                    }
                }
            }

            // Draw start point (green)
            var startIntersection = intersections.Find(i => i.id == route[0]);
            if (startIntersection != default)
            {
                DrawSpecialPoint(g, startIntersection, "Start",
                               new SolidBrush(Color.Green), false);
            }

            // Draw end point (purple with star)
            var endIntersection = intersections.Find(i => i.id == route[route.Count - 1]);
            if (endIntersection != default)
            {
                DrawSpecialPoint(g, endIntersection, "Destination",
                               new SolidBrush(Color.Purple), true);
            }

            // Draw current position (blue)
            if (currentPosition >= 0 && currentPosition < route.Count)
            {
                var currentIntersection = intersections.Find(i => i.id == route[currentPosition]);
                if (currentIntersection != default)
                {
                    DrawSpecialPoint(g, currentIntersection, "Current Location",
                                   new SolidBrush(Color.Blue), false);
                }
            }
        }

        private void UpdateCurrentLocationDisplay()
        {
            if (currentRoute != null && currentRoute.Count > 0 && currentStepIndex < currentRoute.Count)
            {
                var currentIntersection = intersections.Find(i => i.id == currentRoute[currentStepIndex]);
                if (currentIntersection != default)
                {
                    curr_loc_label.Text = $"( {currentIntersection.x}, {currentIntersection.y} )";
                }

                var endIntersection = intersections.Find(i => i.id == currentRoute[currentRoute.Count - 1]);
                if (endIntersection != default)
                {
                    dest_label.Text = $"( {endIntersection.x}, {endIntersection.y} )";
                }
            }
        }

        private void DrawSpecialPoint(Graphics g, (int id, int x, int y) intersection,
                                    string label, Brush color, bool isDestination)
        {
            int nodeSize = isDestination ? 20 : 18;
            int outlineSize = nodeSize + 4;
            Font labelFont = new Font("Arial", 9, FontStyle.Bold);

            // Draw white outline
            g.FillEllipse(Brushes.White,
                         panel_location.X + intersection.x - outlineSize / 2,
                         panel_location.Y + intersection.y - outlineSize / 2,
                         outlineSize, outlineSize);

            // Draw main colored circle
            g.FillEllipse(color,
                         panel_location.X + intersection.x - nodeSize / 2,
                         panel_location.Y + intersection.y - nodeSize / 2,
                         nodeSize, nodeSize);

            // Draw destination star icon
            if (isDestination)
            {
                PointF center = new PointF(panel_location.X + intersection.x,
                                         panel_location.Y + intersection.y);
                DrawStar(g, center, nodeSize / 2 - 2, nodeSize / 4, 5, Brushes.White);
            }

            // Draw the label
            SizeF textSize = g.MeasureString(label, labelFont);
            float labelY = panel_location.Y + intersection.y - nodeSize - textSize.Height - 2;

            // Label background
            g.FillRectangle(Brushes.White,
                           panel_location.X + intersection.x - textSize.Width / 2 - 2,
                           labelY - 1,
                           textSize.Width + 4,
                           textSize.Height + 2);

            g.DrawString(label, labelFont, Brushes.Black,
                         panel_location.X + intersection.x - textSize.Width / 2,
                         labelY);

            // ID in center
            Font idFont = new Font("Arial", 8, FontStyle.Bold);
            g.DrawString(intersection.id.ToString(), idFont,
                         isDestination ? Brushes.White : Brushes.Black,
                         panel_location.X + intersection.x - 4,
                         panel_location.Y + intersection.y - 6);
        }

        private void DrawStar(Graphics g, PointF center, float outerRadius, float innerRadius, int points, Brush brush)
        {
            double angle = Math.PI / points;
            PointF[] starPoints = new PointF[points * 2];

            for (int i = 0; i < points * 2; i++)
            {
                float radius = i % 2 == 0 ? outerRadius : innerRadius;
                starPoints[i] = new PointF(
                    center.X + (float)(radius * Math.Sin(i * angle)),
                    center.Y + (float)(radius * Math.Cos(i * angle))
                );
            }

            g.FillPolygon(brush, starPoints);
        }

        private void plot_route_btn_click(object sender, EventArgs e)
        {
            DrawBlackEdgesBetween(new List<int> { 1, 3, 4 });
        }

        private void next_step_btn_Click(object sender, EventArgs e)
        {
            if (currentRoute == null || currentRoute.Count == 0)
            {
                MessageBox.Show("No route is currently active.");
                return;
            }

            if (currentStepIndex < currentRoute.Count - 1)
            {
                currentStepIndex++;
                UpdateCurrentLocationDisplay();
                mapPanel.Invalidate(); // This will trigger mapPanel_Paint with updated position
            }
            else
            {
                MessageBox.Show("Already at the destination!");
            }
        }
    }
}