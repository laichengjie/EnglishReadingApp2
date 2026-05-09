namespace EnglishReadingApp;

public partial class MathPage : ContentPage
{
    private Random _random = new Random();
    private int _currentAnswer;
    private string _currentExpression;
    private int _correctCount = 0;
    private int _incorrectCount = 0;
    private string _userInput = "";

    public MathPage()
    {
        InitializeComponent();
        GenerateNewQuestion();
    }

    private void GenerateNewQuestion()
    {
        int num1 = _random.Next(1, 101);
        int num2 = _random.Next(1, 101);
        bool isAddition = _random.Next(0, 2) == 0;

        if (isAddition)
        {
            _currentAnswer = num1 + num2;
            _currentExpression = $"{num1} + {num2} = ?";
        }
        else
        {
            // 确保结果为正数
            if (num1 < num2)
            {
                (num1, num2) = (num2, num1);
            }
            _currentAnswer = num1 - num2;
            _currentExpression = $"{num1} - {num2} = ?";
        }

        QuestionLabel.Text = _currentExpression;
        _userInput = "";
        InputLabel.Text = "";
        FeedbackLabel.Text = "";
        NextButton.IsVisible = false;
    }

    private void OnNumberClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        _userInput += button.Text;
        InputLabel.Text = _userInput;
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_userInput.Length > 0)
        {
            _userInput = _userInput.Substring(0, _userInput.Length - 1);
            InputLabel.Text = _userInput;
        }
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        _userInput = "";
        InputLabel.Text = "";
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_userInput))
        {
            await DisplayAlert("提示", "请输入答案", "确定");
            return;
        }

        int userAnswer;
        if (!int.TryParse(_userInput, out userAnswer))
        {
            await DisplayAlert("提示", "请输入有效的数字", "确定");
            return;
        }

        if (userAnswer == _currentAnswer)
        {
            _correctCount++;
            CorrectCountLabel.Text = _correctCount.ToString();
            FeedbackLabel.Text = "✅ 回答正确！很棒！";
            FeedbackLabel.TextColor = Colors.Green;

            // 播放正确提示音
            await TextToSpeech.Default.SpeakAsync("正确，继续加油");
        }
        else
        {
            _incorrectCount++;
            IncorrectCountLabel.Text = _incorrectCount.ToString();
            FeedbackLabel.Text = $"❌ 回答错误！正确答案是 {_currentAnswer}，继续努力！";
            FeedbackLabel.TextColor = Colors.Red;

            // 播放错误提示音
            await TextToSpeech.Default.SpeakAsync($"错误，答案是{_currentAnswer}");
        }

        NextButton.IsVisible = true;
    }

    private async void OnNextQuestionClicked(object sender, EventArgs e)
    {
        GenerateNewQuestion();
    }
}