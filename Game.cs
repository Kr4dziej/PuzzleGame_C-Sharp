using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

public class PuzzlePiece
{
    public int Id { get; set; }
    public Bitmap Image { get; set; }
    public Point OriginalLocation { get; set; }
    public Point CurrentLocation { get; set; }
}

public class PuzzleGame
{
    private readonly List<PuzzlePiece> _pieces = new List<PuzzlePiece>();
    private readonly Random _random = new Random();
    public int Rows { get; private set; }
    public int Columns { get; private set; }

    public void LoadImage(string imagePath, int rows, int columns, int pieceWidth, int pieceHeight)
    {
        _pieces.Clear();

        Rows = rows;
        Columns = columns;

        using (var image = new Bitmap(imagePath))
        {
            int originalPieceWidth = image.Width / columns;
            int originalPieceHeight = image.Height / rows;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    var piece = new PuzzlePiece
                    {
                        Id = row * columns + column,
                        Image = new Bitmap(pieceWidth, pieceHeight),
                        OriginalLocation = new Point(column * pieceWidth , row * pieceHeight), 
                    };

                    using (var g = Graphics.FromImage(piece.Image))
                    {
                        var sourceRectangle = new Rectangle(column * originalPieceWidth, row * originalPieceHeight, originalPieceWidth, originalPieceHeight);
                        g.DrawImage(image, new Rectangle(0, 0, pieceWidth, pieceHeight), sourceRectangle, GraphicsUnit.Pixel);
                    }

                    _pieces.Add(piece);
                }
            }
        }
    }


    public void ShufflePieces(int canvasWidth, int canvasHeight)
    {
        int pieceWidth = canvasWidth / Columns;   
        int pieceHeight = canvasHeight / Rows;

        // Stworzenie listy wszystkich możliwych pozycji na planszy
        var locations = new List<Point>();
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                locations.Add(new Point(column * pieceWidth, row * pieceHeight));
            }
        }

        // Przemieszanie listy
        locations = locations.OrderBy(x => _random.Next()).ToList();

        // Przypisanie fragmentom pozycji
        for (int i = 0; i < _pieces.Count; i++)
        {
            _pieces[i].CurrentLocation = locations[i];
        }
    }

    public void SwapPieces(int id1, int id2)
    {
        var piece1 = _pieces.First(p => p.Id == id1);
        var piece2 = _pieces.First(p => p.Id == id2);

        var temp = piece1.CurrentLocation;
        piece1.CurrentLocation = piece2.CurrentLocation;
        piece2.CurrentLocation = temp;
    }

    public PuzzlePiece GetPiece(int row, int column)
    {
        return _pieces.First(p => p.Id == row * Columns + column);
    }

    public List<PuzzlePiece> GetPieces()
    {
        return _pieces;
    }

    public bool IsSolved()
    {
        return _pieces.All(p => p.CurrentLocation == p.OriginalLocation);
    }

}
