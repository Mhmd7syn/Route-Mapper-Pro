namespace Route_Mapper_Pro
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.next_route_btn = new System.Windows.Forms.Button();
            this.plot_route_btn = new System.Windows.Forms.Button();
            this.load_route_btn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dest_label = new System.Windows.Forms.Label();
            this.curr_loc_label = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.route_number_label = new System.Windows.Forms.Label();
            this.time_with_IO_label = new System.Windows.Forms.Label();
            this.vehicle_dist_label = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.time_label = new System.Windows.Forms.Label();
            this.path_length_label = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.shortest_time_label = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.walking_dist_label = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.mapPanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.zoomLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.mapPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.next_route_btn);
            this.groupBox1.Controls.Add(this.plot_route_btn);
            this.groupBox1.Controls.Add(this.load_route_btn);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI Emoji", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(342, 168);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Route Actions";
            // 
            // next_route_btn
            // 
            this.next_route_btn.BackColor = System.Drawing.SystemColors.Info;
            this.next_route_btn.Font = new System.Drawing.Font("Lucida Fax", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.next_route_btn.Location = new System.Drawing.Point(6, 117);
            this.next_route_btn.Name = "next_route_btn";
            this.next_route_btn.Size = new System.Drawing.Size(330, 38);
            this.next_route_btn.TabIndex = 6;
            this.next_route_btn.Text = "Next Route";
            this.next_route_btn.UseVisualStyleBackColor = false;
            this.next_route_btn.Click += new System.EventHandler(this.next_route_btn_Click);
            // 
            // plot_route_btn
            // 
            this.plot_route_btn.BackColor = System.Drawing.SystemColors.Window;
            this.plot_route_btn.Font = new System.Drawing.Font("Lucida Fax", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plot_route_btn.Location = new System.Drawing.Point(6, 73);
            this.plot_route_btn.Name = "plot_route_btn";
            this.plot_route_btn.Size = new System.Drawing.Size(330, 38);
            this.plot_route_btn.TabIndex = 5;
            this.plot_route_btn.Text = "Plot Route";
            this.plot_route_btn.UseVisualStyleBackColor = false;
            this.plot_route_btn.Click += new System.EventHandler(this.plot_route_btn_click);
            // 
            // load_route_btn
            // 
            this.load_route_btn.BackColor = System.Drawing.Color.Navy;
            this.load_route_btn.Font = new System.Drawing.Font("Lucida Fax", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.load_route_btn.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.load_route_btn.Location = new System.Drawing.Point(6, 29);
            this.load_route_btn.Name = "load_route_btn";
            this.load_route_btn.Size = new System.Drawing.Size(330, 38);
            this.load_route_btn.TabIndex = 4;
            this.load_route_btn.Text = "Load Route File";
            this.load_route_btn.UseVisualStyleBackColor = false;
            this.load_route_btn.Click += new System.EventHandler(this.load_route_btn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dest_label);
            this.groupBox2.Controls.Add(this.curr_loc_label);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI Emoji", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 256);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(342, 159);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Points";
            // 
            // dest_label
            // 
            this.dest_label.AutoSize = true;
            this.dest_label.Font = new System.Drawing.Font("Ebrima", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dest_label.ForeColor = System.Drawing.Color.Navy;
            this.dest_label.Location = new System.Drawing.Point(20, 123);
            this.dest_label.Name = "dest_label";
            this.dest_label.Size = new System.Drawing.Size(203, 23);
            this.dest_label.TabIndex = 7;
            this.dest_label.Text = "Origin: Central Park, NY";
            // 
            // curr_loc_label
            // 
            this.curr_loc_label.AutoSize = true;
            this.curr_loc_label.Font = new System.Drawing.Font("Ebrima", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.curr_loc_label.ForeColor = System.Drawing.Color.Navy;
            this.curr_loc_label.Location = new System.Drawing.Point(20, 57);
            this.curr_loc_label.Name = "curr_loc_label";
            this.curr_loc_label.Size = new System.Drawing.Size(203, 23);
            this.curr_loc_label.TabIndex = 6;
            this.curr_loc_label.Text = "Origin: Central Park, NY";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label2.Location = new System.Drawing.Point(6, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(174, 22);
            this.label2.TabIndex = 5;
            this.label2.Text = "Target Destination:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 22);
            this.label1.TabIndex = 4;
            this.label1.Text = "Current Location:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.route_number_label);
            this.groupBox3.Controls.Add(this.time_with_IO_label);
            this.groupBox3.Controls.Add(this.vehicle_dist_label);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.time_label);
            this.groupBox3.Controls.Add(this.path_length_label);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.shortest_time_label);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.walking_dist_label);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI Emoji", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 421);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(342, 240);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Route Details";
            // 
            // route_number_label
            // 
            this.route_number_label.AutoSize = true;
            this.route_number_label.Font = new System.Drawing.Font("Palatino Linotype", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.route_number_label.ForeColor = System.Drawing.SystemColors.Highlight;
            this.route_number_label.Location = new System.Drawing.Point(7, 26);
            this.route_number_label.Name = "route_number_label";
            this.route_number_label.Size = new System.Drawing.Size(117, 27);
            this.route_number_label.TabIndex = 18;
            this.route_number_label.Text = "Route 0 of 0";
            // 
            // time_with_IO_label
            // 
            this.time_with_IO_label.AutoSize = true;
            this.time_with_IO_label.Font = new System.Drawing.Font("Ebrima", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.time_with_IO_label.ForeColor = System.Drawing.Color.Navy;
            this.time_with_IO_label.Location = new System.Drawing.Point(177, 214);
            this.time_with_IO_label.Name = "time_with_IO_label";
            this.time_with_IO_label.Size = new System.Drawing.Size(58, 23);
            this.time_with_IO_label.TabIndex = 17;
            this.time_with_IO_label.Text = "10 ms";
            // 
            // vehicle_dist_label
            // 
            this.vehicle_dist_label.AutoSize = true;
            this.vehicle_dist_label.Font = new System.Drawing.Font("Ebrima", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vehicle_dist_label.ForeColor = System.Drawing.Color.Navy;
            this.vehicle_dist_label.Location = new System.Drawing.Point(255, 147);
            this.vehicle_dist_label.Name = "vehicle_dist_label";
            this.vehicle_dist_label.Size = new System.Drawing.Size(76, 23);
            this.vehicle_dist_label.TabIndex = 13;
            this.vehicle_dist_label.Text = "1.44 km";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label14.Location = new System.Drawing.Point(6, 214);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(136, 22);
            this.label14.TabIndex = 16;
            this.label14.Text = "Time With I/O:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label12.Location = new System.Drawing.Point(28, 147);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(218, 22);
            this.label12.TabIndex = 12;
            this.label12.Text = "B.Total vehicle Distance:";
            // 
            // time_label
            // 
            this.time_label.AutoSize = true;
            this.time_label.Font = new System.Drawing.Font("Ebrima", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.time_label.ForeColor = System.Drawing.Color.Navy;
            this.time_label.Location = new System.Drawing.Point(177, 191);
            this.time_label.Name = "time_label";
            this.time_label.Size = new System.Drawing.Size(48, 23);
            this.time_label.TabIndex = 15;
            this.time_label.Text = "5 ms";
            // 
            // path_length_label
            // 
            this.path_length_label.AutoSize = true;
            this.path_length_label.Font = new System.Drawing.Font("Ebrima", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.path_length_label.ForeColor = System.Drawing.Color.Navy;
            this.path_length_label.Location = new System.Drawing.Point(147, 94);
            this.path_length_label.Name = "path_length_label";
            this.path_length_label.Size = new System.Drawing.Size(76, 23);
            this.path_length_label.TabIndex = 11;
            this.path_length_label.Text = "1.72 km";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label16.Location = new System.Drawing.Point(6, 192);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(165, 22);
            this.label16.TabIndex = 14;
            this.label16.Text = "Time Without I/O:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label9.Location = new System.Drawing.Point(8, 94);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(119, 22);
            this.label9.TabIndex = 10;
            this.label9.Text = "Path Length:";
            // 
            // shortest_time_label
            // 
            this.shortest_time_label.AutoSize = true;
            this.shortest_time_label.Font = new System.Drawing.Font("Ebrima", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.shortest_time_label.ForeColor = System.Drawing.Color.Navy;
            this.shortest_time_label.Location = new System.Drawing.Point(147, 65);
            this.shortest_time_label.Name = "shortest_time_label";
            this.shortest_time_label.Size = new System.Drawing.Size(88, 23);
            this.shortest_time_label.TabIndex = 9;
            this.shortest_time_label.Text = "4.63 mins";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label7.Location = new System.Drawing.Point(8, 66);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(133, 22);
            this.label7.TabIndex = 8;
            this.label7.Text = "Shortest Time:";
            // 
            // walking_dist_label
            // 
            this.walking_dist_label.AutoSize = true;
            this.walking_dist_label.Font = new System.Drawing.Font("Ebrima", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.walking_dist_label.ForeColor = System.Drawing.Color.Navy;
            this.walking_dist_label.Location = new System.Drawing.Point(255, 121);
            this.walking_dist_label.Name = "walking_dist_label";
            this.walking_dist_label.Size = new System.Drawing.Size(76, 23);
            this.walking_dist_label.TabIndex = 7;
            this.walking_dist_label.Text = "0.28 km";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label5.Location = new System.Drawing.Point(28, 122);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(228, 22);
            this.label5.TabIndex = 5;
            this.label5.Text = "A.Total Walking Distance:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, -9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1238, 69);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // mapPanel
            // 
            this.mapPanel.BackColor = System.Drawing.Color.Transparent;
            this.mapPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapPanel.Controls.Add(this.label3);
            this.mapPanel.Controls.Add(this.zoomLabel);
            this.mapPanel.Location = new System.Drawing.Point(360, 94);
            this.mapPanel.Name = "mapPanel";
            this.mapPanel.Size = new System.Drawing.Size(890, 567);
            this.mapPanel.TabIndex = 4;
            this.mapPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mapPanel_Paint);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(794, 549);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "Zoom:";
            // 
            // zoomLabel
            // 
            this.zoomLabel.AutoSize = true;
            this.zoomLabel.BackColor = System.Drawing.Color.White;
            this.zoomLabel.Location = new System.Drawing.Point(845, 549);
            this.zoomLabel.Name = "zoomLabel";
            this.zoomLabel.Size = new System.Drawing.Size(40, 16);
            this.zoomLabel.TabIndex = 0;
            this.zoomLabel.Text = "100%";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1262, 673);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.mapPanel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.mapPanel.ResumeLayout(false);
            this.mapPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button load_route_btn;
        private System.Windows.Forms.Button next_route_btn;
        private System.Windows.Forms.Button plot_route_btn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label curr_loc_label;
        private System.Windows.Forms.Label dest_label;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label shortest_time_label;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label walking_dist_label;
        private System.Windows.Forms.Label vehicle_dist_label;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label path_length_label;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label time_with_IO_label;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label time_label;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Panel mapPanel;
        private System.Windows.Forms.Label zoomLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label route_number_label;
    }
}

