using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Dots
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            Form1 pvp = new Form1();
            pvp.ShowDialog();
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            Form2 pvc = new Form2();
            pvc.ShowDialog();
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("\t    INSTRUCTION\n" +
                "------------------------------------------\n" +
                "# Opponents take turns putting dots    #\n" +
                "# at the intersection of the sheet lines,  #\n" +
                "# each with its color. The goal of the     #\n" +
                "# game is to surround and capture       #\n" +
                "# more enemy points.                           #\n" +
                "------------------------------------------");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}