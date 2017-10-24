using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class TileDictionary {

	[SerializeField] List<string> keys;
	[SerializeField] List<string> values;

	public TileDictionary() {
		keys = new List<string> ();
		values = new List<string> ();
	}
	public TileDictionary(List<string> keys, List<string> values) {
		this.keys = new List<string>(keys);
		this.values = new List<string>(values);
	}

	public void Remove(string key) {
		for (int i = 0, len = keys.Count; i < len; i++) {
			if (keys [i] == key) {
				keys.RemoveAt (i);
				values.RemoveAt (i);
				return;
			}
		}
	}
	public void Add(string key, string value) {
		int ind = _Get (key);

		if (ind == -1) values.Add (value);
		else values [ind] = value;
	}
	public string Get(string key) {
		int ind = _Get (key);

		return ind == -1 ? null : values[ind];
	}

	int _Get(string key) {
		for (int i = 0, len = keys.Count; i < len; i++) {
			if (keys [i] == key) {
				return i;
			}
		}

		return -1;
	}

	public TileDictionary Clone() {
		return new TileDictionary (keys, values);
	}
}

