using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OTTER
{
    public partial class Nova : Form
    {
        public Nova()
        {
            InitializeComponent();
        }

        private void Nova_Load(object sender, EventArgs e)
        {

        }
        




        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            BGL bgl = new BGL();
            bgl.ShowDialog();
            BGL.allSprites.Clear();
            bgl.Dispose();
            this.Close();
        }
    }
}
