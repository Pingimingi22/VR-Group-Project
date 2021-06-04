using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class FileManager
{
	public static void Save()
	{

		// Only want to save if they have a score greater than the highscore.
		if (GameManager.m_currentScore > GameManager.m_highScore)
		{ 
			string destination = Application.persistentDataPath + "/save.dat";
			FileStream file;

			if (File.Exists(destination))
			{
				file = File.OpenWrite(destination);
			}
			else
			{
				file = File.Create(destination);
				File.OpenWrite(destination);
			}


			// Clearing all previous contents.
			//File.WriteAllText(destination, string.Empty);
			
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(file, GameManager.m_currentScore);

			Debug.Log("Saved game score.");

			file.Close();
		}


	}

	public static void Load()
	{
		string destination = Application.persistentDataPath + "/save.dat";
		FileStream file;
		
		if (File.Exists(destination))
		{
			file = File.OpenRead(destination);
		}
		else
		{
			file = File.Create(destination);
		}
		
		BinaryFormatter bf = new BinaryFormatter();

		try
		{
			GameManager.m_highScore = (int)bf.Deserialize(file);
		}
		catch
		{
			file.Close();
			return;
		}
		
		file.Close();
	}
	 
		
	

   
}
