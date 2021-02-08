using System;
using System.Collections.Generic;
using System.Linq;

namespace yield
{
	public static class MovingMaxTask
	{
		public static IEnumerable<DataPoint> MovingMax(this IEnumerable<DataPoint> data, int windowWidth)
		{
			if (windowWidth <= 0)
				throw new System.ArgumentOutOfRangeException();
			int i = 1;
			LinkedList<double> maxPotentials = new LinkedList<double>();
			Queue<double> windowNumbers = new Queue<double>();
			foreach (DataPoint dataPoint in data)
			{
				if (i <= windowWidth)
					i++;
				else if (maxPotentials.Count == 0)
					windowNumbers.Dequeue();
				else if (maxPotentials.First.Value == windowNumbers.Dequeue())
					maxPotentials.RemoveFirst();
				windowNumbers.Enqueue(dataPoint.OriginalY);
				while (maxPotentials.Count > 0 && maxPotentials.Last.Value <= dataPoint.OriginalY)
					maxPotentials.RemoveLast();
				maxPotentials.AddLast(dataPoint.OriginalY);
				var newDataPoint = dataPoint.WithMaxY(maxPotentials.First.Value);
				yield return newDataPoint;
			}
		}
	}
}