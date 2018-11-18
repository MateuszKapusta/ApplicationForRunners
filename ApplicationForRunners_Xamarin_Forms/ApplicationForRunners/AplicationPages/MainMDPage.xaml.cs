using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ApplicationForRunners.AplicationPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMDPage : MasterDetailPage
    {   
        MasterMDPage masterMDPage;
        private List<Page> detailMDActivePages = new List<Page>();

        public MainMDPage()
        {
           //InitializeComponent();

            masterMDPage = new MasterMDPage();
            Master = masterMDPage;
            masterMDPage.ListView.ItemSelected += ListView_ItemSelected;
          
            SelectedItemChangedEventArgs firstMapPage = new SelectedItemChangedEventArgs(masterMDPage.firstPage);
            ListView_ItemSelected(masterMDPage.ListView, firstMapPage);
        }

        MDMenuItem listTempUWP =null;

        async private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MDMenuItem;
            if (item == null)
                return;

            Page detailPage=null;
         
            if (item.OlwaysActive == true) {
                //object is always active and exist
                foreach (Page element in detailMDActivePages){
                    if (element.GetType() == item.TargetType && element.Title==item.Title){
                        detailPage = element;
                        break;
                    }
                }

                // has not been created yet
                if (detailPage == null) {                 
                    detailPage = (Page)Activator.CreateInstance(item.TargetType);
                    detailPage.Title = item.Title;
                    detailMDActivePages.Add(detailPage);
                }
            }
            else {
                //object not always exist
                detailPage = (Page)Activator.CreateInstance(item.TargetType);
                detailPage.Title = item.Title;
            }

            Detail = new NavigationPage(detailPage) {
                BarBackgroundColor = Color.FromHex("5ABAFF"),
                //BarTextColor = Color.FromHex("555555")
            };

            IsPresented = false;

            switch (Device.RuntimePlatform){
                case Device.iOS:
                case Device.Android:
                    ((ListView)sender).SelectedItem = null;
                    break;
                case Device.UWP:
                     await Task.Delay(10);
                    ((ListView)sender).SelectedItem = null;
                                  
                    var numberColl = ((ObservableCollection<MDMenuItem>)masterMDPage.ListView.ItemsSource).Count+1;
                    MDMenuItem[] colArray = new MDMenuItem[numberColl];

                    if (listTempUWP != null)
                        colArray[listTempUWP.Id] = listTempUWP;

                    foreach (ApplicationForRunners.AplicationPages.MDMenuItem itemLV in masterMDPage.ListView.ItemsSource)
                    {
                        if (itemLV == e.SelectedItem)
                            listTempUWP = itemLV;                        
                        else
                            colArray[itemLV.Id] = itemLV;
                    }

                    ObservableCollection<MDMenuItem> coll = new ObservableCollection<MDMenuItem>();
                    for (int i = 0; i < numberColl; i++) {

                        if (colArray[i] != null)
                            coll.Add(colArray[i]);
                    }                   
                    masterMDPage.ListView.ItemsSource = coll;
                    break;
            }        
        }
    }
}