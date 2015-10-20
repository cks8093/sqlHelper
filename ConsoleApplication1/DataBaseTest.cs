using NBWare.CMS.Framework.SqlHelper;
using System;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class DataBaseTest
    {
        public void MultiTransationTest()
        {
            using (MultiDLManager dl = new MultiDLManager())
            {
                Console.WriteLine(dl.multiTest());
            }
        }

        public void OutputTest()
        {
            using (MultiDLManager dl = new MultiDLManager())
            {
                Console.WriteLine(dl.outputTest1());
                Console.WriteLine(dl.outputTest2());
            }
        }

        public void PerformanceTest()
        {
            string s = DateTime.Now.ToString("mm:ss:ms");

            Random ra = new Random();
            Parallel.For(0, 500, index =>
            {
                int a = ra.Next(1, 4);
                switch (a)
                {
                    case 1: cms(index);
                        break;

                    case 2: mng(index);
                        break;

                    case 3: mtv(index);
                        break;

                    default: cms(index);
                        break;
                }
            }); 
            
            Console.WriteLine(s);
            Console.WriteLine(DateTime.Now.ToString("mm:ss:ms"));
        }

        private static void mtv(long i)
        {
            using (VodDLManager dl = new VodDLManager(DBNameType.MTV))
            {
                var result = dl.API1();
                var result1 = dl.API2();
            }
            Console.WriteLine(string.Format("MTV_{0}_{1}", i, DateTime.Now.ToString("mm:ss:ms")));
        }

        private static void mng(long i)
        {
            using (VodDLManager dl = new VodDLManager(DBNameType.MNG))
            {
                var result = dl.MNG1();
                var result1 = dl.MNG2();
            }
            Console.WriteLine(string.Format("MNG_{0}_{1}", i, DateTime.Now.ToString("mm:ss:ms")));
        }

        private static void cms(long i)
        {
            using (VodDLManager dl = new VodDLManager())
            {
                var result = dl.CMS1();
                var result1 = dl.CMS2();
            }
            Console.WriteLine(string.Format("CMS_{0}_{1}", i, DateTime.Now.ToString("mm:ss:ms")));
        }
    }
}
