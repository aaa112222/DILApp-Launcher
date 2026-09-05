using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using CmlLib.Core.Auth;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DILApp;

public partial class LauncherPanel : UserControl
{
	private class VersionComparer : IComparer<string>
	{
		public int Compare(string? x, string? y)
		{
			if (x == null || y == null)
			{
				return 0;
			}
			string[] array = x.Split('.');
			string[] array2 = y.Split('.');
			for (int i = 0; i < Math.Min(array.Length, array2.Length); i++)
			{
				if (int.TryParse(array[i], out var result) && int.TryParse(array2[i], out var result2))
				{
					if (result != result2)
					{
						return result.CompareTo(result2);
					}
					continue;
				}
				return string.CompareOrdinal(array[i], array2[i]);
			}
			return array.Length.CompareTo(array2.Length);
		}
	}

	private TranslateTransform _panelSlide;

	private readonly List<(string id, string type)> _allVersions = new List<(string, string)>();

	private static readonly Brush CardBgBrush;

	private static readonly Brush CardHoverBrush;

	private static readonly Brush CardSelectedBrush;

	private static readonly Brush IconBgBrush;

	private static readonly Brush TextWhiteBrush;

	private static readonly Brush TextDescBrush;

	private static readonly Brush TextGrayBrush;

	private static readonly Brush TextHintBrush;

	private static readonly Brush TagBgBrush;

	private static readonly Brush VersionCardBgBrush;

	private static readonly Brush VersionCardHoverBrush;

	private static readonly Brush VersionCardBorderBrush;

	private static readonly Brush CategoryInactiveBgBrush;

	private static readonly Brush CategoryInactiveTextBrush;

	private static readonly Brush ErrorBrush;

	private static readonly Brush HintBrush;

	private static readonly HttpClient SharedImageClient;

	private static readonly DropShadowEffect SharedCardShadow;

	private static readonly SolidColorBrush Bg37Brush;
	private static readonly SolidColorBrush Bg42Brush;
	private static readonly SolidColorBrush Bg32Brush;
	private static readonly SolidColorBrush Bg34Brush;
	private static readonly SolidColorBrush Bg45Brush;
	private static readonly SolidColorBrush Bg50Brush;
	private static readonly SolidColorBrush Bg55Brush;
	private static readonly SolidColorBrush Bg60Brush;
	private static readonly SolidColorBrush Border60Brush;
	private static readonly SolidColorBrush Border45Brush;
	private static readonly SolidColorBrush Blue72Brush;
	private static readonly SolidColorBrush Blue100Brush;
	private static readonly SolidColorBrush Blue160Brush;
	private static readonly SolidColorBrush Green80Brush;
	private static readonly SolidColorBrush Yellow200Brush;
	private static readonly SolidColorBrush Gray100Brush;
	private static readonly SolidColorBrush Gray110Brush;
	private static readonly SolidColorBrush Gray120Brush;
	private static readonly SolidColorBrush Gray130Brush;
	private static readonly SolidColorBrush Gray136Brush;
	private static readonly SolidColorBrush Gray140Brush;
	private static readonly SolidColorBrush Gray150Brush;
	private static readonly SolidColorBrush Gray153Brush;
	private static readonly SolidColorBrush Gray170Brush;
	private static readonly SolidColorBrush Gray200Brush;
	private static readonly SolidColorBrush Gray220Brush;
	private static readonly SolidColorBrush Red80Brush;
	private static readonly SolidColorBrush Red200Brush;
	private static readonly SolidColorBrush Bg30Brush;
	private static readonly SolidColorBrush Bg38Brush;
	private static readonly SolidColorBrush Bg44Brush;
	private static readonly SolidColorBrush Bg48Brush;
	private static readonly SolidColorBrush Bg52Brush;
	private static readonly SolidColorBrush Bg62Brush;
	private static readonly SolidColorBrush Blue40Brush;
	private static readonly SolidColorBrush Gray80Brush;
	private static readonly SolidColorBrush Gray102Brush;
	private static readonly SolidColorBrush Gray160Brush;
	private static readonly SolidColorBrush Gray204Brush;
	private static readonly SolidColorBrush Red245Brush;
	private static readonly SolidColorBrush Border61Brush;

	private string _selectedVersionId = "";

	private string? _selectedLoaderName = null;

	private string? _selectedLoaderVersion = null;

	private string? _selectedOptifineVersion = null;

	private string _currentCategory = "\u5168\u90E8";

	private Border? _selectedCategoryBorder = null;

	private int _versionRenderToken;

	private int _listEnterMode;

	private int _staggerIndex;

	private static readonly IEasingFunction EaseOut;

	private static readonly IEasingFunction EaseIn;

	private string _currentResourceType = "\u6E38\u620F";

	private List<ModrinthProject> _searchResults = new List<ModrinthProject>();

	private Dictionary<int, Border> _taskCards = new Dictionary<int, Border>();

	private Dictionary<int, ProgressBar> _taskCardProgresses = new Dictionary<int, ProgressBar>();

	private Dictionary<int, TextBlock> _taskCardSpeeds = new Dictionary<int, TextBlock>();

	private Dictionary<int, TextBlock> _taskCardSteps = new Dictionary<int, TextBlock>();

	private Dictionary<int, TextBlock> _taskCardStatuses = new Dictionary<int, TextBlock>();

	private Dictionary<int, TextBlock> _taskCardTitles = new Dictionary<int, TextBlock>();

	private DateTime _lastCardUpdate = DateTime.MinValue;

	private readonly Dictionary<ScrollViewer, double> _scrollTargets = new Dictionary<ScrollViewer, double>();

	private readonly Dictionary<ScrollViewer, double> _scrollStartOffsets = new Dictionary<ScrollViewer, double>();

	private readonly Dictionary<ScrollViewer, DateTime> _scrollStartTimes = new Dictionary<ScrollViewer, DateTime>();

	private const double ScrollDurationMs = 350.0;

	private const double ScrollStep = 90.0;

	private bool _scrolling = false;

	private Border? _versionSelectOverlay;

	private Border? _versionSelectPanel;

	private string? _selectedLaunchVersionId;

	private bool _isMicrosoftLogin = false;

	private bool _msAuthDialogOpen = false;

	private Border? _selectedItem = null;

	private Color _currentThemeColor = Color.FromRgb(72, 144, 245);

	private string? _verSettingsVersionId;

	private string? _returnPageAfterVerSettings;

	private bool _languageApplying = false;

	private bool _settingsInitialized = false;

	private bool _suppressConfigSave = false;

	private Border? _selectedSettingsTab = null;

	private static readonly string[] SettingsTabKeys;

	private static readonly string[] ThemeNames;

	public event EventHandler? CollapseRequested;

	public event EventHandler? ExitRequested;

	public LauncherPanel()
	{
		InitializeComponent();
		_panelSlide = PanelSlide;
		base.Loaded += LauncherPanel_Loaded;
		DownloadManager.ProgressChanged += OnDownloadProgressChanged;
		DownloadManager.DownloadCompleted += OnDownloadCompleted;
		DownloadManager.DownloadFailed += OnDownloadFailed;
		LaunchManager.LaunchFailed += OnLaunchFailed;
		LaunchManager.LaunchCompleted += OnLaunchCompleted;
		LaunchManager.ProgressChanged += OnLaunchProgressChanged;
		LauncherConfig.Changed += OnConfigChanged;
		UpdateChecker.UpdateAvailable += OnUpdateAvailable;
	}

	private void OnUpdateAvailable(UpdateInfo info)
	{
		UpdateInfo info2 = info;
		base.Dispatcher.BeginInvoke((Action)delegate
		{
			try
			{
				string arg = info2.TagName.TrimStart('v', 'V');
				NotificationManager.Show(string.Format(LanguageManager.Get("MsgUpdateFound"), arg));
				if (!string.IsNullOrEmpty(info2.HtmlUrl))
				{
					Process.Start(new ProcessStartInfo(info2.HtmlUrl)
					{
						UseShellExecute = true
					});
				}
			}
			catch
			{
			}
		});
	}

	private void OnLaunchProgressChanged(LaunchProgress p)
	{
		LaunchProgress p2 = p;
		base.Dispatcher.BeginInvoke((Action)delegate
		{
			if (!LaunchButton.IsEnabled)
			{
				LaunchButton.Content = $"{p2.Stage} {p2.Progress:0}%";
			}
		});
	}

	private void OnLaunchCompleted()
	{
		base.Dispatcher.BeginInvoke((Action)delegate
		{
			LaunchButton.IsEnabled = true;
			LaunchButton.SetResourceReference(ContentControl.ContentProperty, "LaunchStart");
			LaunchStatusText.SetResourceReference(TextBlock.TextProperty, "LaunchSuccess");
			LaunchStatusText.Foreground = Green80Brush;
			LaunchStatusText.Visibility = Visibility.Visible;
			LaunchStatusText.TextDecorations = null;
			LaunchStatusText.Cursor = Cursors.Arrow;
			LaunchStatusText.ToolTip = null;
			LaunchStatusText.MouseLeftButtonUp -= LaunchStatusText_Click;
		});
	}

	private void OnLaunchFailed(Exception ex, string logFilePath)
	{
		base.Dispatcher.BeginInvoke((Action)delegate
		{
			LaunchButton.IsEnabled = true;
			LaunchButton.SetResourceReference(ContentControl.ContentProperty, "LaunchStart");
			LaunchStatusText.SetResourceReference(TextBlock.TextProperty, "LaunchFailed");
			LaunchStatusText.Foreground = Red80Brush;
			LaunchStatusText.Visibility = Visibility.Visible;
			if (!string.IsNullOrEmpty(logFilePath) && File.Exists(logFilePath))
			{
				LaunchStatusText.ToolTip = logFilePath;
				LaunchStatusText.TextDecorations = TextDecorations.Underline;
				LaunchStatusText.Cursor = Cursors.Hand;
				LaunchStatusText.MouseLeftButtonUp -= LaunchStatusText_Click;
				LaunchStatusText.MouseLeftButtonUp += LaunchStatusText_Click;
			}
			else
			{
				LaunchStatusText.ToolTip = null;
				LaunchStatusText.TextDecorations = null;
				LaunchStatusText.Cursor = Cursors.Arrow;
				LaunchStatusText.MouseLeftButtonUp -= LaunchStatusText_Click;
			}
		});
	}

	private void LaunchStatusText_Click(object sender, MouseButtonEventArgs e)
	{
		if (sender is TextBlock tb && tb.ToolTip is string path && File.Exists(path))
		{
			Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
		}
	}

	private string? _lobbyRoomCode;
	private bool _isLobbyActive;

	private async void CreateLobby_Click(object sender, RoutedEventArgs e)
	{
		if (_isLobbyActive)
		{
			return;
		}
		CreateLobbyBtn.IsEnabled = false;
		try
		{
			var playerName = GetPlayerName();
			var result = await MultiplayerLobby.CreateLobbyAsync(playerName);
			_lobbyRoomCode = result.RoomCode;
			_isLobbyActive = true;
			UpdateLobbyUI(result);
		}
		catch (Exception ex)
		{
			CreateLobbyBtn.IsEnabled = true;
			ShowJoinStatus(ex.Message);
		}
	}

	private async void JoinLobby_Click(object sender, RoutedEventArgs e)
	{
		var roomCode = RoomCodeBox.Text.Trim();
		if (string.IsNullOrEmpty(roomCode))
		{
			ShowJoinStatus(LanguageManager.Get("MultiplayerRoomCodeEmpty"));
			return;
		}
		if (_isLobbyActive)
		{
			return;
		}
		JoinLobbyBtn.IsEnabled = false;
		ShowJoinStatus(LanguageManager.Get("MultiplayerJoining") ?? "Joining lobby...");
		try
		{
			var playerName = GetPlayerName();
			var result = await MultiplayerLobby.JoinLobbyAsync(roomCode, playerName);
			_lobbyRoomCode = result.RoomCode;
			_isLobbyActive = true;
			UpdateLobbyUI(result);
		}
		catch (Exception ex)
		{
			JoinLobbyBtn.IsEnabled = true;
			ShowJoinStatus(string.Format(LanguageManager.Get("MultiplayerLobbyFailed") + ": {0}", ex.Message));
		}
	}

	private void PasteRoomCode_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			var text = Clipboard.GetText();
			if (!string.IsNullOrEmpty(text))
			{
				RoomCodeBox.Text = text.Trim();
			}
		}
		catch { }
	}

	private void CopyRoomCode_Click(object sender, RoutedEventArgs e)
	{
		if (!string.IsNullOrEmpty(_lobbyRoomCode))
		{
			try
			{
				Clipboard.SetText(_lobbyRoomCode);
			}
			catch { }
		}
	}

	private async void LeaveLobby_Click(object sender, RoutedEventArgs e)
	{
		_isLobbyActive = false;
		_lobbyRoomCode = null;
		LobbyInfoCard.Visibility = Visibility.Collapsed;
		CreateLobbyBtn.IsEnabled = true;
		JoinLobbyBtn.IsEnabled = true;
		JoinStatusText.Visibility = Visibility.Collapsed;
		try
		{
			await MultiplayerLobby.LeaveLobbyAsync();
		}
		catch { }
	}

	private void UpdateLobbyUI(MultiplayerLobbyResult result)
	{
		LobbyInfoCard.Visibility = Visibility.Visible;
		RoomCodeLabel.Text = result.RoomCode;
		LobbyOwnerLabel.Text = result.HostName;
		PlayerCountLabel.Text = result.PlayerCount.ToString();
		CreateLobbyBtn.IsEnabled = false;
		JoinStatusText.Visibility = Visibility.Collapsed;
	}

	private void ShowJoinStatus(string message)
	{
		JoinStatusText.Text = message;
		JoinStatusText.Visibility = Visibility.Visible;
	}

	private string GetPlayerName()
	{
		if (_isMicrosoftLogin && !string.IsNullOrEmpty(MicrosoftAuthService.PlayerName))
		{
			return MicrosoftAuthService.PlayerName;
		}
		if (!string.IsNullOrEmpty(LauncherConfig.Current.PlayerName))
		{
			return LauncherConfig.Current.PlayerName;
		}
		return "Player";
	}

	private void LauncherPanel_Loaded(object sender, RoutedEventArgs e)
	{
		_panelSlide.X = -200.0;
		base.Opacity = 0.0;
		base.Visibility = Visibility.Collapsed;
		LanguageManager.Apply(LauncherConfig.Current.Language);
		InitSettingsTabs();
		InitSettingsControls();
		ApplyConfigToLaunchPage();
		ApplyPersonalization();
	}

	public static double GetAnimationSpeedFactor()
	{
		return (double)LauncherConfig.Current.AnimationSpeed / 100.0;
	}

	public static TimeSpan AnimDuration(double baseMs)
	{
		double num = GetAnimationSpeedFactor();
		if (num < 0.05)
		{
			num = 0.05;
		}
		return TimeSpan.FromMilliseconds(baseMs / num);
	}

	private static SolidColorBrush Freeze(Color c)
	{
		SolidColorBrush solidColorBrush = new SolidColorBrush(c);
		solidColorBrush.Freeze();
		return solidColorBrush;
	}

	private static HttpClient CreateImageClient()
	{
		HttpClient httpClient = new HttpClient
		{
			Timeout = TimeSpan.FromSeconds(10L)
		};
		httpClient.DefaultRequestHeaders.Add("User-Agent", "DIL/1.0");
		return httpClient;
	}

	static LauncherPanel()
	{
		CardBgBrush = Freeze(Color.FromRgb(42, 42, 46));
		CardHoverBrush = Freeze(Color.FromRgb(52, 52, 56));
		CardSelectedBrush = Freeze(Color.FromRgb(72, 144, 245));
		IconBgBrush = Freeze(Color.FromRgb(55, 55, 58));
		TextWhiteBrush = Freeze(Colors.White);
		TextDescBrush = Freeze(Color.FromRgb(140, 140, 140));
		TextGrayBrush = Freeze(Color.FromRgb(170, 170, 170));
		TextHintBrush = Freeze(Color.FromRgb(120, 120, 120));
		TagBgBrush = Freeze(Color.FromRgb(90, 90, 94));
		VersionCardBgBrush = Freeze(Color.FromRgb(45, 45, 48));
		VersionCardHoverBrush = Freeze(Color.FromRgb(62, 62, 66));
		VersionCardBorderBrush = Freeze(Color.FromRgb(60, 60, 64));
		CategoryInactiveBgBrush = Freeze(Color.FromRgb(40, 40, 44));
		CategoryInactiveTextBrush = Freeze(Color.FromRgb(170, 170, 170));
		ErrorBrush = Freeze(Color.FromRgb(245, 80, 80));
		HintBrush = Freeze(Color.FromRgb(136, 136, 136));
		SharedImageClient = CreateImageClient();
		SharedCardShadow = new DropShadowEffect
		{
			Color = Colors.Black,
			Opacity = 0.15,
			BlurRadius = 8.0,
			ShadowDepth = 1.0
		};
		EaseOut = FreezeEase(new CubicEase
		{
			EasingMode = EasingMode.EaseOut
		});
		EaseIn = FreezeEase(new CubicEase
		{
			EasingMode = EasingMode.EaseIn
		});
		SettingsTabKeys = new string[4] { "launch", "ui", "link", "system" };
		ThemeNames = new string[15]
		{
			"龙猫蓝", "甜柠青", "小草绿", "菠萝黄", "橡木棕", "玄素黑", "滑稽彩", "铁杆粉", "神秘紫", "欧皇彩",
			"秋仪金", "活跃橙", "跳票红", "极客蓝", "自定义"
		};
		SharedCardShadow.Freeze();
		Bg37Brush = Freeze(Color.FromRgb(37, 37, 38));
		Bg42Brush = Freeze(Color.FromRgb(42, 42, 46));
		Bg32Brush = Freeze(Color.FromRgb(32, 32, 36));
		Bg34Brush = Freeze(Color.FromRgb(34, 34, 38));
		Bg45Brush = Freeze(Color.FromRgb(45, 45, 48));
		Bg50Brush = Freeze(Color.FromRgb(50, 50, 52));
		Bg55Brush = Freeze(Color.FromRgb(55, 55, 58));
		Bg60Brush = Freeze(Color.FromRgb(60, 60, 65));
		Border60Brush = Freeze(Color.FromRgb(60, 60, 64));
		Border45Brush = Freeze(Color.FromRgb(45, 45, 48));
		Blue72Brush = Freeze(Color.FromRgb(72, 144, 245));
		Blue100Brush = Freeze(Color.FromRgb(100, 160, 255));
		Blue160Brush = Freeze(Color.FromRgb(140, 190, 255));
		Green80Brush = Freeze(Color.FromRgb(80, 200, 120));
		Yellow200Brush = Freeze(Color.FromRgb(200, 160, 60));
		Gray100Brush = Freeze(Color.FromRgb(100, 100, 100));
		Gray110Brush = Freeze(Color.FromRgb(110, 110, 110));
		Gray120Brush = Freeze(Color.FromRgb(120, 120, 120));
		Gray130Brush = Freeze(Color.FromRgb(130, 130, 130));
		Gray136Brush = Freeze(Color.FromRgb(136, 136, 136));
		Gray140Brush = Freeze(Color.FromRgb(140, 140, 140));
		Gray150Brush = Freeze(Color.FromRgb(150, 150, 150));
		Gray153Brush = Freeze(Color.FromRgb(153, 153, 153));
		Gray170Brush = Freeze(Color.FromRgb(170, 170, 170));
		Gray200Brush = Freeze(Color.FromRgb(200, 200, 200));
		Gray220Brush = Freeze(Color.FromRgb(220, 220, 220));
		Red80Brush = Freeze(Color.FromRgb(232, 90, 90));
		Red200Brush = Freeze(Color.FromRgb(200, 80, 80));
		Bg30Brush = Freeze(Color.FromRgb(30, 30, 32));
		Bg38Brush = Freeze(Color.FromRgb(38, 38, 42));
		Bg44Brush = Freeze(Color.FromRgb(44, 44, 48));
		Bg48Brush = Freeze(Color.FromRgb(48, 48, 52));
		Bg52Brush = Freeze(Color.FromRgb(52, 52, 56));
		Bg62Brush = Freeze(Color.FromRgb(62, 62, 66));
		Blue40Brush = Freeze(Color.FromRgb(40, 60, 100));
		Gray80Brush = Freeze(Color.FromRgb(80, 80, 85));
		Gray102Brush = Freeze(Color.FromRgb(102, 102, 102));
		Gray160Brush = Freeze(Color.FromRgb(160, 160, 160));
		Gray204Brush = Freeze(Color.FromRgb(204, 204, 204));
		Red245Brush = Freeze(Color.FromRgb(245, 80, 80));
		Border61Brush = Freeze(Color.FromRgb(61, 61, 66));
	}

	private static IEasingFunction FreezeEase(IEasingFunction e)
	{
		return e;
	}

	private void InitCategoryFilter()
	{
		CategoryFilterPanel.Children.Clear();
		_selectedCategoryBorder = null;
		string[] array = new string[5] { "\u5168\u90E8", "\u6B63\u5F0F\u7248", "\u5FEB\u7167", "\u8FDC\u53E4", "\u611A\u4EBA\u8282" };
		string[] array2 = new string[5] { "CatAll", "CatRelease", "CatSnapshot", "CatOld", "CatAprilFool" };
		for (int i = 0; i < array.Length; i++)
		{
			string category = array[i];
			Border border = new Border
			{
				Height = 28.0,
				Padding = new Thickness(14.0, 0.0, 14.0, 0.0),
				Margin = new Thickness(0.0, 0.0, 6.0, 0.0),
				CornerRadius = new CornerRadius(14.0),
				Cursor = Cursors.Hand,
				Tag = category
			};
			TextBlock textBlock = new TextBlock
			{
				FontSize = 12.0,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			textBlock.SetResourceReference(TextBlock.TextProperty, array2[i]);
			border.Child = textBlock;
			border.MouseLeftButtonUp += delegate
			{
				SelectCategory(border, category);
			};
			if (category == "\u5168\u90E8")
			{
				SelectCategory(border, category);
			}
			else
			{
				border.Background = CategoryInactiveBgBrush;
				textBlock.Foreground = CategoryInactiveTextBrush;
			}
			CategoryFilterPanel.Children.Add(border);
		}
	}

	private void SelectCategory(Border border, string category)
	{
		if (_selectedCategoryBorder != null)
		{
			_selectedCategoryBorder.Background = CategoryInactiveBgBrush;
			if (_selectedCategoryBorder.Child is TextBlock textBlock)
			{
				textBlock.Foreground = CategoryInactiveTextBrush;
			}
		}
		_selectedCategoryBorder = border;
		border.Background = CardSelectedBrush;
		if (border.Child is TextBlock textBlock2)
		{
			textBlock2.Foreground = TextWhiteBrush;
		}
		_currentCategory = category;
		SwitchCategoryAnimation();
	}

	private bool MatchCategory(string versionId, string versionType)
	{
		bool flag = versionType == "snapshot";
		bool flag2 = versionType == "old_alpha" || versionType == "old_beta";
		bool flag3 = IsAprilFoolsVersion(versionId);
		if (_currentCategory == "\u5168\u90E8")
		{
			if (flag && !LauncherConfig.Current.ShowDownloadSnapshot)
			{
				return false;
			}
			if (flag2 && !LauncherConfig.Current.ShowDownloadOldBeta)
			{
				return false;
			}
			if (flag3 && !LauncherConfig.Current.ShowDownloadAprilFool)
			{
				return false;
			}
			return true;
		}
		string currentCategory = _currentCategory;
		if (1 == 0)
		{
		}
		bool result = currentCategory switch
		{
			"\u6B63\u5F0F\u7248" => versionType == "release", 
			"快照" => flag,
			"远古" => flag2,
			"愚人节" => flag3, 
			_ => true, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private static bool IsAprilFoolsVersion(string versionId)
	{
		string versionId2 = versionId;
		string[] source = new string[8] { "2.0", "1.RV-Pre1", "3D Shareware v1.34", "20w14infinite", "22w13oneBlockAtATime", "23w13a_or_b", "24w14potato", "25w14craftmine" };
		return source.Any((string f) => versionId2.Equals(f, StringComparison.OrdinalIgnoreCase));
	}

	private static void ApplyEnterAnimation(FrameworkElement element, int mode, int index)
	{
		if (mode != 0)
		{
			int cappedIndex = Math.Min(index, 6);
			TranslateTransform translateTransform = (TranslateTransform)(element.RenderTransform = new TranslateTransform());
			element.Opacity = 0.0;
			double x = ((mode == 1) ? 45 : (-45));
			int num = ((mode == 2) ? cappedIndex * 30 : 0);
			int num2 = ((mode == 1) ? 200 : 250);
			translateTransform.X = x;
			DoubleAnimation animation = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(num2, 0L))
			{
				BeginTime = TimeSpan.FromMilliseconds(num, 0L),
				EasingFunction = EaseOut
			};
			DoubleAnimation animation2 = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(num2, 0L))
			{
				BeginTime = TimeSpan.FromMilliseconds(num, 0L),
				EasingFunction = EaseOut
			};
			translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);
			element.BeginAnimation(UIElement.OpacityProperty, animation2);
		}
	}

	private bool IsElementInViewport(FrameworkElement element)
	{
		if (VersionScrollViewer == null)
		{
			return true;
		}
		try
		{
			ScrollViewer versionScrollViewer = VersionScrollViewer;
			GeneralTransform generalTransform = element.TransformToVisual(versionScrollViewer);
			Rect rect = generalTransform.TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
			Rect rect2 = new Rect(0.0, 0.0, versionScrollViewer.ViewportWidth, versionScrollViewer.ViewportHeight);
			return rect.IntersectsWith(rect2);
		}
		catch
		{
			return false;
		}
	}

	private void AnimateExitToLeft(FrameworkElement element)
	{
		if (!IsElementInViewport(element))
		{
			element.Opacity = 0.0;
			return;
		}
		TranslateTransform translateTransform = (TranslateTransform)(element.RenderTransform = new TranslateTransform());
		DoubleAnimation animation = new DoubleAnimation(-55.0, TimeSpan.FromMilliseconds(200L, 0L))
		{
			EasingFunction = EaseIn
		};
		DoubleAnimation animation2 = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(200L, 0L));
		translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);
		element.BeginAnimation(UIElement.OpacityProperty, animation2);
	}

	private void SwitchCategoryAnimation()
	{
		_versionRenderToken++;
		int token = _versionRenderToken;
		List<FrameworkElement> list = VersionList.Children.OfType<FrameworkElement>().ToList();
		if (list.Count <= 0 || !list.Any((FrameworkElement c) => c.Opacity > 0.05))
		{
			_listEnterMode = 1;
			_staggerIndex = 0;
			RefreshVersionList();
			return;
		}
		foreach (FrameworkElement item in list)
		{
			AnimateExitToLeft(item);
		}
		DispatcherTimer timer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(180L, 0L)
		};
		timer.Tick += delegate
		{
			timer.Stop();
			if (token == _versionRenderToken)
			{
				_listEnterMode = 1;
				_staggerIndex = 0;
				RefreshVersionList();
			}
		};
		timer.Start();
	}

	private void RefreshVersionList()
	{
		_versionRenderToken++;
		int versionRenderToken = _versionRenderToken;
		VersionList.Children.Clear();
		List<(string, string)> list = new List<(string, string)>();
		foreach (var allVersion in _allVersions)
		{
			if (MatchCategory(allVersion.id, allVersion.type))
			{
				list.Add(allVersion);
			}
		}
		if (list.Count == 0)
		{
			TextBlock textBlock = new TextBlock
			{
				Foreground = HintBrush,
				FontSize = 13.0,
				Margin = new Thickness(12.0, 20.0, 12.0, 0.0),
				HorizontalAlignment = HorizontalAlignment.Center
			};
			textBlock.SetResourceReference(TextBlock.TextProperty, "StatusNoVersionInCategory");
			VersionList.Children.Add(textBlock);
		}
		else
		{
			for (int i = 0; i < Math.Min(16, list.Count); i++)
			{
				CreateVersionItem(list[i].Item1, list[i].Item2);
			}
			if (list.Count > 16)
			{
				RenderVersionBatch(list, 16, 16, versionRenderToken);
			}
		}
	}

	private void RenderVersionBatch(List<(string id, string type)> matched, int index, int step, int token)
	{
		List<(string id, string type)> matched2 = matched;
		if (token != _versionRenderToken)
		{
			return;
		}
		int end = Math.Min(index + step, matched2.Count);
		for (int i = index; i < end; i++)
		{
			CreateVersionItem(matched2[i].id, matched2[i].type);
		}
		if (end < matched2.Count)
		{
			base.Dispatcher.BeginInvoke((Action)delegate
			{
				RenderVersionBatch(matched2, end, step, token);
			}, DispatcherPriority.Background);
		}
	}

	private void ShowDownloadPage()
	{
		SelectNavItem("download");
		SwitchToPage(DownloadPage, DownloadPageSlide, delegate
		{
			InitResourceTypeFilter();
			if (_currentResourceType == "\u6E38\u620F")
			{
				GameCategoryBar.Visibility = Visibility.Visible;
				ResourceSearchBar.Visibility = Visibility.Collapsed;
				BottomActionBar.Visibility = Visibility.Visible;
				ListArea.CornerRadius = new CornerRadius(0.0);
				InitCategoryFilter();
				if (_allVersions.Count == 0)
				{
					LoadVersionList();
				}
			}
			else
			{
				GameCategoryBar.Visibility = Visibility.Collapsed;
				ResourceSearchBar.Visibility = Visibility.Visible;
				BottomActionBar.Visibility = Visibility.Collapsed;
				ListArea.CornerRadius = new CornerRadius(0.0, 0.0, 8.0, 8.0);
			}
		});
	}

	private void ShowDownloadCenterPage()
	{
		SelectNavItem("downloadcenter");
		SwitchToPage(DownloadCenterPage, DownloadCenterPageSlide, delegate
		{
			UpdateDownloadTaskList();
		});
	}

	private void NewDownload_Click(object sender, RoutedEventArgs e)
	{
		SwitchToPage(DownloadPage, DownloadPageSlide, delegate
		{
			_currentResourceType = "\u6E38\u620F";
			InitResourceTypeFilter();
			InitCategoryFilter();
			base.Dispatcher.BeginInvoke((Action)delegate
			{
				LoadVersionList();
			}, DispatcherPriority.Input);
		});
	}

	private void InitResourceTypeFilter()
	{
		ResourceTypePanel.Children.Clear();
		string[] array = new string[4] { "\u6E38\u620F", "\u6A21\u7EC4", "\u5149\u5F71", "\u6750\u8D28" };
		string[] array2 = new string[4] { "ResTypeGame", "ResTypeMod", "ResTypeShader", "ResTypeMaterial" };
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			Border border = new Border
			{
				Height = 28.0,
				Padding = new Thickness(14.0, 0.0, 14.0, 0.0),
				Margin = new Thickness(0.0, 0.0, 6.0, 0.0),
				CornerRadius = new CornerRadius(14.0),
				Cursor = Cursors.Hand,
				Tag = text
			};
			TextBlock textBlock = new TextBlock
			{
				FontSize = 12.0,
				VerticalAlignment = VerticalAlignment.Center
			};
			textBlock.SetResourceReference(TextBlock.TextProperty, array2[i]);
			border.Child = textBlock;
			if (text == _currentResourceType)
			{
				border.Background = Blue72Brush;
				textBlock.Foreground = new SolidColorBrush(Colors.White);
			}
			else
			{
				border.Background = Bg45Brush;
				textBlock.Foreground = Gray170Brush;
			}
			border.MouseLeftButtonUp += ResourceType_Click;
			border.MouseEnter += delegate(object s, MouseEventArgs _)
			{
				if (s is Border border3 && border3.Tag as string != _currentResourceType)
				{
					border3.Background = Bg55Brush;
				}
			};
			border.MouseLeave += delegate(object s, MouseEventArgs _)
			{
				if (s is Border border2 && border2.Tag as string != _currentResourceType)
				{
					border2.Background = Bg45Brush;
				}
			};
			ResourceTypePanel.Children.Add(border);
		}
	}

	private void ResourceType_Click(object sender, MouseButtonEventArgs e)
	{
		if (!(sender is Border border))
		{
			return;
		}
		string text = (border.Tag as string) ?? "";
		if (text == _currentResourceType)
		{
			return;
		}
		_currentResourceType = text;
		InitResourceTypeFilter();
		_versionRenderToken++;
		VersionList.Children.Clear();
		TextBlock textBlock = new TextBlock
		{
			Foreground = Gray136Brush,
			FontSize = 13.0,
			Margin = new Thickness(12.0, 20.0, 12.0, 0.0),
			HorizontalAlignment = HorizontalAlignment.Center
		};
		textBlock.SetResourceReference(TextBlock.TextProperty, "StatusLoading");
		VersionList.Children.Add(textBlock);
		if (text == "\u6E38\u620F")
		{
			GameCategoryBar.Visibility = Visibility.Visible;
			ResourceSearchBar.Visibility = Visibility.Collapsed;
			BottomActionBar.Visibility = Visibility.Visible;
			ListArea.CornerRadius = new CornerRadius(0.0);
			if (_allVersions.Count > 0)
			{
				RefreshVersionList();
				return;
			}
			TextBlock textBlock2 = new TextBlock
			{
				Foreground = Gray136Brush,
				FontSize = 13.0,
				Margin = new Thickness(12.0, 20.0, 12.0, 0.0),
				HorizontalAlignment = HorizontalAlignment.Center
			};
			textBlock2.SetResourceReference(TextBlock.TextProperty, "StatusLoadingVersionList");
			VersionList.Children.Add(textBlock2);
			LoadVersionList();
			return;
		}
		GameCategoryBar.Visibility = Visibility.Collapsed;
		ResourceSearchBar.Visibility = Visibility.Visible;
		BottomActionBar.Visibility = Visibility.Collapsed;
		ListArea.CornerRadius = new CornerRadius(0.0, 0.0, 8.0, 8.0);
		ResourceSearchBox.Text = "";
		DoResourceSearch();
		base.Dispatcher.BeginInvoke((Action)delegate
		{
			Focus();
			if (ResourceSearchBox != null)
			{
				ResourceSearchBox.Focus();
				Keyboard.Focus(ResourceSearchBox);
			}
		}, DispatcherPriority.ContextIdle);
	}

	private void ResourceSearch_Click(object sender, RoutedEventArgs e)
	{
		DoResourceSearch();
	}

	private void ResourceSearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Return)
		{
			e.Handled = true;
			base.Dispatcher.BeginInvoke((Action)delegate
			{
				DoResourceSearch();
			}, DispatcherPriority.Background);
		}
	}

	private void ResourceSearchBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Return)
		{
			e.Handled = true;
		}
	}

	private async Task DoResourceSearch()
	{
		string query = ResourceSearchBox.Text.Trim();
		int currentToken = ++_versionRenderToken;
		string currentResourceType = _currentResourceType;
		if (1 == 0)
		{
		}
		ResourceType resourceType = currentResourceType switch
		{
			"\u6A21\u7EC4" => ResourceType.Mod, 
			"\u5149\u5F71" => ResourceType.Shader, 
			"\u6750\u8D28" => ResourceType.ResourcePack, 
			_ => ResourceType.Mod, 
		};
		if (1 == 0)
		{
		}
		ResourceType resType = resourceType;
		string cacheKey = $"search_{resType}_{query}";
		List<ModrinthProject> cached = DataCache.Get<List<ModrinthProject>>(new object[1] { cacheKey });
		if (cached != null)
		{
			_searchResults = cached;
			RefreshResourceList();
			return;
		}
		VersionList.Children.Clear();
		string displayText = (string.IsNullOrEmpty(query) ? LanguageManager.Get("ResLoadingRecommend") : LanguageManager.Get("ResSearching"));
		TextBlock loading = new TextBlock
		{
			Text = displayText,
			Foreground = Gray136Brush,
			FontSize = 13.0,
			Margin = new Thickness(12.0, 20.0, 12.0, 0.0),
			HorizontalAlignment = HorizontalAlignment.Center,
			TextWrapping = TextWrapping.Wrap
		};
		VersionList.Children.Add(loading);
		try
		{
			List<ModrinthProject> result = await Task.Run(() => ModrinthApi.SearchAsync(query, resType));
			if (currentToken == _versionRenderToken)
			{
				DataCache.Set(result, cacheKey);
				_searchResults = result;
				RefreshResourceList();
			}
		}
		catch (Exception ex)
		{
			if (_searchResults.Count == 0)
			{
				VersionList.Children.Clear();
				TextBlock err = new TextBlock
				{
					Text = string.Format(LanguageManager.Get("ResLoadFailed"), ex.Message),
					Foreground = Red245Brush,
					FontSize = 13.0,
					Margin = new Thickness(12.0, 20.0, 12.0, 0.0),
					HorizontalAlignment = HorizontalAlignment.Center
				};
				VersionList.Children.Add(err);
			}
		}
	}

	private void RefreshResourceList()
	{
		if (_currentResourceType == "\u6E38\u620F")
		{
			return;
		}
		_listEnterMode = 2;
		_staggerIndex = 0;
		VersionList.Children.Clear();
		if (_searchResults.Count == 0)
		{
			string text = ResourceSearchBox?.Text?.Trim() ?? "";
			string text2 = (string.IsNullOrEmpty(text) ? LanguageManager.Get("ResNoRecommend") : string.Format(LanguageManager.Get("ResNoResult"), text));
			StackPanel stackPanel = new StackPanel
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(12.0, 30.0, 12.0, 0.0)
			};
			stackPanel.Children.Add(new TextBlock
			{
				Text = text2,
				Foreground = Gray136Brush,
				FontSize = 13.0,
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			});
			VersionList.Children.Add(stackPanel);
			return;
		}
		foreach (ModrinthProject searchResult in _searchResults)
		{
			CreateResourceItem(searchResult);
		}
	}

	private Border CreateTag(string text, Color color)
	{
		Brush background = ((color == Color.FromRgb(72, 144, 245)) ? CardSelectedBrush : ((color == Color.FromRgb(90, 90, 94)) ? TagBgBrush : Freeze(color)));
		return new Border
		{
			Background = background,
			CornerRadius = new CornerRadius(4.0),
			Padding = new Thickness(6.0, 1.0, 6.0, 1.0),
			Margin = new Thickness(0.0, 0.0, 4.0, 0.0),
			Child = new TextBlock
			{
				Text = text,
				Foreground = TextWhiteBrush,
				FontSize = 10.0,
				VerticalAlignment = VerticalAlignment.Center
			}
		};
	}

	private static string FormatVersionRange(List<string> versions)
	{
		if (versions.Count == 0)
		{
			return "";
		}
		var list = (from v in versions
			where v.Contains('.')
			select new
			{
				Raw = v,
				Parts = v.Split('.')
			} into v
			where v.Parts.Length >= 2
			select v).OrderByDescending(v => v.Raw, new VersionComparer()).ToList();
		if (list.Count == 0)
		{
			return versions.First();
		}
		string raw = list.Last().Raw;
		string raw2 = list.First().Raw;
		if (raw == raw2)
		{
			return raw2;
		}
		return raw + " ~ " + raw2;
	}

	private void CreateResourceItem(ModrinthProject proj)
	{
		Border border = new Border
		{
			Background = CardBgBrush,
			CornerRadius = new CornerRadius(8.0),
			Padding = new Thickness(14.0, 10.0, 14.0, 10.0),
			Margin = new Thickness(0.0, 0.0, 0.0, 6.0),
			Cursor = Cursors.Hand,
			Tag = proj
		};
		Grid grid = new Grid();
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = new GridLength(1.0, GridUnitType.Star)
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		bool isShader = _currentResourceType == "\u5149\u5F71";
		Border border2 = new Border
		{
			Width = 44.0,
			Height = 44.0,
			CornerRadius = new CornerRadius(8.0),
			Background = isShader ? Brushes.Transparent : IconBgBrush,
			ClipToBounds = true
		};
		if (!string.IsNullOrEmpty(proj.IconUrl))
		{
			LoadImageAsync(border2, proj.IconUrl, isShader);
		}
		else
		{
			border2.Child = new TextBlock
			{
				Text = "?",
				FontSize = 20.0,
				Foreground = TextHintBrush,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
		}
		Grid.SetColumn(border2, 0);
		StackPanel stackPanel = new StackPanel
		{
			Margin = new Thickness(12.0, 0.0, 0.0, 0.0),
			VerticalAlignment = VerticalAlignment.Center
		};
		TextBlock element = new TextBlock
		{
			Text = proj.Title,
			Foreground = TextWhiteBrush,
			FontSize = 13.0,
			FontWeight = FontWeights.SemiBold,
			TextTrimming = TextTrimming.CharacterEllipsis
		};
		stackPanel.Children.Add(element);
		string text = (string.IsNullOrEmpty(proj.Description) ? LanguageManager.Get("ResNoDesc") : ((proj.Description.Length > 40) ? (proj.Description.Substring(0, 40) + "...") : proj.Description));
		TextBlock element2 = new TextBlock
		{
			Text = text,
			Foreground = TextDescBrush,
			FontSize = 11.0,
			Margin = new Thickness(0.0, 3.0, 0.0, 0.0),
			TextTrimming = TextTrimming.CharacterEllipsis
		};
		stackPanel.Children.Add(element2);
		WrapPanel wrapPanel = new WrapPanel
		{
			Margin = new Thickness(0.0, 4.0, 0.0, 0.0)
		};
		foreach (string item in proj.Loaders.Take(3))
		{
			wrapPanel.Children.Add(CreateTag(item, Color.FromRgb(72, 144, 245)));
		}
		if (proj.GameVersions.Count > 0)
		{
			string text2 = FormatVersionRange(proj.GameVersions);
			if (!string.IsNullOrEmpty(text2))
			{
				wrapPanel.Children.Add(CreateTag(text2, Color.FromRgb(90, 90, 94)));
			}
		}
		stackPanel.Children.Add(wrapPanel);
		Grid.SetColumn(stackPanel, 1);
		Button dlBtn = new Button
		{
			Style = (Style)FindResource("CardButton"),
			Height = 30.0,
			FontSize = 12.0,
			Tag = proj,
			VerticalAlignment = VerticalAlignment.Center
		};
		dlBtn.SetResourceReference(ContentControl.ContentProperty, "DownloadStart");
		dlBtn.Click += ResourceDownload_Click;
		Grid.SetColumn(dlBtn, 2);
		grid.Children.Add(border2);
		grid.Children.Add(stackPanel);
		grid.Children.Add(dlBtn);
		border.Child = grid;
		border.MouseLeftButtonUp += delegate
		{
			ResourceDownload_Click(dlBtn, null);
		};
		border.MouseEnter += delegate(object s, MouseEventArgs _)
		{
			if (s is Border border4)
			{
				border4.Background = CardHoverBrush;
				border4.Effect = SharedCardShadow;
			}
		};
		border.MouseLeave += delegate(object s, MouseEventArgs _)
		{
			if (s is Border border3)
			{
				border3.Background = CardBgBrush;
				border3.Effect = null;
			}
		};
		VersionList.Children.Add(border);
		ApplyEnterAnimation(border, _listEnterMode, _staggerIndex);
		_staggerIndex++;
	}

	private async Task LoadImageAsync(Border border, string url, bool uniformStretch = false)
	{
		Border border2 = border;
		string url2 = url;
		Stretch stretch = uniformStretch ? Stretch.Uniform : Stretch.UniformToFill;
		try
		{
			BitmapImage cachedBitmap = DataCache.Get<BitmapImage>(new object[2] { "img", url2 });
			if (cachedBitmap != null)
			{
				base.Dispatcher.Invoke(delegate
				{
					if (!(border2.Tag?.ToString() == "loaded"))
					{
						border2.Tag = "loaded";
						border2.Background = Brushes.Transparent;
						border2.Child = new Image
						{
							Source = cachedBitmap,
							Stretch = stretch
						};
					}
				});
				return;
			}
			using MemoryStream ms = new MemoryStream(await Task.Run(async () => await SharedImageClient.GetByteArrayAsync(url2)));
			BitmapImage bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.CacheOption = BitmapCacheOption.OnLoad;
			bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
			bitmap.DecodePixelWidth = 128;
			bitmap.StreamSource = ms;
			bitmap.EndInit();
			bitmap.Freeze();
			DataCache.Set(bitmap, "img", url2);
			base.Dispatcher.Invoke(delegate
			{
				if (!(border2.Tag?.ToString() == "loaded"))
				{
					border2.Tag = "loaded";
					border2.Background = Brushes.Transparent;
					Image child = new Image
					{
						Source = bitmap,
						Stretch = stretch
					};
					border2.Child = child;
				}
			});
		}
		catch (Exception)
		{
			base.Dispatcher.Invoke(delegate
			{
				border2.Tag = "failed";
				border2.Background = uniformStretch ? Brushes.Transparent : Bg55Brush;
				border2.Child = new TextBlock
				{
					Text = "?",
					FontSize = 20.0,
					Foreground = Gray120Brush,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};
			});
		}
	}

	private void UpdateDownloadTaskList()
	{
		var allTasks = DownloadManager.AllTasks;
		var activeOrRecent = allTasks.Where(t => t.IsActive || t.Step == DownloadStep.Completed || t.Step == DownloadStep.Failed || t.Step == DownloadStep.Cancelled).ToList();

		if (activeOrRecent.Count == 0)
		{
			DownloadTaskList.Children.Clear();
			_taskCards.Clear();
			_taskCardProgresses.Clear();
			_taskCardSpeeds.Clear();
			_taskCardSteps.Clear();
			_taskCardStatuses.Clear();
			_taskCardTitles.Clear();
			Border border = new Border
			{
				Background = Bg42Brush,
				CornerRadius = new CornerRadius(8.0),
				Padding = new Thickness(20.0, 40.0, 20.0, 40.0),
				Margin = new Thickness(0.0, 20.0, 0.0, 0.0)
			};
			StackPanel stackPanel = new StackPanel
			{
				HorizontalAlignment = HorizontalAlignment.Center
			};
			TextBlock element = new TextBlock
			{
				Text = "\u2205",
				FontSize = 32.0,
				Foreground = Gray80Brush,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(0.0, 0.0, 0.0, 8.0)
			};
			TextBlock textBlock = new TextBlock
			{
				FontSize = 14.0,
				Foreground = Gray136Brush,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			textBlock.SetResourceReference(TextBlock.TextProperty, "StatusNoDownloadTask");
			TextBlock textBlock2 = new TextBlock
			{
				FontSize = 11.0,
				Foreground = Gray102Brush,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(0.0, 4.0, 0.0, 0.0)
			};
			textBlock2.SetResourceReference(TextBlock.TextProperty, "StatusClickDownloadStart");
			stackPanel.Children.Add(element);
			stackPanel.Children.Add(textBlock);
			stackPanel.Children.Add(textBlock2);
			border.Child = stackPanel;
			DownloadTaskList.Children.Add(border);
			return;
		}

		DateTime now = DateTime.Now;
		if ((now - _lastCardUpdate).TotalMilliseconds < 150.0)
		{
			bool needsFullRefresh = false;
			foreach (var t in activeOrRecent)
			{
				if (!_taskCards.ContainsKey(t.Id))
				{
					needsFullRefresh = true;
					break;
				}
			}
			if (!needsFullRefresh)
			{
				foreach (var t in activeOrRecent)
				{
					if (_taskCardProgresses.TryGetValue(t.Id, out var prog)) prog.Value = t.Progress;
					if (_taskCardSpeeds.TryGetValue(t.Id, out var spd)) spd.Text = FormatSpeed(t.Speed);
					if (_taskCardSteps.TryGetValue(t.Id, out var stp)) stp.Text = t.StepText;
					string text = ((t.Step == DownloadStep.Completed) ? LanguageManager.Get("DlStatusCompleted") : ((t.Step == DownloadStep.Failed) ? LanguageManager.Get("DlStatusFailed") : ((t.Step == DownloadStep.Cancelled) ? LanguageManager.Get("DlStatusCancelled") : LanguageManager.Get("DlStatusDownloading"))));
					Color color = ((t.Step == DownloadStep.Completed) ? Color.FromRgb(100, 200, 100) : ((t.Step == DownloadStep.Failed) ? Color.FromRgb(byte.MaxValue, 100, 100) : ((t.Step == DownloadStep.Cancelled) ? Color.FromRgb(170, 170, 170) : Color.FromRgb(72, 144, 245))));
					if (_taskCardStatuses.TryGetValue(t.Id, out var sts))
					{
						sts.Text = text;
						sts.Foreground = new SolidColorBrush(color);
					}
				}
				return;
			}
		}
		_lastCardUpdate = now;

		DownloadTaskList.Children.Clear();
		_taskCards.Clear();
		_taskCardProgresses.Clear();
		_taskCardSpeeds.Clear();
		_taskCardSteps.Clear();
		_taskCardStatuses.Clear();
		_taskCardTitles.Clear();

		foreach (var task in activeOrRecent)
		{
			CreateDownloadTaskCard(task);
		}
	}

	private void CreateDownloadTaskCard(DownloadTask task)
	{
		Border border = new Border
		{
			Background = Bg42Brush,
			CornerRadius = new CornerRadius(8.0),
			Padding = new Thickness(18.0, 14.0, 18.0, 14.0),
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0)
		};
		StackPanel stackPanel = new StackPanel();
		Grid grid = new Grid();
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = new GridLength(1.0, GridUnitType.Star)
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		TextBlock textBlock = new TextBlock
		{
			Text = task.Name,
			Foreground = Brushes.White,
			FontSize = 14.0,
			FontWeight = FontWeights.SemiBold,
			VerticalAlignment = VerticalAlignment.Center
		};
		Grid.SetColumn(textBlock, 0);
		grid.Children.Add(textBlock);
		bool flag = task.Step != DownloadStep.Completed && task.Step != DownloadStep.Failed && task.Step != DownloadStep.Cancelled;
		if (flag)
		{
			Button button = new Button
			{
				Style = (Style)FindResource("CardButton"),
				Height = 28.0,
				FontSize = 12.0,
				Margin = new Thickness(12.0, 0.0, 0.0, 0.0),
				VerticalAlignment = VerticalAlignment.Center
			};
			int taskId = task.Id;
			button.Click += delegate
			{
				CancelDownload_Click(taskId);
			};
			button.SetResourceReference(ContentControl.ContentProperty, "CommonCancel");
			Grid.SetColumn(button, 1);
			grid.Children.Add(button);
		}
		string text = ((task.Step == DownloadStep.Completed) ? LanguageManager.Get("DlStatusCompleted") : ((task.Step == DownloadStep.Failed) ? LanguageManager.Get("DlStatusFailed") : ((task.Step == DownloadStep.Cancelled) ? LanguageManager.Get("DlStatusCancelled") : LanguageManager.Get("DlStatusDownloading"))));
		Color color = ((task.Step == DownloadStep.Completed) ? Color.FromRgb(100, 200, 100) : ((task.Step == DownloadStep.Failed) ? Color.FromRgb(byte.MaxValue, 100, 100) : ((task.Step == DownloadStep.Cancelled) ? Color.FromRgb(170, 170, 170) : Color.FromRgb(72, 144, 245))));
		TextBlock textBlock2 = new TextBlock
		{
			Text = text,
			Foreground = new SolidColorBrush(color),
			FontSize = 12.0,
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Right,
			Margin = (flag ? new Thickness(10.0, 0.0, 0.0, 0.0) : new Thickness(0.0))
		};
		Grid.SetColumn(textBlock2, (!flag) ? 1 : 2);
		grid.Children.Add(textBlock2);
		stackPanel.Children.Add(grid);
		DockPanel dockPanel = new DockPanel
		{
			Margin = new Thickness(0.0, 10.0, 0.0, 0.0),
			LastChildFill = true
		};
		TextBlock textBlock3 = new TextBlock
		{
			Text = FormatSpeed(task.Speed),
			Foreground = Gray170Brush,
			FontSize = 11.0,
			Margin = new Thickness(10.0, 0.0, 0.0, 0.0),
			VerticalAlignment = VerticalAlignment.Center,
			Width = 80.0,
			TextAlignment = TextAlignment.Right
		};
		DockPanel.SetDock(textBlock3, Dock.Right);
		dockPanel.Children.Add(textBlock3);
		ProgressBar progressBar = new ProgressBar
		{
			Height = 4.0,
			Minimum = 0.0,
			Maximum = 100.0,
			Value = task.Progress,
			Background = Bg60Brush,
			Foreground = Blue72Brush,
			BorderThickness = new Thickness(0.0),
			VerticalAlignment = VerticalAlignment.Center
		};
		dockPanel.Children.Add(progressBar);
		stackPanel.Children.Add(dockPanel);
		TextBlock textBlock4 = new TextBlock
		{
			Text = task.StepText,
			Foreground = Gray153Brush,
			FontSize = 12.0,
			Margin = new Thickness(0.0, 8.0, 0.0, 0.0)
		};
		stackPanel.Children.Add(textBlock4);
		border.Child = stackPanel;
		DownloadTaskList.Children.Add(border);
		_taskCards[task.Id] = border;
		_taskCardProgresses[task.Id] = progressBar;
		_taskCardSpeeds[task.Id] = textBlock3;
		_taskCardSteps[task.Id] = textBlock4;
		_taskCardStatuses[task.Id] = textBlock2;
		_taskCardTitles[task.Id] = textBlock;
	}

	private string FormatSpeed(double bytesPerSecond)
	{
		if (!(bytesPerSecond < 1024.0))
		{
			if (!(bytesPerSecond < 1048576.0))
			{
				return $"{bytesPerSecond / 1048576.0:F1} MB/s";
			}
			return $"{bytesPerSecond / 1024.0:F1} KB/s";
		}
		return $"{(int)bytesPerSecond} B/s";
	}

	private void CancelDownload_Click(int taskId)
	{
		DownloadManager.CancelDownload(taskId);
		UpdateDownloadTaskList();
	}

	private void OnDownloadProgressChanged(DownloadTask task)
	{
		base.Dispatcher.BeginInvoke((Action)delegate
		{
			if (DownloadCenterPage.Visibility == Visibility.Visible)
			{
				UpdateDownloadTaskList();
			}
		}, DispatcherPriority.Background);
	}

	private void OnDownloadCompleted(DownloadTask task)
	{
		base.Dispatcher.BeginInvoke((Action)delegate
		{
			if (DownloadCenterPage.Visibility == Visibility.Visible)
			{
				UpdateDownloadTaskList();
			}
		}, DispatcherPriority.Background);
	}

	private void OnDownloadFailed(DownloadTask task, Exception ex)
	{
		base.Dispatcher.BeginInvoke((Action)delegate
		{
			if (DownloadCenterPage.Visibility == Visibility.Visible)
			{
				UpdateDownloadTaskList();
			}
		}, DispatcherPriority.Background);
	}

	private void SmoothScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		if (sender is ScrollViewer scrollViewer)
		{
			e.Handled = true;
			double value;
			double num = (_scrollTargets.TryGetValue(scrollViewer, out value) ? value : scrollViewer.VerticalOffset);
			double num2 = ((e.Delta > 0) ? (-90.0) : 90.0);
			double value2 = Math.Max(0.0, Math.Min(num + num2, scrollViewer.ScrollableHeight));
			_scrollStartOffsets[scrollViewer] = scrollViewer.VerticalOffset;
			_scrollTargets[scrollViewer] = value2;
			_scrollStartTimes[scrollViewer] = DateTime.Now;
			if (!_scrolling)
			{
				_scrolling = true;
				CompositionTarget.Rendering += SmoothScroll_Tick;
			}
		}
	}

	private void SmoothScroll_Tick(object? sender, EventArgs e)
	{
		bool flag = false;
		foreach (ScrollViewer item in _scrollStartTimes.Keys.ToList())
		{
			if (_scrollStartOffsets.TryGetValue(item, out var value) && _scrollTargets.TryGetValue(item, out var value2))
			{
				double totalMilliseconds = (DateTime.Now - _scrollStartTimes[item]).TotalMilliseconds;
				double num = Math.Min(totalMilliseconds / 350.0, 1.0);
				double num2 = 1.0 - Math.Pow(1.0 - num, 3.0);
				double offset = value + (value2 - value) * num2;
				item.ScrollToVerticalOffset(offset);
				if (num >= 1.0)
				{
					item.ScrollToVerticalOffset(value2);
					_scrollStartTimes.Remove(item);
					_scrollStartOffsets.Remove(item);
					_scrollTargets.Remove(item);
				}
				else
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			_scrolling = false;
			CompositionTarget.Rendering -= SmoothScroll_Tick;
		}
	}

	private async void LoadVersionList()
	{
		List<(string id, string type)> cached = DataCache.Get<List<(string, string)>>(new object[1] { "versions" });
		if (cached != null && cached.Count > 0)
		{
			_allVersions.Clear();
			_allVersions.AddRange(cached);
			_listEnterMode = 2;
			_staggerIndex = 0;
			RefreshVersionList();
		}
		else
		{
			VersionList.Children.Clear();
			TextBlock loadingText = new TextBlock
			{
				Foreground = Gray136Brush,
				FontSize = 13.0,
				Margin = new Thickness(12.0, 20.0, 12.0, 0.0),
				HorizontalAlignment = HorizontalAlignment.Center
			};
			loadingText.SetResourceReference(TextBlock.TextProperty, "StatusLoadingVersionList");
			VersionList.Children.Add(loadingText);
		}
		try
		{
			List<(string id, string type)> result = await Task.Run(async delegate
			{
				using HttpClient client = new HttpClient
				{
					Timeout = TimeSpan.FromSeconds(15L)
				};
				string url = "https://launchermeta.mojang.com/mc/game/version_manifest.json";
				JsonDocument jsonDoc = JsonDocument.Parse(await client.GetStringAsync(url));
				JsonElement versions = jsonDoc.RootElement.GetProperty("versions");
				List<(string id, string type)> list = new List<(string, string)>();
				foreach (JsonElement version in versions.EnumerateArray())
				{
					string id = version.GetProperty("id").GetString() ?? "unknown";
					string type = version.GetProperty("type").GetString() ?? "unknown";
					list.Add((id, type));
				}
				return list;
			});
			DataCache.Set(result, "versions");
			_allVersions.Clear();
			_allVersions.AddRange(result);
			_listEnterMode = 2;
			_staggerIndex = 0;
			RefreshVersionList();
		}
		catch (Exception ex)
		{
			if (_allVersions.Count == 0)
			{
				VersionList.Children.Clear();
				TextBlock errorText = new TextBlock
				{
					Text = string.Format(LanguageManager.Get("ResLoadFailed"), ex.Message),
					Foreground = Red245Brush,
					FontSize = 13.0,
					Margin = new Thickness(12.0, 20.0, 12.0, 0.0),
					HorizontalAlignment = HorizontalAlignment.Center
				};
				VersionList.Children.Add(errorText);
			}
		}
	}

	private void CreateVersionItem(string versionId, string versionType)
	{
		string versionId2 = versionId;
		StackPanel stackPanel = new StackPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0)
		};
		Border headerBorder = new Border
		{
			Height = 48.0,
			Padding = new Thickness(16.0, 0.0, 16.0, 0.0),
			Background = VersionCardBgBrush,
			BorderBrush = VersionCardBorderBrush,
			BorderThickness = new Thickness(1.0),
			CornerRadius = new CornerRadius(6.0),
			Cursor = Cursors.Hand,
			Tag = versionId2
		};
		Grid grid = new Grid();
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = new GridLength(1.0, GridUnitType.Star)
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = new GridLength(1.0, GridUnitType.Auto)
		});
		StackPanel stackPanel2 = new StackPanel
		{
			Orientation = Orientation.Horizontal,
			VerticalAlignment = VerticalAlignment.Center
		};
		string typeIcon = versionType switch
		{
			"release" => "assests/release.png",
			_ => "assests/sna.png"
		};
		Image typeImg = new Image
		{
			Source = GetCachedIcon(typeIcon),
			Width = 16.0,
			Height = 16.0,
			Margin = new Thickness(0.0, 0.0, 6.0, 0.0),
			VerticalAlignment = VerticalAlignment.Center,
			ToolTip = (versionType == "release") ? LanguageManager.Get("ResTypeRelease") : LanguageManager.Get("ResTypeSnapshot")
		};
		RenderOptions.SetBitmapScalingMode(typeImg, BitmapScalingMode.LowQuality);
		stackPanel2.Children.Add(typeImg);
		TextBlock nameText = new TextBlock
		{
			Text = versionId2,
			Foreground = TextWhiteBrush,
			FontSize = 14.0,
			FontWeight = FontWeights.Medium,
			VerticalAlignment = VerticalAlignment.Center
		};
		stackPanel2.Children.Add(nameText);
		Grid.SetColumn(stackPanel2, 0);
		grid.Children.Add(stackPanel2);
		TextBlock arrowText = new TextBlock
		{
			Text = "\u25BC",
			Foreground = TextGrayBrush,
			FontSize = 12.0,
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Right,
			RenderTransformOrigin = new Point(0.5, 0.5),
			RenderTransform = new RotateTransform(0.0)
		};
		Grid.SetColumn(arrowText, 1);
		grid.Children.Add(arrowText);
		headerBorder.Child = grid;
		stackPanel.Children.Add(headerBorder);
		StackPanel expandPanel = new StackPanel
		{
			Margin = new Thickness(0.0, 2.0, 0.0, 0.0),
			Opacity = 0.0,
			Height = 0.0,
			Visibility = Visibility.Collapsed
		};
		stackPanel.Children.Add(expandPanel);
		bool isExpanded = false;
		headerBorder.MouseEnter += delegate
		{
			if (!isExpanded)
			{
				headerBorder.Background = VersionCardHoverBrush;
			}
		};
		headerBorder.MouseLeave += delegate
		{
			if (!isExpanded)
			{
				headerBorder.Background = VersionCardBgBrush;
			}
		};
		headerBorder.MouseLeftButtonUp += delegate
		{
			isExpanded = !isExpanded;
			if (isExpanded)
			{
				headerBorder.Background = CardSelectedBrush;
				nameText.Foreground = TextWhiteBrush;
				DoubleAnimation animation = new DoubleAnimation(180.0, TimeSpan.FromMilliseconds(250L, 0L))
				{
					EasingFunction = new CubicEase
					{
						EasingMode = EasingMode.EaseOut
					}
				};
				if (arrowText.RenderTransform is RotateTransform rotateTransform)
				{
					rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
				}
				if (expandPanel.Children.Count == 0)
				{
					CreateModLoaderSection(expandPanel, versionId2);
				}
				expandPanel.Visibility = Visibility.Visible;
				DoubleAnimation animation2 = new DoubleAnimation(260.0, TimeSpan.FromMilliseconds(350L, 0L))
				{
					EasingFunction = new CubicEase
					{
						EasingMode = EasingMode.EaseOut
					}
				};
				expandPanel.BeginAnimation(FrameworkElement.HeightProperty, animation2);
				DoubleAnimation animation3 = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(300L, 0L))
				{
					BeginTime = TimeSpan.FromMilliseconds(100L, 0L),
					EasingFunction = new CubicEase
					{
						EasingMode = EasingMode.EaseOut
					}
				};
				expandPanel.BeginAnimation(UIElement.OpacityProperty, animation3);
				SelectedVersionText.Text = versionId2;
				_selectedVersionId = versionId2;
				_selectedLoaderName = null;
				_selectedLoaderVersion = null;
				_selectedOptifineVersion = null;
				DownloadButton.IsEnabled = true;
			}
			else
			{
				headerBorder.Background = VersionCardBgBrush;
				nameText.Foreground = TextWhiteBrush;
				DoubleAnimation animation4 = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(250L, 0L))
				{
					EasingFunction = new CubicEase
					{
						EasingMode = EasingMode.EaseIn
					}
				};
				if (arrowText.RenderTransform is RotateTransform rotateTransform2)
				{
					rotateTransform2.BeginAnimation(RotateTransform.AngleProperty, animation4);
				}
				DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(300L, 0L))
				{
					EasingFunction = new CubicEase
					{
						EasingMode = EasingMode.EaseIn
					}
				};
				doubleAnimation.Completed += delegate
				{
					expandPanel.Visibility = Visibility.Collapsed;
				};
				expandPanel.BeginAnimation(FrameworkElement.HeightProperty, doubleAnimation);
				DoubleAnimation animation5 = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(200L, 0L));
				expandPanel.BeginAnimation(UIElement.OpacityProperty, animation5);
			}
		};
		VersionList.Children.Add(stackPanel);
		ApplyEnterAnimation(stackPanel, _listEnterMode, _staggerIndex);
		_staggerIndex++;
	}
	private void CreateModLoaderSection(StackPanel parent, string gameVersion)
	{
		StackPanel parent2 = parent;
		string gameVersion2 = gameVersion;
		string[] mainLoaders = new string[4] { "Forge", "Fabric", "NeoForge", "Quilt" };
		Border selectedMainLoader = null;
		StackPanel mainVersionPanel = null;
		Border optifineBorder = null;
		StackPanel optifineVersionPanel = null;
		TextBlock optifineCheckText = null;
		TextBlock optName = null;
		bool optifineSelected = false;
		bool optifineEnabled = true;

		foreach (string loader in mainLoaders)
		{
			Border loaderBorder = new Border
			{
				Height = 44.0,
				Margin = new Thickness(0.0, 2.0, 0.0, 2.0),
				Padding = new Thickness(16.0, 0.0, 16.0, 0.0),
				Background = Bg37Brush,
				BorderBrush = Border60Brush,
				BorderThickness = new Thickness(1.0),
				CornerRadius = new CornerRadius(6.0),
				Cursor = Cursors.Hand
			};
			Grid grid = new Grid();
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Auto) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Auto) });
			TextBlock crossText = new TextBlock
			{
				Text = "\u2715",
				Foreground = Blue72Brush,
				FontSize = 14.0,
				FontWeight = FontWeights.Bold,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = new Thickness(0.0, 0.0, 12.0, 0.0),
				Visibility = Visibility.Collapsed
			};
			Grid.SetColumn(crossText, 0);
			grid.Children.Add(crossText);
			TextBlock loaderName = new TextBlock
			{
				Text = loader,
				Foreground = Gray220Brush,
				FontSize = 13.0,
				FontWeight = FontWeights.Medium,
				VerticalAlignment = VerticalAlignment.Center
			};
			Grid.SetColumn(loaderName, 1);
			grid.Children.Add(loaderName);
			TextBlock loaderArrow = new TextBlock
			{
				Text = "\u25BC",
				Foreground = Gray120Brush,
				FontSize = 10.0,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Right,
				RenderTransformOrigin = new Point(0.5, 0.5),
				RenderTransform = new RotateTransform(0.0)
			};
			Grid.SetColumn(loaderArrow, 2);
			grid.Children.Add(loaderArrow);
			loaderBorder.Child = grid;
			parent2.Children.Add(loaderBorder);
			StackPanel loaderVersionPanel = new StackPanel
			{
				Margin = new Thickness(16.0, 2.0, 0.0, 4.0),
				Opacity = 0.0,
				Height = 0.0,
				Visibility = Visibility.Collapsed
			};
			parent2.Children.Add(loaderVersionPanel);
			loaderBorder.MouseEnter += delegate
			{
				if (selectedMainLoader != loaderBorder)
					loaderBorder.Background = Bg50Brush;
			};
			loaderBorder.MouseLeave += delegate
			{
				if (selectedMainLoader != loaderBorder)
					loaderBorder.Background = Bg37Brush;
			};
			loaderBorder.MouseLeftButtonUp += delegate
			{
				if (selectedMainLoader == loaderBorder)
				{
					crossText.Visibility = Visibility.Collapsed;
					loaderBorder.Background = Bg37Brush;
					loaderBorder.BorderBrush = Border60Brush;
					loaderName.Foreground = Gray220Brush;
					if (loaderArrow.RenderTransform is RotateTransform rt)
						rt.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(200L)));
					DoubleAnimation collapseAnim = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(250L))
					{
						EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
					};
					collapseAnim.Completed += delegate { loaderVersionPanel.Visibility = Visibility.Collapsed; };
					loaderVersionPanel.BeginAnimation(FrameworkElement.HeightProperty, collapseAnim);
					loaderVersionPanel.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(150L)));
					selectedMainLoader = null;
					mainVersionPanel = null;
					_selectedLoaderName = null;
					_selectedLoaderVersion = null;
					SetOptifineEnabled(true, ref optifineSelected, optifineBorder, optifineCheckText, optName, optifineVersionPanel, parent2, gameVersion2);
					UpdateParentHeight(parent2, false, null, optifineSelected, optifineVersionPanel);
					UpdateSelectedVersionText(gameVersion2);
				}
				else
				{
					if (selectedMainLoader != null)
						DeselectMainLoader(selectedMainLoader, parent2);
					crossText.Visibility = Visibility.Visible;
					loaderBorder.Background = Blue40Brush;
					loaderBorder.BorderBrush = Blue72Brush;
					loaderName.Foreground = Brushes.White;
					if (loaderArrow.RenderTransform is RotateTransform rt2)
						rt2.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation(180.0, TimeSpan.FromMilliseconds(250L)) { EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } });
					if (loaderVersionPanel.Children.Count == 0)
						CreateLoaderVersionItems(loaderVersionPanel, loader, gameVersion2);
					loaderVersionPanel.Visibility = Visibility.Visible;
					loaderVersionPanel.BeginAnimation(FrameworkElement.HeightProperty, new DoubleAnimation(200.0, TimeSpan.FromMilliseconds(300L)) { EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } });
					loaderVersionPanel.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(250L)) { BeginTime = TimeSpan.FromMilliseconds(80L) });
					selectedMainLoader = loaderBorder;
					mainVersionPanel = loaderVersionPanel;
					bool isForge = loader == "Forge";
					SetOptifineEnabled(isForge, ref optifineSelected, optifineBorder, optifineCheckText, optName, optifineVersionPanel, parent2, gameVersion2);
					UpdateParentHeight(parent2, true, mainVersionPanel, optifineSelected, optifineVersionPanel);
				}
			};
		}

		Border separatorBorder = new Border
		{
			Height = 1,
			Margin = new Thickness(0, 6, 0, 6),
			Background = Border60Brush
		};
		parent2.Children.Add(separatorBorder);

		optifineBorder = new Border
		{
			Height = 44.0,
			Margin = new Thickness(0.0, 2.0, 0.0, 2.0),
			Padding = new Thickness(16.0, 0.0, 16.0, 0.0),
			Background = Bg37Brush,
			BorderBrush = Border60Brush,
			BorderThickness = new Thickness(1.0),
			CornerRadius = new CornerRadius(6.0),
			Cursor = Cursors.Hand
		};
		Grid optGrid = new Grid();
		optGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Auto) });
		optGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });
		optGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Auto) });
		optifineCheckText = new TextBlock
		{
			Text = "\u2713",
			Foreground = Blue72Brush,
			FontSize = 14.0,
			FontWeight = FontWeights.Bold,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 12.0, 0.0),
			Visibility = Visibility.Collapsed
		};
		Grid.SetColumn(optifineCheckText, 0);
		optGrid.Children.Add(optifineCheckText);
		optName = new TextBlock
		{
			Text = "Optifine",
			Foreground = Gray220Brush,
			FontSize = 13.0,
			FontWeight = FontWeights.Medium,
			VerticalAlignment = VerticalAlignment.Center
		};
		Grid.SetColumn(optName, 1);
		optGrid.Children.Add(optName);
		TextBlock optArrow = new TextBlock
		{
			Text = "\u25BC",
			Foreground = Gray120Brush,
			FontSize = 10.0,
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Right,
			RenderTransformOrigin = new Point(0.5, 0.5),
			RenderTransform = new RotateTransform(0.0)
		};
		Grid.SetColumn(optArrow, 2);
		optGrid.Children.Add(optArrow);
		optifineBorder.Child = optGrid;
		parent2.Children.Add(optifineBorder);
		optifineVersionPanel = new StackPanel
		{
			Margin = new Thickness(16.0, 2.0, 0.0, 4.0),
			Opacity = 0.0,
			Height = 0.0,
			Visibility = Visibility.Collapsed
		};
		parent2.Children.Add(optifineVersionPanel);

		optifineBorder.MouseEnter += delegate
		{
			if (!optifineSelected && optifineEnabled)
				optifineBorder.Background = Bg50Brush;
		};
		optifineBorder.MouseLeave += delegate
		{
			if (!optifineSelected)
				optifineBorder.Background = optifineEnabled ? Bg37Brush : Bg30Brush;
		};
		optifineBorder.MouseLeftButtonUp += delegate
		{
			if (!optifineEnabled)
				return;
			if (optifineSelected)
			{
				optifineCheckText.Visibility = Visibility.Collapsed;
				optifineBorder.Background = Bg37Brush;
				optifineBorder.BorderBrush = Border60Brush;
				optName.Foreground = Gray220Brush;
				if (optArrow.RenderTransform is RotateTransform rt)
					rt.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(200L)));
				DoubleAnimation collapseAnim = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(250L))
				{
					EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
				};
				collapseAnim.Completed += delegate { optifineVersionPanel.Visibility = Visibility.Collapsed; };
				optifineVersionPanel.BeginAnimation(FrameworkElement.HeightProperty, collapseAnim);
				optifineVersionPanel.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(150L)));
				optifineSelected = false;
				_selectedOptifineVersion = null;
				UpdateParentHeight(parent2, selectedMainLoader != null, mainVersionPanel, false, null);
				UpdateSelectedVersionText(gameVersion2);
			}
			else
			{
				optifineCheckText.Visibility = Visibility.Visible;
				optifineBorder.Background = Blue40Brush;
				optifineBorder.BorderBrush = Blue72Brush;
				optName.Foreground = Brushes.White;
				if (optArrow.RenderTransform is RotateTransform rt2)
					rt2.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation(180.0, TimeSpan.FromMilliseconds(250L)) { EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } });
				if (optifineVersionPanel.Children.Count == 0)
					CreateLoaderVersionItems(optifineVersionPanel, "Optifine", gameVersion2);
				optifineVersionPanel.Visibility = Visibility.Visible;
				optifineVersionPanel.BeginAnimation(FrameworkElement.HeightProperty, new DoubleAnimation(200.0, TimeSpan.FromMilliseconds(300L)) { EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } });
				optifineVersionPanel.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(250L)) { BeginTime = TimeSpan.FromMilliseconds(80L) });
				optifineSelected = true;
				UpdateParentHeight(parent2, selectedMainLoader != null, mainVersionPanel, true, optifineVersionPanel);
			}
		};

		UpdateParentHeight(parent2, false, null, false, null);
	}

	private void UpdateSelectedVersionText(string gameVersion)
	{
		string text = gameVersion;
		if (!string.IsNullOrEmpty(_selectedLoaderName) && !string.IsNullOrEmpty(_selectedLoaderVersion))
			text += " - " + _selectedLoaderName + " " + _selectedLoaderVersion;
		if (!string.IsNullOrEmpty(_selectedOptifineVersion))
		{
			string optDisplay = _selectedOptifineVersion;
			if (optDisplay.Contains('|'))
				optDisplay = optDisplay.Substring(0, optDisplay.IndexOf('|'));
			text += " + Optifine " + optDisplay;
		}
		SelectedVersionText.Text = text;
	}

	private void UpdateParentHeight(StackPanel parent, bool mainExpanded, StackPanel mainPanel, bool optExpanded, StackPanel optPanel)
	{
		double h = 4 * 48 + 13;
		if (mainExpanded)
			h += 204;
		if (optExpanded)
			h += 204;
		parent.BeginAnimation(FrameworkElement.HeightProperty, null);
		parent.Height = h;
	}

	private void SetOptifineEnabled(bool enabled, ref bool optifineSelected, Border optifineBorder, TextBlock optifineCheckText, TextBlock optName, StackPanel optifineVersionPanel, StackPanel parent, string gameVersion)
	{
		if (enabled)
		{
			optifineBorder.Opacity = 1.0;
			optifineBorder.Cursor = Cursors.Hand;
			optifineBorder.Background = optifineSelected ? Blue40Brush : Bg37Brush;
			optifineBorder.BorderBrush = optifineSelected ? Blue72Brush : Border60Brush;
			optName.Foreground = optifineSelected ? Brushes.White : Gray220Brush;
		}
		else
		{
			if (optifineSelected)
			{
				optifineCheckText.Visibility = Visibility.Collapsed;
				if (optifineVersionPanel != null)
				{
					DoubleAnimation collapseAnim = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(250L))
					{
						EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
					};
					collapseAnim.Completed += delegate { optifineVersionPanel.Visibility = Visibility.Collapsed; };
					optifineVersionPanel.BeginAnimation(FrameworkElement.HeightProperty, collapseAnim);
					optifineVersionPanel.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(150L)));
				}
				optifineSelected = false;
				_selectedOptifineVersion = null;
				UpdateSelectedVersionText(gameVersion);
			}
			optifineBorder.Opacity = 0.4;
			optifineBorder.Cursor = Cursors.No;
			optifineBorder.Background = Bg30Brush;
			optifineBorder.BorderBrush = Bg45Brush;
			optName.Foreground = Gray100Brush;
		}
	}

	private void DeselectMainLoader(Border loaderBorder, StackPanel parent)
	{
		for (int i = 0; i < parent.Children.Count; i++)
		{
			if (parent.Children[i] != loaderBorder || !(loaderBorder.Child is Grid grid))
				continue;
			foreach (object child in grid.Children)
			{
				if (child is TextBlock textBlock)
				{
					if (textBlock.Text == "\u2715")
						textBlock.Visibility = Visibility.Collapsed;
					if (textBlock.Text == "Forge" || textBlock.Text == "Fabric" || textBlock.Text == "NeoForge" || textBlock.Text == "Quilt")
						textBlock.Foreground = Gray220Brush;
					if (textBlock.Text == "\u25BC" && textBlock.RenderTransform is RotateTransform rotateTransform)
						rotateTransform.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(200L)));
				}
			}
			loaderBorder.Background = Bg37Brush;
			loaderBorder.BorderBrush = Border60Brush;
			if (i + 1 < parent.Children.Count)
			{
				if (parent.Children[i + 1] is StackPanel versionPanel)
				{
					DoubleAnimation collapseAnim = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(250L))
					{
						EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
					};
					collapseAnim.Completed += delegate { versionPanel.Visibility = Visibility.Collapsed; };
					versionPanel.BeginAnimation(FrameworkElement.HeightProperty, collapseAnim);
					versionPanel.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(150L)));
				}
			}
			break;
		}
	}
	private async void CreateLoaderVersionItems(StackPanel parent, string loaderName, string gameVersion)
	{
		string loaderName2 = loaderName;
		string gameVersion2 = gameVersion;
		string cacheKey = "loader_" + loaderName2 + "_" + gameVersion2;
		List<string> cachedVersions = DataCache.Get<List<string>>(new object[1] { cacheKey });
		if (cachedVersions != null && cachedVersions.Count > 0)
		{
			RenderLoaderVersions(parent, cachedVersions, loaderName2, gameVersion2);
		}
		else
		{
			parent.Children.Clear();
			TextBlock loadingText = new TextBlock
			{
				Foreground = Gray136Brush,
				FontSize = 12.0,
				Margin = new Thickness(12.0, 8.0, 0.0, 8.0)
			};
			loadingText.SetResourceReference(TextBlock.TextProperty, "StatusLoading");
			parent.Children.Add(loadingText);
		}
		List<string> versions = await Task.Run(() => DownloadManager.GetLoaderVersionsAsync(loaderName2, gameVersion2));
		DataCache.Set(versions, cacheKey);
		RenderLoaderVersions(parent, versions, loaderName2, gameVersion2);
	}

	private void RenderLoaderVersions(StackPanel parent, List<string> versions, string loaderName, string gameVersion)
	{
		string gameVersion2 = gameVersion;
		string loaderName2 = loaderName;
		parent.Children.Clear();
		if (versions == null || versions.Count == 0)
		{
			TextBlock textBlock = new TextBlock
			{
				Foreground = Gray136Brush,
				FontSize = 12.0,
				Margin = new Thickness(12.0, 8.0, 0.0, 8.0)
			};
			textBlock.SetResourceReference(TextBlock.TextProperty, "StatusNoAvailableVersion");
			parent.Children.Add(textBlock);
			return;
		}
		ScrollViewer scrollViewer = new ScrollViewer
		{
			VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
			HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
			Height = 190.0
		};
		scrollViewer.PreviewMouseWheel += SmoothScroll_PreviewMouseWheel;
		StackPanel stackPanel = new StackPanel();
		foreach (string version in versions)
		{
			string displayText = version;
			if (loaderName2 == "Optifine" && version.Contains('|'))
				displayText = version.Substring(0, version.IndexOf('|'));
			Border item = new Border
			{
				Height = 36.0,
				Margin = new Thickness(0.0, 1.0, 0.0, 1.0),
				Padding = new Thickness(12.0, 0.0, 12.0, 0.0),
				Background = Bg30Brush,
				CornerRadius = new CornerRadius(4.0),
				Cursor = Cursors.Hand
			};
			TextBlock text = new TextBlock
			{
				Text = displayText,
				Foreground = Gray200Brush,
				FontSize = 12.0,
				VerticalAlignment = VerticalAlignment.Center
			};
			item.Child = text;
			item.MouseEnter += delegate
			{
				item.Background = Bg50Brush;
				text.Foreground = Brushes.White;
			};
			item.MouseLeave += delegate
			{
				item.Background = Bg30Brush;
				text.Foreground = Gray200Brush;
			};
			string capturedVer = version;
			item.MouseLeftButtonUp += delegate
			{
				_selectedVersionId = gameVersion2;
				if (loaderName2 == "Optifine")
				{
					_selectedOptifineVersion = capturedVer;
				}
				else
				{
					_selectedLoaderName = loaderName2;
					_selectedLoaderVersion = capturedVer;
				}
				UpdateSelectedVersionText(gameVersion2);
				DownloadButton.IsEnabled = true;
			};
			stackPanel.Children.Add(item);
		}
		scrollViewer.Content = stackPanel;
		parent.Children.Add(scrollViewer);
	}

	private void DownloadCategory_Enter(object sender, MouseEventArgs e)
	{
		if (sender is Border { Child: TextBlock child } border)
		{
			border.Background = Bg62Brush;
			child.Foreground = Brushes.White;
		}
	}

	private void DownloadCategory_Leave(object sender, MouseEventArgs e)
	{
		if (sender is Border { Child: TextBlock child } border)
		{
			border.Background = Brushes.Transparent;
			child.Foreground = Gray204Brush;
		}
	}

	private void ResourceDownload_Click(object sender, RoutedEventArgs e)
	{
		if (sender is Button { Tag: ModrinthProject tag })
		{
			ShowVersionSelectPage(tag);
		}
	}

	private async void ShowVersionSelectPage(ModrinthProject proj)
	{
		ModrinthProject proj2 = proj;
		try
		{
			DownloadPage.Visibility = Visibility.Collapsed;
			DownloadCenterPage.Visibility = Visibility.Collapsed;
			ContentArea.Visibility = Visibility.Visible;
			string currentResourceType = _currentResourceType;
			if (1 == 0)
			{
			}
			ResourceType resourceType = currentResourceType switch
			{
				"\u6A21\u7EC4" => ResourceType.Mod, 
				"\u5149\u5F71" => ResourceType.Shader, 
				"\u6750\u8D28" => ResourceType.ResourcePack, 
				_ => ResourceType.Mod, 
			};
			if (1 == 0)
			{
			}
			ResourceType resType = resourceType;
			_versionSelectOverlay = new Border
			{
				Background = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
				Opacity = 0.0,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				Cursor = Cursors.Arrow
			};
			_versionSelectOverlay.MouseLeftButtonUp += delegate
			{
				CloseVersionSelectPage();
			};
			_versionSelectPanel = new Border
			{
				Background = new SolidColorBrush(Color.FromRgb(28, 28, 32)),
				CornerRadius = new CornerRadius(12.0),
				Width = 460.0,
				HorizontalAlignment = HorizontalAlignment.Right,
				VerticalAlignment = VerticalAlignment.Stretch,
				Margin = new Thickness(0.0, 0.0, 0.0, 0.0),
				ClipToBounds = true
			};
			_versionSelectPanel.RenderTransform = new TranslateTransform
			{
				X = 460.0
			};
			Grid root = new Grid
			{
				RowDefinitions = 
				{
					new RowDefinition
					{
						Height = GridLength.Auto
					},
					new RowDefinition
					{
						Height = new GridLength(1.0, GridUnitType.Star)
					}
				}
			};
			Grid header = BuildVersionSelectHeader(proj2);
			root.Children.Add(header);
			Grid.SetRow(header, 0);
			ScrollViewer bodyScroll = new ScrollViewer
			{
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
				Margin = new Thickness(0.0),
				Padding = new Thickness(20.0, 0.0, 20.0, 20.0)
			};
			bodyScroll.PreviewMouseWheel += SmoothScroll_PreviewMouseWheel;
			StackPanel bodyStack = new StackPanel();
			TextBlock loadingText = new TextBlock
			{
				Foreground = Gray140Brush,
				FontSize = 13.0,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(0.0, 40.0, 0.0, 0.0)
			};
			loadingText.SetResourceReference(TextBlock.TextProperty, "StatusLoadingVersionList");
			bodyStack.Children.Add(loadingText);
			bodyScroll.Content = bodyStack;
			root.Children.Add(bodyScroll);
			Grid.SetRow(bodyScroll, 1);
			_versionSelectPanel.Child = root;
			ContentArea.Children.Add(_versionSelectOverlay);
			ContentArea.Children.Add(_versionSelectPanel);
			CubicEase ease = new CubicEase
			{
				EasingMode = EasingMode.EaseOut
			};
			TimeSpan dur = TimeSpan.FromMilliseconds(350L, 0L);
			_versionSelectOverlay.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0.6, dur)
			{
				EasingFunction = ease
			});
			((TranslateTransform)_versionSelectPanel.RenderTransform).BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(0.0, dur)
			{
				EasingFunction = ease
			});
			List<ModrinthVersion> cachedVersions = DataCache.Get<List<ModrinthVersion>>(new object[2] { "project_versions", proj2.ProjectId });
			if (cachedVersions != null && cachedVersions.Count > 0)
			{
				RenderGroupedVersions(bodyStack, proj2, cachedVersions, resType);
			}
			List<ModrinthVersion> versions = await Task.Run(() => ModrinthApi.GetProjectVersions(proj2.ProjectId));
			var depIds = versions.SelectMany(v => v.Dependencies.Where(d => d.DependencyType == "required" && !string.IsNullOrEmpty(d.ProjectId)).Select(d => d.ProjectId)).Distinct().ToList();
			if (depIds.Count > 0)
			{
				var depNames = await Task.Run(() => ModrinthApi.GetProjectNamesAsync(depIds));
				foreach (var ver in versions)
				{
					foreach (var dep in ver.Dependencies)
					{
						if (depNames.TryGetValue(dep.ProjectId, out var title))
						{
							dep.ProjectTitle = title;
						}
					}
				}
			}
			DataCache.Set(versions, "project_versions", proj2.ProjectId);
			RenderGroupedVersions(bodyStack, proj2, versions, resType);
		}
		catch (Exception)
		{
		}
	}

	private Grid BuildVersionSelectHeader(ModrinthProject proj)
	{
		Grid grid = new Grid
		{
			Background = new SolidColorBrush(Color.FromRgb(24, 24, 28)),
			Height = 110.0,
			ClipToBounds = true
		};
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = new GridLength(1.0, GridUnitType.Star)
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		bool isShader = _currentResourceType == "\u5149\u5F71";
		Border border = new Border
		{
			Width = 64.0,
			Height = 64.0,
			CornerRadius = new CornerRadius(12.0),
			Background = isShader ? Brushes.Transparent : Bg55Brush,
			ClipToBounds = true,
			Margin = new Thickness(20.0, 0.0, 0.0, 0.0),
			VerticalAlignment = VerticalAlignment.Center
		};
		if (!string.IsNullOrEmpty(proj.IconUrl))
		{
			LoadImageAsync(border, proj.IconUrl, isShader);
		}
		else
		{
			border.Child = new TextBlock
			{
				Text = "?",
				FontSize = 24.0,
				Foreground = Gray120Brush,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
		}
		Grid.SetColumn(border, 0);
		grid.Children.Add(border);
		StackPanel stackPanel = new StackPanel
		{
			Margin = new Thickness(14.0, 0.0, 0.0, 0.0),
			VerticalAlignment = VerticalAlignment.Center
		};
		TextBlock element = new TextBlock
		{
			Text = proj.Title,
			Foreground = Brushes.White,
			FontSize = 15.0,
			FontWeight = FontWeights.SemiBold,
			TextTrimming = TextTrimming.CharacterEllipsis
		};
		stackPanel.Children.Add(element);
		string text = (string.IsNullOrEmpty(proj.Description) ? LanguageManager.Get("ResNoDesc") : ((proj.Description.Length > 50) ? (proj.Description.Substring(0, 50) + "...") : proj.Description));
		TextBlock element2 = new TextBlock
		{
			Text = text,
			Foreground = Gray150Brush,
			FontSize = 11.0,
			Margin = new Thickness(0.0, 4.0, 0.0, 0.0),
			TextTrimming = TextTrimming.CharacterEllipsis,
			MaxWidth = 280.0
		};
		stackPanel.Children.Add(element2);
		Grid.SetColumn(stackPanel, 1);
		grid.Children.Add(stackPanel);
		return grid;
	}

	private void CloseVersionSelectPage()
	{
		if (_versionSelectPanel == null || _versionSelectOverlay == null)
		{
			return;
		}
		CubicEase easingFunction = new CubicEase
		{
			EasingMode = EasingMode.EaseIn
		};
		TimeSpan timeSpan = TimeSpan.FromMilliseconds(280L, 0L);
		DoubleAnimation doubleAnimation = new DoubleAnimation(460.0, timeSpan)
		{
			EasingFunction = easingFunction
		};
		DoubleAnimation animation = new DoubleAnimation(0.0, timeSpan)
		{
			EasingFunction = easingFunction
		};
		doubleAnimation.Completed += delegate
		{
			ContentArea.Children.Remove(_versionSelectOverlay);
			ContentArea.Children.Remove(_versionSelectPanel);
			_versionSelectOverlay = null;
			_versionSelectPanel = null;
			DownloadPage.Visibility = Visibility.Visible;
			ContentArea.Visibility = Visibility.Visible;
			if (_currentResourceType == "\u6E38\u620F")
			{
				GameCategoryBar.Visibility = Visibility.Visible;
				ResourceSearchBar.Visibility = Visibility.Collapsed;
				BottomActionBar.Visibility = Visibility.Visible;
				ListArea.CornerRadius = new CornerRadius(0.0);
				if (_allVersions.Count > 0)
				{
					RefreshVersionList();
				}
			}
			else
			{
				GameCategoryBar.Visibility = Visibility.Collapsed;
				ResourceSearchBar.Visibility = Visibility.Visible;
				BottomActionBar.Visibility = Visibility.Collapsed;
				ListArea.CornerRadius = new CornerRadius(0.0, 0.0, 8.0, 8.0);
				base.Dispatcher.BeginInvoke((Action)delegate
				{
					Focus();
					if (ResourceSearchBox != null)
					{
						ResourceSearchBox.Focus();
						Keyboard.Focus(ResourceSearchBox);
					}
				}, DispatcherPriority.ContextIdle);
			}
		};
		((TranslateTransform)_versionSelectPanel.RenderTransform).BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
		_versionSelectOverlay.BeginAnimation(UIElement.OpacityProperty, animation);
	}

	private void RenderGroupedVersions(StackPanel bodyStack, ModrinthProject proj, List<ModrinthVersion> versions, ResourceType resType)
	{
		bodyStack.Children.Clear();
		if (versions.Count == 0)
		{
			TextBlock textBlock = new TextBlock
			{
				Foreground = Gray140Brush,
				FontSize = 13.0,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(0.0, 40.0, 0.0, 0.0)
			};
			textBlock.SetResourceReference(TextBlock.TextProperty, "StatusNoAvailableVersion");
			bodyStack.Children.Add(textBlock);
			return;
		}
		var orderedEnumerable = (from x in versions.SelectMany((ModrinthVersion v) => v.GameVersions.Select((string gv) => new
			{
				Version = v,
				GameVersion = gv
			}))
			group x by x.GameVersion).OrderByDescending(g => g.Key, new VersionComparer());
		string[] loaderOrder = new string[6] { "fabric", "forge", "quilt", "neoforge", "iris", "optifine" };
		foreach (var item in orderedEnumerable)
		{
			var orderedEnumerable2 = from x in item.Select(x => x.Version).Distinct().SelectMany((ModrinthVersion v) => ((v.Loaders.Count == 0) ? ((IEnumerable<string>)new List<string> { "any" }) : ((IEnumerable<string>)v.Loaders)).Select((string l) => new
				{
					Version = v,
					Loader = l.ToLowerInvariant()
				}))
				group x by x.Loader into g
				orderby (Array.IndexOf(loaderOrder, g.Key) < 0) ? 99 : Array.IndexOf(loaderOrder, g.Key), g.Key
				select g;
			foreach (var item2 in orderedEnumerable2)
			{
				object obj;
				if (!(item2.Key == "any"))
				{
					if (!(item2.Key == "neoforge"))
					{
						char reference = char.ToUpper(item2.Key[0]);
						obj = string.Concat(new ReadOnlySpan<char>(ref reference), item2.Key.Substring(1));
					}
					else
					{
						obj = "NeoForge";
					}
				}
				else
				{
					obj = LanguageManager.Get("ResLoaderAny");
				}
				string text = (string)obj;
				List<ModrinthVersion> versions2 = item2.Select(x => x.Version).Distinct().ToList();
				Border element = CreateVersionDropdown(text + " \u00B7 " + item.Key, versions2, proj, resType);
				bodyStack.Children.Add(element);
			}
		}
	}

	private Border CreateVersionDropdown(string title, List<ModrinthVersion> versions, ModrinthProject proj, ResourceType resType)
	{
		Border border = new Border
		{
			Background = Bg38Brush,
			CornerRadius = new CornerRadius(8.0),
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0),
			ClipToBounds = true
		};
		StackPanel stackPanel = new StackPanel();
		Border border2 = new Border
		{
			Height = 40.0,
			Padding = new Thickness(14.0, 0.0, 14.0, 0.0),
			Background = Bg38Brush,
			Cursor = Cursors.Hand
		};
		Grid grid = new Grid();
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = new GridLength(1.0, GridUnitType.Star)
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		TextBlock element = new TextBlock
		{
			Text = title,
			Foreground = Brushes.White,
			FontSize = 13.0,
			FontWeight = FontWeights.Medium,
			VerticalAlignment = VerticalAlignment.Center
		};
		Grid.SetColumn(element, 0);
		grid.Children.Add(element);
		TextBlock element2 = new TextBlock
		{
			Text = string.Format(LanguageManager.Get("ResVersionCount"), versions.Count),
			Foreground = Gray130Brush,
			FontSize = 11.0,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 10.0, 0.0)
		};
		Grid.SetColumn(element2, 1);
		grid.Children.Add(element2);
		TextBlock arrow = new TextBlock
		{
			Text = "\u25BE",
			Foreground = Gray170Brush,
			FontSize = 12.0,
			VerticalAlignment = VerticalAlignment.Center,
			RenderTransformOrigin = new Point(0.5, 0.5),
			RenderTransform = new RotateTransform(0.0)
		};
		Grid.SetColumn(arrow, 2);
		grid.Children.Add(arrow);
		border2.Child = grid;
		StackPanel contentPanel = new StackPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 0.0)
		};
		contentPanel.RenderTransform = new ScaleTransform(1.0, 0.0);
		contentPanel.RenderTransformOrigin = new Point(0.5, 0.0);
		contentPanel.Visibility = Visibility.Collapsed;
		foreach (ModrinthVersion version in versions)
		{
			contentPanel.Children.Add(CreateVersionItem(version, proj, resType));
		}
		bool expanded = false;
		border2.MouseLeftButtonUp += delegate
		{
			expanded = !expanded;
			CubicEase easingFunction = new CubicEase
			{
				EasingMode = EasingMode.EaseOut
			};
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(250L, 0L);
			if (expanded)
			{
				contentPanel.Visibility = Visibility.Visible;
				DoubleAnimation animation = new DoubleAnimation(0.0, 1.0, timeSpan)
				{
					EasingFunction = easingFunction
				};
				((ScaleTransform)contentPanel.RenderTransform).BeginAnimation(ScaleTransform.ScaleYProperty, animation);
				((RotateTransform)arrow.RenderTransform).BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation(180.0, timeSpan)
				{
					EasingFunction = easingFunction
				});
			}
			else
			{
				DoubleAnimation doubleAnimation = new DoubleAnimation(1.0, 0.0, timeSpan)
				{
					EasingFunction = easingFunction
				};
				doubleAnimation.Completed += delegate
				{
					contentPanel.Visibility = Visibility.Collapsed;
				};
				((ScaleTransform)contentPanel.RenderTransform).BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation);
				((RotateTransform)arrow.RenderTransform).BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation(0.0, timeSpan)
				{
					EasingFunction = easingFunction
				});
			}
		};
		border2.MouseEnter += delegate(object s, MouseEventArgs _)
		{
			if (s is Border border4)
			{
				border4.Background = Bg48Brush;
			}
		};
		border2.MouseLeave += delegate(object s, MouseEventArgs _)
		{
			if (s is Border border3)
			{
				border3.Background = Bg38Brush;
			}
		};
		stackPanel.Children.Add(border2);
		stackPanel.Children.Add(contentPanel);
		border.Child = stackPanel;
		return border;
	}

	private static readonly Dictionary<string, ImageSource> _iconCache = new Dictionary<string, ImageSource>();

	private static ImageSource GetCachedIcon(string packPath)
	{
		if (!_iconCache.TryGetValue(packPath, out ImageSource source))
		{
			BitmapImage bmp = new BitmapImage();
			bmp.BeginInit();
			bmp.UriSource = new Uri($"pack://application:,,,/{packPath}", UriKind.Absolute);
			bmp.DecodePixelWidth = 32;
			bmp.CacheOption = BitmapCacheOption.OnLoad;
			bmp.EndInit();
			bmp.Freeze();
			_iconCache[packPath] = bmp;
			source = bmp;
		}
		return source;
	}

	private static Image MakeIcon(string packPath, string toolTip)
	{
		Image img = new Image
		{
			Source = GetCachedIcon(packPath),
			Width = 16.0,
			Height = 16.0,
			Margin = new Thickness(0.0, 0.0, 4.0, 0.0),
			VerticalAlignment = VerticalAlignment.Center,
			ToolTip = toolTip
		};
		RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.LowQuality);
		return img;
	}

	private Border CreateVersionItem(ModrinthVersion ver, ModrinthProject proj, ResourceType resType)
	{
		Border border = new Border
		{
			Background = Bg32Brush,
			CornerRadius = new CornerRadius(6.0),
			Padding = new Thickness(12.0, 8.0, 12.0, 8.0),
			Margin = new Thickness(8.0, 2.0, 8.0, 2.0),
			Cursor = Cursors.Hand
		};
		Grid grid = new Grid();
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = new GridLength(1.0, GridUnitType.Star)
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		StackPanel stackPanel = new StackPanel
		{
			VerticalAlignment = VerticalAlignment.Center
		};
		StackPanel stackPanel2 = new StackPanel
		{
			Orientation = Orientation.Horizontal
		};
		if (ver.Loaders != null && ver.Loaders.Count > 0)
		{
			foreach (string loader in ver.Loaders)
			{
				string loaderIcon = loader.ToLowerInvariant() switch
				{
					"forge" => "assests/forge.png",
					"fabric" => "assests/fabric.png",
					"quilt" => "assests/quilt_x16.png",
					"neoforge" => "assests/neo_logo.png",
					"optifine" or "optifabric" => "assests/optifine.png",
					_ => null
				};
				if (loaderIcon != null)
				{
					Image loaderImg = new Image
					{
						Source = GetCachedIcon(loaderIcon),
						Width = 14.0,
						Height = 14.0,
						Margin = new Thickness(0.0, 0.0, 4.0, 0.0),
						VerticalAlignment = VerticalAlignment.Center,
						ToolTip = loader
					};
					RenderOptions.SetBitmapScalingMode(loaderImg, BitmapScalingMode.LowQuality);
					stackPanel2.Children.Add(loaderImg);
				}
			}
		}
		string versionType = ver.VersionType;
		string typeIcon = versionType switch
		{
			"release" => "assests/release.png",
			"beta" => "assests/sna.png",
			"alpha" => "assests/old.png",
			_ => "assests/release.png"
		};
		Image typeImg = new Image
		{
			Source = GetCachedIcon(typeIcon),
			Width = 14.0,
			Height = 14.0,
			Margin = new Thickness(0.0, 0.0, 4.0, 0.0),
			VerticalAlignment = VerticalAlignment.Center,
			ToolTip = versionType
		};
		RenderOptions.SetBitmapScalingMode(typeImg, BitmapScalingMode.LowQuality);
		stackPanel2.Children.Add(typeImg);
		TextBlock element = new TextBlock
		{
			Text = (string.IsNullOrEmpty(ver.Name) ? ver.VersionNumber : ver.Name),
			Foreground = Brushes.White,
			FontSize = 12.0,
			FontWeight = FontWeights.Medium,
			TextTrimming = TextTrimming.CharacterEllipsis,
			MaxWidth = 220.0,
			VerticalAlignment = VerticalAlignment.Center
		};
		stackPanel2.Children.Add(element);
		stackPanel.Children.Add(stackPanel2);
		if (!string.IsNullOrEmpty(ver.DatePublished))
		{
			try
			{
				DateTime dateTime = DateTime.Parse(ver.DatePublished);
				stackPanel.Children.Add(new TextBlock
				{
					Text = dateTime.ToString("yyyy-MM-dd"),
					Foreground = Gray130Brush,
					FontSize = 10.0,
					Margin = new Thickness(0.0, 2.0, 0.0, 0.0)
				});
			}
			catch
			{
			}
		}
		var requiredDeps = ver.Dependencies.Where(d => d.DependencyType == "required").ToList();
		if (requiredDeps.Count > 0)
		{
			StackPanel depPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Margin = new Thickness(0.0, 3.0, 0.0, 0.0)
			};
			TextBlock depLabel = new TextBlock
			{
				Text = LanguageManager.Get("DepRequired") + " ",
				Foreground = Yellow200Brush,
				FontSize = 10.0,
				VerticalAlignment = VerticalAlignment.Center
			};
			depPanel.Children.Add(depLabel);
			for (int di = 0; di < requiredDeps.Count; di++)
			{
				if (di > 0)
				{
					depPanel.Children.Add(new TextBlock
					{
						Text = ", ",
						Foreground = Gray130Brush,
						FontSize = 10.0,
						VerticalAlignment = VerticalAlignment.Center
					});
				}
				string depProjectId = requiredDeps[di].ProjectId;
				string depDisplay = !string.IsNullOrEmpty(requiredDeps[di].ProjectTitle) ? requiredDeps[di].ProjectTitle : depProjectId;
				var linkBlock = new TextBlock
				{
					Text = depDisplay,
					Foreground = Blue100Brush,
					FontSize = 10.0,
					VerticalAlignment = VerticalAlignment.Center,
					TextTrimming = TextTrimming.CharacterEllipsis,
					MaxWidth = 120.0,
					Cursor = Cursors.Hand,
					TextDecorations = TextDecorations.Underline
				};
				linkBlock.MouseLeftButtonUp += async (s, e) =>
				{
					e.Handled = true;
					CloseVersionSelectPage();
					var depProj = await Task.Run(() => ModrinthApi.GetProjectAsync(depProjectId));
					if (depProj != null)
					{
						Dispatcher.Invoke(() => ShowVersionSelectPage(depProj));
					}
				};
				linkBlock.MouseEnter += (s, e) =>
				{
					if (s is TextBlock tb) tb.Foreground = Blue160Brush;
				};
				linkBlock.MouseLeave += (s, e) =>
				{
					if (s is TextBlock tb) tb.Foreground = Blue100Brush;
				};
				depPanel.Children.Add(linkBlock);
			}
			stackPanel.Children.Add(depPanel);
		}
		Grid.SetColumn(stackPanel, 0);
		grid.Children.Add(stackPanel);
		Button dlBtn = new Button
		{
			Style = (Style)FindResource("CardButton"),
			Height = 26.0,
			FontSize = 11.0,
			VerticalAlignment = VerticalAlignment.Center,
			Tag = new
			{
				Project = proj,
				Version = ver,
				ResType = resType
			}
		};
		dlBtn.SetResourceReference(ContentControl.ContentProperty, "DownloadStart");
		dlBtn.Click += VersionItemDownload_Click;
		Grid.SetColumn(dlBtn, 1);
		grid.Children.Add(dlBtn);
		border.Child = grid;
		border.MouseEnter += delegate(object s, MouseEventArgs _)
		{
			if (s is Border border3)
			{
				border3.Background = Bg42Brush;
			}
		};
		border.MouseLeave += delegate(object s, MouseEventArgs _)
		{
			if (s is Border border2)
			{
				border2.Background = Bg32Brush;
			}
		};
		border.MouseLeftButtonUp += delegate
		{
			VersionItemDownload_Click(dlBtn, null);
		};
		return border;
	}

	private void VersionItemDownload_Click(object sender, RoutedEventArgs e)
	{
		if (sender is Button { Tag: not null, Tag: var tag })
		{
			ModrinthProject proj = (ModrinthProject)((dynamic)tag).Project;
			ModrinthVersion ver = (ModrinthVersion)((dynamic)tag).Version;
			ResourceType resType = (ResourceType)((dynamic)tag).ResType;
			ShowDownloadSourcePopup(proj, ver, resType);
		}
	}

	private void ShowDownloadSourcePopup(ModrinthProject proj, ModrinthVersion ver, ResourceType resType)
	{
		ModrinthProject proj2 = proj;
		ModrinthVersion ver2 = ver;
		if (_versionSelectPanel == null)
		{
			return;
		}
		Border overlay = new Border
		{
			Background = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0)),
			HorizontalAlignment = HorizontalAlignment.Stretch,
			VerticalAlignment = VerticalAlignment.Stretch,
			Cursor = Cursors.Arrow
		};
		Border popup = new Border
		{
			Background = Bg32Brush,
			CornerRadius = new CornerRadius(12.0),
			Width = 340.0,
			Padding = new Thickness(20.0),
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			RenderTransform = new ScaleTransform(0.9, 0.9),
			RenderTransformOrigin = new Point(0.5, 0.5)
		};
		StackPanel stackPanel = new StackPanel();
		TextBlock textBlock = new TextBlock
		{
			Foreground = Brushes.White,
			FontSize = 15.0,
			FontWeight = FontWeights.SemiBold,
			HorizontalAlignment = HorizontalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 0.0, 4.0)
		};
		textBlock.SetResourceReference(TextBlock.TextProperty, "StatusSelectDownloadUrl");
		stackPanel.Children.Add(textBlock);
		TextBlock element = new TextBlock
		{
			Text = (string.IsNullOrEmpty(ver2.Name) ? ver2.VersionNumber : ver2.Name),
			Foreground = Gray140Brush,
			FontSize = 11.0,
			HorizontalAlignment = HorizontalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 0.0, 16.0),
			TextTrimming = TextTrimming.CharacterEllipsis
		};
		stackPanel.Children.Add(element);
		var array = new[]
		{
			new
			{
				Name = LanguageManager.Get("MirrorOfficialName"),
				Desc = LanguageManager.Get("MirrorOfficialDesc"),
				Mirror = ""
			},
			new
			{
				Name = LanguageManager.Get("MirrorGithubName"),
				Desc = LanguageManager.Get("MirrorGithubDesc"),
				Mirror = "ghproxy"
			},
			new
			{
				Name = LanguageManager.Get("MirrorCustomName"),
				Desc = LanguageManager.Get("MirrorCustomDesc"),
				Mirror = "custom"
			}
		};
		TextBox customMirrorBox = null;
		var array2 = array;
		foreach (var anon in array2)
		{
			Border border = new Border
			{
				Background = Bg42Brush,
				CornerRadius = new CornerRadius(8.0),
				Padding = new Thickness(14.0, 10.0, 14.0, 10.0),
				Margin = new Thickness(0.0, 0.0, 0.0, 8.0),
				Cursor = Cursors.Hand,
				Tag = anon.Mirror
			};
			Grid grid = new Grid();
			grid.ColumnDefinitions.Add(new ColumnDefinition
			{
				Width = new GridLength(1.0, GridUnitType.Star)
			});
			grid.ColumnDefinitions.Add(new ColumnDefinition
			{
				Width = GridLength.Auto
			});
			StackPanel stackPanel2 = new StackPanel();
			stackPanel2.Children.Add(new TextBlock
			{
				Text = anon.Name,
				Foreground = Brushes.White,
				FontSize = 13.0,
				FontWeight = FontWeights.Medium
			});
			stackPanel2.Children.Add(new TextBlock
			{
				Text = anon.Desc,
				Foreground = Gray130Brush,
				FontSize = 10.0,
				Margin = new Thickness(0.0, 2.0, 0.0, 0.0)
			});
			Grid.SetColumn(stackPanel2, 0);
			grid.Children.Add(stackPanel2);
			TextBlock element2 = new TextBlock
			{
				Text = "\u25BC",
				Foreground = Gray130Brush,
				FontSize = 14.0,
				VerticalAlignment = VerticalAlignment.Center
			};
			Grid.SetColumn(element2, 1);
			grid.Children.Add(element2);
			border.Child = grid;
			string capturedMirror = anon.Mirror;
			border.MouseLeftButtonUp += delegate
			{
				if (capturedMirror == "custom")
				{
					string text = customMirrorBox?.Text?.Trim() ?? "";
					if (string.IsNullOrEmpty(text))
					{
						customMirrorBox?.Focus();
						return;
					}
					StartVersionDownload(proj2, ver2, resType, text);
				}
				else
				{
					StartVersionDownload(proj2, ver2, resType, capturedMirror);
				}
				CloseDownloadSourcePopup(overlay, popup);
			};
			border.MouseEnter += delegate(object s, MouseEventArgs _)
			{
				if (s is Border border3)
				{
					border3.Background = Bg52Brush;
				}
			};
			border.MouseLeave += delegate(object s, MouseEventArgs _)
			{
				if (s is Border border2)
				{
					border2.Background = Bg42Brush;
				}
			};
			stackPanel.Children.Add(border);
		}
		customMirrorBox = new TextBox
		{
			Background = Bg42Brush,
			Foreground = Brushes.White,
			FontSize = 12.0,
			BorderThickness = new Thickness(1.0),
			BorderBrush = Border60Brush,
			Padding = new Thickness(10.0, 6.0, 10.0, 6.0),
			Margin = new Thickness(0.0, 4.0, 0.0, 0.0),
			Tag = "StatusCustomMirrorHint"
		};
		TextBlock placeholder = new TextBlock
		{
			Foreground = Gray100Brush,
			FontSize = 12.0,
			IsHitTestVisible = false,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(10.0, 0.0, 0.0, 0.0)
		};
		placeholder.SetResourceReference(TextBlock.TextProperty, "StatusCustomMirrorHint");
		Grid grid2 = new Grid();
		grid2.Children.Add(customMirrorBox);
		grid2.Children.Add(placeholder);
		customMirrorBox.TextChanged += delegate
		{
			placeholder.Visibility = ((!string.IsNullOrEmpty(customMirrorBox.Text)) ? Visibility.Collapsed : Visibility.Visible);
		};
		stackPanel.Children.Add(grid2);
		Button button = new Button
		{
			Style = (Style)FindResource("CardButton"),
			Height = 34.0,
			FontSize = 12.0,
			Margin = new Thickness(0.0, 8.0, 0.0, 0.0),
			Background = new SolidColorBrush(Color.FromRgb(50, 50, 54)),
			Foreground = Gray170Brush
		};
		button.Click += delegate
		{
			CloseDownloadSourcePopup(overlay, popup);
		};
		button.SetResourceReference(ContentControl.ContentProperty, "CommonCancel");
		stackPanel.Children.Add(button);
		popup.Child = stackPanel;
		if (_versionSelectPanel.Child is Grid grid3)
		{
			grid3.Children.Add(overlay);
			grid3.Children.Add(popup);
		}
		CubicEase easingFunction = new CubicEase
		{
			EasingMode = EasingMode.EaseOut
		};
		TimeSpan timeSpan = TimeSpan.FromMilliseconds(200L, 0L);
		overlay.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0.0, 1.0, timeSpan)
		{
			EasingFunction = easingFunction
		});
		DoubleAnimation animation = new DoubleAnimation(0.9, 1.0, timeSpan)
		{
			EasingFunction = easingFunction
		};
		((ScaleTransform)popup.RenderTransform).BeginAnimation(ScaleTransform.ScaleXProperty, animation);
		((ScaleTransform)popup.RenderTransform).BeginAnimation(ScaleTransform.ScaleYProperty, animation);
	}

	private void CloseDownloadSourcePopup(Border overlay, Border popup)
	{
		Border overlay2 = overlay;
		Border popup2 = popup;
		CubicEase easingFunction = new CubicEase
		{
			EasingMode = EasingMode.EaseIn
		};
		TimeSpan timeSpan = TimeSpan.FromMilliseconds(150L, 0L);
		DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, timeSpan)
		{
			EasingFunction = easingFunction
		};
		doubleAnimation.Completed += delegate
		{
			if (_versionSelectPanel?.Child is Grid grid)
			{
				grid.Children.Remove(overlay2);
				grid.Children.Remove(popup2);
			}
		};
		overlay2.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
		popup2.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
	}

	private void StartVersionDownload(ModrinthProject proj, ModrinthVersion ver, ResourceType resType, string mirror)
	{
		ModrinthVersion ver2 = ver;
		string mirror2 = mirror;
		if (1 == 0)
		{
		}
		string text = resType switch
		{
			ResourceType.Mod => "Mod", 
			ResourceType.Shader => "Shader", 
			ResourceType.ResourcePack => "ResourcePack", 
			_ => "Mod", 
		};
		if (1 == 0)
		{
		}
		string type = text;
		CloseVersionSelectPage();
		DownloadManager.StartResourceDownload(proj.Title + " " + ver2.VersionNumber, type, () => ModrinthApi.DownloadVersionAsync(ver2, resType, mirror2));
		ShowDownloadCenterPage();
	}

	private void StartDownload_Click(object sender, RoutedEventArgs e)
	{
		if (!string.IsNullOrEmpty(_selectedVersionId))
		{
			string loaderName = _selectedLoaderName ?? "";
			string loaderVersion = _selectedLoaderVersion ?? "";
			string optifineVersion = _selectedOptifineVersion ?? "";
			DownloadManager.StartDownload(_selectedVersionId, loaderName, loaderVersion, optifineVersion);
			ShowDownloadCenterPage();
		}
	}

	private void SelectNavItem(string tag)
	{
		foreach (object child in LeftNav.Children)
		{
			if (child is Border border && border.Tag?.ToString() == tag)
			{
				_selectedItem = border;
				TextBlock textBlock = FindTextBlock(border);
				if (textBlock != null)
				{
					textBlock.Foreground = new SolidColorBrush(_currentThemeColor);
				}
				if (border.RenderTransform is TransformGroup transformGroup && transformGroup.Children[0] is TranslateTransform translateTransform && transformGroup.Children[1] is ScaleTransform scaleTransform)
				{
					translateTransform.BeginAnimation(TranslateTransform.XProperty, null);
					translateTransform.X = 8;
					scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
					scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
					scaleTransform.ScaleX = 1.08;
					scaleTransform.ScaleY = 1.08;
				}
			}
			else if (child is Border border2)
			{
				TextBlock textBlock2 = FindTextBlock(border2);
				if (textBlock2 != null)
				{
					textBlock2.Foreground = TextWhiteBrush;
				}
				if (border2.RenderTransform is TransformGroup transformGroup2 && transformGroup2.Children[0] is TranslateTransform translateTransform2 && transformGroup2.Children[1] is ScaleTransform scaleTransform2)
				{
					translateTransform2.BeginAnimation(TranslateTransform.XProperty, null);
					translateTransform2.X = 0;
					scaleTransform2.BeginAnimation(ScaleTransform.ScaleXProperty, null);
					scaleTransform2.BeginAnimation(ScaleTransform.ScaleYProperty, null);
					scaleTransform2.ScaleX = 1.0;
					scaleTransform2.ScaleY = 1.0;
				}
			}
		}
	}

	private void ShowLaunchPage()
	{
		SelectNavItem("launch");
		SwitchToPage(LaunchPage, LaunchPageSlide, delegate
		{
			ApplyConfigToLaunchPage();
		});
	}

	private void ShowMultiplayerPage()
	{
		SelectNavItem("multiplayer");
		SwitchToPage(MultiplayerPage, MultiplayerPageSlide);
	}

	private void OfflineTab_Click(object sender, MouseButtonEventArgs e)
	{
		if (_isMicrosoftLogin) SwitchToOfflineTab();
	}

	private void MicrosoftTab_Click(object sender, MouseButtonEventArgs e)
	{
		if (!_isMicrosoftLogin) SwitchToMicrosoftTab();
	}

	private void SwitchToOfflineTab()
	{
		_isMicrosoftLogin = false;
		AnimateTabSwitch(OfflineTab, OfflineTabText, MicrosoftTab, MicrosoftTabText);
		AnimatePanelSwitch(MicrosoftPanel, OfflinePanel);
	}

	private void SwitchToMicrosoftTab()
	{
		_isMicrosoftLogin = true;
		AnimateTabSwitch(MicrosoftTab, MicrosoftTabText, OfflineTab, OfflineTabText);
		AnimatePanelSwitch(OfflinePanel, MicrosoftPanel);
		UpdateMicrosoftLoginUI();
	}

	private void AnimateTabSwitch(Border activeTab, TextBlock activeText, Border inactiveTab, TextBlock inactiveText)
	{
		TimeSpan duration = AnimDuration(200);
		CubicEase ease = new CubicEase { EasingMode = EasingMode.EaseOut };
		ColorAnimation activeBg = new ColorAnimation(Color.FromRgb(72, 144, 245), duration) { EasingFunction = ease };
		ColorAnimation inactiveBg = new ColorAnimation(Color.FromRgb(42, 42, 46), duration) { EasingFunction = ease };
		activeTab.Background = new SolidColorBrush(Color.FromRgb(72, 144, 245));
		inactiveTab.Background = new SolidColorBrush(Color.FromRgb(42, 42, 46));
		((SolidColorBrush)activeTab.Background).BeginAnimation(SolidColorBrush.ColorProperty, activeBg);
		((SolidColorBrush)inactiveTab.Background).BeginAnimation(SolidColorBrush.ColorProperty, inactiveBg);
		activeText.Foreground = TextWhiteBrush;
		inactiveText.Foreground = Gray136Brush;
	}

	private void AnimatePanelSwitch(FrameworkElement hidePanel, FrameworkElement showPanel)
	{
		TimeSpan duration = AnimDuration(180);
		CubicEase ease = new CubicEase { EasingMode = EasingMode.EaseOut };
		DoubleAnimation fadeOut = new DoubleAnimation(0, duration) { EasingFunction = ease };
		DoubleAnimation fadeIn = new DoubleAnimation(0, 1, duration) { EasingFunction = ease };
		fadeOut.Completed += delegate
		{
			hidePanel.Visibility = Visibility.Collapsed;
			showPanel.Visibility = Visibility.Visible;
			showPanel.Opacity = 0;
			showPanel.BeginAnimation(UIElement.OpacityProperty, fadeIn);
		};
		hidePanel.BeginAnimation(UIElement.OpacityProperty, fadeOut);
	}

	private void UpdateMicrosoftLoginUI()
	{
		if (MicrosoftAuthService.IsLoggedIn)
		{
			MsLoginButton.Visibility = Visibility.Collapsed;
			MsLoggedInPanel.Visibility = Visibility.Visible;
			MsPlayerNameLabel.Text = MicrosoftAuthService.PlayerName ?? "?";
		}
		else
		{
			MsLoginButton.Visibility = Visibility.Visible;
			MsLoggedInPanel.Visibility = Visibility.Collapsed;
		}
	}

	private async void MsLogin_Click(object sender, RoutedEventArgs e)
	{
		if (_msAuthDialogOpen) return;
		_msAuthDialogOpen = true;
		MsLoginButton.IsEnabled = false;
		MsLoginButton.SetResourceReference(ContentControl.ContentProperty, "StatusLoading");

		Border? overlay = null;
		Border? dialog = null;

		try
		{
			bool dialogShown = false;
			TextBlock? statusTextRef = null;

			var result = await Task.Run(async () =>
			{
				return await MicrosoftAuthService.LoginAsync(deviceCode =>
				{
					Dispatcher.Invoke((Action)(() =>
					{
						ShowDeviceCodeDialog(deviceCode, out overlay, out dialog, out statusTextRef);
						dialogShown = true;
					}));
				}, new Progress<string>(msg =>
				{
					Dispatcher.Invoke((Action)(() =>
					{
						if (statusTextRef != null) statusTextRef.Text = msg;
					}));
				}));
			});

			if (overlay != null && dialog != null)
			{
				Grid parent = (Grid)LaunchPage.Parent;
				parent.Children.Remove(overlay);
				parent.Children.Remove(dialog);
			}
			UpdateMicrosoftLoginUI();
			if (result.Success)
			{
				NotificationManager.Show(LanguageManager.Get("MsAuthSuccess"));
			}
			else
			{
				string errMsg = result.ErrorMessage ?? LanguageManager.Get("MsAuthFailed");
				string detail = MicrosoftAuthService.LastError ?? "";
				if (!string.IsNullOrEmpty(detail) && detail != errMsg)
					errMsg = $"{errMsg}\n{detail}";
				NotificationManager.Show(errMsg);
			}
		}
		catch (Exception ex)
		{
			NotificationManager.Show(ex.Message);
		}
		finally
		{
			MsLoginButton.IsEnabled = true;
			MsLoginButton.SetResourceReference(ContentControl.ContentProperty, "LaunchMsLogin");
			_msAuthDialogOpen = false;
		}
	}

	private void ShowDeviceCodeDialog(MicrosoftAuthService.DeviceCodeInfo deviceCode, out Border overlay, out Border dialog, out TextBlock? statusTextRef)
	{
		Grid parent = (Grid)LaunchPage.Parent;
		overlay = new Border
		{
			Background = new SolidColorBrush(Color.FromArgb(160, 0, 0, 0))
		};
		parent.Children.Add(overlay);

		dialog = new Border
		{
			Width = 340,
			Background = Bg32Brush,
			CornerRadius = new CornerRadius(12),
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center,
			Padding = new Thickness(28, 24, 28, 20)
		};

		StackPanel panel = new StackPanel();

		TextBlock title = new TextBlock
		{
			Text = LanguageManager.Get("MsAuthTitle"),
			Foreground = TextWhiteBrush,
			FontSize = 16,
			FontWeight = FontWeights.SemiBold,
			HorizontalAlignment = HorizontalAlignment.Center,
			Margin = new Thickness(0, 0, 0, 16)
		};
		panel.Children.Add(title);

		TextBlock step1 = new TextBlock
		{
			Text = LanguageManager.Get("MsAuthStep1"),
			Foreground = Gray136Brush,
			FontSize = 12,
			TextWrapping = TextWrapping.Wrap,
			Margin = new Thickness(0, 0, 0, 6)
		};
		panel.Children.Add(step1);

		Border urlBorder = new Border
		{
			Background = Bg42Brush,
			CornerRadius = new CornerRadius(6),
			Padding = new Thickness(10, 8, 10, 8),
			Margin = new Thickness(0, 0, 0, 12),
			Cursor = Cursors.Hand
		};
		TextBlock urlText = new TextBlock
		{
			Text = deviceCode.VerificationUrl,
			Foreground = Blue72Brush,
			FontSize = 12,
			HorizontalAlignment = HorizontalAlignment.Center
		};
		urlBorder.Child = urlText;
		urlBorder.MouseLeftButtonUp += delegate
		{
			try { Process.Start(new ProcessStartInfo(deviceCode.VerificationUrl) { UseShellExecute = true }); } catch { }
		};
		panel.Children.Add(urlBorder);

		TextBlock step2 = new TextBlock
		{
			Text = LanguageManager.Get("MsAuthStep2"),
			Foreground = Gray136Brush,
			FontSize = 12,
			Margin = new Thickness(0, 0, 0, 6)
		};
		panel.Children.Add(step2);

		Border codeBorder = new Border
		{
			Background = Bg42Brush,
			CornerRadius = new CornerRadius(6),
			Padding = new Thickness(10, 8, 10, 8),
			Margin = new Thickness(0, 0, 0, 12),
			Cursor = Cursors.Hand
		};
		TextBlock codeText = new TextBlock
		{
			Text = deviceCode.UserCode,
			Foreground = TextWhiteBrush,
			FontSize = 18,
			FontWeight = FontWeights.Bold,
			FontFamily = new FontFamily("Consolas"),
			HorizontalAlignment = HorizontalAlignment.Center
		};
		codeBorder.Child = codeText;
		codeBorder.MouseLeftButtonUp += delegate
		{
			try { Clipboard.SetText(deviceCode.UserCode); codeText.Text = LanguageManager.Get("MsAuthCopied"); } catch { }
		};
		panel.Children.Add(codeBorder);

		StackPanel btnRow = new StackPanel
		{
			Orientation = Orientation.Horizontal,
			HorizontalAlignment = HorizontalAlignment.Center,
			Margin = new Thickness(0, 0, 0, 12)
		};
		Button openBrowserBtn = new Button
		{
			Content = LanguageManager.Get("MsAuthOpenBrowser"),
			Style = (Style)FindResource("CardButton"),
			Height = 32,
			FontSize = 12,
			Margin = new Thickness(0, 0, 8, 0),
			Padding = new Thickness(14, 4, 14, 4)
		};
		openBrowserBtn.Click += delegate
		{
			try { Process.Start(new ProcessStartInfo(deviceCode.VerificationUrl) { UseShellExecute = true }); } catch { }
		};
		btnRow.Children.Add(openBrowserBtn);

		Button copyBtn = new Button
		{
			Content = LanguageManager.Get("MsAuthCopyCode"),
			Style = (Style)FindResource("CardButton"),
			Height = 32,
			FontSize = 12,
			Padding = new Thickness(14, 4, 14, 4)
		};
		copyBtn.Click += delegate
		{
			try { Clipboard.SetText(deviceCode.UserCode); } catch { }
		};
		btnRow.Children.Add(copyBtn);
		panel.Children.Add(btnRow);

		TextBlock statusText = new TextBlock
		{
			Text = LanguageManager.Get("MsAuthWaiting"),
			Foreground = Gray136Brush,
			FontSize = 12,
			HorizontalAlignment = HorizontalAlignment.Center,
			Tag = "status"
		};
		statusTextRef = statusText;
		panel.Children.Add(statusText);

		dialog.Child = panel;
		parent.Children.Add(dialog);
	}

	private void VersionSelector_Click(object sender, MouseButtonEventArgs e)
	{
		ShowVersionSelectPopup();
	}

	private void ShowVersionSelectPopup()
	{
		if (_versionSelectOverlay != null)
		{
			CloseVersionSelectPopup();
			return;
		}
		Grid parent = (Grid)LaunchPage.Parent;
		Border overlay = new Border
		{
			Background = new SolidColorBrush(Color.FromArgb(120, 0, 0, 0))
		};
		overlay.MouseLeftButtonUp += delegate { CloseVersionSelectPopup(); };
		parent.Children.Add(overlay);
		_versionSelectOverlay = overlay;

		Border panel = new Border
		{
			Width = 300,
			MaxHeight = 400,
			Background = Bg32Brush,
			CornerRadius = new CornerRadius(8),
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center,
			Padding = new Thickness(8)
		};
		parent.Children.Add(panel);
		_versionSelectPanel = panel;

		StackPanel list = new StackPanel();
		_ = LoadVersionsIntoList(list);
		panel.Child = new ScrollViewer
		{
			Content = list,
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
			MaxHeight = 380
		};
	}

	private void CloseVersionSelectPopup()
	{
		Grid parent = (Grid)LaunchPage.Parent;
		if (_versionSelectOverlay != null)
		{
			parent.Children.Remove(_versionSelectOverlay);
			_versionSelectOverlay = null;
		}
		if (_versionSelectPanel != null)
		{
			parent.Children.Remove(_versionSelectPanel);
			_versionSelectPanel = null;
		}
	}

	private async Task LoadVersionsIntoList(StackPanel list)
	{
		TextBlock loading = new TextBlock
		{
			Foreground = Gray136Brush,
			FontSize = 13,
			TextAlignment = TextAlignment.Center,
			Margin = new Thickness(0, 20, 0, 0)
		};
		loading.SetResourceReference(TextBlock.TextProperty, "StatusLoading");
		list.Children.Add(loading);

		List<InstalledVersion> versions = await Task.Run(() => LaunchManager.GetInstalledVersions());
		list.Children.Clear();

		if (versions.Count == 0)
		{
			TextBlock empty = new TextBlock
			{
				Foreground = Gray136Brush,
				FontSize = 13,
				TextAlignment = TextAlignment.Center,
				Margin = new Thickness(0, 20, 0, 0)
			};
			empty.SetResourceReference(TextBlock.TextProperty, "StatusNoInstalledVersion");
			list.Children.Add(empty);
			return;
		}

		foreach (InstalledVersion ver in versions)
		{
			Border item = new Border
			{
				Height = 48,
				Margin = new Thickness(0, 2, 0, 2),
				Padding = new Thickness(12, 0, 12, 0),
				Background = Bg34Brush,
				CornerRadius = new CornerRadius(6),
				Cursor = Cursors.Hand,
				Tag = ver.Id
			};
			StackPanel infoStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
			StackPanel nameRow = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
			string typeIcon = ver.Type switch
			{
				"release" => "assests/release.png",
				"snapshot" => "assests/sna.png",
				"old_beta" or "old_alpha" => "assests/old.png",
				_ => "assests/old.png"
			};
			nameRow.Children.Add(MakeIcon(typeIcon, ver.Type));
			TextBlock nameText = new TextBlock
			{
				Text = VersionSettingsManager.GetDisplayName(ver.Id),
				Foreground = TextWhiteBrush,
				FontSize = 13,
				FontWeight = FontWeights.Medium,
				TextTrimming = TextTrimming.CharacterEllipsis,
				VerticalAlignment = VerticalAlignment.Center
			};
			nameRow.Children.Add(nameText);
			infoStack.Children.Add(nameRow);

			StackPanel iconRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 3, 0, 0) };
			if (ver.IsForge) iconRow.Children.Add(MakeIcon("assests/forge.png", "Forge"));
			if (ver.IsFabric) iconRow.Children.Add(MakeIcon("assests/fabric.png", "Fabric"));
			if (ver.IsQuilt) iconRow.Children.Add(MakeIcon("assests/quilt_x16.png", "Quilt"));
			if (ver.IsOptifine) iconRow.Children.Add(MakeIcon("assests/optifine.png", "OptiFine"));
			if (iconRow.Children.Count > 0) infoStack.Children.Add(iconRow);

			bool isModded = ver.IsForge || ver.IsFabric || ver.IsQuilt;
			MenuItem verSettingsItem = new MenuItem
			{
				Header = LanguageManager.Get("VerSettingsTitle"),
				Foreground = TextWhiteBrush,
				Tag = ver.Id
			};
			verSettingsItem.SetResourceReference(FrameworkElement.StyleProperty, "DarkMenuItem");
			verSettingsItem.Click += delegate(object s, RoutedEventArgs _)
			{
				if (s is FrameworkElement { Tag: string vid })
				{
					CloseVersionSelectPopup();
					ShowVersionSettings(vid, LaunchPage, LaunchPageSlide, isModded);
				}
			};
			ContextMenu ctxMenu = new ContextMenu();
			ctxMenu.SetResourceReference(FrameworkElement.StyleProperty, "DarkContextMenu");
			ctxMenu.Items.Add(verSettingsItem);
			item.ContextMenu = ctxMenu;
			item.Child = infoStack;
			item.MouseEnter += delegate(object s, MouseEventArgs _) { if (s is Border b) b.Background = Bg44Brush; };
			item.MouseLeave += delegate(object s, MouseEventArgs _) { if (s is Border b) b.Background = Bg34Brush; };
			item.MouseLeftButtonUp += delegate(object s, MouseButtonEventArgs _)
			{
				if (s is Border { Tag: string tag })
				{
					_selectedLaunchVersionId = tag;
					SelectedLaunchVersion.Text = VersionSettingsManager.GetDisplayName(tag);
					SelectedLaunchVersion.Foreground = TextWhiteBrush;
					LaunchButton.IsEnabled = true;
					CloseVersionSelectPopup();
				}
			};
			list.Children.Add(item);
		}
	}

	private async void LaunchGame_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(_selectedLaunchVersionId))
		{
			NotificationManager.Show(LanguageManager.Get("MsgSelectVersion"));
			return;
		}
		string username;
		MSession? msSession = null;
		if (_isMicrosoftLogin)
		{
			if (!MicrosoftAuthService.IsLoggedIn)
			{
				NotificationManager.Show(LanguageManager.Get("LaunchMsLogin"));
				return;
			}
			username = MicrosoftAuthService.PlayerName ?? "Player";
			msSession = new MSession
			{
				Username = MicrosoftAuthService.PlayerName ?? "Player",
				UUID = MicrosoftAuthService.PlayerUuid ?? "",
				AccessToken = MicrosoftAuthService.AccessToken ?? "0",
				UserType = "msa"
			};
		}
		else
		{
			username = PlayerNameBox.Text.Trim();
			if (string.IsNullOrEmpty(username))
			{
				NotificationManager.Show(LanguageManager.Get("MsgEnterPlayerName"));
				return;
			}
		}
		int globalRam = LauncherConfig.Current.MaxRamMb;
		if (globalRam < 512) globalRam = 2048;
		VersionSettings verSettings = VersionSettingsManager.Get(_selectedLaunchVersionId);
		int ram = (verSettings.UseCustomMemory ? verSettings.CustomMemoryMb : globalRam);
		if (ram < 512) ram = 2048;
		LauncherConfig.Current.PlayerName = username;
		LauncherConfig.Current.MaxRamMb = globalRam;
		LauncherConfig.Save();
		LaunchButton.IsEnabled = false;
		LaunchButton.SetResourceReference(ContentControl.ContentProperty, "LaunchLaunching");
		LaunchStatusText.Visibility = Visibility.Collapsed;
		await LaunchManager.LaunchAsync(_selectedLaunchVersionId, username, ram, false, msSession);
	}

	private void ApplyConfigToLaunchPage()
	{
		if (PlayerNameBox != null)
		{
			PlayerNameBox.Text = LauncherConfig.Current.PlayerName;
		}
		if (MicrosoftAuthService.IsLoggedIn)
		{
			UpdateMicrosoftLoginUI();
		}
	}

	private void ShowMorePage()
	{
		SelectNavItem("more");
		SwitchToPage(MorePage, MorePageSlide, delegate
		{
			try
			{
				if (MoreVersionText != null)
				{
					MoreVersionText.Text = UpdateChecker.CurrentVersion;
				}
				if (MoreRuntimeText != null)
				{
					string text = Environment.Version.ToString();
					MoreRuntimeText.Text = ".NET " + text;
				}
			}
			catch
			{
			}
		});
	}

	private void MoreOpenGameDir_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			string minecraftPath = DownloadManager.MinecraftPath;
			if (!Directory.Exists(minecraftPath))
			{
				Directory.CreateDirectory(minecraftPath);
			}
			Process.Start(new ProcessStartInfo("explorer.exe", minecraftPath)
			{
				UseShellExecute = true
			});
		}
		catch (Exception ex)
		{
			NotificationManager.Show(string.Format(LanguageManager.Get("MsgOpenDirFailed"), ex.Message));
		}
	}

	private void MoreOpenJavaDir_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			string text = System.IO.Path.Combine(DownloadManager.MinecraftPath, "runtime");
			if (Directory.Exists(text))
			{
				Process.Start(new ProcessStartInfo("explorer.exe", text)
				{
					UseShellExecute = true
				});
			}
			else
			{
				NotificationManager.Show(LanguageManager.Get("MsgNoRuntimeDir"));
			}
		}
		catch (Exception ex)
		{
			NotificationManager.Show(string.Format(LanguageManager.Get("MsgOpenDirFailed"), ex.Message));
		}
	}

	private void MoreClearCache_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			string path = System.IO.Path.Combine(DownloadManager.GetExeDirectory(), "cache");
			if (Directory.Exists(path))
			{
				Directory.Delete(path, recursive: true);
				Directory.CreateDirectory(path);
			}
			NotificationManager.Show(LanguageManager.Get("MsgCacheCleared"));
		}
		catch (Exception ex)
		{
			NotificationManager.Show(string.Format(LanguageManager.Get("MsgClearCacheFailed"), ex.Message));
		}
	}

	private async void MoreCheckUpdate_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			if (MoreCheckUpdateBtn != null)
			{
				MoreCheckUpdateBtn.SetResourceReference(ContentControl.ContentProperty, "MoreChecking");
				MoreCheckUpdateBtn.IsEnabled = false;
			}
			bool found = false;
			UpdateInfo foundInfo = null;
			string errorMsg = null;
			UpdateChecker.UpdateAvailable += OnFound;
			UpdateChecker.CheckFailed += OnFail;
			await UpdateChecker.CheckAsync(silent: false);
			UpdateChecker.UpdateAvailable -= OnFound;
			UpdateChecker.CheckFailed -= OnFail;
			if (found && foundInfo != null)
			{
				NotificationManager.Show(string.Format(arg0: foundInfo.TagName.TrimStart('v', 'V'), format: LanguageManager.Get("MsgUpdateFound")));
				if (!string.IsNullOrEmpty(foundInfo.HtmlUrl))
				{
					Process.Start(new ProcessStartInfo(foundInfo.HtmlUrl)
					{
						UseShellExecute = true
					});
				}
			}
			else
			{
				NotificationManager.Show(string.Format(LanguageManager.Get("MsgAlreadyLatest"), UpdateChecker.CurrentVersion));
			}
			void OnFail(string msg)
			{
				errorMsg = msg;
			}
			void OnFound(UpdateInfo info)
			{
				found = true;
				foundInfo = info;
			}
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			NotificationManager.Show(string.Format(LanguageManager.Get("MsgCheckUpdateFailed"), ex.Message));
		}
		finally
		{
			if (MoreCheckUpdateBtn != null)
			{
				MoreCheckUpdateBtn.SetResourceReference(ContentControl.ContentProperty, "MoreCheckUpdate");
				MoreCheckUpdateBtn.IsEnabled = true;
			}
		}
	}

	public void PlayEnterAnimation()
	{
		base.Visibility = Visibility.Visible;
		base.Opacity = 1.0;
		_panelSlide.BeginAnimation(TranslateTransform.XProperty, null);
		_panelSlide.X = 200.0;
		CubicEase easingFunction = new CubicEase
		{
			EasingMode = EasingMode.EaseOut
		};
		DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(350L, 0L))
		{
			EasingFunction = easingFunction,
			BeginTime = TimeSpan.FromMilliseconds(20L, 0L)
		};
		doubleAnimation.Completed += delegate
		{
			Focus();
			if (!IsAnyPageVisible())
			{
				ShowLaunchPage();
			}
			base.Dispatcher.BeginInvoke((Action)delegate
			{
				Focus();
				if (_currentResourceType != "\u6E38\u620F" && ResourceSearchBox != null)
				{
					ResourceSearchBox.Focus();
					base.Dispatcher.BeginInvoke((Action)delegate
					{
						ResourceSearchBox.Focus();
						Keyboard.Focus(ResourceSearchBox);
					}, DispatcherPriority.Input);
				}
			}, DispatcherPriority.ContextIdle);
		};
		_panelSlide.BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
		AnimateNavItems(isEnter: true);
	}

	public void PlayExitAnimation()
	{
		_panelSlide.BeginAnimation(TranslateTransform.XProperty, null);
		CubicEase easingFunction = new CubicEase
		{
			EasingMode = EasingMode.EaseIn
		};
		DoubleAnimation doubleAnimation = new DoubleAnimation(-200.0, TimeSpan.FromMilliseconds(220L, 0L))
		{
			EasingFunction = easingFunction
		};
		doubleAnimation.Completed += delegate
		{
			base.Visibility = Visibility.Collapsed;
			base.Opacity = 0.0;
		};
		_panelSlide.BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
	}

	private void CollapseNav_Click(object sender, MouseButtonEventArgs e)
	{
		this.CollapseRequested?.Invoke(this, EventArgs.Empty);
	}

	private void ExitNav_Click(object sender, MouseButtonEventArgs e)
	{
		this.ExitRequested?.Invoke(this, EventArgs.Empty);
	}

	private void CloseBtn_Click(object sender, MouseButtonEventArgs e)
	{
		this.CollapseRequested?.Invoke(this, EventArgs.Empty);
	}

	private void CloseBtn_MouseEnter(object sender, MouseEventArgs e)
	{
		if (sender is Border border)
		{
			ColorAnimation animation = new ColorAnimation(Color.FromRgb(70, 70, 74), TimeSpan.FromMilliseconds(150L, 0L));
			border.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
		}
	}

	private void CloseBtn_MouseLeave(object sender, MouseEventArgs e)
	{
		if (sender is Border border)
		{
			ColorAnimation animation = new ColorAnimation(Color.FromRgb(45, 45, 48), TimeSpan.FromMilliseconds(150L, 0L));
			border.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
		}
	}

	private void AnimateNavItems(bool isEnter)
	{
		if (!base.IsLoaded)
		{
			return;
		}
		int num = 60;
		CubicEase easingFunction = new CubicEase
		{
			EasingMode = EasingMode.EaseOut
		};
		List<FrameworkElement> list = new List<FrameworkElement> { CollapseNav };
		foreach (object child in LeftNav.Children)
		{
			list.Add((FrameworkElement)child);
		}
		list.Add(ExitNav);
		foreach (FrameworkElement item in list)
		{
			Border border = item as Border;
			if (border == null || !(border.RenderTransform is TransformGroup transformGroup))
			{
				continue;
			}
			Transform transform2 = transformGroup.Children[0];
			TranslateTransform transform = transform2 as TranslateTransform;
			if (transform == null)
			{
				continue;
			}
			if (isEnter)
			{
				border.BeginAnimation(UIElement.OpacityProperty, null);
				transform.BeginAnimation(TranslateTransform.XProperty, null);
				transform.X = -60.0;
				border.Opacity = 0.0;
				DoubleAnimation doubleAnimation = new DoubleAnimation(-60.0, 0.0, TimeSpan.FromMilliseconds(280L, 0L))
				{
					EasingFunction = easingFunction,
					BeginTime = TimeSpan.FromMilliseconds(num, 0L)
				};
				doubleAnimation.Completed += delegate
				{
					transform.BeginAnimation(TranslateTransform.XProperty, null);
					transform.X = 0.0;
				};
				transform.BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
				DoubleAnimation doubleAnimation2 = new DoubleAnimation(0.0, 1.0, TimeSpan.FromMilliseconds(220L, 0L))
				{
					BeginTime = TimeSpan.FromMilliseconds(num, 0L)
				};
				doubleAnimation2.Completed += delegate
				{
					border.BeginAnimation(UIElement.OpacityProperty, null);
					border.Opacity = 1.0;
				};
				border.BeginAnimation(UIElement.OpacityProperty, doubleAnimation2);
			}
			else
			{
				border.BeginAnimation(UIElement.OpacityProperty, null);
				transform.BeginAnimation(TranslateTransform.XProperty, null);
				DoubleAnimation doubleAnimation3 = new DoubleAnimation(0.0, -60.0, TimeSpan.FromMilliseconds(180L, 0L))
				{
					EasingFunction = easingFunction
				};
				doubleAnimation3.Completed += delegate
				{
					transform.BeginAnimation(TranslateTransform.XProperty, null);
					transform.X = 0.0;
				};
				transform.BeginAnimation(TranslateTransform.XProperty, doubleAnimation3);
				DoubleAnimation doubleAnimation4 = new DoubleAnimation(1.0, 0.0, TimeSpan.FromMilliseconds(150L, 0L));
				doubleAnimation4.Completed += delegate
				{
					border.BeginAnimation(UIElement.OpacityProperty, null);
					border.Opacity = 0.0;
				};
				border.BeginAnimation(UIElement.OpacityProperty, doubleAnimation4);
			}
			num += 50;
		}
	}

	private void NavItem_MouseEnter(object sender, MouseEventArgs e)
	{
		if (sender is Border { RenderTransform: TransformGroup renderTransform } && renderTransform.Children[0] is TranslateTransform translateTransform)
		{
			DoubleAnimation animation = new DoubleAnimation(8.0, TimeSpan.FromMilliseconds(250L, 0L))
			{
				EasingFunction = new CubicEase
				{
					EasingMode = EasingMode.EaseOut
				}
			};
			translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);
		}
	}

	private void NavItem_MouseLeave(object sender, MouseEventArgs e)
	{
		if (sender is Border { RenderTransform: TransformGroup renderTransform } border && renderTransform.Children[0] is TranslateTransform translateTransform)
		{
			int num = ((border == _selectedItem) ? 8 : 0);
			DoubleAnimation animation = new DoubleAnimation(num, TimeSpan.FromMilliseconds(200L, 0L))
			{
				EasingFunction = new CubicEase
				{
					EasingMode = EasingMode.EaseIn
				}
			};
			translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);
		}
	}

	private void Nav_Click(object sender, MouseButtonEventArgs e)
	{
		if (!(sender is Border { Tag: not null, Tag: var tag } border))
		{
			return;
		}
		string text = tag?.ToString() ?? "";
		foreach (object child in LeftNav.Children)
		{
			if (child is Border border2)
			{
				TextBlock textBlock = FindTextBlock(border2);
				if (textBlock != null)
				{
					textBlock.Foreground = ((border2 == border) ? new SolidColorBrush(_currentThemeColor) : new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue)));
				}
				if (border2.RenderTransform is TransformGroup transformGroup && transformGroup.Children[0] is TranslateTransform translateTransform && transformGroup.Children[1] is ScaleTransform scaleTransform)
				{
					bool flag = border2 == border;
					int num = (flag ? 8 : 0);
					double toValue = (flag ? 1.08 : 1.0);
					CubicEase easingFunction = new CubicEase
					{
						EasingMode = EasingMode.EaseOut
					};
					TimeSpan timeSpan = AnimDuration(200.0);
					DoubleAnimation animation = new DoubleAnimation(num, timeSpan)
					{
						EasingFunction = easingFunction
					};
					translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);
					DoubleAnimation animation2 = new DoubleAnimation(toValue, timeSpan)
					{
						EasingFunction = easingFunction
					};
					scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation2);
					scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation2);
				}
			}
		}
		_selectedItem = border;
		switch (text)
		{
		case "launch":
			ShowLaunchPage();
			break;
		case "download":
			ShowDownloadPage();
			break;
		case "settings":
			ShowSettingsPage();
			break;
		case "more":
			ShowMorePage();
			break;
		case "downloadcenter":
			ShowDownloadCenterPage();
			break;
		case "multiplayer":
			ShowMultiplayerPage();
			break;
		}
	}

	private TextBlock? FindTextBlock(DependencyObject parent)
	{
		for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(parent, i);
			if (child is TextBlock result)
			{
				return result;
			}
			TextBlock textBlock = FindTextBlock(child);
			if (textBlock != null)
			{
				return textBlock;
			}
		}
		return null;
	}

	private bool IsAnyPageVisible()
	{
		return LaunchPage.Visibility == Visibility.Visible || DownloadPage.Visibility == Visibility.Visible || DownloadCenterPage.Visibility == Visibility.Visible || MultiplayerPage.Visibility == Visibility.Visible || VersionSettingsPage.Visibility == Visibility.Visible || SettingsPage.Visibility == Visibility.Visible || MorePage.Visibility == Visibility.Visible;
	}

	private Grid? GetVisiblePage()
	{
		if (LaunchPage.Visibility == Visibility.Visible)
		{
			return LaunchPage;
		}
		if (DownloadPage.Visibility == Visibility.Visible)
		{
			return DownloadPage;
		}
		if (DownloadCenterPage.Visibility == Visibility.Visible)
		{
			return DownloadCenterPage;
		}
		if (MultiplayerPage.Visibility == Visibility.Visible)
		{
			return MultiplayerPage;
		}
		if (VersionSettingsPage.Visibility == Visibility.Visible)
		{
			return VersionSettingsPage;
		}
		if (SettingsPage.Visibility == Visibility.Visible)
		{
			return SettingsPage;
		}
		if (MorePage.Visibility == Visibility.Visible)
		{
			return MorePage;
		}
		return null;
	}

	private void AnimatePageOut(Grid page, Action? onCompleted = null)
	{
		Grid page2 = page;
		Action onCompleted2 = onCompleted;
		if (!(page2.RenderTransform is TranslateTransform translateTransform))
		{
			page2.Visibility = Visibility.Collapsed;
			onCompleted2?.Invoke();
			return;
		}
		translateTransform.BeginAnimation(TranslateTransform.XProperty, null);
		page2.BeginAnimation(UIElement.OpacityProperty, null);
		CubicEase easingFunction = new CubicEase
		{
			EasingMode = EasingMode.EaseIn
		};
		TimeSpan timeSpan = AnimDuration(260.0);
		DoubleAnimation doubleAnimation = new DoubleAnimation(-60.0, timeSpan)
		{
			EasingFunction = easingFunction
		};
		DoubleAnimation animation = new DoubleAnimation(0.0, timeSpan)
		{
			EasingFunction = easingFunction
		};
		doubleAnimation.Completed += delegate
		{
			page2.Visibility = Visibility.Collapsed;
			onCompleted2?.Invoke();
		};
		translateTransform.BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
		page2.BeginAnimation(UIElement.OpacityProperty, animation);
	}

	private void AnimatePageIn(Grid page, TranslateTransform slide, Action? afterShow = null)
	{
		ContentArea.Visibility = Visibility.Visible;
		slide.BeginAnimation(TranslateTransform.XProperty, null);
		page.BeginAnimation(UIElement.OpacityProperty, null);
		slide.X = 60.0;
		page.Opacity = 0.0;
		page.Visibility = Visibility.Visible;
		CubicEase easingFunction = new CubicEase
		{
			EasingMode = EasingMode.EaseOut
		};
		TimeSpan timeSpan = AnimDuration(320.0);
		DoubleAnimation animation = new DoubleAnimation(0.0, timeSpan)
		{
			EasingFunction = easingFunction
		};
		DoubleAnimation animation2 = new DoubleAnimation(1.0, timeSpan)
		{
			EasingFunction = easingFunction
		};
		slide.BeginAnimation(TranslateTransform.XProperty, animation);
		page.BeginAnimation(UIElement.OpacityProperty, animation2);
		afterShow?.Invoke();
	}

	private bool _isPageSwitching = false;

	private void SwitchToPage(Grid newPage, TranslateTransform newSlide, Action? afterShow = null)
	{
		if (_isPageSwitching)
		{
			return;
		}
		Grid visiblePage = GetVisiblePage();
		if (visiblePage == null || visiblePage == newPage)
		{
			AnimatePageIn(newPage, newSlide, afterShow);
			return;
		}
		_isPageSwitching = true;
		if (visiblePage.RenderTransform is TranslateTransform oldSlide)
		{
			oldSlide.BeginAnimation(TranslateTransform.XProperty, null);
		}
		visiblePage.BeginAnimation(UIElement.OpacityProperty, null);
		visiblePage.Visibility = Visibility.Collapsed;
		AnimatePageIn(newPage, newSlide, delegate
		{
			_isPageSwitching = false;
			afterShow?.Invoke();
		});
	}

	private void HideContentArea()
	{
		ContentArea.Visibility = Visibility.Collapsed;
		DownloadPage.Visibility = Visibility.Collapsed;
		DownloadCenterPage.Visibility = Visibility.Collapsed;
		LaunchPage.Visibility = Visibility.Collapsed;
		VersionSettingsPage.Visibility = Visibility.Collapsed;
		SettingsPage.Visibility = Visibility.Collapsed;
		MorePage.Visibility = Visibility.Collapsed;
	}

	private void ApplyPersonalization()
	{
		try
		{
			Color themeColor = LauncherConfig.GetThemeColor();
			Color color = LauncherConfig.ApplyHslAdjust(themeColor);
			SolidColorBrush solidColorBrush = new SolidColorBrush(color);
			solidColorBrush.Freeze();
			_currentThemeColor = color;
			if (base.Resources != null)
			{
				base.Resources["ColorBrush3"] = solidColorBrush;
			}
			if (_selectedSettingsTab != null)
			{
				_selectedSettingsTab.Background = solidColorBrush;
			}
			foreach (object child in SettingsTabPanel.Children)
			{
				if (child is Border border && border == _selectedSettingsTab)
				{
					border.Background = solidColorBrush;
				}
			}
			if (_selectedItem != null)
			{
				TextBlock textBlock = FindTextBlock(_selectedItem);
				if (textBlock != null)
				{
					textBlock.Foreground = solidColorBrush;
				}
			}
			double num = (double)LauncherConfig.Current.Opacity / 100.0;
			if (ContentArea != null)
			{
				ContentArea.Opacity = 0.3 + num * 0.7;
			}
		}
		catch
		{
		}
	}

	private void ShowVersionSettings(string versionId, Grid returnPage, TranslateTransform returnSlide, bool isModded = false)
	{
		_verSettingsVersionId = versionId;
		_returnPageAfterVerSettings = returnPage.Name;
		string displayName = VersionSettingsManager.GetDisplayName(versionId);
		VerSettingsTitleText.Text = displayName;
		VerSettingsNameBox.Text = displayName;
		VersionSettings versionSettings = VersionSettingsManager.Get(versionId);
		if (versionSettings.UseCustomMemory)
		{
			VerSettingsMemCustom.IsChecked = true;
			VerSettingsMemBox.IsEnabled = true;
			VerSettingsMemBox.Text = versionSettings.CustomMemoryMb.ToString();
		}
		else
		{
			VerSettingsMemGlobal.IsChecked = true;
			VerSettingsMemBox.IsEnabled = false;
			VerSettingsMemBox.Text = LauncherConfig.Current.MaxRamMb.ToString();
		}
		VerSettingsJvmBox.Text = versionSettings.JvmArgs ?? "";
		VerSettingsGameArgsBox.Text = versionSettings.GameArgs ?? "";
		if (isModded)
		{
			VerSettingsModsArea.Visibility = Visibility.Visible;
			RefreshModsList();
		}
		else
		{
			VerSettingsModsArea.Visibility = Visibility.Collapsed;
		}
		SwitchToPage(VersionSettingsPage, VersionSettingsPageSlide);
	}

	private void RefreshModsList()
	{
		VerSettingsModsListPanel.Children.Clear();
		if (string.IsNullOrEmpty(_verSettingsVersionId))
		{
			return;
		}
		LaunchManager.EnsureVersionIsolationDirs(_verSettingsVersionId);
		string path = System.IO.Path.Combine(LaunchManager.GetVersionGameDir(_verSettingsVersionId), "mods");
		List<FileInfo> list = new List<FileInfo>();
		if (Directory.Exists(path))
		{
			list = (from f in Directory.GetFiles(path, "*.jar")
				select new FileInfo(f) into f
				orderby f.Name
				select f).ToList();
		}
		if (list.Count == 0)
		{
			TextBlock textBlock = new TextBlock();
			textBlock.SetResourceReference(TextBlock.TextProperty, "VerSettingsModsEmpty");
			textBlock.Foreground = Gray110Brush;
			textBlock.FontSize = 12.0;
			textBlock.HorizontalAlignment = HorizontalAlignment.Center;
			VerSettingsModsListPanel.Children.Add(textBlock);
			return;
		}
		TextBlock textBlock2 = new TextBlock
		{
			Foreground = Gray160Brush,
			FontSize = 12.0,
			HorizontalAlignment = HorizontalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0)
		};
		textBlock2.SetResourceReference(TextBlock.TextProperty, "VerSettingsModsCount");
		textBlock2.Text = $"{list.Count} {textBlock2.Text}";
		VerSettingsModsListPanel.Children.Add(textBlock2);
		foreach (FileInfo item in list)
		{
			Border border = new Border
			{
				Height = 30.0,
				Margin = new Thickness(0.0, 2.0, 0.0, 0.0),
				Padding = new Thickness(10.0, 0.0, 10.0, 0.0),
				Background = Bg38Brush,
				CornerRadius = new CornerRadius(4.0),
				Cursor = Cursors.Hand,
				Tag = item.FullName
			};
			Grid grid = new Grid();
			grid.ColumnDefinitions.Add(new ColumnDefinition
			{
				Width = new GridLength(1.0, GridUnitType.Star)
			});
			grid.ColumnDefinitions.Add(new ColumnDefinition
			{
				Width = GridLength.Auto
			});
			TextBlock element = new TextBlock
			{
				Text = item.Name,
				Foreground = Brushes.White,
				FontSize = 12.0,
				VerticalAlignment = VerticalAlignment.Center,
				TextTrimming = TextTrimming.CharacterEllipsis
			};
			Grid.SetColumn(element, 0);
			grid.Children.Add(element);
			TextBlock textBlock3 = new TextBlock
		{
			Text = "\u2715",
			Foreground = Red200Brush,
			FontSize = 12.0,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(8.0, 0.0, 0.0, 0.0),
			Cursor = Cursors.Hand,
			Tag = item.FullName
			};
			textBlock3.MouseLeftButtonUp += ModDelete_Click;
			Grid.SetColumn(textBlock3, 1);
			grid.Children.Add(textBlock3);
			border.Child = grid;
			VerSettingsModsListPanel.Children.Add(border);
		}
	}

	private void ModDelete_Click(object sender, MouseButtonEventArgs e)
	{
		if (!(sender is TextBlock { Tag: string tag }))
		{
			return;
		}
		try
		{
			if (File.Exists(tag))
			{
				File.Delete(tag);
			}
			RefreshModsList();
			NotificationManager.Show(LanguageManager.Get("VerSettingsModRemoved"));
		}
		catch (Exception)
		{
		}
	}

	private void VerSettingsBack_Click(object sender, RoutedEventArgs e)
	{
		ReturnToLaunchPage();
	}

	private void ReturnToLaunchPage()
	{
		if (_returnPageAfterVerSettings == "LaunchPage")
		{
			SwitchToPage(LaunchPage, LaunchPageSlide, delegate
			{
				ApplyConfigToLaunchPage();
			});
		}
		else
		{
			SwitchToPage(LaunchPage, LaunchPageSlide);
		}
	}

	private void VerSettingsMem_Checked(object sender, RoutedEventArgs e)
	{
		bool valueOrDefault = VerSettingsMemCustom.IsChecked.GetValueOrDefault();
		VerSettingsMemBox.IsEnabled = valueOrDefault;
		if (!valueOrDefault)
		{
			VerSettingsMemBox.Text = LauncherConfig.Current.MaxRamMb.ToString();
		}
	}

	private void VerSettingsOpenFolder_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(_verSettingsVersionId))
		{
			return;
		}
		try
		{
			LaunchManager.EnsureVersionIsolationDirs(_verSettingsVersionId);
			string versionGameDir = LaunchManager.GetVersionGameDir(_verSettingsVersionId);
			if (!Directory.Exists(versionGameDir))
			{
				Directory.CreateDirectory(versionGameDir);
			}
			Process.Start(new ProcessStartInfo
			{
				FileName = versionGameDir,
				UseShellExecute = true,
				Verb = "open"
			});
		}
		catch (Exception)
		{
			NotificationManager.Show(LanguageManager.Get("VerSettingsOpenFolderFail"));
		}
	}

	private void VerSettingsMods_DragEnter(object sender, DragEventArgs e)
	{
		if (HasJarFiles(e.Data))
		{
			VerSettingsModsDropArea.Background = new SolidColorBrush(Color.FromRgb(45, 60, 90));
			VerSettingsModsDropArea.BorderBrush = Blue72Brush;
			VerSettingsModsDropText.Visibility = Visibility.Visible;
			VerSettingsModsListPanel.Visibility = Visibility.Collapsed;
		}
		e.Effects = (HasJarFiles(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
		e.Handled = true;
	}

	private void VerSettingsMods_DragLeave(object sender, DragEventArgs e)
	{
		VerSettingsModsDropArea.Background = Bg45Brush;
		VerSettingsModsDropArea.BorderBrush = Border61Brush;
		VerSettingsModsDropText.Visibility = Visibility.Collapsed;
		VerSettingsModsListPanel.Visibility = Visibility.Visible;
		e.Handled = true;
	}

	private void VerSettingsMods_Drop(object sender, DragEventArgs e)
	{
		VerSettingsModsDropArea.Background = Bg45Brush;
		VerSettingsModsDropArea.BorderBrush = Border61Brush;
		VerSettingsModsDropText.Visibility = Visibility.Collapsed;
		VerSettingsModsListPanel.Visibility = Visibility.Visible;
		if (string.IsNullOrEmpty(_verSettingsVersionId) || !HasJarFiles(e.Data))
		{
			return;
		}
		string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
		if (array == null)
		{
			return;
		}
		LaunchManager.EnsureVersionIsolationDirs(_verSettingsVersionId);
		string text = System.IO.Path.Combine(LaunchManager.GetVersionGameDir(_verSettingsVersionId), "mods");
		Directory.CreateDirectory(text);
		int num = 0;
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			if (text2.EndsWith(".jar", StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					string destFileName = System.IO.Path.Combine(text, System.IO.Path.GetFileName(text2));
					File.Copy(text2, destFileName, overwrite: true);
					num++;
				}
				catch (Exception)
				{
				}
			}
		}
		if (num > 0)
		{
			RefreshModsList();
			NotificationManager.Show(string.Format(LanguageManager.Get("VerSettingsModInstalled"), System.IO.Path.GetFileName(array[0])));
		}
		e.Handled = true;
	}

	private static bool HasJarFiles(IDataObject data)
	{
		if (!data.GetDataPresent(DataFormats.FileDrop))
		{
			return false;
		}
		return ((string[])data.GetData(DataFormats.FileDrop))?.Any((string f) => f.EndsWith(".jar", StringComparison.OrdinalIgnoreCase)) ?? false;
	}

	private void VerSettingsDelete_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(_verSettingsVersionId))
		{
			return;
		}
		string versionDir = System.IO.Path.Combine(DownloadManager.MinecraftPath, "versions", _verSettingsVersionId);
		if (!System.IO.Directory.Exists(versionDir))
		{
			return;
		}
		try
		{
			System.IO.Directory.Delete(versionDir, recursive: true);
			VersionSettingsManager.Remove(_verSettingsVersionId);
			DataCache.Clear();
			NotificationManager.Show(LanguageManager.Get("VerSettingsDeleted"));
			ReturnToLaunchPage();
			RefreshVersionList();
		}
		catch (Exception ex)
		{
			NotificationManager.Show(string.Format(LanguageManager.Get("MsgOpenConfigFailed"), ex.Message));
		}
	}

	private void VerSettingsSave_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(_verSettingsVersionId))
		{
			return;
		}
		VersionSettings versionSettings = new VersionSettings
		{
			VersionId = _verSettingsVersionId,
			DisplayName = VerSettingsNameBox.Text.Trim(),
			UseCustomMemory = VerSettingsMemCustom.IsChecked.GetValueOrDefault(),
			JvmArgs = VerSettingsJvmBox.Text.Trim(),
			GameArgs = VerSettingsGameArgsBox.Text.Trim()
		};
		if (versionSettings.UseCustomMemory)
		{
			if (!int.TryParse(VerSettingsMemBox.Text.Trim(), out var result) || result < 512)
			{
				NotificationManager.Show(LanguageManager.Get("MsgInvalidRam"));
				return;
			}
			versionSettings.CustomMemoryMb = result;
		}
		VersionSettingsManager.Set(versionSettings);
		NotificationManager.Show(LanguageManager.Get("VerSettingsSaved"));
		if (sender is Button button)
		{
			ScaleTransform scaleTransform = new ScaleTransform(0.92, 0.92);
			button.RenderTransformOrigin = new Point(0.5, 0.5);
			button.RenderTransform = scaleTransform;
			DoubleAnimation animation = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(180L, 0L))
			{
				EasingFunction = new BackEase
				{
					Amplitude = 0.3,
					EasingMode = EasingMode.EaseOut
				}
			};
			scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
			scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
		}
	}

	private void CfgRamType_Changed(object sender, RoutedEventArgs e)
	{
		if (!_suppressConfigSave && _settingsInitialized)
		{
			bool valueOrDefault = CfgRamCustom.IsChecked.GetValueOrDefault();
			CfgRamSlider.IsEnabled = valueOrDefault;
			LauncherConfig.Current.RamType = (valueOrDefault ? 1 : 0);
			if (!valueOrDefault)
			{
				LauncherConfig.Current.MaxRamMb = (int)CfgRamSlider.Value;
			}
			LauncherConfig.Save();
			ApplyConfigToLaunchPage();
		}
	}

	private void CfgJavaPathBrowse_Click(object sender, RoutedEventArgs e)
	{
		var dlg = new Microsoft.Win32.OpenFileDialog
		{
			Title = "\u9009\u62E9 Java \u53EF\u6267\u884C\u6587\u4EF6 (javaw.exe / java.exe)",
			Filter = "Java \u53EF\u6267\u884C\u6587\u4EF6|javaw.exe;java.exe|\u6240\u6709\u6587\u4EF6|*.*",
			FileName = "javaw.exe"
		};
		if (!string.IsNullOrEmpty(CfgCustomJavaPath.Text) && File.Exists(CfgCustomJavaPath.Text))
		{
			try { dlg.InitialDirectory = System.IO.Path.GetDirectoryName(CfgCustomJavaPath.Text); }
			catch { }
		}
		if (dlg.ShowDialog() == true)
		{
			CfgCustomJavaPath.Text = dlg.FileName;
			LauncherConfig.Current.CustomJavaPath = dlg.FileName;
			LauncherConfig.Save();
		}
	}

	private void CfgRamSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		if (!_suppressConfigSave && _settingsInitialized)
		{
			int num = (int)CfgRamSlider.Value;
			CfgRamLabel.Text = $"{num} MB";
			if (CfgRamCustom.IsChecked.GetValueOrDefault())
			{
				LauncherConfig.Current.MaxRamMb = num;
				LauncherConfig.Save();
				ApplyConfigToLaunchPage();
			}
		}
	}

	private void CfgPersonalization_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		if (!_suppressConfigSave && _settingsInitialized)
		{
			LauncherConfig.Current.Opacity = (int)CfgOpacity.Value;
			LauncherConfig.Current.Hue = (int)CfgHue.Value;
			LauncherConfig.Current.Saturation = (int)CfgSaturation.Value;
			LauncherConfig.Current.Lightness = (int)CfgLightness.Value;
			LauncherConfig.Current.HueDelta = (int)CfgHueDelta.Value;
			LauncherConfig.Current.AnimationSpeed = (int)CfgAnimationSpeed.Value;
			if (CfgAnimationSpeedLabel != null)
			{
				CfgAnimationSpeedLabel.Text = $"{(int)CfgAnimationSpeed.Value}%";
			}
			LauncherConfig.Save();
			ApplyPersonalization();
		}
	}

	private void CfgLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (!_suppressConfigSave && _settingsInitialized && !_languageApplying && CfgLanguage.SelectedIndex >= 0)
		{
			string text = ((CfgLanguage.SelectedIndex == 1) ? "zh_CN" : "en_US");
			LauncherConfig.Current.Language = text;
			LauncherConfig.Save();
			LanguageManager.Apply(text);
		}
	}

	private void CfgOpenBackgroundFolder_Click(object sender, RoutedEventArgs e)
	{
		string path = System.IO.Path.Combine(DownloadManager.GetExeDirectory(), "Backgrounds");
		Directory.CreateDirectory(path);
		OpenFolderInExplorer(path);
	}

	private void CfgRefreshBackground_Click(object sender, RoutedEventArgs e)
	{
		NotificationManager.Show(LanguageManager.Get("MsgBackgroundRefreshed"));
	}

	private void CfgOpenMusicFolder_Click(object sender, RoutedEventArgs e)
	{
		string path = System.IO.Path.Combine(DownloadManager.GetExeDirectory(), "Music");
		Directory.CreateDirectory(path);
		OpenFolderInExplorer(path);
	}

	private void CfgRefreshMusic_Click(object sender, RoutedEventArgs e)
	{
		NotificationManager.Show(LanguageManager.Get("MsgMusicRefreshed"));
	}

	private void CfgOpenConfigFile_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			if (!File.Exists(LauncherConfig.ConfigFilePath))
			{
				LauncherConfig.Save();
			}
			OpenFolderInExplorer(LauncherConfig.ConfigFilePath);
		}
		catch (Exception ex)
		{
			NotificationManager.Show(string.Format(LanguageManager.Get("MsgOpenConfigFailed"), ex.Message));
		}
	}

	private void CfgResetAll_Click(object sender, RoutedEventArgs e)
	{
		LauncherConfig.Reset();
		InitSettingsControls();
		ApplyPersonalization();
		ApplyConfigToLaunchPage();
		NotificationManager.Show(LanguageManager.Get("MsgSettingsReset"));
	}

	private static void OpenFolderInExplorer(string path)
	{
		try
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = "explorer.exe",
				Arguments = "\"" + path + "\"",
				UseShellExecute = true
			});
		}
		catch
		{
		}
	}

	private void InitSettingsControls()
	{
		_suppressConfigSave = true;
		LauncherConfigData current = LauncherConfig.Current;
		CfgVersionIsolation.SelectedIndex = current.VersionIsolation;
		CfgWindowTitle.Text = current.WindowTitle;
		CfgCustomInfo.Text = current.CustomInfo;
		CfgLauncherVisibility.SelectedIndex = current.LauncherVisibility;
		CfgProcessPriority.SelectedIndex = current.ProcessPriority;
		CfgWindowType.SelectedIndex = current.WindowType;
		CfgWindowWidth.Text = current.WindowWidth.ToString();
		CfgWindowHeight.Text = current.WindowHeight.ToString();
		UpdateWindowCustomVisibility();
		CfgRamAuto.IsChecked = current.RamType == 0;
		CfgRamCustom.IsChecked = current.RamType == 1;
		CfgRamSlider.IsEnabled = current.RamType == 1;
		CfgRamSlider.Value = current.MaxRamMb;
		CfgRamLabel.Text = $"{current.MaxRamMb} MB";
		CfgOptimizeMemory.IsChecked = current.OptimizeMemoryBeforeLaunch;
		SetSkinRadio(current.SkinType);
		CfgSkinId.Text = current.SkinId;
		UpdateSkinIdVisibility();
		CfgJvmArgs.Text = current.JvmArgs;
		CfgCustomJavaPath.Text = current.CustomJavaPath;
		CfgGameArgs.Text = current.GameArgs;
		CfgPreLaunch.Text = current.PreLaunchCommand;
		CfgWaitPreLaunch.IsChecked = current.WaitForPreLaunch;
		CfgGcType.SelectedIndex = current.GcType;
		CfgDisableJlw.IsChecked = current.DisableJlw;
		CfgDisableLua.IsChecked = current.DisableLua;
		CfgHighPerfGpu.IsChecked = current.UseHighPerfGpu;
		CfgOpacity.Value = current.Opacity;
		CfgHue.Value = current.Hue;
		CfgSaturation.Value = current.Saturation;
		CfgLightness.Value = current.Lightness;
		CfgHueDelta.Value = current.HueDelta;
		CfgShowLogo.IsChecked = current.ShowLogo;
		CfgLanguage.SelectedIndex = ((current.Language == "zh_CN") ? 1 : 0);
		InitThemePanel(current.Theme);
		CfgBackgroundFit.SelectedIndex = MapBackgroundFitIndex(current.BackgroundFit);
		CfgBackgroundOpacity.Value = current.BackgroundOpacity;
		CfgBackgroundBlur.Value = current.BackgroundBlur;
		CfgColorfulBackground.IsChecked = current.ColorfulBackground;
		CfgMusicVolume.Value = current.MusicVolume;
		CfgMusicRandom.IsChecked = current.MusicRandom;
		CfgMusicAuto.IsChecked = current.MusicAuto;
		CfgMusicStart.IsChecked = current.MusicStart;
		CfgMusicStop.IsChecked = current.MusicStop;
		SetLogoRadio(current.LogoType);
		CfgEnableAnimation.IsChecked = current.EnableAnimation;
		CfgAnimationSpeed.Value = current.AnimationSpeed;
		CfgAnimationSpeedLabel.Text = $"{current.AnimationSpeed}%";
		CfgLinkLatencyMode.SelectedIndex = current.LinkLatencyMode;
		CfgLinkCustomPeer.Text = current.LinkCustomPeer;
		CfgLinkPort.Text = current.LinkPort;
		CfgLinkMaxPlayers.Value = current.LinkMaxPlayers;
		CfgLinkHeartbeat.Value = current.LinkHeartbeat;
		CfgLinkHeartbeatLabel.Text = $"{current.LinkHeartbeat}s";
		CfgLinkTimeout.Value = current.LinkTimeout;
		CfgLinkTimeoutLabel.Text = $"{current.LinkTimeout}s";
		CfgLinkUpnp.IsChecked = current.LinkUpnp;
		CfgLinkCompress.IsChecked = current.LinkCompress;
		CfgLinkEncrypt.IsChecked = current.LinkEncrypt;
		CfgLinkRelayServer.SelectedIndex = current.LinkRelayServer;
		CfgLinkMtu.SelectedIndex = current.LinkMtu;
		CfgLinkAllowSpectator.IsChecked = current.LinkAllowSpectator;
		CfgLinkWhitelist.IsChecked = current.LinkWhitelist;
		CfgLinkAutoKick.IsChecked = current.LinkAutoKick;
		CfgLinkShowPing.IsChecked = current.LinkShowPing;
		CfgDownloadSource.SelectedIndex = current.DownloadSource;
		CfgVersionListSource.SelectedIndex = current.VersionListSource;
		CfgMaxThreads.Value = current.MaxThreads;
		CfgMaxThreadsLabel.Text = current.MaxThreads.ToString();
		CfgSpeedLimit.Value = current.SpeedLimit;
		CfgSpeedLimitLabel.Text = ((current.SpeedLimit >= 42) ? LanguageManager.Get("ResSpeedUnlimited") : string.Format(LanguageManager.Get("ResSpeedValue"), current.SpeedLimit));
		CfgVerifySsl.IsChecked = current.VerifySsl;
		CfgModSource.SelectedIndex = current.ModSource;
		CfgModNameFormat.SelectedIndex = current.ModNameFormat;
		CfgModLocalNameStyle.SelectedIndex = current.ModLocalNameStyle;
		CfgUpdateRelease.IsChecked = current.UpdateRelease;
		CfgUpdateSnapshot.IsChecked = current.UpdateSnapshot;
		CfgAutoChinese.IsChecked = current.AutoChinese;
		CfgAutoCheckUpdate.IsChecked = current.AutoCheckUpdate;
		CfgShowSnapshot.IsChecked = current.ShowDownloadSnapshot;
		CfgShowOldBeta.IsChecked = current.ShowDownloadOldBeta;
		CfgShowAprilFool.IsChecked = current.ShowDownloadAprilFool;
		_suppressConfigSave = false;
	}

	private void SaveSettingsFromControls()
	{
		if (!_suppressConfigSave)
		{
			LauncherConfigData current = LauncherConfig.Current;
			current.VersionIsolation = CfgVersionIsolation.SelectedIndex;
			current.WindowTitle = CfgWindowTitle.Text;
			current.CustomInfo = CfgCustomInfo.Text;
			current.LauncherVisibility = CfgLauncherVisibility.SelectedIndex;
			current.ProcessPriority = CfgProcessPriority.SelectedIndex;
			current.WindowType = CfgWindowType.SelectedIndex;
			if (int.TryParse(CfgWindowWidth.Text, out var result))
			{
				current.WindowWidth = result;
			}
			if (int.TryParse(CfgWindowHeight.Text, out var result2))
			{
				current.WindowHeight = result2;
			}
			current.RamType = (CfgRamCustom.IsChecked.GetValueOrDefault() ? 1 : 0);
			current.MaxRamMb = (int)CfgRamSlider.Value;
			current.OptimizeMemoryBeforeLaunch = CfgOptimizeMemory.IsChecked.GetValueOrDefault();
			current.SkinType = GetSkinRadio();
			current.SkinId = CfgSkinId.Text;
			current.JvmArgs = CfgJvmArgs.Text;
			current.CustomJavaPath = CfgCustomJavaPath.Text;
			current.GameArgs = CfgGameArgs.Text;
			current.PreLaunchCommand = CfgPreLaunch.Text;
			current.WaitForPreLaunch = CfgWaitPreLaunch.IsChecked.GetValueOrDefault();
			current.GcType = CfgGcType.SelectedIndex;
			current.DisableJlw = CfgDisableJlw.IsChecked.GetValueOrDefault();
			current.DisableLua = CfgDisableLua.IsChecked.GetValueOrDefault();
			current.UseHighPerfGpu = CfgHighPerfGpu.IsChecked.GetValueOrDefault();
			current.Opacity = (int)CfgOpacity.Value;
			current.Hue = (int)CfgHue.Value;
			current.Saturation = (int)CfgSaturation.Value;
			current.Lightness = (int)CfgLightness.Value;
			current.HueDelta = (int)CfgHueDelta.Value;
			current.ShowLogo = CfgShowLogo.IsChecked.GetValueOrDefault();
			current.Language = ((CfgLanguage.SelectedIndex == 1) ? "zh_CN" : "en_US");
			current.BackgroundFit = MapBackgroundFitValue(CfgBackgroundFit.SelectedIndex);
			current.BackgroundOpacity = (int)CfgBackgroundOpacity.Value;
			current.BackgroundBlur = (int)CfgBackgroundBlur.Value;
			current.ColorfulBackground = CfgColorfulBackground.IsChecked.GetValueOrDefault();
			current.MusicVolume = (int)CfgMusicVolume.Value;
			current.MusicRandom = CfgMusicRandom.IsChecked.GetValueOrDefault();
			current.MusicAuto = CfgMusicAuto.IsChecked.GetValueOrDefault();
			current.MusicStart = CfgMusicStart.IsChecked.GetValueOrDefault();
			current.MusicStop = CfgMusicStop.IsChecked.GetValueOrDefault();
			current.LogoType = GetLogoRadio();
			current.EnableAnimation = CfgEnableAnimation.IsChecked.GetValueOrDefault();
			current.AnimationSpeed = (int)CfgAnimationSpeed.Value;
			current.LinkLatencyMode = CfgLinkLatencyMode.SelectedIndex;
			current.LinkCustomPeer = CfgLinkCustomPeer.Text;
			current.LinkPort = CfgLinkPort.Text;
			current.LinkMaxPlayers = (int)CfgLinkMaxPlayers.Value;
			current.LinkHeartbeat = (int)CfgLinkHeartbeat.Value;
			current.LinkTimeout = (int)CfgLinkTimeout.Value;
			current.LinkUpnp = CfgLinkUpnp.IsChecked.GetValueOrDefault();
			current.LinkCompress = CfgLinkCompress.IsChecked.GetValueOrDefault();
			current.LinkEncrypt = CfgLinkEncrypt.IsChecked.GetValueOrDefault();
			current.LinkRelayServer = CfgLinkRelayServer.SelectedIndex;
			current.LinkMtu = CfgLinkMtu.SelectedIndex;
			current.LinkAllowSpectator = CfgLinkAllowSpectator.IsChecked.GetValueOrDefault();
			current.LinkWhitelist = CfgLinkWhitelist.IsChecked.GetValueOrDefault();
			current.LinkAutoKick = CfgLinkAutoKick.IsChecked.GetValueOrDefault();
			current.LinkShowPing = CfgLinkShowPing.IsChecked.GetValueOrDefault();
			current.DownloadSource = CfgDownloadSource.SelectedIndex;
			current.VersionListSource = CfgVersionListSource.SelectedIndex;
			current.MaxThreads = (int)CfgMaxThreads.Value;
			current.SpeedLimit = (int)CfgSpeedLimit.Value;
			current.VerifySsl = CfgVerifySsl.IsChecked.GetValueOrDefault();
			current.ModSource = CfgModSource.SelectedIndex;
			current.ModNameFormat = CfgModNameFormat.SelectedIndex;
			current.ModLocalNameStyle = CfgModLocalNameStyle.SelectedIndex;
			current.UpdateRelease = CfgUpdateRelease.IsChecked.GetValueOrDefault();
			current.UpdateSnapshot = CfgUpdateSnapshot.IsChecked.GetValueOrDefault();
			current.AutoChinese = CfgAutoChinese.IsChecked.GetValueOrDefault();
			current.AutoCheckUpdate = CfgAutoCheckUpdate.IsChecked.GetValueOrDefault();
			current.ShowDownloadSnapshot = CfgShowSnapshot.IsChecked.GetValueOrDefault();
			current.ShowDownloadOldBeta = CfgShowOldBeta.IsChecked.GetValueOrDefault();
			current.ShowDownloadAprilFool = CfgShowAprilFool.IsChecked.GetValueOrDefault();
			LauncherConfig.Save();
		}
	}

	private int GetSkinRadio()
	{
		if (CfgSkin0.IsChecked.GetValueOrDefault())
		{
			return 0;
		}
		if (CfgSkin1.IsChecked.GetValueOrDefault())
		{
			return 1;
		}
		if (CfgSkin2.IsChecked.GetValueOrDefault())
		{
			return 2;
		}
		if (CfgSkin3.IsChecked.GetValueOrDefault())
		{
			return 3;
		}
		if (CfgSkin4.IsChecked.GetValueOrDefault())
		{
			return 4;
		}
		return 0;
	}

	private int GetLogoRadio()
	{
		if (CfgLogo0.IsChecked.GetValueOrDefault())
		{
			return 0;
		}
		if (CfgLogo1.IsChecked.GetValueOrDefault())
		{
			return 1;
		}
		if (CfgLogo2.IsChecked.GetValueOrDefault())
		{
			return 2;
		}
		if (CfgLogo3.IsChecked.GetValueOrDefault())
		{
			return 3;
		}
		return 1;
	}

	private void SetSkinRadio(int type)
	{
		CfgSkin0.IsChecked = type == 0;
		CfgSkin1.IsChecked = type == 1;
		CfgSkin2.IsChecked = type == 2;
		CfgSkin3.IsChecked = type == 3;
		CfgSkin4.IsChecked = type == 4;
	}

	private void SetLogoRadio(int type)
	{
		CfgLogo0.IsChecked = type == 0;
		CfgLogo1.IsChecked = type == 1;
		CfgLogo2.IsChecked = type == 2;
		CfgLogo3.IsChecked = type == 3;
	}

	private void UpdateSkinIdVisibility()
	{
		bool valueOrDefault = CfgSkin3.IsChecked.GetValueOrDefault();
		CfgSkinIdPanel.Visibility = ((!valueOrDefault) ? Visibility.Collapsed : Visibility.Visible);
	}

	private void UpdateWindowCustomVisibility()
	{
		bool flag = CfgWindowType.SelectedIndex == 3;
		CfgWindowWidth.Visibility = ((!flag) ? Visibility.Collapsed : Visibility.Visible);
		CfgWindowHeight.Visibility = ((!flag) ? Visibility.Collapsed : Visibility.Visible);
		CfgWindowX.Visibility = ((!flag) ? Visibility.Collapsed : Visibility.Visible);
	}

	private static int MapBackgroundFitIndex(int fit)
	{
		if (1 == 0)
		{
		}
		int result = fit switch
		{
			0 => 0, 
			4 => 1, 
			1 => 2, 
			3 => 3, 
			2 => 4, 
			_ => 0, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private static int MapBackgroundFitValue(int index)
	{
		if (1 == 0)
		{
		}
		int result = index switch
		{
			0 => 0, 
			1 => 4, 
			2 => 1, 
			3 => 3, 
			4 => 2, 
			_ => 0, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private void OnConfigChanged(object? sender, EventArgs e)
	{
		base.Dispatcher.BeginInvoke((Action)delegate
		{
			ApplyConfigToLaunchPage();
			ApplyPersonalization();
		});
	}

	private void ShowSettingsPage()
	{
		SelectNavItem("settings");
		SwitchToPage(SettingsPage, SettingsPageSlide, delegate
		{
			if (_settingsInitialized)
			{
				InitSettingsControls();
			}
			string initialTab = (_selectedSettingsTab?.Tag as string) ?? "launch";
			UpdateSettingsTabVisibility(initialTab);
			base.Dispatcher.BeginInvoke((Action)delegate
			{
				AnimateSettingsCards(initialTab);
			}, DispatcherPriority.Loaded);
		});
	}

	private void UpdateSettingsTabVisibility(string key)
	{
		SettingsTabLaunch.Visibility = ((!(key == "launch")) ? Visibility.Collapsed : Visibility.Visible);
		SettingsTabUI.Visibility = ((!(key == "ui")) ? Visibility.Collapsed : Visibility.Visible);
		SettingsTabLink.Visibility = ((!(key == "link")) ? Visibility.Collapsed : Visibility.Visible);
		SettingsTabSystem.Visibility = ((!(key == "system")) ? Visibility.Collapsed : Visibility.Visible);
	}

	private void InitSettingsTabs()
	{
		if (SettingsTabPanel.Children.Count > 0)
		{
			return;
		}
		string[] array = new string[4] { "SettingsTabLaunch", "SettingsTabUI", "SettingsTabLink", "SettingsTabSystem" };
		for (int i = 0; i < SettingsTabKeys.Length; i++)
		{
			Border border = new Border
			{
				Tag = SettingsTabKeys[i],
				Height = 28.0,
				CornerRadius = new CornerRadius(6.0),
				Padding = new Thickness(14.0, 0.0, 14.0, 0.0),
				Margin = new Thickness(0.0, 0.0, 6.0, 0.0),
				Background = Brushes.Transparent,
				Cursor = Cursors.Hand,
				RenderTransformOrigin = new Point(0.5, 0.5),
				RenderTransform = new ScaleTransform(1.0, 1.0)
			};
			TextBlock textBlock = new TextBlock
			{
				Foreground = Gray170Brush,
				FontSize = 12.0,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			textBlock.SetResourceReference(TextBlock.TextProperty, array[i]);
			border.Child = textBlock;
			border.MouseEnter += SettingsTab_MouseEnter;
			border.MouseLeave += SettingsTab_MouseLeave;
			border.MouseLeftButtonUp += SettingsTab_Click;
			SettingsTabPanel.Children.Add(border);
			if (i == 0)
			{
				_selectedSettingsTab = border;
				border.Background = Blue72Brush;
				textBlock.Foreground = Brushes.White;
			}
		}
		if (CfgPathLabel != null)
		{
			CfgPathLabel.Text = LauncherConfig.ConfigFilePath;
		}
		AttachSettingsHandlers(SettingsTabLaunch);
		AttachSettingsHandlers(SettingsTabUI);
		AttachSettingsHandlers(SettingsTabLink);
		AttachSettingsHandlers(SettingsTabSystem);
		_settingsInitialized = true;
	}

	private void AttachSettingsHandlers(DependencyObject root)
	{
		int childrenCount = VisualTreeHelper.GetChildrenCount(root);
		for (int i = 0; i < childrenCount; i++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(root, i);
			if (child is ComboBox comboBox)
			{
				comboBox.SelectionChanged += CfgGeneric_Changed;
				if (comboBox == CfgWindowType)
				{
					comboBox.SelectionChanged += CfgWindowType_Changed;
				}
			}
			else if (child is CheckBox checkBox)
			{
				checkBox.Checked += CfgGeneric_Changed;
				checkBox.Unchecked += CfgGeneric_Changed;
			}
			else if (child is RadioButton radioButton)
			{
				radioButton.Checked += CfgGeneric_Changed;
				if (radioButton == CfgSkin3)
				{
					radioButton.Checked += CfgSkin_Changed;
				}
			}
			else if (child is TextBox textBox)
			{
				textBox.LostFocus += CfgGeneric_Changed;
			}
			else if (child is Slider slider && slider != CfgRamSlider && slider != CfgOpacity && slider != CfgHue && slider != CfgSaturation && slider != CfgLightness && slider != CfgHueDelta && slider != CfgAnimationSpeed)
			{
				slider.ValueChanged += CfgGenericSlider_Changed;
			}
			AttachSettingsHandlers(child);
		}
	}

	private void CfgGeneric_Changed(object sender, RoutedEventArgs e)
	{
		if (!_suppressConfigSave && _settingsInitialized)
		{
			SaveSettingsFromControls();
		}
	}

	private void CfgGenericSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		if (!_suppressConfigSave && _settingsInitialized)
		{
			if (sender == CfgMaxThreads && CfgMaxThreadsLabel != null)
			{
				CfgMaxThreadsLabel.Text = ((int)CfgMaxThreads.Value).ToString();
			}
			else if (sender == CfgSpeedLimit && CfgSpeedLimitLabel != null)
			{
				CfgSpeedLimitLabel.Text = ((CfgSpeedLimit.Value >= 42.0) ? LanguageManager.Get("ResSpeedUnlimited") : string.Format(LanguageManager.Get("ResSpeedValue"), (int)CfgSpeedLimit.Value));
			}
			else if (sender == CfgLinkHeartbeat && CfgLinkHeartbeatLabel != null)
			{
				CfgLinkHeartbeatLabel.Text = $"{(int)CfgLinkHeartbeat.Value}s";
			}
			else if (sender == CfgLinkTimeout && CfgLinkTimeoutLabel != null)
			{
				CfgLinkTimeoutLabel.Text = $"{(int)CfgLinkTimeout.Value}s";
			}
			else if (sender == CfgBackgroundOpacity)
			{
				LauncherConfig.Current.BackgroundOpacity = (int)CfgBackgroundOpacity.Value;
			}
			else if (sender == CfgBackgroundBlur)
			{
				LauncherConfig.Current.BackgroundBlur = (int)CfgBackgroundBlur.Value;
			}
			else if (sender == CfgMusicVolume)
			{
				LauncherConfig.Current.MusicVolume = (int)CfgMusicVolume.Value;
			}
			SaveSettingsFromControls();
		}
	}

	private void CfgWindowType_Changed(object sender, SelectionChangedEventArgs e)
	{
		UpdateWindowCustomVisibility();
	}

	private void CfgSkin_Changed(object sender, RoutedEventArgs e)
	{
		UpdateSkinIdVisibility();
	}

	private void SettingsTab_MouseEnter(object sender, MouseEventArgs e)
	{
		if (sender is Border border && border != _selectedSettingsTab)
		{
			border.Background = new SolidColorBrush(Color.FromRgb(50, 50, 54));
			if (border.Child is TextBlock textBlock)
			{
				textBlock.Foreground = Brushes.White;
			}
		}
	}

	private void SettingsTab_MouseLeave(object sender, MouseEventArgs e)
	{
		if (sender is Border border && border != _selectedSettingsTab)
		{
			border.Background = Brushes.Transparent;
			if (border.Child is TextBlock textBlock)
			{
				textBlock.Foreground = Gray170Brush;
			}
		}
	}

	private void SettingsTab_Click(object sender, MouseButtonEventArgs e)
	{
		if (!(sender is Border { Tag: string tag } border))
		{
			return;
		}
		foreach (object child in SettingsTabPanel.Children)
		{
			if (child is Border border2)
			{
				bool flag = border2 == border;
				border2.Background = (flag ? new SolidColorBrush(_currentThemeColor) : Brushes.Transparent);
				if (border2.Child is TextBlock textBlock)
				{
					textBlock.Foreground = (flag ? Brushes.White : Gray170Brush);
				}
			}
		}
		_selectedSettingsTab = border;
		UpdateSettingsTabVisibility(tag);
		AnimateSettingsCards(tag);
	}

	private void AnimateSettingsCards(string key)
	{
		if (1 == 0)
		{
		}
		ScrollViewer scrollViewer = key switch
		{
			"launch" => SettingsTabLaunch, 
			"ui" => SettingsTabUI, 
			"link" => SettingsTabLink, 
			"system" => SettingsTabSystem, 
			_ => null, 
		};
		if (1 == 0)
		{
		}
		if (!(scrollViewer?.Content is StackPanel stackPanel))
		{
			return;
		}
		CubicEase easingFunction = new CubicEase
		{
			EasingMode = EasingMode.EaseOut
		};
		TimeSpan timeSpan = AnimDuration(360.0);
		int num = 0;
		foreach (object child in stackPanel.Children)
		{
			if (child is Border border)
			{
				TranslateTransform translateTransform = border.RenderTransform as TranslateTransform;
				if (translateTransform == null)
				{
					translateTransform = (TranslateTransform)(border.RenderTransform = new TranslateTransform());
				}
				translateTransform.BeginAnimation(TranslateTransform.YProperty, null);
				border.BeginAnimation(UIElement.OpacityProperty, null);
				translateTransform.Y = 16.0;
				border.Opacity = 0.0;
				DoubleAnimation animation = new DoubleAnimation(16.0, 0.0, timeSpan)
				{
					EasingFunction = easingFunction,
					BeginTime = TimeSpan.FromMilliseconds(num, 0L)
				};
				DoubleAnimation animation2 = new DoubleAnimation(0.0, 1.0, timeSpan)
				{
					EasingFunction = easingFunction,
					BeginTime = TimeSpan.FromMilliseconds(num, 0L)
				};
				translateTransform.BeginAnimation(TranslateTransform.YProperty, animation);
				border.BeginAnimation(UIElement.OpacityProperty, animation2);
				num += 60;
				if (num > 360)
				{
					num = 360;
				}
			}
		}
	}

	private void InitThemePanel(int selectedTheme)
	{
		if (CfgThemePanel.Children.Count > 0)
		{
			return;
		}
		for (int i = 0; i < ThemeNames.Length; i++)
		{
			Color themeColor = LauncherConfig.GetThemeColor(i);
			Border border = new Border
			{
				Width = 28.0,
				Height = 28.0,
				CornerRadius = new CornerRadius(14.0),
				Margin = new Thickness(0.0, 0.0, 8.0, 8.0),
				Cursor = Cursors.Hand,
				Tag = i,
				Background = new SolidColorBrush(themeColor),
				RenderTransformOrigin = new Point(0.5, 0.5),
				RenderTransform = new ScaleTransform(1.0, 1.0),
				ToolTip = ThemeNames[i]
			};
			if (i == selectedTheme)
			{
				border.BorderBrush = Brushes.White;
				border.BorderThickness = new Thickness(2.0);
			}
			border.MouseEnter += ThemeSwatch_MouseEnter;
			border.MouseLeave += ThemeSwatch_MouseLeave;
			border.MouseLeftButtonUp += ThemeSwatch_Click;
			CfgThemePanel.Children.Add(border);
		}
	}

	private void ThemeSwatch_MouseEnter(object sender, MouseEventArgs e)
	{
		if (sender is Border { RenderTransform: ScaleTransform renderTransform })
		{
			renderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(1.15, TimeSpan.FromMilliseconds(150L, 0L)));
			renderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1.15, TimeSpan.FromMilliseconds(150L, 0L)));
		}
	}

	private void ThemeSwatch_MouseLeave(object sender, MouseEventArgs e)
	{
		if (sender is Border { RenderTransform: ScaleTransform renderTransform })
		{
			renderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(150L, 0L)));
			renderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(150L, 0L)));
		}
	}

	private void ThemeSwatch_Click(object sender, MouseButtonEventArgs e)
	{
		if (!(sender is Border { Tag: var tag }) || !(tag is int num))
		{
			return;
		}
		foreach (object child in CfgThemePanel.Children)
		{
			if (child is Border border2)
			{
				border2.BorderBrush = ((border2.Tag is int num2 && num2 == num) ? Brushes.White : null);
				border2.BorderThickness = ((border2.Tag is int num3 && num3 == num) ? new Thickness(2.0) : new Thickness(0.0));
			}
		}
		LauncherConfig.Current.Theme = num;
		LauncherConfig.Save();
		ApplyPersonalization();
	}
}