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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int[] A = new int[] { 0, 1, 2, 3, 3, 2, 6, 6 };
            int[] B = new int[] { 1, 2, 3, 4, 5, 6, 8, 7 };
            int[] C = new int[] { 1, 6 };
            Solution sol = new Solution();
            sol.solution(A, B, C);
        }
    }
}
