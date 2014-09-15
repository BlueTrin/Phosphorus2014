using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

class Solution
{
    public void GenerateGraph(int[] A, int[] B, int[] C, bool[] hasPrisonner, List<int> exitList, Form form, PictureBox pictureBox1)
    {
        //string graphtext = "graph graphname { a -- b -- c; b -- d; }";
        string graphtext = "graph graphname { node [shape = circle, width = 0.5, fixedsize = true, style = filled, fillcolor = palegreen];";
        foreach (int prisonerId in C)
        {
            hasPrisonner[prisonerId] = true;
            graphtext += prisonerId.ToString() + " [fillcolor = orange]; ";
        }

        foreach (int exit in exitList)
        {
            if (!hasPrisonner[exit])
                graphtext += exit.ToString() + " [fillcolor = red]; ";

        }
        for (int i = 0; i < A.Length; ++i)
        {
            graphtext += A[i].ToString() + " -- " + B[i].ToString() + "; ";
        }
        graphtext += " }\n";

        Image graph = GraphViz.RenderImage(graphtext, "dot", "png");
        pictureBox1.Image = graph;

        form.Width = pictureBox1.Width + 50;
        form.Height = pictureBox1.Height + 50;

    }
    public int solution(int[] A, int[] B, int[] C)
    {
        // write your code in C# 5.0 with .NET 4.5 (Mono)
        int[] nbCorridors = new int[A.Length+1];
        List<int> exitList = new List<int>(A.Length+1);
        bool[] hasPrisonner = new bool[A.Length+1];
        List<int>[] nextRooms = new List<int>[A.Length+1];

        for (int idx = 0 ; idx < A.Length ; ++idx)
        {
            nbCorridors[A[idx]]++;
            if (nextRooms[A[idx]] == null)
                nextRooms[A[idx]] = new List<int>(A.Length);
            nextRooms[A[idx]].Add(B[idx]);
            nbCorridors[B[idx]]++;
            if (nextRooms[B[idx]] == null)
                nextRooms[B[idx]] = new List<int>(A.Length);
            nextRooms[B[idx]].Add(A[idx]);
        }

        for (int nbCorridorIdx = 0; nbCorridorIdx < nbCorridors.Length; ++nbCorridorIdx)
        {
            if (nbCorridors[nbCorridorIdx] == 1) 
                exitList.Add(nbCorridorIdx);
        }

        Form form = new Form();
        form.AutoSizeMode = AutoSizeMode.GrowAndShrink;

        PictureBox pictureBox1 = new PictureBox();
        pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
        form.Controls.Add(pictureBox1);
        GenerateGraph(A, B, C, hasPrisonner, exitList, form, pictureBox1);
        form.ShowDialog(); // Or Application.Run(form)

        bool reduced = true;
        while (reduced)
        {
            reduced = false;
            for (int exitId = exitList.Count - 1; exitId >= 0; --exitId)
            {
                if (nbCorridors[exitId] == 1)
                {
                    reduced = true;
                    
                }
            }
        }





        
            
            
        return 1;
    }
}