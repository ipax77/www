
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
