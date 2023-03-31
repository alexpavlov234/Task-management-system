using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Schedule;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Pages.Issues;

namespace Task_management_system.Pages
{
    public partial class Index
    {
        [Inject]
        private NavigationManager NavMgr { get; set; }

        [Inject]
        private IIssueService IssueService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private IUserService userService { get; set; }

        string userFullName = "";
        string image = "";
        string result = "";
        string theTime;
        string theDate;

        private List<Issue> issues { get; set; }
        CurrentCondition response;
        private IssueModal issueModal = new IssueModal();
        Timer aTimer;
        ElementReference timeDiv;
        ElementReference dateDiv;
        private async Task NavToLogin()
        {
            NavMgr.NavigateTo($"/Login", true);
        }

        protected override async Task OnInitializedAsync()
        {
            if (userService.GetLoggedUser() != null)
            {
                issues = IssueService.GetAllIssues(userService.GetLoggedUser().Id);
            }
            if (userService.GetLoggedUser() != null)
            {
                var user = userService.GetLoggedUser();
                userFullName = user.FirstName + " " + user.LastName;
            }
            //try
            //{
            //    CurrentConditions currentConditions = new CurrentConditions("bXyjKZghQjnIwBB2WKEHdNsVyZdaJECK", "bg-bg");
            //    result = await currentConditions.Get(51097, false);
            //    response = currentConditions.ConvertData(result);
            //}
            //catch
            //{
            response = new CurrentCondition();
            response.Temperature = new Temperature();
            response.Temperature.Metric = new Metric();

            response.Temperature.Metric.Value = 12;
            response.Temperature.Metric.Unit = "C";
            response.WeatherText = "Гръмотевични бури";
            response.WeatherIcon = 17;
            response.Link = "https://www.accuweather.com/bg/bg/sofia/51097/weather-forecast/51097";
            //}
            image = "img/icons/" + response.WeatherIcon + ".svg";

        }
        public async void GetWeather()
        {

            StateHasChanged();
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("startTime1", timeDiv);
                await JSRuntime.InvokeVoidAsync("startTime2", dateDiv);
            }
        }

        private async Task UpdateAfterIssueModalSubmitAsync()
        {
            if (userService.GetLoggedUser() != null)
            {
                issues = IssueService.GetAllIssues(userService.GetLoggedUser().Id);
            }
            this.StateHasChanged();
        }
        public void Dispose()
        {
            JSRuntime.InvokeVoidAsync("stopTime");
        }


        public void OnPopupOpen(PopupOpenEventArgs<Issue> args)
        {

            args.Cancel = true;
            issueModal.OpenDialog(args.Data);
        }
    }
}
