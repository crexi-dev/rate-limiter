using System.Collections.Generic;

namespace RateLimiter.HistoryInspectors
{
	public static class Combination
	{
		public static IHistoryInspector<T> And<T>(IHistoryInspector<T> a, IHistoryInspector<T> b) =>
			new HistoryInspectorBuilder<T>(history => a.IsRateLimited(history) && b.IsRateLimited(history));

		public static IHistoryInspector<T> Or<T>(IHistoryInspector<T> a, IHistoryInspector<T> b) =>
			new HistoryInspectorBuilder<T>(history => a.IsRateLimited(history) || b.IsRateLimited(history));

		public static IHistoryInspector<T> Not<T>(IHistoryInspector<T> inspector) =>
			new HistoryInspectorBuilder<T>(history => !inspector.IsRateLimited(history));


		#region HistoryInspectorBuilder

		private delegate bool Inspector<T>(IEnumerable<IApiRequest<T>> history);

		private class HistoryInspectorBuilder<T> : IHistoryInspector<T>
		{
			private readonly Inspector<T> _inspector;

			public HistoryInspectorBuilder(Inspector<T> inspector)
			{
				_inspector = inspector;
			}

			public bool IsRateLimited(IEnumerable<IApiRequest<T>> history) =>
				_inspector.Invoke(history);
		}

		#endregion HistoryInspectorBuilder
	}
}
