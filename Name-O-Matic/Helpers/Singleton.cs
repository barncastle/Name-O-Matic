namespace NameOMatic.Helpers
{
    internal class Singleton<T> where T : class, new()
    {
        private static volatile T instance;
        private static readonly object syncRoot = new object();

        public static T Instance
        {
            get
            {
                if (instance == null)
                    lock (syncRoot)
                        return instance = instance ?? new T();

                return instance;
            }
        }
    }
}
