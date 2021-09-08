using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CPU_Scheduling
{
    public partial class ChartForm : Form
    {
        private static double ratio;
        private static int labelHeight = 45;

        private static Color[] colors = new[] { Color.Red, Color.Blue, Color.Lime, Color.Magenta, Color.DarkCyan,  Color.DarkViolet, Color.LawnGreen, Color.Yellow};
        private static List<Result> _results = new List<Result>();

        public ChartForm(List<Result> results)
        {
            InitializeComponent();
            _results = results;
            ratio = (double)Width / results.Select(r => r.EndTime).Max();
        }

        private void ChartForm_Load(object sender, EventArgs e)
        {
            foreach (var result in _results)
            {
                var label = new Label();
                label.ForeColor = Color.White;
                label.BackColor = colors[(result.Id) % colors.Length];
                label.AutoSize = false;
                label.Text = $"{result.Name}\n[{result.StartTime}-{result.EndTime}]";
                label.Height = labelHeight;

                label.Width = GetNumber((result.EndTime - result.StartTime) * ratio);
                label.Left = GetNumber(result.StartTime * ratio);
                label.Top = (result.Id) * labelHeight;
                Controls.Add(label);
            }
        }

        private int GetNumber(double d)
        {
            return int.Parse(Math.Round(d).ToString());
        }
    }
}