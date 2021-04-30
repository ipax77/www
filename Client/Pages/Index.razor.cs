using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;
using www.pwa.Client.Services;
using www.pwa.Shared;

namespace www.pwa.Client.Pages
{
    public partial class Index : ComponentBase, IDisposable
    {
        [Inject]
        protected DataService dataService { get; set; }
        [Inject]
        protected ILogger<Index> logger { get; set; }
        [Inject]
        protected IJSRuntime js { get; set; }

        public EditContext editContext;
        public WwwFeedback feedback;
        public EntityRunFormData formData;
        string badgeClass = "d-none";
        string badgeInfo = "";
        RunInfo SchoolInfo;

        protected override void OnInitialized()
        {
            formData = new EntityRunFormData();
            editContext = new EditContext(formData);
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var schools = await dataService.GetTableData("school");
                if (schools.Any())
                    SchoolInfo = schools.First();
                StateHasChanged();
            }
        }

        private async Task HandleInvalidSubmit()
        {
            // await _js.InvokeVoidAsync("Scroll", "runform");
        }

        private async Task HandleValidSubmit()
        {
            badgeClass = "d-none";
            badgeInfo = "";

            if (String.IsNullOrEmpty(formData.Walk))
                formData.Walk = WwwData.s_walk;

            if (String.IsNullOrEmpty(formData.School))
                formData.School = WwwData.s_school;

            feedback = await dataService.Submit(formData);
            if (feedback == null)
            {
                if (dataService.isAdmin)
                {
                    badgeClass = "badge badge-success";
                    badgeInfo = "Admin mode enabled.";
                }
                else
                {
                    badgeClass = "badge badge-warning";
                    badgeInfo = "Datenbank Fehler, bitter versuche es später noch einmal.";
                }
            }
            else
            {
                if (feedback.EntPosition == -1)
                {
                    badgeClass = "badge badge-danger";
                    badgeInfo = "Das eingegebene Passwort stimmt nicht.";
                    formData.Credential = String.Empty;
                    feedback = null;
                }
                else if (feedback.EntPosition == -2)
                {
                    badgeClass = "badge badge-warning";
                    badgeInfo = "Das Pseudonym ist schon in einer anderen Klasse vergeben.";
                    feedback = null;
                }
                else if (feedback.EntPosition < 0)
                {
                    badgeClass = "badge badge-warning";
                    badgeInfo = "Datenbank Fehler, bitter versuche es später noch einmal.";
                    feedback = null;
                }
                else
                {
                    badgeClass = "badge badge-success";
                    badgeInfo = "Der Lauf wurde erfolgreich gespeichert.";
                    formData.Credential = String.Empty;
                }
            }
            var schools = await dataService.GetTableData("school");
            if (schools.Any())
                SchoolInfo = schools.First();
            StateHasChanged();
            if (feedback != null)
                await js.InvokeVoidAsync("Scroll", "map");
        }
        public void Dispose()
        {

        }
    }
}
