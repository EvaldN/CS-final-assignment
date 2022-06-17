using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalAss
{
    public partial class Form_LoadingScreen : Form
    {
        public Form_LoadingScreen()
        {
            InitializeComponent();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            loadingProgress.Width += 2;
            if(loadingProgress.Width > 200)
            {
                loadingProgress.Width = 0;
            }
        }
    }
}
