
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using www.pwa.Shared;

namespace www.pwa.Client.Shared
{
    public partial class TableComponent : ComponentBase
    {
        [Inject]
        protected IHttpClientFactory httpClient { get; set; }
        [Inject]
        protected IJSRuntime _js { get; set; }
        [Parameter]
        public string walkGuid { get; set; }
        [Parameter]
        public bool isAdmin { get; set; } = false;

        HttpClient Http;
        private List<SchoolInfoModel> Schools { get; set; } = new List<SchoolInfoModel>();
        bool isLoading = true;
        private List<SchoolInfoModel> schools = new List<SchoolInfoModel>();
        private List<SchoolClassInfoModel> schoolClasses = new List<SchoolClassInfoModel>();
        private List<EntityInfoModel> entities = new List<EntityInfoModel>();
        private List<EntityRunFormData> entityRuns = new List<EntityRunFormData>();

        protected override Task OnInitializedAsync()
        {
            Http = httpClient.CreateClient("www.pwa.ServerAPI.NoAuth");
            GetSchools(walkGuid);
            return base.OnInitializedAsync();
        }

        public void Reset(float CurrentDistance = 0)
        {

            schoolClasses = new List<SchoolClassInfoModel>();
            entities = new List<EntityInfoModel>();
            entityRuns = new List<EntityRunFormData>();
            if (CurrentDistance > 0)
            {
                Schools.First().Distance = CurrentDistance;
                StateHasChanged();
            }
            else
            {
                Schools = new List<SchoolInfoModel>();
                GetSchools(walkGuid);
            }
        }

        private async Task GetSchools(string walkguid)
        {
            isLoading = true;
            await InvokeAsync(() => StateHasChanged());
            try
            {
                Schools = await Http.GetFromJsonAsync<List<SchoolInfoModel>>($"WwwRun/tabledata/schools/{walkguid}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            isLoading = false;
            await InvokeAsync(() => StateHasChanged());
        }

        private async Task GetSchoolClasses(string walkguid, int schoolId)
        {
            isLoading = true;
            await InvokeAsync(() => StateHasChanged());
            try
            {
                Schools.First(f => f.Id == schoolId).Classes =
                    await Http.GetFromJsonAsync<List<SchoolClassInfoModel>>($"WwwRun/tabledata/classes/{walkguid}/{schoolId}");
            }
            catch { }
            isLoading = false;
            await InvokeAsync(() => StateHasChanged());
        }

        private async Task GetClassEntities(int schoolId, int classId)
        {
            isLoading = true;
            await InvokeAsync(() => StateHasChanged());
            try
            {
                Schools.First(f => f.Id == schoolId).Classes.First(g => g.Id == classId).Entities =
                    await Http.GetFromJsonAsync<List<EntityInfoModel>>($"WwwRun/tabledata/entities/{schoolId}/{classId}");
            }
            catch { }
            isLoading = false;
            await InvokeAsync(() => StateHasChanged());
        }

        private async Task GetEntityRuns(int classId, int entityId)
        {
            isLoading = true;
            await InvokeAsync(() => StateHasChanged());
            try
            {
                Schools.SelectMany(m => m.Classes).First(g => g.Id == classId).Entities.First(h => h.Id == entityId).Runs =
                    await Http.GetFromJsonAsync<List<EntityRunInfoModel>>($"WwwRun/tabledata/runs/{classId}/{entityId}");
            }
            catch { }
            isLoading = false;
            await InvokeAsync(() => StateHasChanged());
        }

        async void ShowSchool(int schoolId)
        {

            var school = Schools.First(f => f.Id == schoolId);
            if (schools.Contains(school))
            {
                schools.Remove(school);
                await InvokeAsync(() => StateHasChanged());
                return;
            }

            if (school.Classes == null)
                await GetSchoolClasses(walkGuid, schoolId);
            if (school.Classes == null)
                school.Classes = new List<SchoolClassInfoModel>();
            schools.Add(school);
            await InvokeAsync(() => StateHasChanged());
            await _js.InvokeVoidAsync("Scroll", $"school_{schoolId}");
        }

        async void ShowClass(int schoolId, int classId)
        {
            var schoolClass = Schools.First(f => f.Id == schoolId).Classes.First(g => g.Id == classId);
            if (schoolClasses.Contains(schoolClass))
            {
                schoolClasses.Remove(schoolClass);
                await InvokeAsync(() => StateHasChanged());
                return;
            }

            if (schoolClass.Entities == null)
                await GetClassEntities(schoolId, classId);
            if (schoolClass.Entities == null)
                schoolClass.Entities = new List<EntityInfoModel>();
            schoolClasses.Add(schoolClass);
            await InvokeAsync(() => StateHasChanged());
            await _js.InvokeVoidAsync("Scroll", $"class_{classId}");
        }

        async void ShowEntity(int classId, int entityId)
        {
            var entity = Schools.SelectMany(m => m.Classes).First(f => f.Id == classId).Entities.First(g => g.Id == entityId);
            if (entities.Contains(entity))
            {
                entities.Remove(entity);
                await InvokeAsync(() => StateHasChanged());
                return;
            }

            if (entity.Runs == null)
                await GetEntityRuns(classId, entityId);
            if (entity.Runs == null)
                entity.Runs = new List<EntityRunInfoModel>();
            entities.Add(entity);
            await InvokeAsync(() => StateHasChanged());
            await _js.InvokeVoidAsync("Scroll", $"ent_{entityId}");
        }

        private async Task Delete(SchoolClassInfoModel schoolClass)
        {
            HttpClient http = httpClient.CreateClient("www.pwa.ServerAPI");
            try
            {
                var response = await http.GetAsync($"WwwRunAdmin/class/{schoolClass.Id}");
                if (response.IsSuccessStatusCode)
                {
                    Reset();
                }
            }
            catch { }
        }
        private async Task Delete(EntityInfoModel entity)
        {
            HttpClient http = httpClient.CreateClient("www.pwa.ServerAPI");
            try
            {
                var response = await http.GetAsync($"WwwRunAdmin/entity/{entity.Id}");
                if (response.IsSuccessStatusCode)
                {
                    Reset();
                }
            }
            catch { }
        }

        private async Task Delete(EntityRunInfoModel run)
        {
            HttpClient http = httpClient.CreateClient("www.pwa.ServerAPI");
            try
            {
                var response = await http.GetAsync($"WwwRunAdmin/run/{run.Id}");
                if (response.IsSuccessStatusCode)
                {
                    Reset();
                }
            }
            catch { }
        }

    }
}