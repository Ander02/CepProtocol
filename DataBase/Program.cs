using System;
using DataBase;
using System.Collections.Generic;

namespace DataBase
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            DBWriter DB = new DBWriter("C:\\Users\\Gu\\Desktop\\Gustavo\\USP\\6 semestre\\Redes\\EP\\CepProtocol\\DataBase\\Server_DB.txt");
            String [] x= {"a","1"};
            
            DB.ShowBuffer();
            Console.WriteLine(DB.GetColumIndex("Cep"));
            List<List<String>> c= DB.GetLines("UserIP","b");
            showList(c);
        }

        static void showList(List<List<String>> l){
            foreach (List<String> line in l){
                foreach (String value in line){
                    Console.Write(value);
                }                        
                Console.WriteLine();
            }
        }
    }

}
