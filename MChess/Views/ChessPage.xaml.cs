using System.Timers;

namespace MChess.Views;

public partial class ChessPage : TabbedPage
{
    public class Piece
    {
        public Image img = new();
        public Color Color { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public Piece(string image, Color color, string name, int number, int startingY, int startingX)
        {
            AbsoluteLayout.SetLayoutBounds(img, new Rect(startingX * 40 - 10, startingY * 40 + 30, 40, 40));
            img.Source = ImageSource.FromFile(image);
            Color = color;
            Name = name;
            Number = number;
            img.ClassId = ((Color == Colors.White) ? "W" : "B") + Name[0] + Number.ToString();
        }
    }
    private readonly Piece[,] arr = new Piece[8, 8];
    bool isSelected = false;
    Piece PieceSelected = null;
    string match = null;
    readonly System.Timers.Timer TimeWhite = new(100);
    readonly System.Timers.Timer TimeBlack = new(100);
    int CurrentTimeWhite, CurrentTimeBlack;
    int timeIncrement = 1;
    int promotionNumber = 3;
    int numberOfMatches = 0;
    readonly Dictionary<string, Dictionary<string, bool>> castling = new()
    {
            {
                "W",
                new Dictionary<string, bool>
                {
                    {"Short", true},
                    {"Long", true }
                }
            },
            {
                "B",
                new Dictionary<string, bool>
                {
                    {"Short", true },
                    {"Long", true }
                }
            }
        };
    Tuple<List<string>, string> EnPassant = null;
    public ChessPage()
    {
        InitializeComponent();
        TimeWhite.Elapsed += new ElapsedEventHandler(ChangeWhiteTimer);
        TimeBlack.Elapsed += new ElapsedEventHandler(ChangeBlackTimer);
        bool isWhite = true;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var bw = new BoxView();
                if (isWhite)
                {
                    bw.BackgroundColor = Colors.White;
                    RadialGradientBrush radialGradientBrush = new();
                    GradientStop gradientStop = new()
                    {
                        Color = Color.FromRgba(0, 100, 0, 0.2),
                        Offset = 0.3f
                    };
                    GradientStop gradientStop2 = new()
                    {
                        Color = Colors.White,
                        Offset = 0.3f
                    };
                    radialGradientBrush.GradientStops.Add(gradientStop);
                    radialGradientBrush.GradientStops.Add(gradientStop2);
                    bw.Background = radialGradientBrush;
                    //bw.Background = new ImageBr
                    isWhite = false;
                }
                else
                {
                    bw.BackgroundColor = Color.FromArgb("#A0522D");
                    isWhite = true;
                }
                bw.ClassId = "i" + i + j;
                Grid.SetRow(bw, i + 1);
                Grid.SetColumn(bw, j + 1);
                var TapGestRecognizer2 = new TapGestureRecognizer
                {
                    NumberOfTapsRequired = 1
                };
                TapGestRecognizer2.Tapped += Chessboard_Clicked;
                grid.Children.Add(bw);
                bw.GestureRecognizers.Add(TapGestRecognizer2);
            }
            if (isWhite)
                isWhite = false;
            else
                isWhite = true;
        }
        CreatePieces();
    }
    /*private double width = 0;
    private double height = 0;
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (this.width != width || this.height != height)
        {
            this.width = width;
            this.height = height;
            if (width > height)
            {
                AbsoluteLayout.SetLayoutBounds(grid, new Rect(0.5, 0.5, 480, 320));
                Grid.SetRow(BlackIcon, 1);
                Grid.SetColumn(BlackIcon, 0);
                BlackIcon.WidthRequest = 39;
                BlackIcon.HeightRequest = 39;
                BlackIcon.Margin = new Thickness(20.5, 0.5, 20.5, 0.5);
                Grid.SetRow(WhiteIcon, 6);
                Grid.SetColumn(WhiteIcon, 9);
                WhiteIcon.WidthRequest = 39;
                WhiteIcon.WidthRequest = 39;
                WhiteIcon.Margin = new Thickness(20.5, 0.5, 20.5, 0.5);
                Grid.SetRow(BlackName, 2);
                Grid.SetColumn(BlackName, 0);
                Grid.SetColumnSpan(BlackName, 1);
                BlackName.WidthRequest = 78;
                BlackName.HeightRequest = 38;
                BlackName.Margin = new Thickness(1);
                Grid.SetRow(WhiteName, 7);
                Grid.SetColumn(WhiteName, 9);
                Grid.SetColumnSpan(WhiteName, 1);
                WhiteName.WidthRequest = 78;
                WhiteName.HeightRequest = 38;
                WhiteName.Margin = new Thickness(1);
                Grid.SetRow(timeBlack, 3);
                Grid.SetColumn(timeBlack, 0);
                Grid.SetColumnSpan(timeBlack, 1);
                timeBlack.WidthRequest = 78;
                timeBlack.HeightRequest = 38;
                timeBlack.Margin = new Thickness(1);
                Grid.SetRow(timeWhite, 8);
                Grid.SetColumn(timeWhite, 9);
                Grid.SetColumnSpan(timeWhite, 1);
                timeWhite.WidthRequest = 78;
                timeWhite.HeightRequest = 38;
                timeWhite.Margin = new Thickness(1);
                absolute.Scale = 0.85;
                imgBackground.Source = null;
            }
            else
            {
                AbsoluteLayout.SetLayoutBounds(grid, new Rect(0.5, 0.5, 320, 400));
                Grid.SetRow(BlackIcon, 0);
                Grid.SetColumn(BlackIcon, 1);
                BlackIcon.HeightRequest = 36;
                BlackIcon.WidthRequest = 36;
                BlackIcon.Margin = new Thickness(2, 0, 2, 4);
                Grid.SetRow(WhiteIcon, 9);
                Grid.SetColumn(WhiteIcon, 1);
                WhiteIcon.HeightRequest = 36;
                WhiteIcon.WidthRequest = 36;
                WhiteIcon.Margin = new Thickness(2, 4, 2, 0);
                Grid.SetRow(BlackName, 0);
                Grid.SetColumn(BlackName, 2);
                Grid.SetColumnSpan(BlackName, 4);
                BlackName.WidthRequest = 155;
                BlackName.HeightRequest = 40;
                BlackName.Margin = new Thickness(5, 0, 0, 0);
                Grid.SetRow(WhiteName, 9);
                Grid.SetColumn(WhiteName, 2);
                Grid.SetColumnSpan(WhiteName, 4);
                WhiteName.WidthRequest = 155;
                WhiteName.HeightRequest = 35;
                WhiteName.Margin = new Thickness(5, 5, 0, 0);
                Grid.SetRow(timeBlack, 0);
                Grid.SetColumn(timeBlack, 7);
                Grid.SetColumnSpan(timeBlack, 2);
                timeBlack.WidthRequest = 74;
                timeBlack.HeightRequest = 34;
                timeBlack.Margin = new Thickness(3);
                Grid.SetRow(timeWhite, 9);
                Grid.SetColumn(timeWhite, 7);
                Grid.SetColumnSpan(timeWhite, 2);
                timeWhite.WidthRequest = 74;
                timeWhite.HeightRequest = 34;
                timeWhite.Margin = new Thickness(3);
                absolute.Scale = 1;
                imgBackground.Source = ImageSource.FromFile("background.jpg");
            }
        }
    }*/
    private void CreatePieces()
    {
        arr[0, 0] = new Piece("br.png", Colors.Black, "Rook", 1, 0, 0);
        arr[0, 1] = new Piece("bn.png", Colors.Black, "Night", 1, 0, 1);
        arr[0, 2] = new Piece("bb.png", Colors.Black, "Bishop", 1, 0, 2);
        arr[0, 3] = new Piece("bq.png", Colors.Black, "Queen", 1, 0, 3);
        arr[0, 4] = new Piece("bk.png", Colors.Black, "King", 1, 0, 4);
        arr[0, 5] = new Piece("bb.png", Colors.Black, "Bishop", 2, 0, 5);
        arr[0, 6] = new Piece("bn.png", Colors.Black, "Night", 2, 0, 6);
        arr[0, 7] = new Piece("br.png", Colors.Black, "Rook", 2, 0, 7);
        for (int i = 0; i < 8; i++)
            arr[1, i] = new Piece("bp.png", Colors.Black, "Pawn", i + 1, 1, i);
        for (int i = 0; i < 8; i++)
            arr[6, i] = new Piece("wp.png", Colors.White, "Pawn", i + 1, 6, i);
        arr[7, 0] = new Piece("wr.png", Colors.White, "Rook", 1, 7, 0);
        arr[7, 1] = new Piece("wn.png", Colors.White, "Night", 1, 7, 1);
        arr[7, 2] = new Piece("wb.png", Colors.White, "Bishop", 1, 7, 2);
        arr[7, 3] = new Piece("wq.png", Colors.White, "Queen", 1, 7, 3);
        arr[7, 4] = new Piece("wk.png", Colors.White, "King", 1, 7, 4);
        arr[7, 5] = new Piece("wb.png", Colors.White, "Bishop", 2, 7, 5);
        arr[7, 6] = new Piece("wn.png", Colors.White, "Night", 2, 7, 6);
        arr[7, 7] = new Piece("wr.png", Colors.White, "Rook", 2, 7, 7);
        var TapGestRecognizer = new TapGestureRecognizer
        {
            NumberOfTapsRequired = 1
        };
        TapGestRecognizer.Tapped += Piece_Clicked;
        foreach (var i in arr)
        {
            if (i != null)
            {
                absolute.Children.Add(i.img);
                i.img.GestureRecognizers.Add(TapGestRecognizer);
            }

        }
    }
    private List<int> CheckPosition(Piece p, string id = null)
    {
        List<int> list = [];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (id == null)
                {
                    if (arr[i, j] == p)
                    {
                        list.Add(i);
                        list.Add(j);
                        return list;
                    }
                }
                else
                {
                    if (arr[i, j] != null)
                    {
                        if (arr[i, j].img.ClassId == id)
                        {
                            list.Add(i);
                            list.Add(j);
                            return list;
                        }
                    }
                }
            }
        }
        return null;
    }
    private List<string> CheckBishop(int r, int c)
    {
        List<string> list = [];
        string PieceColor = arr[r, c].img.ClassId[0].ToString();
        int x = r;
        int y = c;
        while (--x >= 0 && ++y < 8)
        {
            if (arr[x, y] != null)
            {
                if (arr[x, y].img.ClassId[0].ToString() == PieceColor)
                    break;
                else
                {
                    list.Add(x.ToString() + y.ToString());
                    break;
                }
            }
            else
                list.Add(x.ToString() + y.ToString());
        }
        x = r;
        y = c;
        while (--x >= 0 && --y >= 0)
        {
            if (arr[x, y] != null)
            {
                if (arr[x, y].img.ClassId[0].ToString() == PieceColor)
                    break;
                else
                {
                    list.Add(x.ToString() + y.ToString());
                    break;
                }
            }
            else
                list.Add(x.ToString() + y.ToString());
        }
        x = r;
        y = c;
        while (++x < 8 && ++y < 8)
        {
            if (arr[x, y] != null)
            {
                if (arr[x, y].img.ClassId[0].ToString() == PieceColor)
                    break;
                else
                {
                    list.Add(x.ToString() + y.ToString());
                    break;
                }
            }
            else
                list.Add(x.ToString() + y.ToString());
        }
        x = r;
        y = c;
        while (++x < 8 && --y >= 0)
        {
            if (arr[x, y] != null)
            {
                if (arr[x, y].img.ClassId[0].ToString() == PieceColor)
                    break;
                else
                {
                    list.Add(x.ToString() + y.ToString());
                    break;
                }
            }
            else
                list.Add(x.ToString() + y.ToString());
        }
        return list;
    }
    private List<string> CheckRook(int r, int c)
    {
        List<string> list = [];
        string PieceColor = arr[r, c].img.ClassId[0].ToString();
        for (int x = r - 1; x >= 0; x--)
        {
            if (arr[x, c] != null)
            {

                if (arr[x, c].img.ClassId[0].ToString() == PieceColor)
                    break;
                else
                {
                    list.Add(x.ToString() + c.ToString());
                    break;
                }
            }
            else
                list.Add(x.ToString() + c.ToString());
        }
        for (int x = r + 1; x < 8; x++)
        {
            if (arr[x, c] != null)
            {
                if (arr[x, c].img.ClassId[0].ToString() == PieceColor)
                    break;
                else
                {
                    list.Add(x.ToString() + c.ToString());
                    break;
                }
            }
            else
                list.Add(x.ToString() + c.ToString());
        }
        for (int x = c - 1; x >= 0; x--)
        {
            if (arr[r, x] != null)
            {
                if (arr[r, x].img.ClassId[0].ToString() == PieceColor)
                    break;
                else
                {
                    list.Add(r.ToString() + x.ToString());
                    break;
                }
            }
            else
                list.Add(r.ToString() + x.ToString());
        }
        for (int x = c + 1; x < 8; x++)
        {
            if (arr[r, x] != null)
            {
                if (arr[r, x].img.ClassId[0].ToString() == PieceColor)
                    break;
                else
                {
                    list.Add(r.ToString() + x.ToString());
                    break;
                }
            }
            else
                list.Add(r.ToString() + x.ToString());
        }
        return list;
    }
    private List<string> CheckPiece(int RowPos, int ColPos)
    {
        List<string> list = [];
        if (arr[RowPos, ColPos].Name == "Night")
        {
            list.Add((RowPos - 2).ToString() + (ColPos + 1).ToString());
            list.Add((RowPos - 1).ToString() + (ColPos + 2).ToString());
            list.Add((RowPos + 1).ToString() + (ColPos + 2).ToString());
            list.Add((RowPos + 2).ToString() + (ColPos + 1).ToString());
            list.Add((RowPos + 2).ToString() + (ColPos - 1).ToString());
            list.Add((RowPos + 1).ToString() + (ColPos - 2).ToString());
            list.Add((RowPos - 1).ToString() + (ColPos - 2).ToString());
            list.Add((RowPos - 2).ToString() + (ColPos - 1).ToString());
        }
        if (arr[RowPos, ColPos].Name == "King")
        {
            list.Add((RowPos + 1).ToString() + (ColPos + 1).ToString());
            list.Add((RowPos + 1).ToString() + ColPos.ToString());
            list.Add((RowPos - 1).ToString() + ColPos.ToString());
            list.Add(RowPos.ToString() + (ColPos + 1).ToString());
            list.Add(RowPos.ToString() + (ColPos - 1).ToString());
            list.Add((RowPos - 1).ToString() + (ColPos - 1).ToString());
            list.Add((RowPos + 1).ToString() + (ColPos - 1).ToString());
            list.Add((RowPos - 1).ToString() + (ColPos + 1).ToString());
            if ((RowPos == 0 || RowPos == 7) && ColPos == 4)
            {
                if (arr[RowPos, 5] == null && arr[RowPos, 6] == null && arr[RowPos, 7] != null && arr[RowPos, 7].img.ClassId == arr[RowPos, ColPos].img.ClassId[0].ToString() + "R2" && castling[arr[RowPos, ColPos].img.ClassId[0].ToString()]["Short"])
                    list.Add(RowPos.ToString() + "6");
                if (arr[RowPos, 1] == null && arr[RowPos, 2] == null && arr[RowPos, 3] == null && arr[RowPos, 0] != null && arr[RowPos, 0].img.ClassId == arr[RowPos, ColPos].img.ClassId[0].ToString() + "R1" && castling[arr[RowPos, ColPos].img.ClassId[0].ToString()]["Long"])
                    list.Add(RowPos.ToString() + "2");
            }

        }
        if (arr[RowPos, ColPos].Name == "Pawn")
        {
            if (arr[RowPos, ColPos].Color == Colors.White)
            {
                if (arr[RowPos - 1, ColPos] == null)
                    list.Add((RowPos - 1).ToString() + ColPos.ToString());
                if (RowPos == 6 && arr[RowPos - 2, ColPos] == null && arr[RowPos - 1, ColPos] == null)
                    list.Add((RowPos - 2).ToString() + ColPos.ToString());
                if (ColPos > 0 && arr[RowPos - 1, ColPos - 1] != null && arr[RowPos - 1, ColPos - 1].img.ClassId[0].ToString() == "B")
                    list.Add((RowPos - 1).ToString() + (ColPos - 1).ToString());
                if (ColPos < 7 && arr[RowPos - 1, ColPos + 1] != null && arr[RowPos - 1, ColPos + 1].img.ClassId[0].ToString() == "B")
                    list.Add((RowPos - 1).ToString() + (ColPos + 1).ToString());
            }
            else
            {
                if (arr[RowPos + 1, ColPos] == null)
                    list.Add((RowPos + 1).ToString() + ColPos.ToString());
                if (RowPos == 1 && arr[RowPos + 2, ColPos] == null && arr[RowPos + 1, ColPos] == null)
                    list.Add((RowPos + 2).ToString() + ColPos.ToString());
                if (ColPos > 0 && arr[RowPos + 1, ColPos - 1] != null && arr[RowPos + 1, ColPos - 1].img.ClassId[0].ToString() == "W")
                    list.Add((RowPos + 1).ToString() + (ColPos - 1).ToString());
                if (ColPos < 7 && arr[RowPos + 1, ColPos + 1] != null && arr[RowPos + 1, ColPos + 1].img.ClassId[0].ToString() == "W")
                    list.Add((RowPos + 1).ToString() + (ColPos + 1).ToString());
            }
            if (EnPassant != null && EnPassant.Item1.Contains(RowPos.ToString() + ColPos))
                list.Add(EnPassant.Item2);
        }
        if (arr[RowPos, ColPos].Name == "Bishop")
            list = CheckBishop(RowPos, ColPos);
        if (arr[RowPos, ColPos].Name == "Rook")
            list = CheckRook(RowPos, ColPos);
        if (arr[RowPos, ColPos].Name == "Queen")
        {
            List<string> BishopList = CheckBishop(RowPos, ColPos);
            List<string> RookList = CheckRook(RowPos, ColPos);
            list = new List<string>(BishopList.Count + RookList.Count);
            list.AddRange(BishopList);
            list.AddRange(RookList);
        }
        return list;
    }
    private void Chessboard_Clicked(object sender, EventArgs args)
    {
        var bw = (BoxView)sender;
        if (isSelected)
        {
            IEnumerable<int> PositionSelected = CheckPosition(PieceSelected);
            if (CheckPiece(PositionSelected.ElementAt(0), PositionSelected.ElementAt(1)).IndexOf(bw.ClassId[1].ToString() + bw.ClassId[2].ToString()) == -1)
                return;
            if (PieceSelected.Name == "Rook")
            {
                if (PieceSelected.img.ClassId[2].ToString() == "1" && PieceSelected.img.ClassId.Length == 3)
                    castling[PieceSelected.img.ClassId[0].ToString()]["Long"] = false;
                if (PieceSelected.img.ClassId[2].ToString() == "2" && PieceSelected.img.ClassId.Length == 3)
                    castling[PieceSelected.img.ClassId[0].ToString()]["Short"] = false;
            }
            if (PieceSelected.Name == "King")
            {
                castling[PieceSelected.img.ClassId[0].ToString()]["Short"] = false;
                castling[PieceSelected.img.ClassId[0].ToString()]["Long"] = false;
                if (Math.Abs(PositionSelected.ElementAt(1) - int.Parse(bw.ClassId[2].ToString())) == 2)
                {
                    int pos = int.Parse(bw.ClassId[2].ToString()) < (7 - int.Parse(bw.ClassId[2].ToString())) ? 0 : 7;
                    arr[PositionSelected.ElementAt(0), pos].img.TranslateTo(-140 + (PositionSelected.ElementAt(1) + int.Parse(bw.ClassId[2].ToString())) / 2 * 40, arr[PositionSelected.ElementAt(0), pos].img.TranslationY, 300, Easing.SinInOut);
                    arr[PositionSelected.ElementAt(0), (PositionSelected.ElementAt(1) + int.Parse(bw.ClassId[2].ToString())) / 2] = arr[PositionSelected.ElementAt(0), pos];
                    arr[PositionSelected.ElementAt(0), pos] = null;
                }
            }
            if (PieceSelected.Name == "Pawn" && (int.Parse(bw.ClassId[1].ToString()) == 0 || int.Parse(bw.ClassId[1].ToString()) == 7))
            {
                PieceSelected.Name = "Queen";
                PieceSelected.img.Source = ImageSource.FromFile(PieceSelected.img.ClassId[0].ToString().ToLower() + "q.png");
                PieceSelected.img.ClassId = PieceSelected.img.ClassId[0].ToString() + "Q" + promotionNumber.ToString();
                promotionNumber++;
            }
            if (PieceSelected.Name == "Pawn" && EnPassant != null && bw.ClassId.ToString()[1..] == EnPassant.Item2)
            {
                int enPassantRow = (PieceSelected.Color == Colors.White) ? 3 : 4;
                arr[enPassantRow, int.Parse(bw.ClassId[2].ToString())].img.IsVisible = false;
                arr[enPassantRow, int.Parse(bw.ClassId[2].ToString())] = null;
            }
            EnPassant = null;
            if (PieceSelected.Name == "Pawn" && Math.Abs(PositionSelected.ElementAt(0) - int.Parse(bw.ClassId[1].ToString())) == 2)
            {
                if (PieceSelected.Color == Colors.White)
                    EnPassant = new Tuple<List<string>, string>(new List<string>() { "4" + (PositionSelected.ElementAt(1) - 1), "4" + (PositionSelected.ElementAt(1) + 1) }, "5" + PositionSelected.ElementAt(1).ToString());
                else
                    EnPassant = new Tuple<List<string>, string>(new List<string>() { "3" + (PositionSelected.ElementAt(1) - 1), "3" + (PositionSelected.ElementAt(1) + 1) }, "2" + PositionSelected.ElementAt(1).ToString());
            }
            int X = (int)PieceSelected.img.TranslationX + (int.Parse(bw.ClassId[2].ToString()) - PositionSelected.ElementAt(1)) * 40;
            int Y = (int)PieceSelected.img.TranslationY + (int.Parse(bw.ClassId[1].ToString()) - PositionSelected.ElementAt(0)) * 40;
            PieceSelected.img.TranslateTo(X, Y, 200, Easing.SinInOut);
            PieceSelected.img.BackgroundColor = Colors.Transparent;
            arr[PositionSelected.ElementAt(0), PositionSelected.ElementAt(1)] = null;
            arr[int.Parse(bw.ClassId[1].ToString()), int.Parse(bw.ClassId[2].ToString())] = PieceSelected;
            PieceSelected = null;
            isSelected = false;
            ChangeColor();
        }
    }
    private /*async*/ void Piece_Clicked(object sender, EventArgs args)
    {
        var img = (Image)sender;
        if (isSelected == false)
        {
            if (match != null && img.ClassId[0].ToString() == match[0].ToString())
            {
                isSelected = true;
                img.BackgroundColor = Color.FromArgb("#B3228B22");
                foreach (var item in arr)
                {
                    if (item != null)
                    {
                        if (item.img == img)
                        {
                            PieceSelected = item;
                        }
                    }

                }
            }
        }
        else if (img.ClassId == PieceSelected.img.ClassId)
        {
            img.BackgroundColor = Colors.Transparent;
            isSelected = false;
        }
        else if (img.ClassId[0] != PieceSelected.img.ClassId[0])
        {
            IEnumerable<int> PositionSelected = CheckPosition(PieceSelected);
            IEnumerable<int> PositionSelected2 = CheckPosition(null, img.ClassId);
            if (CheckPiece(PositionSelected.ElementAt(0), PositionSelected.ElementAt(1)).IndexOf(PositionSelected2.ElementAt(0).ToString() + PositionSelected2.ElementAt(1).ToString()) == -1)
                return;
            if (PieceSelected.Name == "Rook")
            {
                if (PieceSelected.img.ClassId[2].ToString() == "1" && PieceSelected.img.ClassId.Length == 3)
                    castling[PieceSelected.img.ClassId[0].ToString()]["Long"] = false;
                if (PieceSelected.img.ClassId[2].ToString() == "2" && PieceSelected.img.ClassId.Length == 3)
                    castling[PieceSelected.img.ClassId[0].ToString()]["Short"] = false;
            }
            if (PieceSelected.Name == "King")
            {
                castling[PieceSelected.img.ClassId[0].ToString()]["Short"] = false;
                castling[PieceSelected.img.ClassId[0].ToString()]["Long"] = false;
            }
            if (PieceSelected.Name == "Pawn" && (PositionSelected2.ElementAt(0) == 0 || PositionSelected2.ElementAt(0) == 7))
            {
                /*string[] array = { "aaa", "bbbb", "cccc" };
                var result = await DisplayActionSheet("Select", "abc", "def", array);
                if (result is null)
                    return;
                else
                    await DisplayAlert("Test", result.ToString(), "OK");
                PawnPromotion promotionPopup = new PawnPromotion()
                {
                    Size = new Size(100, 400),
                    //Anchor = test,
                };
                var result = await Navigation.ShowPopupAsync(promotionPopup);
                if (result != null)
                {
                    PieceSelected.Name = (string)result;
                    PieceSelected.img.Source = ImageSource.FromResource("MChess.Images." + PieceSelected.img.ClassId[0].ToString() + result.ToString()[0] + ".png");
                    PieceSelected.img.ClassId = PieceSelected.img.ClassId[0].ToString() + result.ToString()[0] + promotionNumber.ToString();
                    promotionNumber++;
                }
                else
                    return;*/
            }
            EnPassant = null;
            int X = (int)PieceSelected.img.TranslationX + (PositionSelected2.ElementAt(1) - PositionSelected.ElementAt(1)) * 40;
            int Y = (int)PieceSelected.img.TranslationY + (PositionSelected2.ElementAt(0) - PositionSelected.ElementAt(0)) * 40;
            img.IsVisible = false;
            _ = PieceSelected.img.TranslateTo(X, Y, 200, Easing.SinInOut);
            PieceSelected.img.BackgroundColor = Colors.Transparent;
            arr[PositionSelected.ElementAt(0), PositionSelected.ElementAt(1)] = null;
            arr[PositionSelected2.ElementAt(0), PositionSelected2.ElementAt(1)] = PieceSelected;
            PieceSelected = null;
            isSelected = false;
            if (img.ClassId[1].ToString() == "K")
            {
                if (img.ClassId[0].ToString() == "W")
                    EndScreen("black", "checkmate");
                else
                    EndScreen("white", "checkmate");
                match = null;
                TimeWhite.Stop();
                TimeBlack.Stop();
            }
            ChangeColor();
        }
    }
    private async void StartMatch(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Start the game?", "Start a game between " + ((App)App.Current).Player1 + " (" + ((App)App.Current).Player1Elo + ") [White] and " + ((App)App.Current).Player2 + " (" + ((App)App.Current).Player2Elo + ") [Black]? Time: " + ((App)App.Current).StartTime + "+" + ((App)App.Current).TimeAdded + ".", "Start now", "Cancel");
        if (!answer)
            return;
        match = "White";
        timeWhite.IsEnabled = true;
        Start.IsVisible = false;
        CurrentTimeBlack = (int)(((App)App.Current).StartTime * 600);
        TimeWhite.Enabled = true;
        CurrentTimeWhite = (int)(((App)App.Current).StartTime * 600);
        BlackName.Text = ((App)App.Current).Player2 + " (" + ((App)App.Current).Player2Elo + ")";
        WhiteName.Text = ((App)App.Current).Player1 + " (" + ((App)App.Current).Player1Elo + ")";
        timeIncrement = (int)((App)App.Current).TimeAdded;
        if (numberOfMatches > 0)
        {
            promotionNumber = 3;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (arr[i, j] != null)
                    {
                        absolute.Children.Remove(arr[i, j].img);
                        arr[i, j] = null;
                    }
                }
            }
            CreatePieces();
            castling["W"]["Short"] = true;
            castling["W"]["Long"] = true;
            castling["B"]["Short"] = true;
            castling["B"]["Long"] = true;
            EnPassant = null;
        }
        //On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().DisableSwipePaging();
    }
    public void ChangeWhiteTimer(object source, ElapsedEventArgs e)
    {
        ChangeWhiteTimerFunction();
    }
    private void ChangeWhiteTimerFunction()
    {
        CurrentTimeWhite--;
        MainThread.BeginInvokeOnMainThread(() => timeWhite.Text = (CurrentTimeWhite / 600).ToString() + ":" + ((CurrentTimeWhite % 600 < 100) ? ("0" + CurrentTimeWhite / 10 % 60) : ((CurrentTimeWhite / 10 % 60).ToString())));
        if (CurrentTimeWhite <= 0)
        {
            match = null;
            TimeWhite.Stop();
            if (PieceSelected != null)
                PieceSelected.img.BackgroundColor = Colors.Transparent;
            PieceSelected = null;
            isSelected = false;
            MainThread.BeginInvokeOnMainThread(() => EndScreen("black", "time"));
        }
    }
    public void ChangeBlackTimer(object source, ElapsedEventArgs e)
    {
        ChangeBlackTimerFunction();
    }
    private void ChangeBlackTimerFunction()
    {
        CurrentTimeBlack--;
        MainThread.BeginInvokeOnMainThread(() => timeBlack.Text = (CurrentTimeBlack / 600).ToString() + ":" + ((CurrentTimeBlack % 600 < 100) ? ("0" + CurrentTimeBlack / 10 % 60) : ((CurrentTimeBlack / 10 % 60).ToString())));
        if (CurrentTimeBlack <= 0)
        {
            match = null;
            TimeBlack.Stop();
            if (PieceSelected != null)
                PieceSelected.img.BackgroundColor = Colors.Transparent;
            PieceSelected = null;
            isSelected = false;
            MainThread.BeginInvokeOnMainThread(() => EndScreen("white", "time"));
        }
    }
    private void ChangeColor()
    {
        if (match != null && match == "White")
        {
            CurrentTimeWhite += timeIncrement * 10;
            match = "Black";
            timeWhite.IsEnabled = false;
            TimeWhite.Stop();
            timeBlack.IsEnabled = true;
            TimeBlack.Start();
            timeWhite.Text = (CurrentTimeWhite / 600).ToString() + ":" + ((CurrentTimeWhite % 600 < 100) ? ("0" + CurrentTimeWhite / 10 % 60) : (CurrentTimeWhite / 10 % 60).ToString());
            if (switchTap.IsToggled)
                board.Rotation = 180;
        }
        else if (match != null && match == "Black")
        {
            CurrentTimeBlack += timeIncrement * 10;
            match = "White";
            timeWhite.IsEnabled = true;
            TimeWhite.Start();
            timeBlack.IsEnabled = false;
            TimeBlack.Stop();
            timeBlack.Text = (CurrentTimeBlack / 600).ToString() + ":" + ((CurrentTimeBlack % 600 < 100) ? ("0" + CurrentTimeBlack / 10 % 60) : (CurrentTimeBlack / 10 % 60).ToString());
            if (switchTap.IsToggled)
                board.Rotation = 0;
        }
    }
    private void TimeButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new TimeControlPage());
    }
    private void PlayerButton_Clicked(object sender, EventArgs e)
    {
        //Navigation.PushAsync(new PlayerPage());
    }
    private void EndScreen(string color, string reason)
    {
        numberOfMatches++;
        board.Rotation = 0;
        Start.IsVisible = true;
        string whoWon = (color == "white") ? "White" : "Black";
        string number1 = (color == "white") ? "1" : "0";
        string number2 = (color == "white") ? "0" : "1";
        double helper1 = 1 / (1 + Math.Pow(10, (((App)App.Current).Player2Elo - ((App)App.Current).Player1Elo) / 400));
        double helper2 = 1 / (1 + Math.Pow(10, (((App)App.Current).Player1Elo - ((App)App.Current).Player2Elo) / 400));
        double newElo1 = (int)((color == "white") ? (((App)App.Current).Player1Elo + 32 * (1 - helper1)) : (((App)App.Current).Player1Elo + 32 * (0 - helper1)));
        double newElo2 = (int)((color == "black") ? (((App)App.Current).Player2Elo + 32 * (1 - helper2)) : (((App)App.Current).Player2Elo + 32 * (0 - helper2)));
        DisplayAlert("Game has ended!", whoWon + " won by " + reason + ".\n" + ((App)App.Current).Player1 + " (" + ((App)App.Current).Player1Elo + ") " + number1 + "-" + number2 + " " + ((App)App.Current).Player2 + " (" + ((App)App.Current).Player2Elo + ")\nWhite rating [" + ((App)App.Current).Player1 + "] is now " + newElo1 + ".\nBlack rating [" + ((App)App.Current).Player2 + "] is now " + newElo2 + ".", "OK");
        ((App)App.Current).Player1Elo = newElo1;
        ((App)App.Current).Player2Elo = newElo2;
        //App.Database.UpdateUser(((App)App.Current).Player1, newElo1);
        //App.Database.UpdateUser(((App)App.Current).Player2, newElo2);
        //DisplayAlert("aaa", "Memory used before collection: " + 
        //GC.GetTotalMemory(false), "OK");
        //GC.Collect();
        //DisplayAlert("aaa", "Memory used after full collection: " +
        //GC.GetTotalMemory(true), "OK");
    }
    private void ChangeSwitchState(object sender, EventArgs e)
    {
        if (switchTap.IsToggled)
            switchTap.IsToggled = false;
        else
            switchTap.IsToggled = true;
    }
}