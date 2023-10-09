namespace MChess.Views;

public partial class TimeControlPage : ContentPage
{
    public TimeControlPage()
    {
        InitializeComponent();
        time.Value = ((App)App.Current).StartTime;
        increment.Value = ((App)App.Current).TimeAdded;
    }
    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
    private void TimeButton_Clicked(object sender, EventArgs e)
    {
        Button b = (Button)sender;
        int separator = b.Text.IndexOf('+');
        time.Value = int.Parse(b.Text[..separator]);
        increment.Value = int.Parse(b.Text.Substring(separator + 1, b.Text.Length - 1 - separator));
    }
    private void Apply_Changes(object sender, EventArgs e)
    {
        ((App)App.Current).StartTime = time.Value;
        ((App)App.Current).TimeAdded = increment.Value;
        Navigation.PopAsync();
    }
    private void NewValue(object sender, EventArgs e)
    {
        if (time.Value != ((App)App.Current).StartTime || increment.Value != ((App)App.Current).TimeAdded)
            save.IsEnabled = true;
        else
            save.IsEnabled = false;
    }
}