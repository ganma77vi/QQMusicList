using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using Windows.ApplicationModel.Core;
using Windows.UI.Popups;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace QQMusicList
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            MyListView.ItemsSource = list;
        }
        private const string Music_List_Api = "http://i.y.qq.com/s.music/fcgi-bin/search_for_qq_cp?g_tk=5381&uin=0&format=jsonp&inCharset=utf8&outCharset=utf-8&notice=0&platform=h5&needNewCode=1&w={0}&t=0&flag=1&ie=utf-8&sem=1&aggr=0&p=1&remoteplace=txt.mqq.all&_=1460982060643";
        private async Task<string> GetMusicListRequest(string keyword)
        {
            HttpClient httpclient = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();
            string result = null;
            string Api = Music_List_Api.Replace("{0}", keyword);

            try
            {
                response = await httpclient.GetAsync(Api);
                if (response.StatusCode == HttpStatusCode.OK)//此处可判断网络返回码
                {
                    result = await response.Content.ReadAsStringAsync();
                }
                return result;
            }
            catch
            {
                var dialog = new MessageDialog("网络异常", "异常提示");
                dialog.Commands.Add(new UICommand("确定", cmd => { }));
                var a = await dialog.ShowAsync();
                return result = "";
            }
        }
        ObservableCollection<string> list = new ObservableCollection<string>();
        private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            string result =await GetMusicListRequest(SerchBox.Text);
            string reg = "songname\":\"(?<namelist>\\S+?)\"";
            try
            {
                MatchCollection mc = Regex.Matches(result, reg);
                for (int i = 0; i < mc.Count - 1; i++)
                {
                    GroupCollection gc = mc[i].Groups;
                    list.Add(gc["namelist"].Value);
                }
            }
            catch
            {
                var dialog = new MessageDialog("json为空", "异常提示");
                dialog.Commands.Add(new UICommand("确定", cmd => { }));
                var a = await dialog.ShowAsync();
                return;
            }
        }
    }
}
