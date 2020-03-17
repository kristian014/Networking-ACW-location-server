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

        int m_gettimeout;
        int m_settimeout;
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
          m_gettimeout = int.Parse(textBox1.Text);
        }
    }
}
