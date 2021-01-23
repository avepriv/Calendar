using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Grid mainGrid;
        Brush borderBrush = Brushes.Black;
        FontFamily verdana = new FontFamily("Arial");
        Thickness lpadd = new Thickness(3, 0, 0, 0);
        Thickness rpadd = new Thickness(0, 0, 3, 0);
        Thickness thinBorder = new Thickness(1, 0, 0, 1);
        Thickness borderpadd = new Thickness(1, 1, 1, 1);
        Thickness fatBorder = new Thickness(2, 2, 2, 2);
        Thickness thinMargin = new Thickness(0);
        Thickness fatMargin = new Thickness(-1, 0, -1, 0);
        Thickness markerMargin = new Thickness(-5);
        Thickness markerBorder = new Thickness(5);
        Thickness holidaypadd = new Thickness(0, 0, 10, 2);

        public MainWindow()
        {
            InitializeComponent();
            Width = System.Windows.SystemParameters.PrimaryScreenWidth * 0.8;
            Height = System.Windows.SystemParameters.PrimaryScreenHeight * 0.8;
            Left = System.Windows.SystemParameters.PrimaryScreenWidth * 0.1;
            Top = System.Windows.SystemParameters.PrimaryScreenHeight * 0.1;

            // Add a 5-pixel padding to the outer-border to allow space for the marker
            Border outerBorder = Content as Border;
            outerBorder.Margin = new Thickness(5);
            outerBorder.BorderThickness = new Thickness(2, 2, 2, 2);
            outerBorder.BorderBrush = Brushes.Black;

            // get the main-grid in its border
            mainGrid = outerBorder.Child as Grid;

            // create the row- and column definitions
            for(int i = 0; i < 32; ++i) {
                mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }
            for(int i = 0; i < 13; ++i) {
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Now;

            int usedYear = today.Year;
            // check the command-line for another year
            if (Environment.GetCommandLineArgs().Length > 1) {
                usedYear = Int32.Parse(Environment.GetCommandLineArgs()[1]);
            }

            Title = $"{usedYear}";

            Holiday holidays = new Holiday(usedYear);

            // create the title-bar
            for (int i = 0; i <= 12; ++i) {
                String mName = $"{new DateTime(usedYear, (i % 12) + 1, 1):MMMM}";
                Border title_brd = new Border() { BorderThickness = fatBorder, Margin=fatMargin, BorderBrush = borderBrush};

                title_brd.Child = new TextBlock() { Text = mName, VerticalAlignment = VerticalAlignment.Center, Padding = lpadd, FontFamily = verdana, FontWeight = FontWeights.Bold, FontSize = 12, Foreground = Brushes.Black };
                Grid.SetColumn(title_brd, i);
                Grid.SetRow(title_brd, 0);
                mainGrid.Children.Add(title_brd);
            }

            // create the days
            int row, column = 1;
            TimeSpan one_day = new TimeSpan(1, 0, 0, 0);
            for (DateTime day = new DateTime(usedYear, 1, 1); 12*(day.Year - usedYear) + day.Month < 14; day += one_day) {
                int weekDay = (int)day.DayOfWeek;
                bool isWeekend = weekDay == 0 || weekDay == 6;
                bool isHoliday = holidays.IsHoliday(day);
                bool freeDay = isWeekend || isHoliday;

                row = day.Day;
                column = 12 * (day.Year - usedYear) + day.Month -1;

                Border brd = new Border() {
                    BorderThickness = thinBorder,
                    Margin = thinMargin,
                    BorderBrush = borderBrush,
                    Background = freeDay ? Brushes.Yellow : Brushes.Transparent
                };
                brd.Child = new TextBlock() {
                    Text = $"{day:ddd} {day.Day}",
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = lpadd,
                    FontFamily = verdana,

                    FontWeight = FontWeights.Bold,
                    FontSize = 12,
                    Foreground = freeDay ? Brushes.Red : Brushes.Black
                };

                Grid.SetColumn(brd, column);
                Grid.SetRow(brd, row);
                mainGrid.Children.Add(brd);

                if(weekDay == 1) {
                    int calWeek = GetWeekOfYear(day);
                    TextBlock tbl_week = new TextBlock() {
                        Text = $"{calWeek}",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Padding = rpadd,
                        VerticalAlignment = VerticalAlignment.Center,
                        Background = Brushes.Transparent,
                        FontFamily = verdana,
                        Foreground = Brushes.Gray,
                        FontSize = 14,
                        FontWeight = FontWeights.Black };

                    Grid.SetColumn(tbl_week, column);
                    Grid.SetRow(tbl_week, row);

                    mainGrid.Children.Add(tbl_week);
                }

                if(isHoliday) {
                    // write the name of the holiday
                    TextBlock tbl_holiday = new TextBlock() {
                        Text = $"{holidays.GetHolidayName(day)}",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Padding = holidaypadd,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Background = Brushes.Transparent,
                        FontFamily = verdana,
                        Foreground = Brushes.Red,
                        FontSize = 9,
                        FontWeight = FontWeights.Normal
                    };

                    Grid.SetColumn(tbl_holiday, column);
                    Grid.SetRow(tbl_holiday, row);

                    mainGrid.Children.Add(tbl_holiday);
                }

                // fill the empty boxes of the month to have a constant grid
                if ((day + one_day).Month != day.Month) {
                    for(int fill_row = row + 1; fill_row <= 31; ++fill_row) {
                        Border empty_brd = new Border() {
                            BorderThickness = thinBorder,
                            Margin = thinMargin,
                            BorderBrush = borderBrush,
                            Background = freeDay ? Brushes.Yellow : Brushes.Transparent
                        };
                        Grid.SetColumn(empty_brd, column);
                        Grid.SetRow(empty_brd, fill_row);
                        mainGrid.Children.Add(empty_brd);
                    }
                }
            }

            // add the marker of the day
            if((usedYear == today.Year) || ((today.Month == 1) && (usedYear + 1 == today.Year))) {
                Border marker = new Border() {
                    BorderThickness = markerBorder,
                    Margin = markerMargin,
                    BorderBrush = Brushes.Red
                };
                Grid.SetRow(marker, today.Day);
                Grid.SetColumn(marker, today.Month - 1);
                marker.Effect = new DropShadowEffect() { Color = new Color { A = 255, R = 200, G = 200, B = 250 }, Direction = 320, Opacity = 1 };
                mainGrid.Children.Add(marker);
            }
        }

        /// <summary>
        /// get the week of year (ISO8601 German):
        /// The first week is the first week having 4 or more days in that year
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        private int GetWeekOfYear(DateTime day)
        {
            DateTime Jan1 = new DateTime(day.Year, 1, 1);

            // calculate the days before 1st of Jan to Begin of 0'th week (monday)
            // e. g. if Jan 1st is a monday, we need to go back 7 days to monday of 0th week
            int diff_to_week0_start = ((int)Jan1.DayOfWeek + 2) % 7 + 4;

            return ((day - Jan1).Days + diff_to_week0_start) / 7;
        }

        // minimize on ESC
        private void Grid_KeyDown(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) {
                e.Handled = true;
                this.WindowState = WindowState.Minimized;
            }
        }
    }

    public class Holiday
    {
        private Dictionary<DateTime, String> holidays = new Dictionary<DateTime, string>();

        /// <summary>
        ///  fill the holidays dict with the holidays of the given year
        ///  and the january of the following year
        /// </summary>
        /// <param name="year"></param>
        public Holiday (int year)
        {
            // fixed holidays:
            holidays[new DateTime(year, 1, 1)] = "Neujahr";
            holidays[new DateTime(year, 1, 6)] = "Heilige 3 Könige";
            holidays[new DateTime(year, 10, 3)] = "Tag der dt. Einheit";
            holidays[new DateTime(year, 1, 1)] = "Neujahr";
            holidays[new DateTime(year, 12, 25)] = "1. Weihnachtstag";
            holidays[new DateTime(year, 12, 26)] = "2. Weihnachtstag";
            holidays[new DateTime(year+1, 1, 1)] = "Neujahr";
            holidays[new DateTime(year+1, 1, 6)] = "Heilige 3 Könige";

            // calc Date of Ostersonntag
            DateTime easterSunday = GetOsterSonntag(year);
            holidays[easterSunday - new TimeSpan(2, 0, 0, 0)] = "Karfreiag";
            holidays[easterSunday] = "Ostersonntag";
            holidays[easterSunday + new TimeSpan(1, 0, 0, 0)] = "Ostermontag";
            holidays[easterSunday + new TimeSpan(39, 0, 0, 0)] = "Chr Himmelfahrt";
            holidays[easterSunday + new TimeSpan(49, 0, 0, 0)] = "Pfingstsonntag";
            holidays[easterSunday + new TimeSpan(50, 0, 0, 0)] = "Pfingstmontag";
            holidays[easterSunday + new TimeSpan(60, 0, 0, 0)] = "Fronleichnam";
        }

        public bool IsHoliday(DateTime date)
        {
            return holidays.ContainsKey(StripDate(date));
        }

        public String GetHolidayName(DateTime date)
        {
            DateTime sDate = StripDate(date);
            if(holidays.ContainsKey(sDate)) {
                return holidays[sDate];
            } else {
                return "";
            }
        }

        /// <summary>
        /// remove time from date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private DateTime StripDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        private DateTime GetOsterSonntag(int year)
        {
            int c, i, j, k, l, n, easterDay, easterMonth;
            c = year / 100;
            n = year - 19 * ((int)(year / 19));
            k = (c - 17) / 25;
            i = c - c / 4 - ((int)(c - k) / 3) + 19 * n + 15;
            i = i - 30 * ((int)(i / 30));
            i = i - (i / 28) * ((int)(1 - (i / 28)) * ((int)(29 / (i + 1))) * ((int)(21 - n) / 11));
            j = year + ((int)year / 4) + i + 2 - c + ((int)c / 4);
            j = j - 7 * ((int)(j / 7));
            l = i - j;

            easterMonth = 3 + ((int)(l + 40) / 44);
            easterDay = l + 28 - 31 * ((int)easterMonth / 4);

            return new DateTime(year, easterMonth, easterDay);
        }
    }
}
