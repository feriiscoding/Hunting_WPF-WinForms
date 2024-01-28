using Hunting.Persistence;
using System;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;

namespace Hunting.Model
{
    public class HuntingModel
    {
        #region Private Fields
        private Player _table;
        private List<int[]> _currentPlayer;
        private int size;
        private int allturns;
        private int _turnCount;
        private IPersistence _persistence;
        #endregion

        #region Public Properties
        public Player Table { get { return _table; } }
        public List<int[]> CurrentPlayer { get { return _currentPlayer; } }
        public int TableSize { get { return size; } }
        public int AllTruns { get { return allturns; } }
        public int TurnCount { get { return _turnCount; } }
        #endregion

        #region Events
        public event EventHandler<GameWonEventArgs>? GameWon;
        public event EventHandler? GameOver;
        #endregion

        #region Constructor
        public HuntingModel(IPersistence persistence, int size)
        {
            _table = new Player();
            _currentPlayer = new List<int[]>();
            this.size = size;
            allturns = size*4;
            _turnCount = 1;
            _persistence = persistence;
        }
        #endregion

        #region Public Methods
        public void NewGame()
        {
            _table = new Player();
            _turnCount = 0;
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int[] array = new int[2];
                    array[0] = i;
                    array[1] = j;
                    if (i == 0 && j == 0)
                    {
                        _table.Hunters.Add(array);
                    }
                    else if (i == 0 && j == size - 1)
                    {
                        _table.Hunters.Add(array);
                    }
                    else if (i == size - 1 && j == 0)
                    {
                        _table.Hunters.Add(array);
                    }
                    else if (i == size - 1 && j == size - 1)
                    {
                        _table.Hunters.Add(array);
                    }
                    else if (i == size / 2 && j == size / 2)
                    {
                        _table.Prey.Add(array);
                    }
                    else
                    {
                        _table.Empty.Add(array);
                    }
                }
                if (random.Next(0, 2) == 0)
                {
                    _currentPlayer = _table.Hunters;
                }
                else { _currentPlayer = _table.Prey; }
            }
        }
        
        public int[] SelectNextPlayer(int x, int y) 
        {
            int[] array = new int[2];
            array[0] = x;
            array[1] = y;
            foreach (int[] i in _currentPlayer)
            {
                if (i[0] == array[0] && i[1] == array[1])
                {
                    return array;
                }
            }
            throw new InvalidOperationException("Your player is not on this field!");
            
        }

        public void Step(int[] arr, int x, int y)
        {
            int[] next = new int[2];
            next[0] = x;
            next[1] = y;
            bool contains = false;
            if (x < 0 || x >= TableSize) { throw new ArgumentOutOfRangeException("Bad column index!"); }
            if (y < 0 || y >= TableSize) { throw new ArgumentOutOfRangeException("Bad row index!"); }
            if (_turnCount >= allturns) { throw new InvalidOperationException("The game has ended!"); }
            if(!(x == arr[0] && y == arr[1]+1) && !(x == arr[0] && y == arr[1]-1) && !(x == arr[0]+1 && y == arr[1]) && !(x == arr[0]-1 && y == arr[1])) { throw new ArgumentOutOfRangeException("The field is not next your figure!"); }
            foreach (int[] i in _table.Empty)
            {
                if (i[0] == x && i[1] == y)
                {
                    contains = true;
                    break;
                }
            }
            if (!contains) { throw new InvalidOperationException("The field is not empty!"); }
            _table.Empty.Add(arr);
            foreach (int[] i in _table.Empty)
            {
                if (i[0] == x && i[1] == y)
                {
                    _table.Empty.Remove(i);
                    break;
                }
            }
            if (_currentPlayer == _table.Hunters)
            {
                _table.Hunters.Add(next);
                foreach(int[] i in _table.Hunters)
                {
                    if (i[0] == arr[0] && i[1] == arr[1])
                    {
                        _table.Hunters.Remove(i);
                        break;
                    }
                }
                _currentPlayer = _table.Prey;
            }
            else
            {
                _table.Prey.Add(next);
                foreach (int[] i in _table.Prey)
                {
                    if (i[0] == arr[0] && i[1] == arr[1])
                    {
                        _table.Prey.Remove(i);
                        break;
                    }
                }
                _currentPlayer = _table.Hunters;
            }

            _turnCount++;
            CheckIfOver();
        }

        public void Load(String path)
        {
            Player loadPlayer = _persistence.Load(path, out int turns, out string who);
            int size = (int)Math.Sqrt(loadPlayer.Empty.Count + 5);
            _table = new Player();
            _currentPlayer = new List<int[]>();
            this.size = size;
            allturns = size * 4;
            if (loadPlayer.Empty.Count == 0 || loadPlayer.Hunters.Count == 0 || loadPlayer.Prey.Count == 0 || turns>=size*4 || (who!="h" && who!="p"))
            {
                throw new DataException("The game could not load properly!");
            }
            _turnCount = turns;
            _table.Empty = loadPlayer.Empty;
            _table.Hunters = loadPlayer.Hunters;
            _table.Prey = loadPlayer.Prey;
            if(who == "h") { _currentPlayer = _table.Hunters; }
            else { _currentPlayer = _table.Prey; }
            CheckIfOver();
        }

        public void Save(String path)
        {
            string who = "";
            if(_currentPlayer == _table.Hunters) { who = "h"; }
            else { who = "p"; }
            _persistence.Save(path, _table, _turnCount, who);
        }
        #endregion

        #region Private Methods
        private void CheckIfOver()
        {
            List<int[]> winner = new List<int[]>();
            bool broken = false;
            if (_turnCount >= allturns) { GameIsOver(); }
            else
            {
                foreach (int[] fields in _currentPlayer)
                {
                    int row = fields[0];
                    int col = fields[1];
                    foreach (int[] i in _table.Empty)
                    {
                        if ((i[0] == row && i[1] == col + 1) ||(i[0] == row && i[1] == col - 1) || (i[0] == row+1 && i[1] == col) || (i[0] == row -1 && i[1] == col))
                        {
                            broken = true;
                            break;
                        }
                    }
                    if (broken) break;
                }
                if (!broken)
                {
                    if (_currentPlayer == _table.Hunters)
                    {
                        winner = _table.Prey;
                        Winner(_table.Prey);
                    }
                    else
                    {
                        winner = _table.Hunters;
                        Winner(_table.Hunters);
                    }
                }
            }
        }

        private void Winner(List<int[]>winner)
        {
            GameWon?.Invoke(this, new GameWonEventArgs(winner));
        }
        
        private void GameIsOver()
        {
            GameOver?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}