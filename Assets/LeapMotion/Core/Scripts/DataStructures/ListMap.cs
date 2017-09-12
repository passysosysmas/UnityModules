using System;
using System.Collections.Generic;

namespace Leap.Unity {
  using Query;

  public class ListMap<Key, Value> {

    public List<Value> list;
    public Dictionary<Key, int> map;

    public ListMap() {
      list = new List<Value>();
      map = new Dictionary<Key, int>();
    }

    public ListMap(int capacity) {
      list = new List<Value>(capacity);
      map = new Dictionary<Key, int>(capacity);
    }

    public int Count {
      get {
        return list.Count;
      }
    }

    public void Clear() {
      list.Clear();
      map.Clear();
    }

    public int Insert(Key key, Value value) {
      if (map.ContainsKey(key)) {
        throw new InvalidOperationException();
      }
      int index = list.Count;
      
      map[key] = index;
      list.Add(value);
      return index;
    }

    public bool Remove(Key key) {
      int index;
      if (!map.TryGetValue(key, out index)) {
        return false;
      }

      list.RemoveAtUnordered(index);
      return true;
    }

    public bool TryGetValue(Key key, out Value value) {
      int index;
      if (!map.TryGetValue(key, out index)) {
        value = default(Value);
        return false;
      }

      value = list[index];
      return true;
    }
  }
}
