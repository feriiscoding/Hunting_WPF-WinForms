using Hunting.Model;
using Hunting.Persistence;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Moq;
using System;
using System.Data;

namespace Hunting.Test
{
    [TestClass]
    public class HuntingUnitTest
    {
        private Mock<IPersistence> _mock = null!;
        private HuntingModel _model = null!;

        [TestInitialize]
        public void Initialize()
        {
            _mock = new Mock<IPersistence>();
            _mock.Setup(mock => mock.Load(It.IsAny<string>(), out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns((string path, int turns, string who) =>
            {
                turns = 3; 
                who = "h";
                return new Player();
            });

            _model = new HuntingModel(_mock.Object,3);
        }

        [TestMethod]
        public void HuntingConstructorTest()
        {
            _model.NewGame();
            Assert.AreEqual(3, _model.TableSize);
            Assert.AreEqual(1, _model.Table.Prey.Count);
            Assert.AreEqual(4,_model.Table.Hunters.Count);
            Assert.AreEqual(4,_model.Table.Empty.Count);
            int[] array = new int[2];
            array[0] = 1; array[1] = 1;
            Assert.AreEqual(array[0], _model.Table.Prey[0][0]);
            Assert.AreEqual(array[1], _model.Table.Prey[0][1]);
            bool contains = false;
            for(int i = 0; i < 2; i+=2)
            {
                for(int j = 0; j < 2; j+=2)
                {
                    array[0] = i; array[1] = j;
                    foreach (int[] coordinate in _model.Table.Hunters)
                    {
                        if (coordinate[0] == array[0] && coordinate[1] == array[1])
                        {
                            contains = true;
                            break;
                        }
                    }
                    if (!contains) { break; }
                }
                if (!contains) { break; }
            }
            Assert.IsTrue(contains);
            contains = false;
            array[0] = 0; array[1] = 1;
            int[] arr = new int[2];
            arr[0] = 1; arr[1] = 2;
            Assert.AreEqual(array[0], _model.Table.Empty[0][0]);
            Assert.AreEqual(array[1], _model.Table.Empty[0][1]);
            Assert.AreEqual(array[0], _model.Table.Empty[1][1]);
            Assert.AreEqual(array[1], _model.Table.Empty[1][0]);
            array[0] = 1; array[1] = 2;
            Assert.AreEqual(array[0], _model.Table.Empty[2][0]);
            Assert.AreEqual(array[1], _model.Table.Empty[2][1]);
            Assert.AreEqual(array[0], _model.Table.Empty[3][1]);
            Assert.AreEqual(array[1], _model.Table.Empty[3][0]);
        }
        
        [TestMethod]
        public void HuntingSelectNextPlayerTest()
        {
            _model.NewGame();
            if (_model.CurrentPlayer == _model.Table.Hunters)
            {
                int[] array = new int[2];
                array = _model.SelectNextPlayer(0, 0);
                Assert.AreEqual(0, array[0]);
                Assert.AreEqual(0, array[1]);
            }
            else
            {
                int[] array = new int[2];
                array = _model.SelectNextPlayer(1, 1);
                Assert.AreEqual(1, array[0]);
                Assert.AreEqual(1, array[1]);
            }
            _model.NewGame();
            try
            {
                _model.SelectNextPlayer(0, 1);
                Assert.Fail();
            }
            catch (InvalidOperationException) { }
            
        }

        [TestMethod]
        public void HuntingStep()
        {
            _model.NewGame();
            if (_model.CurrentPlayer == _model.Table.Hunters)
            {
                int[] array = _model.SelectNextPlayer(0, 0);
                _model.Step(array, 0, 1);
                Assert.AreEqual(_model.CurrentPlayer, _model.Table.Prey);
                Assert.AreEqual(array[0], _model.Table.Empty[3][0]);
                Assert.AreEqual(array[1], _model.Table.Empty[3][1]);
                array[0] = 0; array[1] = 1;
                Assert.AreEqual(array[0], _model.Table.Hunters[3][0]);
                Assert.AreEqual(array[1], _model.Table.Hunters[3][1]);
            }
            else
            {
                int[] array = _model.SelectNextPlayer(1, 1);
                _model.Step(array, 0, 1);
                Assert.AreEqual(_model.CurrentPlayer, _model.Table.Hunters);
                Assert.AreEqual(array[0], _model.Table.Empty[3][0]);
                Assert.AreEqual(array[1], _model.Table.Empty[3][1]);
                array[0] = 0; array[1] = 1;
                Assert.AreEqual(array[0], _model.Table.Prey[0][0]);
                Assert.AreEqual(array[1], _model.Table.Prey[0][1]);
            }
            _model.NewGame();
            if (_model.CurrentPlayer == _model.Table.Hunters)
            {
                int[] arr = _model.SelectNextPlayer(0, 0);
                try
                {
                    _model.Step(arr, 0, 2);
                    Assert.Fail();
                }
                catch (ArgumentOutOfRangeException) { }
            }
            else
            {
                int[] arr = _model.SelectNextPlayer(1, 1);
                try
                {
                    _model.Step(arr, 0, 2);
                    Assert.Fail();
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }

        [TestMethod]
        public void HuntingLoadTest()
        {
            _model.NewGame();
            if (_model.CurrentPlayer == _model.Table.Hunters)
            {
                int[] array = new int[2];
                array = _model.SelectNextPlayer(0, 0);
                _model.Step(array, 0, 1);
                array = _model.SelectNextPlayer(1, 1);
                _model.Step(array, 1, 0);
                array = _model.SelectNextPlayer(2, 2);
                _model.Step(array, 1, 2);
                array = _model.SelectNextPlayer(1, 0);
                _model.Step(array, 0, 0);
                try
                {
                    _model.Load(string.Empty);
                    Assert.Fail();
                }
                catch (DataException) { }
            }
        }

        [TestMethod]
        public void HuntingSaveTest()
        {
            _model.NewGame();
            List<int[]> currentPlayer = _model.CurrentPlayer;
            int stepCount = _model.TurnCount;

            _model.Save(String.Empty);
            Assert.AreEqual(currentPlayer, _model.CurrentPlayer);
            Assert.AreEqual(stepCount, _model.TurnCount);
        }

        [TestMethod]
        public void HuntingCheckIfOverTest()
        {
            bool eventRaised = false;
            _model.GameWon += delegate (object? sender, GameWonEventArgs e)
            {
                eventRaised = true;
                Assert.IsTrue(e.player == _model.Table.Hunters); 
            };
            _model.NewGame();
            if (_model.CurrentPlayer == _model.Table.Hunters)
            {
                int[] array = new int[2];
                array = _model.SelectNextPlayer(0, 0);
                _model.Step(array, 0, 1);
                array = _model.SelectNextPlayer(1, 1);
                _model.Step(array, 1, 2);
                array = _model.SelectNextPlayer(0, 1);
                _model.Step(array, 1, 1);
            }
            else
            {
                int[] array = new int[2];
                array = _model.SelectNextPlayer(1, 1);
                _model.Step(array, 0, 1);
                array = _model.SelectNextPlayer(2, 0);
                _model.Step(array, 1, 0);
                array = _model.SelectNextPlayer(0, 1);
                _model.Step(array, 1, 1);
                array = _model.SelectNextPlayer(2, 2);
                _model.Step(array, 1, 2);
                array = _model.SelectNextPlayer(1, 1);
                _model.Step(array, 0, 1);
                array = _model.SelectNextPlayer(1, 0);
                _model.Step(array, 1, 1);
            }
            Assert.IsTrue(eventRaised);
            
        }
    }
}