using System;
using System.Collections.Generic;

namespace socks5.Model
{
	public class LRUCache<K, V>
	{
		protected Dictionary<K, V> _store = new Dictionary<K, V>();

		protected Dictionary<K, DateTime> _key_2_time = new Dictionary<K, DateTime>();

		protected Dictionary<DateTime, K> _time_2_key = new Dictionary<DateTime, K>();

		protected object _lock = new object();

		protected int _sweep_time;

		public LRUCache(int sweep_time = 3600)
		{
			this._sweep_time = sweep_time;
		}

		public void SetTimeout(int time)
		{
			this._sweep_time = time;
		}

		public bool isTimeout(K key)
		{
			object @lock = this._lock;
			bool result;
			lock (@lock)
			{
				if ((DateTime.Now - this._key_2_time[key]).TotalSeconds > (double)this._sweep_time)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool ContainsKey(K key)
		{
			object @lock = this._lock;
			bool result;
			lock (@lock)
			{
				result = this._store.ContainsKey(key);
			}
			return result;
		}

		public V Get(K key)
		{
			object @lock = this._lock;
			V result;
			lock (@lock)
			{
				if (this._store.ContainsKey(key))
				{
					DateTime dateTime = this._key_2_time[key];
					this._key_2_time.Remove(key);
					this._time_2_key.Remove(dateTime);
					dateTime = DateTime.Now;
					while (this._time_2_key.ContainsKey(dateTime))
					{
						dateTime = dateTime.AddTicks(1L);
					}
					this._time_2_key[dateTime] = key;
					this._key_2_time[key] = dateTime;
					result = this._store[key];
				}
				else
				{
					result = default(V);
				}
			}
			return result;
		}

		public V Set(K key, V val)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				DateTime dateTime;
				if (this._store.ContainsKey(key))
				{
					dateTime = this._key_2_time[key];
					this._key_2_time.Remove(key);
					this._time_2_key.Remove(dateTime);
				}
				dateTime = DateTime.Now;
				while (this._time_2_key.ContainsKey(dateTime))
				{
					dateTime = dateTime.AddTicks(1L);
				}
				this._time_2_key[dateTime] = key;
				this._key_2_time[key] = dateTime;
				this._store[key] = val;
			}
			return val;
		}

		public void Del(K key)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				if (this._store.ContainsKey(key))
				{
					DateTime key2 = this._key_2_time[key];
					this._key_2_time.Remove(key);
					this._time_2_key.Remove(key2);
					this._store.Remove(key);
				}
			}
		}

		public void Sweep()
		{
			object @lock = this._lock;
			lock (@lock)
			{
				DateTime now = DateTime.Now;
				int num = 0;
				for (int i = 0; i < 100; i++)
				{
					bool flag2 = false;
					using (Dictionary<DateTime, K>.Enumerator enumerator = this._time_2_key.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							KeyValuePair<DateTime, K> current = enumerator.Current;
							if ((now - current.Key).TotalSeconds < (double)this._sweep_time)
							{
								flag2 = true;
							}
							else
							{
								this._key_2_time.Remove(current.Value);
								this._time_2_key.Remove(current.Key);
								this._store.Remove(current.Value);
								num++;
							}
						}
					}
					if (flag2)
					{
						break;
					}
				}
			}
		}
	}
}
