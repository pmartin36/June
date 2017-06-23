using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class Serializer<T> where T : class
{
	public static void Serialize(T obj, string filename) {
		string file = Path.Combine(Application.persistentDataPath, filename);
		using (FileStream stm = new FileStream(file, FileMode.Create))
		{
			BinaryFormatter fmt = new BinaryFormatter();
			fmt.Serialize(stm, obj);
		}
	}

	public static T Deserialize(string filename) {
		string file = Path.Combine(Application.persistentDataPath, filename);
		Debug.Log("Deserializing File: " + file);
		if(!File.Exists(file)) {
			return default(T);
		}
		using (FileStream stm = new FileStream(file, FileMode.Open))
		{
			BinaryFormatter fmt = new BinaryFormatter();
			return fmt.Deserialize(stm) as T;
		}
	}
}

