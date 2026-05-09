using Plugin.Maui.Audio;

namespace EnglishReadingApp;

public partial class ChinesePage : ContentPage
{
    private readonly IAudioManager _audioManager;
    private IAudioRecorder? _audioRecorder;
    private string _currentContent = "";
    private string _currentPinyin = "";
    private string _currentExplanation = "";
    private string _currentCategory = "pinyin";
    private bool _isRecording = false;

    // 题库
    private readonly Dictionary<string, List<(string text, string pinyin, string explanation)>> _questions = new()
    {
        ["pinyin"] = new()
        {
            ("bà ba", "爸爸", "父亲"),
            ("mā ma", "妈妈", "母亲"),
            ("lǎo shī", "老师", "教师"),
            ("tóng xué", "同学", "同学"),
            ("shū", "书", "书籍")
        },
        ["chinese"] = new()
        {
            ("我", "wǒ", "自己"),
            ("你", "nǐ", "对方"),
            ("他", "tā", "他人"),
            ("好", "hǎo", "美好"),
            ("大", "dà", "巨大")
        },
        ["poetry"] = new()
        {
            ("床前明月光", "chuáng qián míng yuè guāng", "《静夜思》- 李白"),
            ("疑是地上霜", "yí shì dì shàng shuāng", "怀疑是地上的霜雪"),
            ("举头望明月", "jǔ tóu wàng míng yuè", "抬头望着天上的明月"),
            ("低头思故乡", "dī tóu sī gù xiāng", "低头思念自己的家乡")
        }
    };

    private int _currentIndex = 0;

    public ChinesePage()
    {
        InitializeComponent();
        _audioManager = AudioManager.Current;

        // 初始化Picker选项
        CategoryPicker.Items.Add("拼音学习");
        CategoryPicker.Items.Add("汉字学习");
        CategoryPicker.Items.Add("古诗词");
        CategoryPicker.SelectedIndex = 0;
    }

    private void OnCategoryChanged(object sender, EventArgs e)
    {
        _currentIndex = 0;
        _currentCategory = CategoryPicker.SelectedIndex switch
        {
            0 => "pinyin",
            1 => "chinese",
            2 => "poetry",
            _ => "pinyin"
        };

        LoadCurrentContent();
        UpdateUIForCategory();
    }

    private void LoadCurrentContent()
    {
        var questions = _questions[_currentCategory];
        if (_currentIndex < questions.Count)
        {
            var item = questions[_currentIndex];
            _currentContent = item.text;
            _currentPinyin = item.pinyin;
            _currentExplanation = item.explanation;

            ContentLabel.Text = _currentContent;

            // 显示拼音和解释
            if (_currentCategory == "pinyin")
            {
                PinyinLabel.Text = _currentPinyin;
                PinyinLabel.IsVisible = true;
                ExplanationLabel.Text = _currentExplanation;
                ExplanationLabel.IsVisible = true;
            }
            else if (_currentCategory == "chinese")
            {
                PinyinLabel.Text = _currentPinyin;
                PinyinLabel.IsVisible = true;
                ExplanationLabel.Text = _currentExplanation;
                ExplanationLabel.IsVisible = true;
            }
            else // poetry
            {
                PinyinLabel.Text = _currentPinyin;
                PinyinLabel.IsVisible = true;
                ExplanationLabel.Text = _currentExplanation;
                ExplanationLabel.IsVisible = true;
            }
        }
    }

    private void UpdateUIForCategory()
    {
        // 根据类别调整界面
        if (_currentCategory == "pinyin")
        {
            ContentLabel.FontSize = 28;
        }
        else if (_currentCategory == "chinese")
        {
            ContentLabel.FontSize = 36;
        }
        else
        {
            ContentLabel.FontSize = 24;
        }

        // 显示下一题按钮
        NextButton.IsVisible = true;
    }

    private async void OnSpeakClicked(object sender, EventArgs e)
    {
        try
        {
            SpeakButton.IsEnabled = false;

            string speakText = _currentCategory == "pinyin" ? _currentPinyin : _currentContent;
            await TextToSpeech.Default.SpeakAsync(speakText, new SpeechOptions
            {
                Volume = 1.0f,
                Pitch = 1.0f
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("错误", $"播放失败: {ex.Message}", "确定");
        }
        finally
        {
            SpeakButton.IsEnabled = true;
        }
    }

    private async void OnRecordClicked(object sender, EventArgs e)
    {
        if (_isRecording)
        {
            await StopRecording();
            return;
        }

        var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("权限不足", "请允许麦克风权限", "好的");
                return;
            }
        }

        try
        {
            _isRecording = true;
            RecordButton.Text = "⏹️ 停止录音";
            RecordButton.BackgroundColor = Colors.Red;
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            // 隐藏之前的结果
            ResultFrame.IsVisible = false;
            ScoreFrame.IsVisible = false;

            _audioRecorder = _audioManager.CreateRecorder();
            await _audioRecorder.StartAsync();

            // 录音3秒
            for (int i = 3; i > 0; i--)
            {
                RecordButton.Text = $"🎤 录音中... {i}秒";
                await Task.Delay(1000);
            }

            await StopRecording();
        }
        catch (Exception ex)
        {
            await DisplayAlert("错误", $"录音失败: {ex.Message}", "确定");
            _isRecording = false;
            RecordButton.Text = "🎤 开始跟读";
            RecordButton.BackgroundColor = Color.FromArgb("#27AE60");
            LoadingIndicator.IsVisible = false;
        }
    }

    private async Task StopRecording()
    {
        if (_audioRecorder == null) return;

        RecordButton.Text = "🎤 识别中...";

        var audioSource = await _audioRecorder.StopAsync();
        _audioRecorder = null;

        // 模拟识别（实际应接入真正的语音识别）
        await Task.Delay(1500);

        // 模拟识别结果
        string recognizedText = _currentCategory == "pinyin" ? _currentPinyin : _currentContent;

        RecognizedTextLabel.Text = $"你说的：{recognizedText}";
        ResultFrame.IsVisible = true;

        // 简单评分
        ScoreLabel.Text = "🎉 很棒！继续加油！";
        ScoreFrame.IsVisible = true;

        _isRecording = false;
        RecordButton.Text = "🎤 开始跟读";
        RecordButton.BackgroundColor = Color.FromArgb("#27AE60");
        LoadingIndicator.IsVisible = false;
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        var questions = _questions[_currentCategory];
        _currentIndex++;

        if (_currentIndex >= questions.Count)
        {
            _currentIndex = 0;
            await DisplayAlert("恭喜", "已完成当前类别所有内容！", "确定");
        }

        LoadCurrentContent();

        // 隐藏结果
        ResultFrame.IsVisible = false;
        ScoreFrame.IsVisible = false;
    }
}