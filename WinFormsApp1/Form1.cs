using Microsoft.VisualBasic;
using System;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly Button[,] _buttons = new Button[8, 8];

        private readonly int[,] _board = new int[8, 8];

        private readonly int _boardSize = 8;

        private int _currentTurn = 1;

        private Label _turnLabel;

        private Boolean endFlag = false;


        //八方向探査ベクトルだぜ
        private readonly Point[] _directions = new Point[]
        {
                new Point(-1, -1),
                new Point(0, -1),
                new Point(1, -1),
                new Point(-1, 0),
                new Point(1, 0),
                new Point(-1, 1),
                new Point(0, 1),
                new Point(1, 1)
        };
        //------------------------

        private Button alertButton;
        private Button alertButton2;

        //フォームロード時の処理だぜ
        private void Form1_Load(object sender, EventArgs e)
        {
            int buttonSize = 60;

            for (int row = 0; row < _boardSize; row++)
            {
                for (int col = 0; col < _boardSize; col++)
                {

                    Button btn = new Button();

                    btn.Size = new Size(buttonSize, buttonSize);

                    btn.Location = new Point(col * buttonSize, (row * buttonSize) + 40);

                    btn.BackColor = Color.Green;

                    btn.Tag = new Point(col, row);

                    btn.Click += BoardButton_Click;

                    this.Controls.Add(btn);

                    _buttons[row, col] = btn;
                }
            }

            _turnLabel = new Label();
            _turnLabel.Location = new Point(10, 10);
            _turnLabel.AutoSize = true; 
            _turnLabel.Font = new Font("MS UI Gothic", 12, FontStyle.Bold); //迷ったときのMSゴシック
            this.Controls.Add(_turnLabel);


            PrepareBoard();

            BoardCheck();
            CanPutAnywhere();
        }
        //------------------------


        //初期盤面用意するメソッドだぜ
        private void PrepareBoard()
        {
            _board[3, 4] = 1;
            _board[4, 3] = 1;
            _board[3, 3] = 2;
            _board[4, 4] = 2;
        }
        //------------------------


        //_board側の数値に対応して黒・白色をつけるメソッドだぜ
        private void BoardCheck()
        {
            for (int i = 0; i < _boardSize; i++)
            {
                for (int n = 0; n < _boardSize; n++)
                {

                    switch (_board[i, n])
                    {
                        case 1:
                            _buttons[i, n].BackColor = Color.Black;
                            break;
                        case 2:
                            _buttons[i, n].BackColor = Color.White;
                            break;
                        default:
                            _buttons[i, n].BackColor = Color.Green;
                            break;
                    }

                }
            }

            if (_currentTurn == 1)
            {
                _turnLabel.Text = "【黒】の番です";
            }
            else
            {
                _turnLabel.Text = "【白】の番です";
            }


        }
        //----------------------------------------------------

        //石を置けるかどうか判定するメソッドだぜ
        private bool PutCheck(int X, int Y)
        {

            if (_board[Y,X]!=0)
            {
                return false;
            }

            for (int i = 0; i < _directions.Length; i++)
            {
                int dx = _directions[i].X;
                int dy = _directions[i].Y;
                int x = X + dx;
                int y = Y + dy;
                bool hasOpponentPieceBetween = false;
                while (x >= 0 && x < _boardSize && y >= 0 && y < _boardSize)
                {
                    if (_board[y, x] == 0)
                    {
                        break;
                    }
                    else if (_board[y, x] != _currentTurn)
                    {
                        hasOpponentPieceBetween = true;
                    }
                    else
                    {
                        if (hasOpponentPieceBetween)
                        {
                            return true;
                        }
                        break;
                    }
                    x += dx;
                    y += dy;
                }
            }

            return false;
        }
        //-----------------------

        //石を置くメソッドだぜ
        private void Put(int X, int Y)
        {
            for (int i = 0; i < _directions.Length; i++)
            {
                int dx = _directions[i].X;
                int dy = _directions[i].Y;
                int x = X + dx;
                int y = Y + dy;
                bool hasOpponentPieceBetween = false;
                while (x >= 0 && x < _boardSize && y >= 0 && y < _boardSize)
                {
                    if (_board[y, x] == 0)
                    {
                        break;
                    }
                    else if (_board[y, x] != _currentTurn)
                    {
                        hasOpponentPieceBetween = true;
                    }
                    else
                    {
                        if (hasOpponentPieceBetween)
                        { 
                            FlipPieces(X, Y, dx, dy);
                        }
                        break;
                    }
                    x += dx;
                    y += dy;
                }
            }
            _board[Y, X] = _currentTurn;
        }
        //------------------------

        //石をひっくり返すメソッドだぜ
        private void FlipPieces(int startX, int startY, int dx, int dy)
        {
            int x = startX + dx;
            int y = startY + dy;
            int count = 0;
            while (x >= 0 && x < _boardSize && y >= 0 && y < _boardSize)
            {
                if (_board[y, x] == _currentTurn)
                {
                    break;
                }
                else
                {
                    count++;
                }
                x += dx;
                y += dy;

                for(int i = 0; i < count; i++)
                {
                    int flipX = startX + dx * (i + 1);
                    int flipY = startY + dy * (i + 1);
                    _board[flipY, flipX] = _currentTurn;
                }
            }
        }
        //------------------------

        //全ての場所からどこが置けるか判定するメソッドだぜ
        private bool CanPutAnywhere()
        {
            bool canPut = false;
            for (int row = 0; row < _boardSize; row++)
            {
                for (int col = 0; col < _boardSize; col++)
                {
                    if (PutCheck(col,row))
                    {
                        _buttons[row,col].Text = "＊";
                        canPut = true;
                    }
                }
            }
            return canPut;
        }
        //------------------------

        //アスタリスクを消すメソッドだぜ
        private void ClearAsterisk()
        {
            for (int row = 0; row < _boardSize; row++)
            {
                for (int col = 0; col < _boardSize; col++)
                {
                    _buttons[row, col].Text = "";

                }
            }
        }

        //ゲーム終了時の処理だぜ
        private void EndGame()
        {
            int blackCount = 0;
            int whiteCount = 0;
            for (int row = 0; row < _boardSize; row++)
            {
                for (int col = 0; col < _boardSize; col++)
                {
                    if (_board[row, col] == 1)
                    {
                        blackCount++;
                    }
                    else if (_board[row, col] == 2)
                    {
                        whiteCount++;
                    }
                }
            }
            string resultMessage;
            if (blackCount > whiteCount)
            {
                resultMessage = $"黒の勝ちです！\n黒: {blackCount} - 白: {whiteCount}";
            }
            else if (whiteCount > blackCount)
            {
                resultMessage = $"白の勝ちです！\n黒: {blackCount} - 白: {whiteCount}";
            }
            else
            {
                resultMessage = $"引き分けです！\n黒: {blackCount} - 白: {whiteCount}";
            }
            _turnLabel.Text = resultMessage.Replace("\n"," ");
            MessageBox.Show(resultMessage, "ゲーム終了", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //------------------------

        //ボタンが押されたときの処理だぜ
        private void BoardButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn?.Tag is Point position)
            {
                bool _puttable = PutCheck(position.X, position.Y);
                if (!_puttable)
                {
                    return;
                }

                Put(position.X, position.Y);

            }

            if (_currentTurn == 1) _currentTurn = 2;
            else _currentTurn = 1;

            BoardCheck();
            ClearAsterisk();

            if (!CanPutAnywhere())
            {
                MessageBox.Show("置ける場所がありません。パスします。", "パス");

                if (_currentTurn == 1) _currentTurn = 2;
                else _currentTurn = 1;

                BoardCheck();
                ClearAsterisk();

                if (!CanPutAnywhere())
                {
                    EndGame();
                }
            }
        }
        //------------------------

        //コンストラクタだぜ
        public Form1()
        {
            this.Size = new Size(510, 570);
            this.MaximumSize = new Size(510, 570);
            this.MinimumSize = new Size(510, 570);
            // 最大化ボタンを無効化するよ
            this.MaximizeBox = false;

            this.Text = "C#でつくったオセロ";

            InitializeComponent();
        }
        //------------------------
    }
}
