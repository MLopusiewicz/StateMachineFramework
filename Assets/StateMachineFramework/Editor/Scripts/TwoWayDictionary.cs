using System;
using System.Collections.Generic;

public class TwoWayDictionary<K, V> {
    public Dictionary<K, V> forward = new();
    public Dictionary<V, K> backward = new();

    public void Add(K key, V value) {
        forward.Add(key, value);
        backward.Add(value, key);
    }

    public void Add(V key, K value) {
        backward.Add(key, value);
        forward.Add(value, key);
    }
    public void Remove(K key) {
        backward.Remove(forward[key]);
        forward.Remove(key);
    }
     
    public void Remove(V key) {
        forward.Remove(backward[key]);
        backward.Remove(key);
    }

    internal void Clear() {

        forward.Clear();
        backward.Clear();
    }

    public V this[K key] {
        get => forward[key];
        set {
            forward[key] = value;
            backward[value] = key;
        }
    }
    public K this[V key] {
        get => backward[key];
        set {
            backward[key] = value;
            forward[value] = key;
        }
    }
}