using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
using System.Drawing.Text;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;
using System.Drawing;
using Hunting.Model;
using Hunting.Persistence.Text;
using Hunting.Persistence;
using System.Collections.Generic;
using System.Security.Policy;
using System.CodeDom;
using System.Data;

namespace Hunting.View
{
    public partial class HuntingForm : Form
    {
        #region Private Fields
        private Button[,] _buttonGrid;
        private HuntingModel _model;
        private bool _isSelected;
        private int _xSelected;
        private int _ySelected;
        private string tunaTurn;
        private string catTurn;
        private GridButton SelectedPlayer;
        private int[] _selected;
        #endregion

        #region Constructor
        public HuntingForm()
        {
            InitializeComponent();
            _isSelected = false;
            _xSelected = 0;
            _ySelected = 0;
            _buttonGrid = null!;
            _model = new HuntingModel(new TextFilePersistence(), 0);
            tunaTurn = "It's the tuna's turn!";
            catTurn = "It's the cats' turn!";
            SelectedPlayer = null!;
            _model.GameOver += new EventHandler(model_GameOver);
            _model.GameWon += new EventHandler<GameWonEventArgs>(Model_GameWon);
            _selected = new int[2];
        }
        #endregion

        #region Private Methods
        private void NewGame()
        {
            tablePanel.ColumnCount = 0;
            tablePanel.RowCount = 0;
            tablePanel.Visible = false;
            startBtn.Enabled = true;
            comboBox1.Enabled = true;
        }
        private void LoadTable(int size)
        {
            CreateTable(size);
            int i = 0;
            for(int j = 0; j < size; j += size - 1)
            {
                for(int k = 0; k<size; k += size - 1)
                {
                    if (_buttonGrid[j,k] is GridButton btn)
                    {
                        int tempX = btn.GridX;
                        int tempY = btn.GridY;
                        btn._x = _model.Table.Hunters[i][0];
                        btn._y = _model.Table.Hunters[i][1];
                        Control firstControl = tablePanel.GetControlFromPosition(tempY, tempX);
                        Control secondControl = tablePanel.GetControlFromPosition(btn._y, btn._x);
                        if (secondControl is GridButton button)
                        {
                            button._x = tempX;
                            button._y = tempY;
                        }

                        int firstControlRow = tablePanel.GetRow(firstControl);
                        int firstControlCol = tablePanel.GetColumn(firstControl);
                        int secondControlRow = tablePanel.GetRow(secondControl);
                        int secondControlCol = tablePanel.GetColumn(secondControl);
                        tablePanel.SetRow(firstControl, secondControlRow);
                        tablePanel.SetColumn(firstControl, secondControlCol);
                        tablePanel.SetRow(secondControl, firstControlRow);
                        tablePanel.SetColumn(secondControl, firstControlCol);
                    }
                    i++;
                }
            }
            if (_buttonGrid[(size-1)/2, (size-1)/2] is GridButton b)
            {
                int tempX = b.GridX;
                int tempY = b.GridY;
                b._x = _model.Table.Prey[0][0];
                b._y = _model.Table.Prey[0][1];
                Control firstControl = tablePanel.GetControlFromPosition(tempY, tempX);
                Control secondControl = tablePanel.GetControlFromPosition(b._y, b._x);
                if (secondControl is GridButton button)
                {
                    button._x = tempX;
                    button._y = tempY;
                }
                int firstControlRow = tablePanel.GetRow(firstControl);
                int firstControlCol = tablePanel.GetColumn(firstControl);
                int secondControlRow = tablePanel.GetRow(secondControl);
                int secondControlCol = tablePanel.GetColumn(secondControl);
                tablePanel.SetRow(firstControl, secondControlRow);
                tablePanel.SetColumn(firstControl, secondControlCol);
                tablePanel.SetRow(secondControl, firstControlRow);
                tablePanel.SetColumn(secondControl, firstControlCol);
            }
        }
        private void CreateTable(int size)
        {
            tablePanel.Controls.Clear();
            tablePanel.Visible = true;
            tablePanel.RowCount = tablePanel.ColumnCount = size;
            SizeChanged += HuntingForm_SizeChanged;

            _buttonGrid = new Button[size, size];
            int buttonWidth = tablePanel.Width / size - 1;
            int buttonHeight = tablePanel.Height / size - 1;

            for (Int32 i = 0; i < size; i++)
                for (Int32 j = 0; j < size; j++)
                {
                    _buttonGrid[i, j] = new GridButton(i, j);
                    _buttonGrid[i, j].Location = new Point(buttonWidth * i, buttonHeight * j);
                    _buttonGrid[i, j].Size = new Size(buttonWidth, buttonHeight);
                    _buttonGrid[i, j].Font = new Font(FontFamily.GenericSansSerif, Height / 10, FontStyle.Bold);
                    _buttonGrid[i, j].Dock = DockStyle.Fill;
                    _buttonGrid[i, j].BackColor = Color.White;
                    if(i == 0 && j == 0) { _buttonGrid[i, j].BackgroundImage = Hunting.View.Images.cat2; }
                    if(i == 0 && j == size - 1) { _buttonGrid[i, j].BackgroundImage = Hunting.View.Images.cat3; }
                    if (i == size-1 && j == 0) { _buttonGrid[i, j].BackgroundImage = Hunting.View.Images.cat4; }
                    if (i == size-1 && j == size - 1) { _buttonGrid[i, j].BackgroundImage = Hunting.View.Images.cat7; }
                    if (i == size / 2 && j == size /2) { _buttonGrid[i, j].BackgroundImage = Hunting.View.Images.tuna; }
                    _buttonGrid[i, j].BackgroundImageLayout = ImageLayout.Stretch;
                    tablePanel.Controls.Add(_buttonGrid[i, j], j, i);
                }

            tablePanel.RowStyles.Clear();
            tablePanel.ColumnStyles.Clear();

            for (Int32 i = 0; i < size; i++)
            {
                tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 1 / 3F));
            }
            for (Int32 j = 0; j < size; j++)
            {
                tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1 / 3F));
            }
        }
        private void SwitchButtons(int x, int y, int newX, int newY)
        {
            Control firstControl = tablePanel.GetControlFromPosition(y, x);
            Control secondControl = tablePanel.GetControlFromPosition(newY, newX);
            int firstControlRow = tablePanel.GetRow(firstControl);
            int firstControlCol = tablePanel.GetColumn(firstControl);
            int secondControlRow = tablePanel.GetRow(secondControl);
            int secondControlCol = tablePanel.GetColumn(secondControl);
            tablePanel.SetRow(firstControl, secondControlRow);
            tablePanel.SetColumn(firstControl, secondControlCol);
            tablePanel.SetRow(secondControl, firstControlRow);
            tablePanel.SetColumn(secondControl, firstControlCol);
        }
        #endregion

        #region Form Event Handlers
        private void GameLoad(object? sender, EventArgs e)
        {
            _model.NewGame();
        }
        private void HuntingForm_SizeChanged(object? sender, EventArgs e)
        {
            tablePanel.Width = Width / 2;
            tablePanel.Height = tablePanel.Width;
        }
        #endregion

        #region Button Event Handlers
        private void startBtn_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                return;
            }
            string size = comboBox1.SelectedItem.ToString()!;
            int gridSize = int.Parse(size.Split('x')[0]);

            CreateTable(gridSize);
            startBtn.Enabled = false;
            comboBox1.Enabled = false;
            _model = new HuntingModel(new TextFilePersistence(), gridSize);
            _model.NewGame();
            _model.GameOver += new EventHandler(model_GameOver);
            _model.GameWon += new EventHandler<GameWonEventArgs>(Model_GameWon);

            if (_model.CurrentPlayer == _model.Table.Prey)
            {
                textBox2.Text = tunaTurn;
            }
            else 
            { 
                textBox2.Text = catTurn;
            }
            for (int i = 0; i < _model.TableSize; i++)
            {
                for (int j = 0; j < _model.TableSize; j++)
                {
                    if (((i == 0 || i == _model.TableSize - 1) && (j == 0 || j == _model.TableSize - 1)) || (i == (_model.TableSize - 1) / 2 && j == (_model.TableSize - 1) / 2))
                    {
                        _buttonGrid[i, j].MouseClick += buttonGrid_MouseClick;
                    }
                    else { _buttonGrid[i, j].MouseClick += buttonGrid_MouseDoubleClick; }
                }
            }
            textBox3.Text = $"The game lasts for {gridSize * 4} turns!";
            textBox4.Text = $"Turn count: {_model.TurnCount+1}";
        }
        private void newGameBtn_Click(object sender, EventArgs e)
        {
            NewGame();
        }
        private void LoadMenu_Click(object sender, EventArgs e)
        {
            if (_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _model.Load(_openFileDialog.FileName);
                    LoadTable(_model.TableSize);
                    if (_model.CurrentPlayer == _model.Table.Prey)
                    {
                        textBox2.Text = tunaTurn;
                    }
                    else
                    {
                        textBox2.Text = catTurn;
                    }
                    for (int i = 0; i < _model.TableSize; i++)
                    {
                        for (int j = 0; j < _model.TableSize; j++)
                        {
                            if (((i == 0 || i == _model.TableSize - 1) && (j == 0 || j == _model.TableSize - 1)) || (i == (_model.TableSize - 1) / 2 && j == (_model.TableSize - 1) / 2))
                            {
                                _buttonGrid[i, j].MouseClick += buttonGrid_MouseClick;
                            }
                            else { _buttonGrid[i, j].MouseClick += buttonGrid_MouseDoubleClick; }
                        }
                    }
                    textBox3.Text = $"The game lasts for {_model.TableSize * 4} turns!";
                    textBox4.Text = $"Turn count: {_model.TurnCount + 1}";
                    startBtn.Enabled = false;
                    comboBox1.Enabled = false;

                }
                catch (DataException)
                {
                     MessageBox.Show("An error occured during loading.", "Hunting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void SaveMenu_Click(object sender, EventArgs e)
        {
            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _model.Save(_saveFileDialog.FileName);
                }
                catch (DataException)
                {
                    MessageBox.Show("An error occured during saving.", "Hunting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void infoButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("How to play:" + "\n" + "There are two players competing against each other; the cats against the tuna can." +
                "\n"+"Firstly select the size of the gameplay and then on the right you will see how many steps there are, and who starts the game." +
                "\n"+"Click on the player you wish to step with then click the empty field where you wish to step.", "How to play", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);


        }
        #endregion

        #region ButtonGrid Event Handlers
        private void buttonGrid_MouseClick(object? sender, MouseEventArgs e)
        {
            _selected = new int[2];
            if (sender is GridButton button)
            {
                _xSelected = button.GridX;
                _ySelected = button.GridY;
                try
                { 
                    _selected = _model.SelectNextPlayer(_xSelected, _ySelected); 
                }
                catch 
                {
                    MessageBox.Show("Select the player you wish to step with!","Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                _isSelected = true;
                SelectedPlayer = button;
            }
        }
        private void buttonGrid_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (!_isSelected) { return; }

            if(sender is GridButton button)
            {
                int x = button.GridX;
                int y = button.GridY;
                try
                {
                    SwitchButtons(_xSelected, _ySelected, x, y);
                    _model.Step(_selected, x, y);
                    SelectedPlayer.MouseClick -= buttonGrid_MouseClick;
                    button.MouseClick -= buttonGrid_MouseDoubleClick;
                    button._x = _xSelected; button._y = _ySelected;
                    SelectedPlayer._x = x; SelectedPlayer._y = y;
                    button.MouseClick += buttonGrid_MouseDoubleClick;
                    SelectedPlayer.MouseClick += buttonGrid_MouseClick;
                }
                catch 
                {
                    SwitchButtons(_xSelected, _ySelected, x, y);
                    MessageBox.Show("Select an empty field next to your selected player!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                if (_model.CurrentPlayer == _model.Table.Hunters)
                {
                    textBox2.Text = catTurn;
                }
                else
                {
                    textBox2.Text = tunaTurn;
                }
                textBox4.Text = $"Turn count: {_model.TurnCount + 1}";
                _isSelected = false;
            }
        }
        #endregion

        #region Model Event Handlers
        private void model_GameOver(object? sender, EventArgs e)
        {
            MessageBox.Show("It is a draw!", "Game over!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            _model.NewGame();
            NewGame();
        }
        private void Model_GameWon(object? sender, GameWonEventArgs e)
        {
            if (e.player.Count == 1) 
            {
                MessageBox.Show("The tuna won the game!", "Game over!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); 
            }
            if (e.player.Count == 4)
            {
                MessageBox.Show("The cats won the game!", "Game over!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            _model.NewGame();
            NewGame();
        }
        #endregion
    }
}