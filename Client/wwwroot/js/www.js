
window.GetLocation = () => {
  if(!navigator.geolocation) {
    return 'Geolocation is not supported by your browser';
  } else {
    navigator.geolocation.getCurrentPosition(success, error);
  }

  async function success(position) {
    const latitude  = position.coords.latitude;
    const longitude = position.coords.longitude;
    await DotNet.invokeMethodAsync('www.pwa.Client', 'UpdateRunCaller', [latitude, longitude]);
  }

  function error() {
    return "";
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
    return String(watchId);
  }

  async function success(position) {
    await DotNet.invokeMethodAsync('www.pwa.Client', 'UpdateRunCaller', [position.coords.latitude, position.coords.longitude]);
  }

  async function error() {
    await DotNet.invokeMethodAsync('www.pwa.Client', 'UpdateRunCaller', [0, 0]);
  }

};

window.StopRun = (watchId) => {
  navigator.geolocation.clearWatch(watchId);
};

window.LoadMap = () => {

  window.map = L.map('mapid', {
      center: [51.318008, 9.468067],
      zoom: 6
  });

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
  }).addTo(window.map);

  return true;
};

window.AddLine = (latlngs) => {
  try {
    var polyline = L.polyline(latlngs, {color: 'red'}).addTo(window.map);
    
    if (polyline != null) {
      // window.map.fitBounds(polyline.getBounds());
      window.map.flyToBounds(plyline.getBounds());
    }
  } catch (error) {
    console.error(error);
  }
};