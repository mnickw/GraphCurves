using System.Collections.Generic;
using System.Linq;

namespace yield
{
	public static class ExpSmoothingTask
	{
		public static IEnumerable<DataPoint> SmoothExponentialy(this IEnumerable<DataPoint> data, double alpha)
		{
			bool isFirstElement = true;
			double lastDataPoint = 0;
			foreach (DataPoint dataPoint in data)
			{
				if (isFirstElement)
				{
					lastDataPoint = dataPoint.OriginalY;
					isFirstElement = false;
				}
				else
					lastDataPoint = alpha * dataPoint.OriginalY + (1 - alpha) * lastDataPoint;
				var newDataPoint = dataPoint.WithExpSmoothedY(lastDataPoint);
				yield return newDataPoint;
			}
		}
	}
}