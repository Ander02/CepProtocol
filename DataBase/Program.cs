using System;
using System.Collections.Generic;

namespace DataBase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var filePath = $"{AppDomain.CurrentDomain.BaseDirectory}User.txt";
            var headers = "UserID;UserName;Password;";
            var db = new DbWriter(filePath, headers);
            var line = new string[] { "user2", "jonas", "password" };
            db.InsertLine(line);
            //DB.ShowBuffer();
            Console.WriteLine(db.GetColumIndex("Cep"));
            List<List<String>> c = db.GetLines("UserId", "user2");
            ShowList(c);
        }

        private static void ShowList(List<List<String>> l)
        {
            foreach (var line in l)
            {
                foreach (var value in line)
                {
                    Console.Write(value + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
