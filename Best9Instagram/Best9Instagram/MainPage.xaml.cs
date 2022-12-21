using System;
using System.Reflection;
using System.Collections.Generic;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using Xamarin.Forms;
using ImageFromXamarinUI;
using System.IO;
using Xamarin.Essentials;

namespace Best9Instagram
{
    public partial class MainPage : ContentPage
    {
        private IInstaApi _api;
        private List<Image> _images;

        private const bool Debug = true;
        private const int ShowCount = 9;

        private string StatusText
        {
            get { return Status.Text; }
            set { Status.Text = value; }
        }

        public MainPage()
        {
            InitializeComponent();

            _images = new List<Image>();
            SetGridImages();
        }

        private async void LogIn_Clicked(object sender, EventArgs e)
        {
            var userSession = new UserSessionData
            {
                UserName = Username.Text,
                Password = Password.Text
            };
            var delay = RequestDelay.FromSeconds(2, 2);
            _api = InstaApiBuilder.CreateBuilder().SetUser(userSession).SetRequestDelay(delay).Build();

            if (!_api.IsUserAuthenticated)
            {
                StatusText = $"Logging in as {userSession.UserName}...";
                delay.Disable();
                var loginResult = await _api.LoginAsync();
                delay.Enable();
                if (!loginResult.Succeeded)
                {
                    StatusText = "Unable to login.";
                    return;
                }
            }

            StatusText = "Logged in.";

            SearchUserMedia();
        }

        private void Update_Clicked(object sender, EventArgs e)
        {
            SetGridImages();
        }

        private async void Screenshot_Clicked(object sender, EventArgs e)
        {
            var stream = await StackLayoutImages.CaptureImageAsync();
            var image = new Image
            {
                Source = ImageSource.FromStream(() => stream)
            };

            string result = await DisplayPromptAsync("Saving...", "Name your picture:");
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                DependencyService.Get<IFileService>().Save(ms.ToArray(), result);
            }
            await DisplayAlert("Saved", $"Your picture was saved in your internal storage.", "OK");
        }

        private async void SearchUserMedia()
        {
            string findUser = Debug ? "100miljonersmannen" : Username.Text;
            var user = await _api.UserProcessor.GetUserAsync(findUser);
            StatusText = $"Finding {findUser}...";
            if (!user.Succeeded)
            {
                StatusText = $"Unable to find {findUser}.";
                return;
            }

            var userMedia = await _api.UserProcessor.GetUserMediaAsync(findUser, InstagramApiSharp.PaginationParameters.MaxPagesToLoad(5));
            StatusText = "Collecting...";
            if (!user.Succeeded)
            {
                StatusText = $"Unable to find any info of {findUser}.";
                return;
            }

            var mediaList = new List<InstagramApiSharp.Classes.Models.InstaMedia>();
            foreach (var media in userMedia.Value)
            {
                if (media.TakenAt.Year == DateTime.Now.Year)
                    mediaList.Add(media);
            }

            if (mediaList.Count == 0)
            {
                StatusText = "Did not find any pictures from current year.";
                return;
            }

            mediaList.Sort((x, y) => y.LikesCount.CompareTo(x.LikesCount));
            if (mediaList.Count > ShowCount)
                mediaList.RemoveRange(ShowCount, mediaList.Count - ShowCount);
            int count = mediaList.Count < ShowCount ? mediaList.Count : ShowCount;
            int row = -1, column = -1;
            for (int i = 0; i < count; i++)
            {
                var media = mediaList[i];
                string uri = string.Empty;
                switch (media.MediaType)
                {
                    case InstagramApiSharp.Classes.Models.InstaMediaType.Carousel:
                        foreach(var carousel in media.Carousel)
                        {
                            if (carousel.Images.Count == 0)
                                continue;
                            uri = carousel.Images[0].Uri;
                            break;
                        }
                        if(string.IsNullOrEmpty(uri))
                            uri = media.Carousel[0].Videos[0].Uri;
                        break;
                    case InstagramApiSharp.Classes.Models.InstaMediaType.Image:
                        uri = media.Images[0].Uri;
                        break;
                    case InstagramApiSharp.Classes.Models.InstaMediaType.Video:
                        uri = media.Videos[0].Uri;
                        break;
                }
                int size = 190;
                Image image = new Image
                {
                    Source = uri,
                    WidthRequest = size,
                    HeightRequest = size,
                    Aspect = Aspect.AspectFit
                };
                column++;
                if (i % 3 == 0)
                    column = 0;
                if (column % 3 == 0)
                {
                    row++;
                    column = 0;
                }
                GridImages.Children.Add(image, column, row);
            }

            StatusText = "Done";
        }

        private void SetGridImages()
        {
            int row = -1, column = -1;
            for (int i = 0; i < 9; i++)
            {
                int size = 161;
                _images.Add(new Image
                {
                    Source = Debug ? ImageSource.FromFile($"pic{i}.jpg") : "",
                    WidthRequest = size,
                    HeightRequest = size,
                    Aspect = Aspect.AspectFill,
                });
                column++;
                if (i % 3 == 0)
                    column = 0;
                if (column % 3 == 0)
                {
                    row++;
                    column = 0;
                }
                GridImages.Children.Add(_images[i], column, row);
            }
        }
    }
}
