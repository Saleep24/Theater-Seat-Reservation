using System.Collections.Specialized;
using System.Reflection.Metadata;

namespace TheaterSeatReservation
{
    public class SeatingUnit
    {
        public string Name { get; set; }
        public bool Reserved { get; set; }

        public SeatingUnit(string name, bool reserved = false)
        {
            Name = name;
            Reserved = reserved;
        }

    }

    public partial class MainPage : ContentPage
    {
        SeatingUnit[,] seatingChart = new SeatingUnit[5, 10];

        public MainPage()
        {
            InitializeComponent();
            GenerateSeatingNames();
            RefreshSeating();
        }

        private async void ButtonReserveSeat(object sender, EventArgs e)
        {
            var seat = await DisplayPromptAsync("Enter Seat Number", "Enter seat number: ");

            if (seat != null)
            {
                for (int i = 0; i < seatingChart.GetLength(0); i++)
                {
                    for (int j = 0; j < seatingChart.GetLength(1); j++)
                    {
                        if (seatingChart[i, j].Name == seat)
                        {
                            seatingChart[i, j].Reserved = true;
                            await DisplayAlert("Successfully Reserverd", "Your seat was reserverd successfully!", "Ok");
                            RefreshSeating();
                            return;
                        }
                    }
                }

                await DisplayAlert("Error", "Seat was not found.", "Ok");
            }
        }

        private void GenerateSeatingNames()
        {
            List<string> letters = new List<string>();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                letters.Add(c.ToString());
            }

            int letterIndex = 0;

            for (int row = 0; row < seatingChart.GetLength(0); row++)
            {
                for (int column = 0; column < seatingChart.GetLength(1); column++)
                {
                    seatingChart[row, column] = new SeatingUnit(letters[letterIndex] + (column + 1).ToString());
                }

                letterIndex++;
            }
        }

        private void RefreshSeating()
        {
            grdSeatingView.RowDefinitions.Clear();
            grdSeatingView.ColumnDefinitions.Clear();
            grdSeatingView.Children.Clear();

            for (int row = 0; row < seatingChart.GetLength(0); row++)
            {
                var grdRow = new RowDefinition();
                grdRow.Height = 50;

                grdSeatingView.RowDefinitions.Add(grdRow);

                for (int column = 0; column < seatingChart.GetLength(1); column++)
                {
                    var grdColumn = new ColumnDefinition();
                    grdColumn.Width = 50;

                    grdSeatingView.ColumnDefinitions.Add(grdColumn);

                    var text = seatingChart[row, column].Name;

                    var seatLabel = new Label();
                    seatLabel.Text = text;
                    seatLabel.HorizontalOptions = LayoutOptions.Center;
                    seatLabel.VerticalOptions = LayoutOptions.Center;
                    seatLabel.BackgroundColor = Color.Parse("#333388");
                    seatLabel.Padding = 10;

                    if (seatingChart[row, column].Reserved == true)
                    {
                        //Change the color of this seat to represent its reserved.
                        seatLabel.BackgroundColor = Color.Parse("#883333");

                    }

                    Grid.SetRow(seatLabel, row);
                    Grid.SetColumn(seatLabel, column);
                    grdSeatingView.Children.Add(seatLabel);

                }
            }
        }

        //Implemented by Saleep Shrestha
        private async void ButtonReserveRange(object sender, EventArgs e)
        {
            //Saleep Shrestha
            //w10167735
            //I am working on the Feature to bulk reserve a range of seats
            // Finished Working On The Feature And The Feature Successfully Reserves Seat Ranges When Valid
            // Proper Error Handling: When Input Format is Invalid - Displays "Invalid format" Error
            // Proper Error Handling: When Seats Are in Different Rows - Displays "Same row" Error
            // Proper Error Handling: When Start Column > End Column - Displays "Start before end" Error
            // Proper Error Handling: When Any Seat in Range is Already Reserved - Displays "Unavailable" Error
            // Successfully Reserves All Seats in Range When Valid and Available
            
            string seatRange = await DisplayPromptAsync("Reserve Seat Range", 
                "Enter range (e.g., A1:A4):", "OK", "Cancel", "A1:A4");
            
            if (!string.IsNullOrEmpty(seatRange))
            {
                try
                {

                    string[] seats = seatRange.Split(':');
                    if (seats.Length != 2)
                    {
                        await DisplayAlert("Error", "Invalid format. Use format like A1:A4", "OK");
                        return;
                    }

                    string startSeat = seats[0].Trim().ToUpper();
                    string endSeat = seats[1].Trim().ToUpper();

                    if (!IsValidSeat(startSeat) || !IsValidSeat(endSeat))
                    {
                        await DisplayAlert("Error", "Invalid seat format", "OK");
                        return;
                    }
                    
    
                    char startRow = startSeat[0];
                    char endRow = endSeat[0];
                    int startCol = int.Parse(startSeat.Substring(1)) - 1; 
                    int endCol = int.Parse(endSeat.Substring(1)) - 1;


                    if (startRow != endRow)
                {
                    await DisplayAlert("Error", "Seats must be in the same row", "OK");
                    return;
                }


                if (startCol > endCol)
                {
                    await DisplayAlert("Error", "Start seat must be before end seat", "OK");
                    return;
                }


                int rowIndex = startRow - 'A';
            

                if (rowIndex < 0 || rowIndex >= seatingChart.GetLength(0))
                {
                    await DisplayAlert("Error", "Invalid row", "OK");
                    return;
                }

 
                bool allAvailable = true;
                for (int col = startCol; col <= endCol; col++)
                {

                    if (col < 0 || col >= seatingChart.GetLength(1))
                    {
                        allAvailable = false;
                        break;
                    }

                    if (seatingChart[rowIndex, col].Reserved)
                    {
                        allAvailable = false;
                        break;
                    }
                }

                if (allAvailable)
                {
     
                    for (int col = startCol; col <= endCol; col++)
                    {
                        seatingChart[rowIndex, col].Reserved = true;
                    }
                    RefreshSeating();
                    await DisplayAlert("Success", $"Reserved seats {startSeat} to {endSeat}", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "One or more seats are unavailable please re-check", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Invalid input: {ex.Message}", "OK");
            }
        }
    }


    private bool IsValidSeat(string seat)
    {
        if (string.IsNullOrEmpty(seat) || seat.Length < 2)
            return false;

        char row = seat[0];
        string colPart = seat.Substring(1);

        if (!char.IsLetter(row))
            return false;

        if (!int.TryParse(colPart, out int col))
            return false;

        return col >= 1 && col <= 10;
            }
    

        // Implemented By Rabindra Giri
        private async void ButtonCancelReservation(object sender, EventArgs e)
        {
            // Rabindra Giri
            // w10176279
            // I am working on the feature to cancel reservation 
            // Finished Working On The Feature And The Feature Successfully UnReserves A Seat If Seat Is Reserved
            // Proper Error Handling: When Seat Number is Invalid : Displays Error No Seat Found
            // Proper Error Handling: When Seat Number is Valid But Unreserved: Displays Error Seat is Unreserved
            // Successfully UnReserves The Seat When The Seat Is Valid and Is Reserved
            var seat = await DisplayPromptAsync("Enter A Seat Number To Remove", "Enter seat number: ");

            if (seat != null)
            {
                
                for (int i = 0; i < seatingChart.GetLength(0); i++)
                {
                    for (int j = 0; j < seatingChart.GetLength(1); j++)
                    {
                        if (seatingChart[i, j].Name == seat)
                        {
                            if(seatingChart[i, j].Reserved == false){
                            await DisplayAlert("Error No Reservation For That Seat Found", "You are lucky there is no reservations for that seat.", "Ok");
                            return;
                            }
                            else if(seatingChart[i, j].Reserved == true){
                            seatingChart[i, j].Reserved = false;
                            await DisplayAlert("Successfully Unreserverd", "The seat reservation was cancelled!", "Ok");
                            RefreshSeating();
                            return;
                            }
                        }
                    }
                }

            }
            await DisplayAlert("Error", "Seat was not found.", "Ok");
        }

        //Assign to Team 3 Member
        private async void ButtonCancelReservationRange(object sender, EventArgs e)
        {
            string seatRange = await DisplayPromptAsync("Cancel Reservation Range",
                "Enter range (e.g., A1:A4):", "OK", "Cancel", "A1:A4");

            if (!string.IsNullOrEmpty(seatRange))
            {
                try
                {
                    string[] seats = seatRange.Split(':');
                    if (seats.Length != 2)
                    {
                        await DisplayAlert("Error", "Invalid format. Use format like A1:A4", "OK");
                        return;
                    }

                    string startSeat = seats[0].Trim().ToUpper();
                    string endSeat = seats[1].Trim().ToUpper();

                    if (!IsValidSeat(startSeat) || !IsValidSeat(endSeat))
                    {
                        await DisplayAlert("Error", "Invalid seat format", "OK");
                        return;
                    }

                    char startRow = startSeat[0];
                    char endRow = endSeat[0];
                    int startCol = int.Parse(startSeat.Substring(1)) - 1;
                    int endCol = int.Parse(endSeat.Substring(1)) - 1;

                    if (startRow != endRow)
                    {
                        await DisplayAlert("Error", "Seats must be in the same row", "OK");
                        return;
                    }

                    if (startCol > endCol)
                    {
                        await DisplayAlert("Error", "Start seat must be before end seat", "OK");
                        return;
                    }

                    int rowIndex = startRow - 'A';

                    if (rowIndex < 0 || rowIndex >= seatingChart.GetLength(0))
                    {
                        await DisplayAlert("Error", "Invalid row", "OK");
                        return;
                    }

                    // Check if all seats in the range are reserved
                    bool allReserved = true;
                    for (int col = startCol; col <= endCol; col++)
                    {
                        if (!seatingChart[rowIndex, col].Reserved)
                        {
                            allReserved = false;
                            break;
                        }
                    }

                    if (allReserved)
                    {
                        // Unreserve the seats
                        for (int col = startCol; col <= endCol; col++)
                        {
                            seatingChart[rowIndex, col].Reserved = false;
                        }
                        RefreshSeating();
                        await DisplayAlert("Success", $"Cancelled reservations for seats {startSeat} to {endSeat}", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "One or more seats are not reserved", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Invalid input: {ex.Message}", "OK");
                }
            }
        }


        //Assign to Team 4 Member
        private void ButtonResetSeatingChart(object sender, EventArgs e)
        {

        }
    }

}