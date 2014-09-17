using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhosphorusForms
{
    public partial class Form1 : Form
    {
        private BackgroundWorker _bg1;
        public Form1()
        {
            InitializeComponent();
            _bg1 = new BackgroundWorker();
            _bg1.WorkerSupportsCancellation = true;
            _bg1.WorkerReportsProgress = true;
            _bg1.DoWork += DoWorkSolve;
        }

        public void SetPicture(Image img)
        {
            if (pictureBox1.InvokeRequired)
            {
                pictureBox1.Invoke(new MethodInvoker(
                delegate()
                {
                    pictureBox1.Image = img;
                }));
            }
            else
            {
                pictureBox1.Image = img;
            }
        }
        private void DoWorkSolve(object sender, DoWorkEventArgs e)
        {
            int[] A = new int[] { 0, 1, 2, 3, 3, 2, 6, 6 };
            int[] B = new int[] { 1, 2, 3, 4, 5, 6, 8, 7 };
            int[] C = new int[] { 1, 6 };
            Solution sol = new Solution();
            sol.solution(A, B, C);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            _bg1.RunWorkerAsync();
        }
    }
}
