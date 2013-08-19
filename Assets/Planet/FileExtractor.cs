using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


namespace AssemblyCSharp
{
	public class FileExtractor
	{
		private string fName;
		
		private Dictionary<string, float> values;	// Einträge aus der init-Datei
		
		public FileExtractor (string fName)
		{
			this.fName = fName;
			values = new Dictionary<string, float>();
			
			ReadFile();
		}
		
		private void ReadFile()
		{
			string line, line2;
			float val;
			using(StreamReader reader = new StreamReader(fName))
			{
				while(!reader.EndOfStream){
						
					line = reader.ReadLine();
					line2 = line.Substring(line.IndexOf('=') + 2);
					line = line.Remove(line.IndexOf('=') - 1);
		
					val = (float)Double.Parse(line2);
					values.Add(line, val);
				}
			}
			
			//4debug...
			//foreach(KeyValuePair<string, float> pair in values){
			//	Debug.Log(pair.Key + "=" + pair.Value);		
			//}
		}
		
		public string GetFileName()
		{
			return fName;	
		}
		
		public float getValue(string entry)
		{
			float val = -1.0f;
			values.TryGetValue(entry, out val);
			return val;
		}
		
	}
}

