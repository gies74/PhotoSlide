using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoSlide
{
    public partial class Form1 : Form
    {
        Form2 _pForm = new Form2();
        List<string> _names = null;
        int _nameIdx = 0;
        Timer _timer = new Timer();

        const int INTERVAL = 10; // seconden
        const string PORTRET  = @"DSC_9321.JPG";
        const string DEFAULT_DIRECTORY = @"C:\Data\Prive\Gerda";


        public Form1()
        {
            InitializeComponent();
            _timer.Interval = INTERVAL * 1000;
            _timer.Tick += _timer_Tick;
            _pForm.Parent = this;
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_names == null || _names.Count == 0)
            {
                return;
            }
            
            ShowImage(_names[_nameIdx]);
            do
            {
                _nameIdx = (_nameIdx + 1) % _names.Count;
            } while (_names[_nameIdx] == textBox2.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            ShowImage(textBox2.Text);
        }

        private void ShowImage(string imgpath)
        {
            if (!_pForm.Visible)
            {
                if (_pForm.IsDisposed)
                {
                    _pForm = new Form2();
                    _pForm.Parent = this;
                }
                _pForm.Show();
                _pForm.WindowState = FormWindowState.Maximized;
            }
            _pForm.SetImage(imgpath);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Directory.Exists(DEFAULT_DIRECTORY))
            {
                folderBrowserDialog1.SelectedPath = DEFAULT_DIRECTORY;
            } else
            {
                folderBrowserDialog1.SelectedPath = Environment.GetFolderPath(folderBrowserDialog1.RootFolder);
            }
            textBox1.Text = folderBrowserDialog1.SelectedPath;
            _names = LoadRandomPaths();
            openFileDialog1.Filter = "JPG|*.jpg";
            openFileDialog1.InitialDirectory = textBox1.Text;
            string d = Path.Combine(textBox1.Text, PORTRET);
            if (!File.Exists(d)) {
                if (_names.Count > 0)
                {
                    d = _names[0];
                } else
                {
                    openFileDialog1.FileName = "";
                    button4_Click(null, null);
                    return;
                }
            }
            openFileDialog1.FileName = textBox2.Text = d;
        }

        private List<string> LoadRandomPaths()
        {
            DirectoryInfo di = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
            var names = di.GetFiles("*.jpg").Select(fi => fi.FullName).ToList();
            names.Shuffle();
            return names;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            _timer.Start();
            _timer_Tick(null, null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            textBox1.Text = folderBrowserDialog1.SelectedPath;
            _names = LoadRandomPaths();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) {
                return;
            }
            textBox2.Text = openFileDialog1.FileName;
        }
    }
}
