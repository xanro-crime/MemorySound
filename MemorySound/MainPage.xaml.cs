using Plugin.Maui.Audio;

namespace MemorySound;

public partial class MainPage : ContentPage
{
    private List<Button> gameSequence = new List<Button>();
    private List<Button> playerSequence = new List<Button>();
    private Random random = new Random();
    private bool isPlayingSequence = false;

    private Button[] buttons;

    public MainPage()
    {
        InitializeComponent();

        buttons = new Button[] { BtnRed, BtnGreen, BtnYellow, BtnBlue };

        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(1000), () =>
        {
            StartNewGame();
        });
    }

    private void StartNewGame()
    {
        gameSequence.Clear();
        NextRound();
    }

    private async void NextRound()
    {
        playerSequence.Clear();

        int randomIndex = random.Next(0, 4);
        gameSequence.Add(buttons[randomIndex]);

        await PlaySequence();
    }

    private async Task PlaySequence()
    {
        isPlayingSequence = true;

        foreach (var button in gameSequence)
        {
            await FlashButton(button);
            await Task.Delay(300);
        }

        isPlayingSequence = false;
    }

    private async Task FlashButton(Button button)
    {
        var originalColor = button.BackgroundColor;

        button.BackgroundColor = originalColor.WithAlpha(0.5f);

        PlaySoundForButton(button);

        await Task.Delay(400);

        button.BackgroundColor = originalColor;
    }

    private async void PlaySoundForButton(Button button)
    {
        string soundName = "";
        if (button == BtnRed) soundName = "sound1.mp3";
        else if (button == BtnGreen) soundName = "sound2.mp3";
        else if (button == BtnYellow) soundName = "sound3.mp3";
        else if (button == BtnBlue) soundName = "sound4.mp3";

        var audioPlayer = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(soundName));
        audioPlayer.Play();
    }

    private async void OnButtonClicked(object sender, EventArgs e)
    {

        if (isPlayingSequence) return;

        Button clickedButton = (Button)sender;

        await FlashButton(clickedButton);

        playerSequence.Add(clickedButton);

        int currentCheckIndex = playerSequence.Count - 1;

        if (playerSequence[currentCheckIndex] != gameSequence[currentCheckIndex])
        {
            await DisplayAlertAsync("Игра окончена", $"Вы набрали {gameSequence.Count - 1} очков!", "Повторить");
            StartNewGame();
            return;
        }

        if (playerSequence.Count == gameSequence.Count)
        {
            await Task.Delay(1000);
            NextRound();
        }
    }
}
