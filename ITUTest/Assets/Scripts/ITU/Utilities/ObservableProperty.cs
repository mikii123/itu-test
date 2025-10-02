using System;

namespace ITU.Utilities
{
	public class ObservableProperty<T>
	{
		public T Value { get; private set; }
		public event Action<T> OnChange;

		public ObservableProperty()
		{ }

		public ObservableProperty(T value)
		{
			Value = value;
		}

		public void Set(T value)
		{
			if (Value.Equals(value) || ReferenceEquals(Value, value))
			{
				return;
			}

			Value = value;
			OnChange?.Invoke(value);
		}
	}
}
