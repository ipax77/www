const geooptions = {
  enableHighAccuracy: true,
  maximumAge: 0,
  timeout: 2900
};


window.GetLocation = () => {
  if(!navigator.geolocation) {
    return 'Geolocation is not supported by your browser';
  } else {
    navigator.geolocation.getCurrentPosition(success, error, geooptions);
  }

  async function success(position) {
    // var speed = position.coords.speed;
    // if (speed == null)
    //   speed = 0;
    await DotNet.invokeMethodAsync('www.pwa.Client', 'UpdateRunCaller', [position.coords.latitude, position.coords.longitude, position.timestamp, position.coords.accuracy, 0]);
  }

  async function error(err) {
    await DotNet.invokeMethodAsync('www.pwa.Client', 'ErrorCaller', 'ERROR(' + err.code + '): ' + err.message);
  }
};

window.StartRun = async() => {

  const options = {
    enableHighAccuracy: true,
    maximumAge: 30000,
    timeout: 27000
  };

  if(!navigator.geolocation) {
    return 'Geolocation is not supported by your browser';
  } else {
    const watchId = navigator.geolocation.watchPosition(success, error, options);
    // const watchId = navigator.geolocation.watchPosition(success, error);
    return String(watchId);
  }

  async function success(position) {
    var speed = position.coords.speed;
    if (speed == null)
      speed = 0;
    await DotNet.invokeMethodAsync('www.pwa.Client', 'UpdateRunCaller', [position.coords.latitude, position.coords.longitude, position.timestamp, position.coords.accuracy, speed]);
  }

  async function error(err) {
    await DotNet.invokeMethodAsync('www.pwa.Client', 'ErrorCaller', 'ERROR(' + err.code + '): ' + err.message);
  }

};

window.StopRun = (watchId) => {
  navigator.geolocation.clearWatch(watchId);
};

window.LoadMap = () => {
  console.log("loading map..");

  window.map = L.map('mapid');
  window.map.on("load", loaded);
  window.map.setView([51.318008, 9.468067], 6);

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
  }).addTo(window.map);

 

  async function loaded() {
    console.log("map loaded");
    await DotNet.invokeMethodAsync('www.pwa.Client', 'MapLoadedCaller');
  }

  return true;
};

window.AddLine = (latlngs, mycolor) => {
  try {
    var polyline = L.polyline(latlngs, {color: mycolor}).addTo(window.map);
    
    if (polyline != null) {
      window.map.fitBounds(polyline.getBounds());
      // window.map.flyToBounds(polyline.getBounds());
    }
  } catch (error) {
    console.error(error);
  }
};