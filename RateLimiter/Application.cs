using Unity;

namespace RateLimiter
{
	public static class Application
	{
		private static IUnityContainer _unityContainer;

		public static void Initialize()
		{
			_unityContainer = new UnityContainer();
		}

		public static IUnityContainer UnityContainer { get { return _unityContainer; } }
	}
}
