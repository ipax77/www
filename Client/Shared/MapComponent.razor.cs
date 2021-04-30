using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Numerics;
using System.Threading.Tasks;
using www.pwa.Client.Services;
using www.pwa.Shared;

namespace www.pwa.Client.Shared
{
    public partial class MapComponent : ComponentBase
    {
        [Inject]
        protected DataService dataService { get; set; }

        [Inject]
        protected ILogger<MapComponent> logger { get; set; }

        [Parameter]
        public WwwFeedback Feedback { get; set; }

        int width = 728;
        int height = 653;
        string currentdata = String.Empty;
        string classdata = String.Empty;
        string entdata = String.Empty;
        string totaldata = String.Empty;
        int map = 0;
        float debug_current = 0;

        string imagesrc = "images/earth-min.PNG";
        string jsonfile = "earth.json";
        // float pathlength = 2025.9127f;
        float length = 0;
        float lengthclass = 0;
        float lengthent = 0;
        float CurrentDistance = 0;  
        string NextTarget = String.Empty;
        bool showPath = false;
        

        protected override void OnParametersSet()
        {
            if (Feedback != null)
                Update();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //var dist = await GetTotalDistance();
                //logger.LogInformation(dist.ToString());

                CurrentDistance = await dataService.GetCurrentDistance();
                await dataService.SetPath(jsonfile);
                (totaldata, _) = BuildPathString(new KeyValuePair<int, int>(width, height), WwwData.pathPoints);
                StateHasChanged();
                
                if (CurrentDistance > 0)
                {
                    float done = CurrentDistance / WwwData.TotalDistance;
                    float per = WwwData.pathPoints.Count * done;
                    // length = pathlength * (per / 100);
                    (currentdata, length) = BuildPathString(new KeyValuePair<int, int>(width, height), WwwData.pathPoints.Take((int)per), true);
                    await Task.Delay(500);
                    showPath = true;
                    StateHasChanged();

                }
            }
        }

        async void DebugSubmit()
        {
            showPath = false;
            StateHasChanged();
            map = 0;
            WwwData.pathPoints.Clear();
            await dataService.SetPath(jsonfile);
            CurrentDistance = debug_current;
            float done = CurrentDistance / WwwData.TotalDistance;
            float per = WwwData.pathPoints.Count * done;
            (totaldata, _) = BuildPathString(new KeyValuePair<int, int>(width, height), WwwData.pathPoints);
            (currentdata, length) = BuildPathString(new KeyValuePair<int, int>(width, height), WwwData.pathPoints.Take((int)per), true);
            showPath = true;
            StateHasChanged();
        }

        async void MoveRight()
        {
            showPath = false;
            StateHasChanged();
            int newmap = map + 1;
            if (newmap > 2)
                newmap = 1;
            map = newmap;
            WwwData.pathPoints.Clear();
            await dataService.SetPath(jsonfile);
            float done = CurrentDistance / WwwData.TotalDistance;
            float per = WwwData.pathPoints.Count * done;
            (totaldata, _) = BuildPathString(new KeyValuePair<int, int>(width, height), WwwData.pathPoints);
            (currentdata, length) = BuildPathString(new KeyValuePair<int, int>(width, height), WwwData.pathPoints.Take((int)per), true);
            showPath = true;
            StateHasChanged();
        }

        async void MoveLeft()
        {
            showPath = false;
            StateHasChanged();
            int newmap = map - 1;
            if (newmap < 0)
                map = 0;
            map = newmap;
            WwwData.pathPoints.Clear();
            await dataService.SetPath(jsonfile);
            float done = CurrentDistance / WwwData.TotalDistance;
            float per = WwwData.pathPoints.Count * done;
            (totaldata, _) = BuildPathString(new KeyValuePair<int, int>(width, height), WwwData.pathPoints);
            (currentdata, length) = BuildPathString(new KeyValuePair<int, int>(width, height), WwwData.pathPoints.Take((int)per), true);
            showPath = true;
            StateHasChanged();
        }

        async Task<float> GetTotalDistance()
        {
            map = 0;
            await dataService.SetPath(jsonfile);
            float dist = 0;
            dist += GetDistance(WwwData.pathPoints, 0, 90);
            WwwData.pathPoints.Clear();
            map = 1;
            await dataService.SetPath(jsonfile);
            dist += GetDistance(WwwData.pathPoints, 10, 93);
            WwwData.pathPoints.Clear();
            map = 2;
            await dataService.SetPath(jsonfile);
            dist += GetDistance(WwwData.pathPoints, 62, 100);
            return dist;
        }

        float GetDistance(List<WwwPathPoint> points, float start, float end)
        {
            int skip = (int)(points.Count * (start / 100));
            int take = (int)(points.Count * (end / 100)) - skip;
            var cpoints = points.Skip(skip).Take(take).ToArray();
            float dist = 0;
            for (int i = 1; i < cpoints.Length; i++)
            {
                dist += Vector2.DistanceSquared(new Vector2(cpoints[i - 1].X, cpoints[i - 1].Y), new Vector2(cpoints[i].X, cpoints[i].Y));
            }
            return dist;
        }

        public void Update()
        {
            showPath = false;
            StateHasChanged();
            CurrentDistance = Feedback.SchoolTotal;
            double perCurrent = Feedback.SchoolTotal / WwwData.TotalDistance;
            if (perCurrent > 1)
                perCurrent = 1;
            int pointsCurrent = (int)(WwwData.pathPoints.Count * perCurrent);

            double perClass = Feedback.ClassTotal / WwwData.TotalDistance;
            if (perClass > 1)
                perClass = 1;
            double perEnt = Feedback.EntTotal / WwwData.TotalDistance;
            if (perEnt > 1)
                perEnt = 1;
            int pointsClass = (int)(WwwData.pathPoints.Count * perClass);
            if (pointsClass < 1)
                pointsClass = 1;
            int pointsEnt = (int)(WwwData.pathPoints.Count * perEnt);
            if (pointsEnt < 1)
                pointsEnt = 1;

            if (pointsCurrent > pointsClass)
                (currentdata, length) = BuildPathString(new KeyValuePair<int, int>(width, height), WwwData.pathPoints.Take(pointsCurrent - pointsClass), true);
            if (pointsClass > pointsEnt)
                (classdata, lengthclass) = BuildPathString(new KeyValuePair<int, int>(width, height), WwwData.pathPoints.Skip(pointsCurrent - pointsClass).Take(pointsClass - pointsEnt), true);
            (entdata, lengthent) = BuildPathString(new KeyValuePair<int, int>(width, height), WwwData.pathPoints.Skip(pointsCurrent - pointsEnt).Take(pointsEnt), true);

            //length = pathlength * (float)perCurrent * lmod;
            //lengthclass = pathlength * (float)perClass * lmod;
            //lengthent = pathlength * (float)perEnt * lmod;
            //length = length - lengthclass;
            //lengthclass = lengthclass - lengthent;

            showPath = true;
            StateHasChanged();
        }

        void SetNextTarget(float per)
        {
            var point = WwwData.Points.OrderByDescending(o => o.Value).FirstOrDefault(f => f.Value <= per * 100);
            if (point.Equals(default(KeyValuePair<string, float>)))
                NextTarget = WwwData.Points.Last().Key;
            else
                NextTarget = point.Key;
        }

        public static (string, float) BuildPathString(KeyValuePair<int, int> size, IEnumerable<WwwPathPoint> list, bool doCalcLength = false)
        {
            string build = String.Empty;
            float length = 0;
            WwwPathPoint lastPoint = null;
            if (list.Any())
            {
                build = $"M0 0 L0 2 M{size.Key} {size.Value}L{size.Key} {size.Value - 2}M{list.ElementAt(0).X} {list.ElementAt(0).Y}";
                for (int i = 1; i < list.Count(); i++)
                {
                    WwwPathPoint currentPoint = list.ElementAt(i);
                    build += $"L{MathF.Round(currentPoint.X, 2)} {MathF.Round(currentPoint.Y, 2)}";
                    if (doCalcLength && lastPoint != null)
                        length += Vector2.DistanceSquared(new Vector2(lastPoint.X, lastPoint.Y), new Vector2(currentPoint.X, currentPoint.Y));
                    lastPoint = currentPoint;
                }
            }
            return (build, length);
        }

    }
}
