using System.Collections.Generic;

namespace yield
{
	public static class MovingAverageTask
	{
		public static IEnumerable<DataPoint> MovingAverage(this IEnumerable<DataPoint> data, int windowWidth)
		{
			if (windowWidth <= 0)
				throw new System.ArgumentOutOfRangeException();
			Queue<double> lastYs = new Queue<double>();
			double sum = 0;
			double result = 0;
			foreach (var dataPoint in data)
			{
				if (lastYs.Count < windowWidth)
				{
					sum += dataPoint.OriginalY;
					result = sum / (lastYs.Count + 1);
				}
				else
					result += (dataPoint.OriginalY - lastYs.Dequeue()) / windowWidth;
				var newDataPoint = dataPoint.WithAvgSmoothedY(result);
				yield return newDataPoint;
				lastYs.Enqueue(dataPoint.OriginalY);
			}
		}
	}
}