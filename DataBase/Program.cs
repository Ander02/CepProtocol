using System;
using System.Collections.Generic;

namespace DataBase
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            String FilePath = AppDomain.CurrentDomain.BaseDirectory;// "C:\\Users\\Gu\\Desktop\\Gustavo\\USP\\6 semestre\\Redes\\EP\\CepProtocol\\DataBase\\UsersTable.txt";
            String Header= "UserID;UserName;Password;";
            DBWriter DB = new DBWriter(FilePath,Header);
            String [] x= {"user2","jonas","password"};
            DB.InsertLine(x);
            //DB.ShowBuffer();
            Console.WriteLine(DB.GetColumIndex("Cep"));
            List<List<String>> c= DB.GetLines("UserId","user2");
            showList(c);
        }

        static void showList(List<List<String>> l){
            foreach (List<String> line in l){
                foreach (String value in line){
                    Console.Write(value+" ");
                }                        
                Console.WriteLine();
            }
        }
    }

}
