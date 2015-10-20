using System;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTableToList datatabletolist = new DataTableToList();
            datatabletolist.SingleTest(); //DataTable => Object
            datatabletolist.ListTest(); //DataTable => List
            datatabletolist.ListFilterTest(); //DataTable => List, filter포함

            DataBaseTest databasetest = new DataBaseTest();
            databasetest.MultiTransationTest(); //멀티트랜잭션 테스트
            databasetest.OutputTest(); //파라미터 output테스트
            databasetest.PerformanceTest(); //성능테스트
            
            Console.ReadLine();
        }
    }
}
