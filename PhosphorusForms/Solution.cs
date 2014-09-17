using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PhosphorusForms;

class Solution
{
    public void GenerateGraph(List<int>[] nextRooms, bool[] hasPrisonner, List<int> exitList)
    {
        //string graphtext = "graph graphname { a -- b -- c; b -- d; }";
        string graphtext = "graph graphname { node [shape = circle, width = 0.5, fixedsize = true, style = filled, fillcolor = palegreen];";
        for (int room = 0 ; room < hasPrisonner.Length ; ++room)
        {
            if (hasPrisonner[room])
                graphtext += room.ToString() + " [fillcolor = orange]; ";
        }

        foreach (int exit in exitList)
        {
            if (!hasPrisonner[exit])
                graphtext += exit.ToString() + " [fillcolor = red]; ";
        }

        for (int room = 0; room < nextRooms.Length; ++room)
        {
            foreach (int roomExit in nextRooms[room])
                if (roomExit > room)
                    graphtext += room.ToString() + " -- " + roomExit.ToString() + "; ";            
        }
        //for (int i = 0; i < A.Length; ++i)
        //{
        //    graphtext += A[i].ToString() + " -- " + B[i].ToString() + "; ";
        //}
        graphtext += " }\n";

        Image graph = GraphViz.RenderImage(graphtext, "dot", "png");
        Program._form.SetPicture(graph);
    }

    public void UpdateImage(PictureBox p1, Image img)
    {
        p1.Image = img;
    }
    public int solution(int[] A, int[] B, int[] C)
    {
        // write your code in C# 5.0 with .NET 4.5 (Mono)
        //int[] nbCorridors = new int[A.Length+1];
        List<int> exitList = new List<int>(A.Length+1);
        bool[] hasPrisonner = new bool[A.Length+1];
        List<int>[] nextRooms = new List<int>[A.Length+1];

        foreach (int prisonerRoom in C)
        {
            hasPrisonner[prisonerRoom] = true;
        }
        for (int idx = 0 ; idx < A.Length ; ++idx)
        {
            //bCorridors[A[idx]]++;
            if (nextRooms[A[idx]] == null)
                nextRooms[A[idx]] = new List<int>(A.Length);
            nextRooms[A[idx]].Add(B[idx]);
            //nbCorridors[B[idx]]++;
            if (nextRooms[B[idx]] == null)
                nextRooms[B[idx]] = new List<int>(A.Length);
            nextRooms[B[idx]].Add(A[idx]);
        }

        for (int room = 0; room < nextRooms.Length; ++room)
        {
            if (nextRooms[room].Count == 1) 
                exitList.Add(room);
        }

        GenerateGraph(nextRooms, hasPrisonner, exitList);

        bool reduced = true;
        while (reduced)
        {
            reduced = false;
            for (int exitId = exitList.Count - 1; exitId >= 0; --exitId)
            {
                int currRoom = exitList[exitId];
                while (nextRooms[currRoom].Count == 1 && !hasPrisonner[nextRooms[currRoom][0]])
                {
                    int nextRoom = nextRooms[currRoom][0];
                    nextRooms[currRoom].Clear();
                    nextRooms[nextRoom].Remove(currRoom);
                    exitList.Remove(currRoom);
                    if (!exitList.Contains(nextRoom))
                    {
                        exitList.Add(nextRoom);
                        Console.WriteLine(nextRoom.ToString() + " is an exit");
                    }

                    currRoom = nextRoom;
                    reduced = true;

                    GenerateGraph(nextRooms, hasPrisonner, exitList);

                    System.Threading.Thread.Sleep(1000);
                }
            }
        }





        
            
            
        return 1;
    }
}