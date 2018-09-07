using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoTrade.Strategy;
using AutoTrade.DTO;
using AutoTrade.FeedProvider;
using System.Threading;
using Unity;

namespace AutoTrade.Tests
{
    [TestClass]
    public class AutoTradeTests

    {
        private Mock<IStrategy> strategy1;
        private Mock<IStrategy> strategy2;
        private Tick tick1;
        private Tick tick2;
        private string symbol1;
        private string symbol2;
        private DataFeed dataFeed;

        [TestInitialize]
        public void TestInitialize()
        {
            symbol1 = "INFY";
            symbol2 = "WIPRO";
            tick1 = new Tick(1, symbol1, 0.1, DateTime.Now);
            tick2 = new Tick(2, symbol2, 2.1, DateTime.Now);

            strategy1 = new Mock<IStrategy>();
            strategy1.Setup(s => s.Name).Returns("strategy1");
            strategy1.Setup(s => s.Symbol).Returns(symbol1);
            strategy1.Setup(s => s.OnTick(tick1)).Verifiable(); // wont work for me as feed wont generate tick 1
            strategy1.Setup(s => s.OnTick(It.IsAny<Tick>())).Verifiable(); // throwing mock exception
            strategy1.Setup(s => s.OnTick(It.IsNotNull<Tick>())).Verifiable(); // throwing mock exception

            strategy2 = new Mock<IStrategy>();
            strategy2.Setup(s => s.Name).Returns("strategy2");
            strategy2.Setup(s => s.Symbol).Returns(symbol2);
            strategy2.Setup(s => s.OnTick(tick2)).Verifiable();


            IUnityContainer container = new UnityContainer();            
            container.RegisterType<IFeedProvider, CsvFeedProvider>();           
            dataFeed = new DataFeed(container.Resolve<IFeedProvider>());
        }

        [TestMethod]
        public void SingleValidStrategy()
        {
            // subscribe with single strategy
            dataFeed.Subscribe(strategy1.Object);

            // verify if on tick was called  on strategy          
            strategy1.Verify(s => It.IsAny<Tick>(),  Times.AtLeastOnce());// not working

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), null)]
        public void SingleInValidSymbolStrategy()
        {
            // set symbol null
            strategy1.Setup(s => s.Symbol).Returns(default(string));

            // subscribe strategy. Should throw exception
            dataFeed.Subscribe(strategy1.Object);

            //strategy1.Setup(s => s.Symbol).Returns(symbol1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), null)]
        public void SingleInValidNameStrategy()
        {
            // set symbol null
            strategy1.Setup(s => s.Name).Returns(default(string));

            // subscribe strategy. Should throw exception
            dataFeed.Subscribe(strategy1.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), null)]
        public void DuplicateStrategy()
        {
            // subscribe with  strategy
            dataFeed.Subscribe(strategy1.Object);

            // subscribe with same strategy again
            // shoud not throw exception, ignore resubscription attempt
            dataFeed.Subscribe(strategy1.Object);

            // verify if on tick was called  on strategy without any exception , but should be called only once.     
            strategy1.Verify(s => s.OnTick(tick1), Times.Once());
        }

        [TestMethod]
        public void MultipleStrategySameSymbol()
        {
            // set both strategy symbol with to return same symbol
            strategy2.Setup(s => s.Symbol).Returns(symbol1);

            // subscribe with multiple strategies
            dataFeed.Subscribe(strategy1.Object);
            dataFeed.Subscribe(strategy2.Object);

            // verify Ontick was called on all subscribed strategies
            strategy1.Verify();
            strategy2.Verify();
        }

        [TestMethod]
        public void MultipleStrategyDifferentSymbol()
        {
            // subscribe with multiple strategies
            dataFeed.Subscribe(strategy1.Object);
            dataFeed.Subscribe(strategy2.Object);

            // verify Ontick was called on all subscribed strategies
            strategy1.Verify();
            strategy2.Verify();
        }

        [TestMethod]
        public void SubscribeWithDelay()
        {
            Thread.Sleep(2000);
            dataFeed.Subscribe(strategy1.Object);
            strategy1.Verify();
        }



        [TestMethod]
        public void SubscrbieToInvalidCompany()
        {
            // change symbol to invalid value
            strategy1.Setup(s => s.Symbol).Returns("invalidSymbol");

            dataFeed.Subscribe(strategy1.Object);

            // verify if on tick was never called            
            strategy1.Verify(s => s.OnTick(tick1), Times.Never());
        }
        

        [TestMethod]
        public void SubscribeWithMultipleThreads()
        {
            var thread1 = new Thread(() => dataFeed.Subscribe(strategy1.Object));
            var thread2 = new Thread(() => dataFeed.Subscribe(strategy2.Object));

            // subscribe with stragies in different threads
            thread1.Start();            
            thread2.Start();

            Thread.Sleep(100);

            strategy1.Verify();
            strategy2.Verify();
        }
    }
}
