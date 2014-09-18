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
    public struct PrisonRepresentation
    {
        public int nbRooms;
        public bool[] hasPrisonner;
        public bool[] isExit;
        public List<int>[] neighbour;

    }
    public PrisonRepresentation transformRepresentation(int[] A, int[] B, int[] C)
    {
        int nbRooms = A.Length + 1;
        bool[] hasPrisonner = new bool[nbRooms];
        bool[] isExit = new bool[nbRooms];
        List<int>[] neighbour = new List<int>[nbRooms];
        List<int> exitList = new List<int>();

        for (int idx = 0; idx < A.Length; ++idx)
        {
            if (neighbour[B[idx]] == null)
                neighbour[B[idx]] = new List<int>();
            neighbour[B[idx]].Add(A[idx]);

            if (neighbour[A[idx]] == null)
                neighbour[A[idx]] = new List<int>();
            neighbour[A[idx]].Add(B[idx]);
        }

        for (int room = 0; room < neighbour.Length; ++room)
        {
            if (neighbour[room].Count == 1)
                isExit[room] = true;
        }

        for (int idx = 0; idx < C.Length; ++idx)
        {
            hasPrisonner[C[idx]] = true;
        }
        PrisonRepresentation toret = new PrisonRepresentation
        {
            hasPrisonner =  hasPrisonner,
            isExit = isExit,
            nbRooms =  nbRooms,
            neighbour = neighbour
        
        };
        return toret;

    }
    public Image GenerateGraph(
        bool[] hasPrisonner,
        List<int>[] neighbour,
        bool[] isExit)
    {


        //string graphtext = "graph graphname { a -- b -- c; b -- d; }";
        string graphtext = "graph graphname { node [shape = circle, width = 0.5, fixedsize = true, style = filled, fillcolor = palegreen];";
        for (int room = 0 ; room < hasPrisonner.Length ; ++room)
        {
            if (hasPrisonner[room])
                graphtext += room.ToString() + " [fillcolor = red]; ";
        }

        for (int room = 0; room < isExit.Length; ++room)
        {
            if (!hasPrisonner[room] && isExit[room])
                graphtext += room.ToString() + " [fillcolor = yellow]; ";
        }

        for (int room = 0; room < neighbour.Length; ++room)
        {
            foreach (int roomExit in neighbour[room])
                if (roomExit > room)
                    graphtext += room.ToString() + " -- " + roomExit.ToString() + "; ";            
        }
        graphtext += " }\n";

        Image graph = GraphViz.RenderImage(graphtext, "dot", "png");
        return graph;
    }

    public void UpdateImage(PictureBox p1, Image img)
    {
        p1.Image = img;
    }

    
    public int solution(int[] A, int[] B, int[] C, out PrisonRepresentation? representation)
    {
        representation = null;
        if (C.Length == 0)
            return 0;
        int nbRooms = A.Length + 1;
        bool[] hasPrisonner = new bool[nbRooms];
        bool[] isExit = new bool[nbRooms];
        List<int>[] neighbour = new List<int>[nbRooms];

        for (int idx = 0; idx < nbRooms; ++idx)
        {
            neighbour[idx] = new List<int>();
        }
        for (int idx = 0; idx < A.Length; ++idx)
        {
            if (A[idx] == B[idx])
                continue;

            neighbour[B[idx]].Add(A[idx]);

            neighbour[A[idx]].Add(B[idx]);
        }

        for (int idx = 0; idx < C.Length; ++idx)
        {
            if (neighbour[C[idx]].Count == 1)
                return -1;
            hasPrisonner[C[idx]] = true;
        }

        int nbExits = 0;
        for (int room = 0; room < neighbour.Length; ++room)
        {
            if (neighbour[room].Count == 1)
            {
                isExit[room] = true;
                nbExits++;
                Console.WriteLine("add exit" + room.ToString());
            }
        }
        Console.WriteLine("*** exit number" + nbExits.ToString());

        bool hasChanged = true;
        while (hasChanged)
        {
            hasChanged = false;
            for (int room = 0; room < neighbour.Length; ++room)
            {
                if (hasPrisonner[room])
                    continue;
                else if (neighbour[room].Count == 0)
                {
                    if (isExit[room])
                    {
                        isExit[room] = false;
                        nbExits--;
                        Console.WriteLine("delete exit" + room.ToString());
                    }
                    continue;
                }
                else if (neighbour[room].Count == 1)
                {
                    int room2 = neighbour[room][0];
                    if (!hasPrisonner[room2])
                    {
                        isExit[room] = false;
                        neighbour[room].Clear();
                        neighbour[room2].Remove(room);
                        if (isExit[room2])
                        {
                            nbExits--;
                            Console.WriteLine("delete exit " + room.ToString());
                        }
                        else
                        {
                            isExit[room2] = true;
                        }
                        hasChanged = true;
                    }
                }
                else if (neighbour[room].Count == 2)
                {
                    int roomLeft = neighbour[room][0];
                    int roomRight = neighbour[room][1];
                    if (!isExit[room] ||
                        (isExit[roomLeft] && isExit[roomRight])
                        )
                    {
                        if (isExit[room])
                        {
                            isExit[room] = false;
                            nbExits--;
                            Console.WriteLine("delete exit" + room.ToString());
                        }
                        
                        neighbour[room].Clear();
                        neighbour[roomLeft].Remove(room);
                        neighbour[roomRight].Remove(room);
                        neighbour[roomLeft].Add(roomRight);
                        neighbour[roomRight].Add(roomLeft);
                        Console.WriteLine("link " + roomLeft.ToString() + roomRight.ToString());
                        hasChanged = true;
                    }
                }
                else if (isExit[room])
                {
                    bool nonExitNeighbour = false;
                    foreach (var neighbourRoom in neighbour[room])
                    {
                        if (!isExit[neighbourRoom])
                        {
                            nonExitNeighbour = true;
                            break;
                        }
                    }

                    if (!nonExitNeighbour)
                    {
                        Console.WriteLine("delete exit surrounded by exits" + room.ToString());
                        foreach (var neighbourRoom in neighbour[room])
                        {
                            neighbour[neighbourRoom].Remove(room);
                            Console.WriteLine("  --> " + neighbourRoom.ToString());
                        }
                        neighbour[room].Clear();
                        nbExits--;
                        hasChanged = true;

                    }
                }
            }
        }

        representation = new PrisonRepresentation
        {
            hasPrisonner = hasPrisonner,
            isExit = isExit,
            nbRooms = nbRooms,
            neighbour = neighbour

        };

        return nbExits;
    }
}

