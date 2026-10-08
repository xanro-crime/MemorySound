using Plugin.Maui.Audio;

namespace MemorySound;

public partial class MainPage : ContentPage
{
    private List<Button> gameSequence = new List<Button>();
    private List<Button> playerSequence = new List<Button>();
    private Random random = new Random();

    private bool isPlayingSequence = false;
    private bool isInputLocked = false;
    private bool isGameOver = false;

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
        playerSequence.Clear();
        isGameOver = false;
        isInputLocked = false;
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
        isInputLocked = true;

        foreach (var button in gameSequence)
        {
            await FlashButton(button);
            await Task.Delay(300);
        }

        isPlayingSequence = false;
        isInputLocked = false;
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
        try
        {
            string soundName = "";
            if (button == BtnRed) soundName = "sound1.mp3";
            else if (button == BtnGreen) soundName = "sound2.mp3";
            else if (button == BtnYellow) soundName = "sound3.mp3";
            else if (button == BtnBlue) soundName = "sound4.mp3";

            var audioPlayer = AudioManager.Current.CreatePlayer(
                await FileSystem.OpenAppPackageFileAsync(soundName));
            audioPlayer.Play();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Sound error: {ex}");
        }
    }

    private async void OnButtonClicked(object sender, EventArgs e)
    {
        if (isPlayingSequence || isInputLocked || isGameOver) return;
        if (sender is not Button clickedButton) return;

        try
        {
            isInputLocked = true;

            await FlashButton(clickedButton);

            playerSequence.Add(clickedButton);

            int i = playerSequence.Count - 1;


            if (i >= gameSequence.Count) return;

            if (playerSequence[i] != gameSequence[i])
            {
                isGameOver = true;
                await DisplayAlertAsync("Игра окончена",
                    $"Вы набрали {gameSequence.Count - 1} очков!", "Повторить");
                StartNewGame();
                return;
            }

            if (playerSequence.Count == gameSequence.Count)
            {
                await Task.Delay(1000);
                if (!isGameOver) NextRound();
                return; 
            }

            isInputLocked = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Click error: {ex}");
            isInputLocked = false;
        }
    }
}