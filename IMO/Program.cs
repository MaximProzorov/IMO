using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*Распределительный центр специализируется на N=5 различных типов товаров.Их поставки осуществляются трейлерами, 
 * которые привозят какую-то одну разновидность товара(определяется случайным образом), объём поставки – 1000 единиц.
 * Изначально на складе находятся по 500 единиц каждого вида товара, ограничений на вместимость склада нет.
 * На разгрузку трейлера тратится 15±5 минут.Единовременно может разгружаться только один трейлер. 
 * Трейлеры приходят в среднем через каждые полчаса, интервалы прибытия распределены по экспоненциальному закону. 
 * Также по экспоненциальному закону, но со средним временем 15 минут поступают заказы от клиентов.
 * В каждом заказе могут содержаться все виды товаров, количество каждого из них определяется в соответствии 
 * с равномерным законом распределения от 0 до 100 штук.Отгрузка товаров начинается только тогда, 
 * когда имеется достаточное количество каждого типа товара.Отгрузка осуществляется в грузовики, 
 * общее число которых составляет M= 20 единиц.Время отгрузки составляет 10±2 минуты.
 * На доставку заказа клиенту они тратят время, распределённое по экспоненциальному закону со средним 2 часа, но не менее 40 минут.
Провести моделирование в течение 8 часов.Определить коэффициент использования автотранспорта. Определить среднее время ожидания комплектации заказов.
*/
namespace IMO
{

    class Program
    {
        static void Main(string[] args)
        {
            int truck = 20;
            Random rand = new Random();
            Queue<int> queueTrailer = new Queue<int>();
            Queue<int[]> queueOrder = new Queue<int[]>();
            List<int> timeTruck = new List<int>();
            int[] product = new int[5];
            for (int i = 0; i < product.Length; i++)
                product[i] = 500;
            int time = 0;
            double randTrailer = rand.NextDouble();
            int timeTrailer = (int)(30.0 * Math.Log(1.0 / (1.0 - randTrailer)));
            int nUseTruck = 0;
            int isComp = 0;
            int timeComp = 0;
            int alltimeComp = 0;
            int timeUnload = 0;
            int nComp = 0;

            double randOrder = rand.NextDouble();
            int timeOrder = (int)(15.0 * Math.Log(1.0 / (1.0 - randOrder)));

            while (time < 480)
            {
                int minTime = 1000;
                while (time == timeTrailer)
                {
                    randTrailer = rand.NextDouble();
                    timeTrailer = time + (int)(30 * Math.Log(1.0 / (1.0 - randTrailer)));
                    int randProduct = rand.Next(1, 5);
                    queueTrailer.Enqueue(randProduct);
                }

                if ((queueTrailer.Count > 0 && time >= timeUnload))
                {
                    product[queueTrailer.Dequeue()] += 1000;
                    int randUnload = rand.Next(10, 20);
                    timeUnload = time + randUnload;

                }

                while (time == timeOrder)
                {
                    int[] productOrder = new int[5];
                    for (int i = 0; i < productOrder.Length; i++)
                    {
                        int randProductOrder = rand.Next(0, 100);
                        productOrder[i] = randProductOrder;
                    }
                    queueOrder.Enqueue(productOrder);
                    randOrder = rand.NextDouble();
                    timeOrder = time + (int)(15.0 * Math.Log(1.0 / (1.0 - randOrder)));

                }

                while (queueOrder.Count > 0 && truck > 0)
                {
                    int[] productOrder = queueOrder.Peek();
                    int flag = 0;
                    for (int i = 0; i < productOrder.Length; i++)
                        if (productOrder[i] > product[i])
                            flag = 1;
                    if (flag == 1)
                    {
                        if (isComp == 0)
                            timeComp = time;
                        isComp = 1;
                        break;
                    }
                    if (isComp == 1)
                    {
                        isComp = 0;
                        nComp++;
                        alltimeComp += (time - timeComp);
                    }
                    for (int i = 0; i < productOrder.Length; i++)
                        product[i] -= productOrder[i];
                    truck--;
                    nUseTruck++;
                    queueOrder.Dequeue();
                    double randTruck = rand.NextDouble();
                    int randLoad = rand.Next(8, 12);
                    int u = (int)(120.0 * Math.Log(1.0 / (1.0 - randTruck)));
                    if (u < 40)
                        u = 40;
                    timeTruck.Add(time + randLoad + u);

                }
                timeTruck.Sort();
                while (timeTruck.Contains(time))
                {
                    timeTruck.Remove(time);
                    truck++;
                }
                if (timeTruck.Count > 0 && timeTruck[0] < minTime)
                    minTime = timeTruck[0];
                if (minTime == 1000)
                    minTime = time + 1;
                time = minTime;
                //  Console.WriteLine(time);
            }
            for (int i = 0; i < 5; i++)
                Console.WriteLine(product[i]);
            Console.ReadKey();
        }
    }
}
