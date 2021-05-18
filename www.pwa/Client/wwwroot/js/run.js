var noSleep = new NoSleep();
var wakeLockEnabled = false;

function openFullscreen() {
  var elem = document.documentElement;
  if (elem.requestFullscreen) {
    elem.requestFullscreen();
  } else if (elem.webkitRequestFullscreen) { /* Safari */
    elem.webkitRequestFullscreen();
  } else if (elem.msRequestFullscreen) { /* IE11 */
    elem.msRequestFullscreen();
  }
};

function closeFullscreen() {
  if (document.exitFullscreen) {
    document.exitFullscreen();
  } else if (document.webkitExitFullscreen) { /* Safari */
    document.webkitExitFullscreen();
  } else if (document.msExitFullscreen) { /* IE11 */
    document.msExitFullscreen();
  }
}

window.StartRun = async() => {

  const options = {
    enableHighAccuracy: true,
    maximumAge: 0,
    timeout: Infinity
  };

  if(!navigator.geolocation) {
    return 'Geolocation is not supported by your browser';
  } else {
    const watchId = navigator.geolocation.watchPosition(success, error, options);
    openFullscreen();
    noSleep.enable();
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
  noSleep.disable();
  closeFullscreen();
};

