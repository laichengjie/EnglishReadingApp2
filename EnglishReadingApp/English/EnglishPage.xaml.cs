using Plugin.Maui.Audio;
using System.Diagnostics;

namespace EnglishReadingApp
{
    public partial class EnglishPage : ContentPage
    {
        private readonly IAudioManager _audioManager;
        private IAudioRecorder? _audioRecorder;
        private readonly string _currentSentence = "Hello, how are you?";
        private bool _isRecording = false;
        private CancellationTokenSource? _recordingCts;

        public EnglishPage(IAudioManager audioManager)
        {
            InitializeComponent();
            _audioManager = audioManager;
             
            // 页面卸载时释放资源
            this.Unloaded += OnPageUnloaded;
        }

        private async void OnSpeakClicked(object sender, EventArgs e)
        {
            try
            {
                SpeakButton.IsEnabled = false;

                // 检查 TTS 是否可用
                var locales = await TextToSpeech.Default.GetLocalesAsync();
                if (locales == null || !locales.Any())
                {
                    await DisplayAlert("提示", "当前设备不支持语音播放", "确定");
                    return;
                }

                await TextToSpeech.Default.SpeakAsync(_currentSentence, new SpeechOptions
                {
                    Volume = 1.0f,
                    Pitch = 1.0f
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"TTS错误: {ex.Message}");
                await DisplayAlert("错误", $"播放失败: {ex.Message}", "确定");
            }
            finally
            {
                SpeakButton.IsEnabled = true;
            }
        }

        private async void OnRecordClicked(object sender, EventArgs e)
        {
            // 如果正在录音，则停止录音
            if (_isRecording)
            {
                await StopRecording();
                return;
            }

            try
            {
                // 1. 请求麦克风权限
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

                // 2. 检查音频管理器
                if (_audioManager == null)
                {
                    await DisplayAlert("错误", "音频服务初始化失败", "确定");
                    return;
                }

                // 3. 开始录音
                await StartRecording();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"录音初始化错误: {ex.Message}");
                await DisplayAlert("错误", $"准备录音失败: {ex.Message}", "确定");

                // 清理资源
               
                _audioRecorder = null;
                _isRecording = false;
                RecordButton.Text = "🎤 开始跟读";
                RecordButton.BackgroundColor = Color.FromArgb("#27AE60");
                RecordButton.IsEnabled = true;
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private async Task StartRecording()
        {
            _isRecording = true;
            _recordingCts = new CancellationTokenSource();

            // 更新UI
            RecordButton.Text = "⏹️ 停止录音";
            RecordButton.BackgroundColor = Colors.Red;
            RecordButton.IsEnabled = true;

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            // 隐藏之前的结果
            var resultFrame = RecognizedTextLabel.Parent as Frame;
            if (resultFrame != null)
                resultFrame.IsVisible = false;

            var scoreFrame = ScoreLabel.Parent as Frame;
            if (scoreFrame != null)
                scoreFrame.IsVisible = false;

            try
            {
                // 创建录音器
                _audioRecorder = _audioManager.CreateRecorder();

                // 开始录音
                await _audioRecorder.StartAsync();

                // 倒计时提示（根据XAML中的提示，4秒）
                await ShowRecordingCountdown();

                // 如果没有被手动停止，自动停止
                if (_isRecording)
                {
                    await StopRecording();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"录音过程错误: {ex.Message}");
                throw;
            }
        }

        private async Task ShowRecordingCountdown()
        {
            for (int i = 4; i > 0 && _isRecording; i--)
            {
                RecordButton.Text = $"🎤 录音中... {i}秒";
                try
                {
                    await Task.Delay(1000, _recordingCts!.Token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private async Task StopRecording()
        {
            if (!_isRecording || _audioRecorder == null)
                return;

            try
            {
                _isRecording = false;
                RecordButton.Text = "🎤 识别中...";
                RecordButton.IsEnabled = false;

                // 停止录音
                var audioSource = await _audioRecorder.StopAsync();

                // 释放资源
                
                _audioRecorder = null;

                // 处理录音结果
                await ProcessRecordingResult(audioSource);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"停止录音错误: {ex.Message}");
                throw;
            }
            finally
            {
                _recordingCts?.Cancel();
                _recordingCts?.Dispose();
                _recordingCts = null;

                RecordButton.Text = "🎤 开始跟读";
                RecordButton.BackgroundColor = Color.FromArgb("#27AE60");
                RecordButton.IsEnabled = true;
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private async Task ProcessRecordingResult(IAudioSource audioSource)
        {
            try
            {
                // 模拟识别过程
                await Task.Delay(1500);

                // 模拟识别结果
                string recognizedText = await SimulateSpeechRecognition();

                // 显示识别结果
                var resultFrame = RecognizedTextLabel.Parent as Frame;
                if (resultFrame != null)
                {
                    RecognizedTextLabel.Text = $"你说的：{recognizedText}";
                    resultFrame.IsVisible = true;
                }

                // 计算评分
                double score = CalculateScore(recognizedText, _currentSentence);

                // 显示评分结果
                var scoreFrame = ScoreLabel.Parent as Frame;
                if (scoreFrame != null)
                {
                    var (message, color) = GetScoreMessage(score);
                    ScoreLabel.Text = message;
                    ScoreLabel.TextColor = color;
                    scoreFrame.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"处理录音结果错误: {ex.Message}");
                await DisplayAlert("错误", $"处理录音失败: {ex.Message}", "确定");
            }
        }

        private async Task<string> SimulateSpeechRecognition()
        {
            await Task.Delay(500);

            var random = new Random();
            int accuracy = random.Next(0, 100);

            if (accuracy > 80)
                return _currentSentence;
            else if (accuracy > 50)
                return _currentSentence.ToLower();
            else if (accuracy > 30)
                return _currentSentence.Replace("how", "howw");
            else
                return "Hello";
        }

        private double CalculateScore(string recognizedText, string referenceText)
        {
            if (string.IsNullOrWhiteSpace(recognizedText))
                return 0;

            var cleanRecognized = new string(recognizedText.ToLower().Where(char.IsLetterOrDigit).ToArray());
            var cleanReference = new string(referenceText.ToLower().Where(char.IsLetterOrDigit).ToArray());

            if (cleanRecognized == cleanReference)
                return 100;

            if (cleanReference.Contains(cleanRecognized) || cleanRecognized.Contains(cleanReference))
                return 70;

            return 50;
        }

        private (string message, Color color) GetScoreMessage(double score)
        {
            if (score >= 90)
                return ("🎉 完美！发音非常标准！", Colors.Green);
            else if (score >= 80)
                return ("👍 很好！继续努力！", Colors.Blue);
            else if (score >= 70)
                return ("📖 不错，再练习一下会更好！", Colors.Orange);
            else if (score >= 60)
                return ("💪 加油，再听一遍示范吧！", Colors.OrangeRed);
            else
                return ("🎧 建议先听示范，然后慢慢跟读", Colors.Red);
        }

        private void OnPageUnloaded(object sender, EventArgs e)
        {
            _recordingCts?.Cancel();
            _recordingCts?.Dispose();
            _audioRecorder = null;
        }
    }
}