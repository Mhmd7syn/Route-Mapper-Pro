﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Route_Mapper_Pro
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            // Run Large File without GUI
            // MapData mapData = new MapData();
            // string folder_path = "D:\\Study\\Algo\\Project\\Route-Mapper-Pro\\[3] Large Cases\\Input\\";
            // mapData.LoadMapFile(folder_path + "SFMap.txt");
            // PathFinder.SolveWithTiming(mapData, folder_path + "SFQueries.txt");
        }
    }
}
