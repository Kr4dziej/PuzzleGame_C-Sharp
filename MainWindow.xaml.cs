using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;

namespace Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PuzzleGame _game;
        private Image _selectedImage;
        private Border _selectedBorder;
        private int _moveCount;
        private Border _firstSelectedBorder;
        private Border _secondSelectedBorder;
        private DispatcherTimer _timer;
        private Stopwatch _stopwatch;
        private int rows;
        private int columns;
        private string imagePath;
        private string difficultyLevel;
        private string imageName;
        private RecordDbContext _context;
        private RecordService _service;

        public MainWindow()
        {
            InitializeComponent();
            _game = new PuzzleGame();

            var optionsBuilder = new DbContextOptionsBuilder<RecordDbContext>();
            // Ścieżka do pliku bazy danych SQLite
            optionsBuilder.UseSqlite("Data Source=records.db");

            _context = new RecordDbContext(optionsBuilder.Options);
            _service = new RecordService(_context);

            // Licznik czasu
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(10);
            _timer.Tick += Timer_Tick;

            // Stoper
            _stopwatch = new Stopwatch();

            // Dodaj obsługę zdarzenia Loaded do Canvas
            FirstColumnCanvas.Loaded += FirstColumnCanvas_Loaded;

            // Początkowe ustawienia
            rows = 4;
            columns = 4;
            difficultyLevel = "Easy";

            imageName = "Beach";
            imagePath = "Images\\Beach.jpeg";

            statusBarText.Text = imageName + " - " + difficultyLevel;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Aktualizacja wyświetlacza stopera
            TimerTextBlock.Text = _stopwatch.Elapsed.ToString(@"mm\:ss\.ff");
        }

        private void FirstColumnCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            // Wyświetlenie przemieszanych fragmentów w pierwszej kolumnie
            DisplayShuffledPieces();
        }

        private void BeachImage_Click(object sender, RoutedEventArgs e)
        {
            imagePath = "Images\\Beach.jpeg";
            GameImage.Source = new BitmapImage(new Uri("Images/Beach.jpeg", UriKind.Relative));
            imageName = "Beach";
            Pause();
        }

        private void TigerImage_Click(object sender, RoutedEventArgs e)
        {
            imagePath = "Images\\Tiger.jpg";
            GameImage.Source = new BitmapImage(new Uri("Images/Tiger.jpg", UriKind.Relative));
            imageName = "Tiger";
            Pause();
        }

        private void RoadImage_Click(object sender, RoutedEventArgs e)
        {
            imagePath = "Images\\Road.jpg";
            GameImage.Source = new BitmapImage(new Uri("Images/Road.jpg", UriKind.Relative));
            imageName = "Road";
            Pause();
        }

        private void CarImage_Click(object sender, RoutedEventArgs e)
        {
            imagePath = "Images\\Car.jpg";
            GameImage.Source = new BitmapImage(new Uri("Images/Car.jpg", UriKind.Relative));
            imageName = "Car";
            Pause();
        }

        private void HogwartImage_Click(object sender, RoutedEventArgs e)
        {
            imagePath = "Images\\Hogwart.jpg";
            GameImage.Source = new BitmapImage(new Uri("Images/Hogwart.jpg", UriKind.Relative));
            imageName = "Hogwart";
            Pause();
        }

        private void MountainsImage_Click(object sender, RoutedEventArgs e)
        {
            imagePath = "Images\\Mountains.jpg";
            GameImage.Source = new BitmapImage(new Uri("Images/Mountains.jpg", UriKind.Relative));
            imageName = "Mountains";
            Pause();
        }

        private void EasyDifficulty_Click(object sender, RoutedEventArgs e)
        {
            rows = 4; 
            columns = 4;
            difficultyLevel = "Easy";
            Pause();
        }

        private void MediumDifficulty_Click(object sender, RoutedEventArgs e)
        {
            rows = 6;
            columns = 6;
            difficultyLevel = "Medium";
            Pause();
        }

        private void HardDifficulty_Click(object sender, RoutedEventArgs e)
        {
            rows = 8;
            columns = 8;
            difficultyLevel = "Hard";
            Pause();
        }

        private async void DeleteRecordData_Click(object sender, RoutedEventArgs e)
        {
            await _service.DeleteAllRecordsAsync();
        }

       
        private async void ShowActualRecords_Click(object sender, RoutedEventArgs e)
        {
            var images = new List<string> { "Beach", "Tiger", "Road", "Car", "Hogwart", "Mountains" };
            var difficulties = new List<string> { "Easy", "Medium", "Hard" };

            var recordsString = new StringBuilder();
            foreach (var image in images)
            {
                foreach (var difficulty in difficulties)
                {
                    var record = await _service.GetRecordsAsync(image, difficulty);
                    if (record != null)
                    {
                        recordsString.AppendLine($"{record.ImageName} - {record.DifficultyLevel}: Best Time: {record.BestTime.ToString(@"mm\:ss\.ff")}, Moves: {record.Moves}");
                    }
                    else
                    {
                        recordsString.AppendLine($"{image} - {difficulty}: No record yet");
                    }
                }
            }
            MessageBox.Show(recordsString.ToString());
        }

        private void AboutMenu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Created by Kamil Pieczyk");
        }


        private void Pause()
        {
            statusBarText.Text = imageName + " - " + difficultyLevel;
            FirstColumnCanvas.Children.Clear();

            _stopwatch.Stop();
            _stopwatch.Reset();
            _timer.Stop();

            TimerTextBlock.Text = _stopwatch.Elapsed.ToString(@"mm\:ss\.ff");

            _moveCount = 0;
            MovesTextBlock.Text = _moveCount.ToString();
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Wymiary Canvas
            int canvasWidth = (int)FirstColumnCanvas.ActualWidth;
            int canvasHeight = (int)FirstColumnCanvas.ActualHeight;

            // Załadowanie obrazu, wymieszanie i wyświetlenie
            _game.LoadImage(imagePath, rows, columns, canvasWidth/rows, canvasHeight/columns);
            _game.ShufflePieces(canvasWidth, canvasHeight);
            DisplayShuffledPieces();

            // Liczba ruchów
            _moveCount = 0;
            MovesTextBlock.Text = _moveCount.ToString();

            // Liczenie czasu
            _stopwatch.Restart();
            _timer.Start();
        }


        private void DisplayShuffledPieces()
        {
            FirstColumnCanvas.Children.Clear();

            var pieceWidth = FirstColumnCanvas.Width / _game.Columns;
            var pieceHeight = FirstColumnCanvas.Height / _game.Rows;

            foreach (var piece in _game.GetPieces())
            {
                var image = new Image
                {
                    Source = Convert(piece.Image), 
                    Width = pieceWidth,
                    Height = pieceHeight,
                };

                var border = new Border
                {
                    Child = image,
                    BorderThickness = new Thickness(0),
                    BorderBrush = Brushes.Transparent,
                    Tag = piece.Id,  // Przypisz Id fragmentu do Tag
                };

                // Ustawienie pozycji
                Canvas.SetLeft(border, piece.CurrentLocation.X);
                Canvas.SetTop(border, piece.CurrentLocation.Y);

                // Dodanie zdarzenie naciśnięcia fragmentu
                border.MouseLeftButtonDown += (sender, e) => Border_MouseLeftButtonDown(border, e);

                FirstColumnCanvas.Children.Add(border);
            }
        }



        private async void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_firstSelectedBorder == null)
            {
                // Zaznaczenie pierwszego fragmentu
                _firstSelectedBorder = (Border)sender;
                _firstSelectedBorder.BorderThickness = new Thickness(2);
                _firstSelectedBorder.BorderBrush = Brushes.Red;
            }
            else if (_secondSelectedBorder == null)
            {
                // Zaznaczenie drugiego fragmentu
                _secondSelectedBorder = (Border)sender;
                _secondSelectedBorder.BorderThickness = new Thickness(2);
                _secondSelectedBorder.BorderBrush = Brushes.Red;

                if (_firstSelectedBorder != _secondSelectedBorder)
                {
                    // Zamiana fragmentów miejscami
                    var id1 = (int)_firstSelectedBorder.Tag;
                    var id2 = (int)_secondSelectedBorder.Tag;
                    _game.SwapPieces(id1, id2);

                    _moveCount++;
                    MovesTextBlock.Text = _moveCount.ToString();
                }

                // Odznaczenie 
                _firstSelectedBorder.BorderThickness = new Thickness(0);
                _secondSelectedBorder.BorderThickness = new Thickness(0);
                _firstSelectedBorder = null;
                _secondSelectedBorder = null;

                // Wyświetlenie fragmentów
                DisplayShuffledPieces();

                // Sprawdzenie rozwiązania puzzli
                if (_game.IsSolved())
                {
                    _timer.Stop();
                    _stopwatch.Stop();
                    TimerTextBlock.Text = _stopwatch.Elapsed.ToString(@"mm\:ss\.ff");

                    // Zapis wyniku
                    var record = new Record
                    {
                        ImageName = imageName,
                        DifficultyLevel = difficultyLevel,
                        BestTime = _stopwatch.Elapsed,
                        Moves = _moveCount
                    };
                    bool isNewRecord = await _service.CreateOrUpdateRecordAsync(record);

                    // MessageBox
                    string message = $"Congratulations! You solved the puzzle in {_stopwatch.Elapsed.ToString(@"mm\:ss\.ff")} with {_moveCount} moves.";
                    if (isNewRecord)
                    {
                        message += "\nNew Best Time!";
                    }
                    MessageBox.Show(message);
                }
            }
        }


        private BitmapImage Convert(System.Drawing.Bitmap bitmap)
        {
            // Konwersja z System.Drawing.Bitmap na System.Windows.Media.Imaging.BitmapImage
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

    }
}
