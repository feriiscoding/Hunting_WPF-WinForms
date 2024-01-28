using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

namespace Hunting.Persistence.Text
{
    public class TextFilePersistence : IPersistence
    {
        public Player Load(String path, out int turnCount, out string who)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    Player player = new Player();
                    String[] fields = reader.ReadLine()!.Split(" ");
                    for (int i = 0; i < fields.Length - 1; i++)
                    {
                        int[] arr = new int[2];
                        String[] coordinates = fields[i].Split(",");
                        arr[0] = int.Parse(coordinates[0]);
                        arr[1] = int.Parse(coordinates[1]);
                        player.Hunters.Add(arr);
                    }
                    fields = reader.ReadLine()!.Split(" ");


                    for (int i = 0; i < fields.Length-1; i++)
                    {
                        int[] arr = new int[2];
                        String[] coordinates = fields[i].Split(",");
                        arr[0] = int.Parse(coordinates[0]);
                        arr[1] = int.Parse(coordinates[1]);
                        player.Prey.Add(arr);
                    }
                    fields = reader.ReadLine()!.Split(" ");


                    for (int i = 0; i < fields.Length-1; i++)
                    {
                        int[] arr = new int[2];
                        String[] coordinates = fields[i].Split(",");
                        arr[0] = int.Parse(coordinates[0]);
                        arr[1] = int.Parse(coordinates[1]);
                        player.Empty.Add(arr);
                    }

                    string[] datas = reader.ReadLine()!.Split(",");
                    turnCount = int.Parse(datas[0]);
                    who = datas[1];
                    return player;
                }
            }
            catch
            {
                throw new DataException("Error occurred during reading.");
            }
        }
        public void Save(String path, Player fields, int turnCount, string who)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {

                    for (int i = 0; i < fields.Hunters.Count; i++)
                    {
                        writer.Write(fields.Hunters[i][0] + "," + fields.Hunters[i][1] + " ");
                    }
                    writer.WriteLine();
                    for (int i = 0; i < fields.Prey.Count; i++)
                    {
                        writer.Write(fields.Prey[i][0] + "," + fields.Prey[i][1] + " ");
                    }
                    writer.WriteLine();
                    for (int i = 0; i < fields.Empty.Count; i++)
                    {
                        writer.Write(fields.Empty[i][0] + "," + fields.Empty[i][1] + " ");
                    }
                    writer.WriteLine();
                    writer.Write(turnCount + "," + who);
                }
            }
            catch
            {
                throw new DataException("Error occurred during writing.");
            }
        }
    }
}