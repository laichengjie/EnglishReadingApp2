using Plugin.Maui.Audio;


namespace EnglishReadingApp
{
    public partial class MainPage : ContentPage
    {
        private readonly IAudioManager _audioManager;
        private IAudioRecorder _audioRecorder;
        private readonly string _currentSentence = "Hello, how are you?";

        // 通过构造函数注入 IAudioManager
        public MainPage(IAudioManager audioManager)
        {
            InitializeComponent();
            _audioManager = audioManager;
        }

        private async void OnSpeakClicked(object sender, EventArgs e)
        {
            await TextToSpeech.Default.SpeakAsync(_currentSentence);
        }

        private async void OnRecordClicked(object sender, EventArgs e)
        {
            // 请求麦克风权限
            var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Microphone>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("权限不足", "请允许麦克风权限以使用跟读功能", "好的");
                    return;
                }
            }

            // 开始录音
            RecordButton.Text = "🎤 正在录音...";
            RecordButton.IsEnabled = false;

            _audioRecorder = _audioManager.CreateRecorder();
            await _audioRecorder.StartAsync();

            // 等待4秒录音
            await Task.Delay(4000);

            // 停止录音
            var audioRecording = await _audioRecorder.StopAsync();
            RecordButton.Text = "🎤 识别中...";

            // TODO: 在这里添加语音识别逻辑
            string recognizedText = _currentSentence; // 临时模拟

            RecognizedTextLabel.Text = $"你说的是：{recognizedText}";

            // 简单评分
            if (recognizedText.Equals(_currentSentence, StringComparison.OrdinalIgnoreCase))
                ScoreLabel.Text = "🎉 完美！发音很棒！";
            else
                ScoreLabel.Text = "👍 还不错，继续努力！";

            RecordButton.Text = "🎤 开始跟读";
            RecordButton.IsEnabled = true;
        }
    }

}