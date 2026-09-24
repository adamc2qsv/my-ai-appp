using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyAiLanguageCompanion;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(IServiceProvider services)
    {
        InitializeComponent();
        _viewModel = new MainViewModel(services);
        DataContext = _viewModel;
    }

    private async void StartLiveShare_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.StartLiveShareAsync();
    }

    private async void StopLiveShare_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.StopLiveShareAsync();
    }

    private async void Translate_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.TranslateAsync();
    }

    private async void Practice_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.RunPracticeAsync();
    }

    private async void SpeakWord_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.SpeakSelectedWordAsync();
    }

    private async void SpeakSentence_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.SpeakSelectedSentenceAsync();
    }
}
