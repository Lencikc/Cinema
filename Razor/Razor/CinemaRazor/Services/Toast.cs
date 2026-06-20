namespace CinemaRazor.Services
{
    public class Toast
    {
        public record Item(int Id, string Text, string Kind);
        private readonly List<Item> _items = new();
        private int _counter;
        public IReadOnlyList<Item> Items => _items;

        public event Action? Changed;

        public void Show(string text, string kind = "info")
        {
            var id = ++_counter;
            _items.Add(new Item(id, text, kind));
            Changed?.Invoke();
            _ = Task.Delay(4000).ContinueWith(_ => Remove(id));
        }

        public void Error(string text) => Show(text, "danger");
        public void Success(string text) => Show(text, "success");
        public void Warn(string text) => Show(text, "warning");

        public void Remove(int id)
        {
            _items.RemoveAll(x => x.Id == id);
            Changed?.Invoke();
        }
    }
}
