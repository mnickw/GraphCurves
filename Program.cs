using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ZedGraph;

namespace yield
{
	class Program
	{
		private readonly Form form;
		private readonly ZedGraphControl chart;
		private volatile bool paused;
		private volatile bool canceled;
		private Thread thread;
		private readonly PointPairList originalPoints;
		private readonly PointPairList expPoints;
		private readonly PointPairList avgPoints;
		private readonly PointPairList maxPoints;

		public Program()
		{
			form = new Form
			{
				WindowState = FormWindowState.Maximized,
				Text = "Press any key to pause / resume"
			};
			chart = new ZedGraphControl()
			{
				Dock = DockStyle.Fill
			};
			chart.GraphPane.Title.Text = "Сравнение методов сглаживания";
			chart.GraphPane.XAxis.Title.Text = "X";
			chart.GraphPane.YAxis.Title.Text = "Y";
			chart.GraphPane.XAxis.Scale.MaxAuto = true;
			chart.GraphPane.XAxis.Scale.MinAuto = true;
			chart.GraphPane.YAxis.Scale.MaxAuto = true;
			chart.GraphPane.YAxis.Scale.MinAuto = true;
			originalPoints = new PointPairList();
			var original = chart.GraphPane.AddCurve("original", originalPoints, Color.Black, SymbolType.None);
			original.Line.IsAntiAlias = true;
			avgPoints = AddCurve("avg", Color.Blue);
			expPoints = AddCurve("exp", Color.Red);
			maxPoints = AddCurve("max", Color.Green);
			form.Controls.Add(chart);
			chart.KeyDown += (sender, args) => paused = !paused;
			form.FormClosing += (sender, args) => { canceled = true; };
			form.Shown += OnShown;
		}

		private PointPairList AddCurve(string label, Color color)
		{
			var curve = new PointPairList();
			var line = chart.GraphPane.AddCurve(label, curve, color, SymbolType.None);
			line.Line.Width = 3;
			line.Line.IsAntiAlias = true;
			return curve;
		}

		private void OnShown(object sender, EventArgs e)
		{
			thread = new Thread(() =>
			{
				try
				{
					foreach (var point in DataSource.GetData(new Random()))
					{
						if (canceled) return;
						var pointCopy = point;
						var invokation = form.BeginInvoke((Action)(() => AddPoint(pointCopy)));
						while (paused && !canceled) Thread.Sleep(20);
						Thread.Sleep(20);
						form.EndInvoke(invokation);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
			})
			{ IsBackground = true };
			thread.Start();
		}

		[STAThread]
		static void Main()
		{
			new Program().Run();
		}

		private void Run()
		{
			Application.Run(form);
		}

		private void AddPoint(DataPoint p)
		{
			originalPoints.Add(p.X, p.OriginalY);
			avgPoints.Add(p.X, p.AvgSmoothedY);
			expPoints.Add(p.X, p.ExpSmoothedY);
			maxPoints.Add(p.X, p.MaxY);
			chart.AxisChange();
			chart.Invalidate();
			chart.Refresh();
		}
	}
}
