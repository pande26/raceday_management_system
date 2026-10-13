using Microsoft.AspNetCore.Http;

namespace raceday_management_system.Tests
{//start of namespace
    public class FakeSession : ISession
    {//start of class

        private readonly Dictionary<string, byte[]> _store = new();

        public string Id => "test_session";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _store.Keys;

        public void Clear() => _store.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _store.Remove(key);
        public void Set(string key, byte[] value) => _store[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value!);

    }//end of class

}//end of namespace