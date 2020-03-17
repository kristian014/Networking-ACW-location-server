using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace locationserver
{
    
    public partial class locationserverwindowsform : Form
    {
      
      public int m_gettimeout = 1000;
        public int m_settimeout = 1000;
        public locationserverwindowsform()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

           new Thread(Program.RunServer).Start();
            Program.readtimeout = m_gettimeout;
            Program.writetimeout = m_settimeout;


            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
          // m_settimeout = int.Parse(textBox2.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
          //m_gettimeout = int.Parse(textBox1.Text);
        }

        private void label2_Click(object sender, EventArgs e)
        {
           
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            string g = numericUpDown1.Value.ToString();
            m_gettimeout = int.Parse(g);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            string s = numericUpDown2.Value.ToString();
            m_settimeout = int.Parse(s);
        }
    }
}
