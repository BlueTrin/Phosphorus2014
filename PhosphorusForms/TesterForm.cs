using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PhosphorusForms
{
    public partial class TesterForm : Form
    {
        private BackgroundWorker _bg1;
        public TesterForm()
        {
            InitializeComponent();
            _bg1 = new BackgroundWorker();
            _bg1.WorkerSupportsCancellation = true;
            _bg1.WorkerReportsProgress = true;
            _bg1.DoWork += DoWorkSolve;
        }

        public void SetTB1(string txt)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke(new MethodInvoker(
                delegate()
                {
                    textBox1.Text = txt;
                }));
            }
            else
            {
                textBox1.Text = txt;
            }
        }


        public static void Shuffle<T>(IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        [Serializable]
        public class CodilityArgs
        {
            [XmlAttribute]
            public int[] A;
            [XmlAttribute]
            public int[] B;
            [XmlAttribute]
            public int[] C;
            
        }

        public class TreeNode
        {
            public TreeNode(int id)
            {
                Id = id;
            }
            public int Id;
            public List<TreeNode> Neighbours = new List<TreeNode>();

        }

        public CodilityArgs GenerateRandomArgsByTree(int nbRooms, int nbPrisonnersLeft)
        {
            int seed;
            Random rnd;
            if (!int.TryParse(tbSeed.Text, out seed))
            {
                MessageBox.Show("Invalid seed" + tbSeed.Text);
                rnd = new Random();
            }
            else
            {
                rnd = new Random(seed);
            }
            //int nbRooms = rnd.Next(1000, 10000); // creates a number between 1 and 12
            int currNodeId = 0;
            TreeNode root = new TreeNode(currNodeId++);
            List<TreeNode> toProcess = new List<TreeNode>();
            Dictionary<int, TreeNode> treeNodeById = new Dictionary<int, TreeNode>();
            treeNodeById[0] = root;

            toProcess.Add(root);
            nbRooms--;
            while (currNodeId < nbRooms)
            {
                int processedId = rnd.Next(0, toProcess.Count - 1);
                TreeNode processed = toProcess[processedId];
                toProcess.RemoveAt(processedId);

                int nbChilds = rnd.Next(1, 4);
                for (int childId = 0; childId < nbChilds; ++childId)
                {
                    TreeNode child = new TreeNode(currNodeId);
                    treeNodeById[currNodeId] = child;
                    toProcess.Add(child);
                    processed.Neighbours.Add(child);

                    currNodeId++;
                    if (currNodeId >= nbRooms)
                        break;
                }

            }

            var remainingRooms = Enumerable.Range(1, nbRooms - 1).ToList();
            var prisonnersList = new List<int>();
            //int nbPrisonnersLeft = rnd.Next(1, 1 + nbRooms/ 3);
            while (nbPrisonnersLeft > 0)
            {
                int tryRoom = remainingRooms[rnd.Next(0, remainingRooms.Count-1)];
                if (treeNodeById[tryRoom].Neighbours.Count != 0)
                {
                    prisonnersList.Add(tryRoom);
                    nbPrisonnersLeft--;
                }
            }

            CodilityArgs toRet = new CodilityArgs
            {
                A = new int[nbRooms-1],
                B = new int[nbRooms-1],
                C = new int[prisonnersList.Count]
            };

            int row = 0;
            for (int nodeId = 0; nodeId < nbRooms; ++nodeId)
            {
                TreeNode currNode = treeNodeById[nodeId];
                foreach (TreeNode child in currNode.Neighbours)
                {
                    toRet.A[row] = currNode.Id;
                    toRet.B[row] = child.Id;
                    row++;
                }
            }

            for (int nodeId = 0; nodeId < prisonnersList.Count; ++nodeId)
            {
                toRet.C[nodeId] = prisonnersList[nodeId];
            }

            return toRet;
        }

        public CodilityArgs GenerateRandomArgs()
        {
            Random rnd = new Random();
            int nbRooms = rnd.Next(1, 50); // creates a number between 1 and 12

            List<int> allRooms = new List<int>();
            List<int> randomRooms = new List<int>();
            for (int i = 0; i < 2*(nbRooms - 1); ++i)
            {
                if (i < nbRooms)
                {
                    randomRooms.Add(i);
                    allRooms.Add(i);
                }

                else
                    randomRooms.Add(rnd.Next(0, nbRooms - 1));
            }
            Shuffle(randomRooms);
            var A = randomRooms.Take(nbRooms-1).ToArray();
            var B = randomRooms.Skip(nbRooms-1).Take(nbRooms-1).ToArray();



            int nbPrisonners = rnd.Next(1, Math.Max(1,nbRooms/5)); // creates a number between 1 and 12
            Shuffle(allRooms);
            int[] C = allRooms.Take(nbPrisonners).ToArray();
            CodilityArgs toret = new CodilityArgs
            {
                A = A,
                B = B,
                C = C
            };



   
            return toret;
        }

        private void DoWorkSolve(object sender, DoWorkEventArgs e)
        {
            CodilityArgs c = (CodilityArgs) e.Argument;
            //int[] A = new int[] { 0, 1, 2, 3, 3, 2, 6, 6 };
            //int[] B = new int[] { 1, 2, 3, 4, 5, 6, 8, 7 };
            //int[] C = new int[] { 1, 6 };
            //int[] A = new int[] { 0, 1, 2, 2, 2, 5, 5, 5, 8, 9, 9, 11, 11, 11 };
            //int[] B = new int[] { 1, 2, 3, 4, 5, 6, 7, 9, 9,10,11, 12, 13, 14 };
            //int[] C = new int[] { 5, 11 };

            Solution sol = new Solution();
            Solution.PrisonRepresentation initial = sol.transformRepresentation(c.A, c.B, c.C);
            Solution.PrisonRepresentation? final;

            string newfolder = null;
            if (cbStepbyStep.Checked)
            {
                newfolder = DateTime.Now.ToString(" yyyy-MM-dd-HH-mm-ss-fff");
                if (!Directory.Exists(newfolder))
                    Directory.CreateDirectory(newfolder);
            }

            int nbGuards = sol.solution(c.A, c.B, c.C, out final, newfolder);
            Image pic1 = sol.GenerateGraph(initial.hasPrisonner, initial.neighbour, initial.isExit);
            pic1.Save("initial.png");
            Process.Start("initial.png");
            if (final.HasValue)
            {
                Image pic2 = sol.GenerateGraph(final.Value.hasPrisonner, final.Value.neighbour, final.Value.isExit);
                //SetPicture2(pic2);
                pic2.Save("after.png");
                Process.Start("after.png");
            }
            //SetPicture1(pic1);
            SetTB1(nbGuards.ToString());
        }


        private void button1_Click(object sender, EventArgs e)
        {

            CodilityArgs c = GenerateRandomArgsByTree(Convert.ToInt32(tbNbNodes.Text), Convert.ToInt32(tbNbPrisonners.Text));
            using (var stream = File.Create("lastvalues.xml"))
            {
                var serializer = new XmlSerializer(typeof(CodilityArgs));
                serializer.Serialize(stream, c);
            }
            _bg1.RunWorkerAsync(c);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CodilityArgs c;
            using (var stream = new StreamReader(tbLastValuesFileName.Text))
            {
                var serializer = new XmlSerializer(typeof(CodilityArgs));
                
                c = (CodilityArgs)serializer.Deserialize(stream);
                
            }
            _bg1.RunWorkerAsync(c);
        }

    }
}
